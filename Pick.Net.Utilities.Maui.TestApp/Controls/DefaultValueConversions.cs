using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.TestApp.Controls;

[BindableProperty<byte>("Byte", DefaultValue = (byte)5)]
[BindableProperty<double>("DecimalInt", DefaultValue = 10)]
[BindableProperty<double>("DecimalLong", DefaultValue = 10L)]
[BindableProperty<double>("DecimalDouble", DefaultValue = 5.5d)]
[BindableProperty<float>("FloatInt", DefaultValue = 10)]
[BindableProperty<float>("FloatLong", DefaultValue = 10L)]
[BindableProperty<uint>("IntUint", DefaultValue = 5u)]
internal partial class DefaultValueConversions : BindableObject
{
}