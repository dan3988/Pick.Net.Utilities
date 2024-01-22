using System.Reflection;

namespace Pick.Net.Utilities.Reflection.Members.Properties;

public interface IReflectedMember
{
	Type DeclaringType { get; }

	string Name { get; }

	MemberInfo Member { get; }
}