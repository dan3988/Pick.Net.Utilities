using System.Reflection;

using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.TestApp.Controls;

[BindableProperty<object>("ObjectProp", DefaultValue = 5)]
[BindableProperty<bool>("BooleanProp", DefaultValue = true)]
[BindableProperty<char>("CharProp", DefaultValue = 'A')]
[BindableProperty<sbyte>("SByteProp", DefaultValue = 1)]
[BindableProperty<byte>("ByteProp", DefaultValue = 1)]
[BindableProperty<short>("Int16Prop", DefaultValue = 1)]
[BindableProperty<ushort>("UInt16Prop", DefaultValue = 1)]
[BindableProperty<int>("Int32Prop", DefaultValue = 1)]
[BindableProperty<uint>("UInt32Prop", DefaultValue = 1)]
[BindableProperty<long>("Int64Prop", DefaultValue = 1)]
[BindableProperty<ulong>("UInt64Prop", DefaultValue = 1)]
[BindableProperty<float>("SingleProp", DefaultValue = 1)]
[BindableProperty<double>("DoubleProp", DefaultValue = 1)]
[BindableProperty<double>("DecimalProp")]
[BindableProperty<string>("StringProp", DefaultValue = "text")]
[BindableProperty<ConsoleColor>("EnumProp", DefaultValue = ConsoleColor.Red)]
[BindableProperty<BindingFlags>("FlagEnumProp", DefaultValue = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)]
internal partial class AllTypes : BindableObject
{
}
