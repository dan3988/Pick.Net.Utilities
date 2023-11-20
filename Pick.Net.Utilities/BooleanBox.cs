namespace Pick.Net.Utilities;

public static class BooleanBox
{
	public static readonly IConvertible True = true;
	public static readonly IConvertible False = false;

	public static IConvertible Box(this bool bln) => bln ? True : False;

	public static IConvertible InverseBox(this bool bln) => bln ? False : True;
}
