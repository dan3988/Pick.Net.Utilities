using System.Diagnostics;
using System.Reflection;

namespace Pick.Net.Utilities.Reflection.Members.Properties;

[DebuggerDisplay("{DebugName,nq} {Member.DeclaringType.FullName,nq}.{Member.Name,nq} {DebugSuffix,nq}")]
internal abstract class ReflectedProperty : ReflectedMember, IReflectedProperty
{
	internal abstract string DebugName { get; }

	internal abstract string DebugSuffix { get; }

	public sealed override PropertyInfo Member { get; }

	public abstract Delegate? Getter { get; }

	public abstract Delegate? Setter { get; }

	private protected ReflectedProperty(PropertyInfo member)
	{
		Member = member;
	}
}
