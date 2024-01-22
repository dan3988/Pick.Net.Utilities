using System.Reflection;

namespace Pick.Net.Utilities.Reflection.Members;

internal abstract class ReflectedMember
{
	public abstract MemberInfo Member { get; }

	public Type DeclaringType => Member.DeclaringType!;

	public string Name => Member.Name;
}