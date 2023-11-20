using System.Reflection;

namespace Pick.Net.Utilities.Reflection;

public static partial class DelegateHelper
{
	public static T CreateDelegate<T>(MethodInfo method) where T : Delegate
		=> (T)Delegate.CreateDelegate(typeof(T), method);
}
