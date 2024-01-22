using System.Reflection;

namespace Pick.Net.Utilities.Reflection.Members.Properties;

public interface IReflectedProperty : IReflectedMember
{
	MemberInfo IReflectedMember.Member => Member;

	new PropertyInfo Member { get; }

	Delegate? Getter { get; }

	Delegate? Setter { get; }
}
