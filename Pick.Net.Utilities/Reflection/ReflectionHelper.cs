using System.Reflection;

namespace Pick.Net.Utilities.Reflection;

public static class ReflectionHelper
{
	public const BindingFlags Declared = DeclaredPublic | BindingFlags.NonPublic;
	public const BindingFlags DeclaredInstance = DeclaredPublicInstance | BindingFlags.NonPublic;
	public const BindingFlags DeclaredStatic = DeclaredPublicStatic | BindingFlags.NonPublic;

	public const BindingFlags Public = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
	public const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;
	public const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;

	public const BindingFlags DeclaredPublic = Public | BindingFlags.DeclaredOnly;
	public const BindingFlags DeclaredPublicInstance = PublicInstance | BindingFlags.DeclaredOnly;
	public const BindingFlags DeclaredPublicStatic = PublicStatic | BindingFlags.DeclaredOnly;

	public const BindingFlags NonPublic = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
	public const BindingFlags NonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;
	public const BindingFlags NonPublicStatic = BindingFlags.NonPublic | BindingFlags.Static;

	public const BindingFlags DeclaredNonPublic = NonPublic | BindingFlags.DeclaredOnly;
	public const BindingFlags DeclaredNonPublicInstance = NonPublicInstance | BindingFlags.DeclaredOnly;
	public const BindingFlags DeclaredNonPublicStatic = NonPublicStatic | BindingFlags.DeclaredOnly;
}
