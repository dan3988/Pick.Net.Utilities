using System.Collections.Immutable;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Model;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using Pick.Net.Utilities.Reflection;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

public abstract class CodeGeneratorTest : CodeActionTest<MSTestVerifier>
{
	private delegate void VerifyDiagnosticResultsDelegate(AnalyzerTest<MSTestVerifier> self, IEnumerable<(Project project, Diagnostic diagnostic)> actualResults, ImmutableArray<DiagnosticAnalyzer> analyzers, DiagnosticResult[] expectedResults, IVerifier verifier);

	private static readonly VerifyDiagnosticResultsDelegate VerifyDiagnosticResults = GetInstanceMethod<VerifyDiagnosticResultsDelegate>("VerifyDiagnosticResults", ReflectionHelper.DeclaredNonPublicInstance);

	private static TDelegate GetInstanceMethod<TDelegate>(string name, BindingFlags flags) where TDelegate : Delegate
	{
		var paramTypes = DelegateHelper.GetArgumentTypes<TDelegate>();
		var instanceType = paramTypes[0];
		var paramTypesArray = paramTypes.Skip(1).ToArray();
		var method = instanceType.GetMethod(name, flags, paramTypesArray) ?? throw new MissingMethodException(instanceType.FullName, name);
		return DelegateHelper.CreateDelegate<TDelegate>(method);
	}

	public CSharpParseOptions Options { get; set; } = CSharpParseOptions.Default;

	public List<CodeGeneratorTestOutput> OutputFiles { get; } = [];

	protected override string DefaultFileExt => "cs";

	public override string Language => LanguageNames.CSharp;

	public override Type SyntaxKindType => typeof(SyntaxKind);

	protected override CompilationOptions CreateCompilationOptions()
		=> new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true);

	protected override ParseOptions CreateParseOptions()
		=> Options;

	protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
		=> [];

	protected abstract IIncrementalGenerator CreateGenerator();

	public CodeGeneratorTest ExpectDiagnostic(DiagnosticDescriptor descriptor, int line, int column, int length, params object[] messageArgs)
		=> ExpectDiagnostic(descriptor, line, column, line, column + length, messageArgs);

	public CodeGeneratorTest ExpectDiagnostic(DiagnosticDescriptor descriptor, int startLine, int startColumn, int endLine, int endColumn, params object[] messageArgs)
	{
		ExpectedDiagnostics.Add(new DiagnosticResult(descriptor).WithSpan(startLine, startColumn, endLine, endColumn).WithArguments(messageArgs));
		return this;
	}

	protected override async Task RunImplAsync(CancellationToken cancellationToken)
	{
		var testState = TestState.WithInheritedValuesApplied(null, []).WithProcessedMarkup(MarkupOptions, null, [], [], DefaultFilePath);
		var state = new EvaluatedProjectState(testState, ReferenceAssemblies);
		var additionalProjects = testState.AdditionalProjects.Values.Select(additionalProject => new EvaluatedProjectState(additionalProject, ReferenceAssemblies)).ToImmutableArray();
		var project = await CreateProjectAsync(state, additionalProjects, cancellationToken);
		var compilation = await GetProjectCompilationAsync(project, Verify, cancellationToken);
		var generator = CreateGenerator();
		var driver = (CSharpGeneratorDriver)CSharpGeneratorDriver.Create(generator).WithUpdatedParseOptions(Options);
		driver.RunGeneratorsAndUpdateCompilation(compilation, out compilation, out var diagnostics, cancellationToken);

		VerifyDiagnosticResults.Invoke(this, diagnostics.Select(v => (project, v)), [], [.. testState.ExpectedDiagnostics], Verify);

		var generatorType = generator.GetType();
		var assemblyName = generatorType.Assembly.GetName();
		var fileNamePrefix = assemblyName.Name + "\\" + generatorType + "\\";

		foreach (var (fileName, code) in OutputFiles)
		{
			var fullName = fileNamePrefix + fileName + ".g.cs";
			var file = compilation.SyntaxTrees.FirstOrDefault(v => v.FilePath == fullName);
			Assert.IsNotNull(file, "Generated file \"{0}\" was not found in compilation", fileName);
			Verify.EqualOrDiff(code, file.ToString());
		}
	}

}

public sealed class CodeGeneratorTest<TGenerator> : CodeGeneratorTest
	where TGenerator : IIncrementalGenerator, new()
{
	protected override IIncrementalGenerator CreateGenerator()
		=> new TGenerator();

	public CodeGeneratorTest<TGenerator> ExpectOutput(string name, string code)
	{
		OutputFiles.Add(new(name, code));
		return this;
	}

	public new CodeGeneratorTest<TGenerator> ExpectDiagnostic(DiagnosticDescriptor descriptor, int line, int column, int length, params object[] messageArgs)
		=> (CodeGeneratorTest<TGenerator>)base.ExpectDiagnostic(descriptor, line, column, length, messageArgs);

	public new CodeGeneratorTest<TGenerator> ExpectDiagnostic(DiagnosticDescriptor descriptor, int startLine, int startColumn, int endLine, int endColumn, params object[] messageArgs)
		=> (CodeGeneratorTest<TGenerator>)base.ExpectDiagnostic(descriptor, startLine, startColumn, endLine, endColumn, messageArgs);
}
