using DotNetUtilities.Maui.Helpers;

namespace DotNetUtilities.Maui.TestApp.Controls;

[BindableProperty("ObjectProp", typeof(object), DefaultValue = 5)]
[BindableProperty("BooleanProp", typeof(bool), DefaultValue = default(bool))]
[BindableProperty("CharProp", typeof(char), DefaultValue = default(char))]
[BindableProperty("SByteProp", typeof(sbyte), DefaultValue = default(sbyte))]
[BindableProperty("ByteProp", typeof(byte), DefaultValue = default(byte))]
[BindableProperty("Int16Prop", typeof(short), DefaultValue = default(short))]
[BindableProperty("UInt16Prop", typeof(ushort), DefaultValue = default(ushort))]
[BindableProperty("Int32Prop", typeof(int), DefaultValue = default(int))]
[BindableProperty("UInt32Prop", typeof(uint), DefaultValue = default(uint))]
[BindableProperty("Int64Prop", typeof(long), DefaultValue = default(long))]
[BindableProperty("UInt64Prop", typeof(ulong), DefaultValue = default(ulong))]
[BindableProperty("SingleProp", typeof(float), DefaultValue = default(float))]
[BindableProperty("DoubleProp", typeof(double), DefaultValue = default(double))]
[BindableProperty("DecimalProp", typeof(decimal), DefaultValue = 0)]
[BindableProperty("StringProp", typeof(string), DefaultValue = "text")]
[BindableProperty("EnumProp", typeof(ConsoleColor), DefaultValue = ConsoleColor.Red)]
internal partial class AllTypes : BindableObject
{
}
