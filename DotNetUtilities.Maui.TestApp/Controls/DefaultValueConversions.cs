using DotNetUtilities.Maui.Helpers;

namespace DotNetUtilities.Maui.TestApp.Controls;

[BindableProperty("Byte", typeof(byte), DefaultValue = (byte)5)]
[BindableProperty("DecimalInt", typeof(decimal), DefaultValue = 10)]
[BindableProperty("DecimalLong", typeof(decimal), DefaultValue = 10L)]
[BindableProperty("DecimalDouble", typeof(decimal), DefaultValue = 5.5d)]
[BindableProperty("FloatInt", typeof(decimal), DefaultValue = 10)]
[BindableProperty("FloatLong", typeof(decimal), DefaultValue = 10L)]
[BindableProperty("FloatDouble", typeof(decimal), DefaultValue = 5.5d)]
[BindableProperty("IntUint", typeof(uint), DefaultValue = 5u)]
internal partial class DefaultValueConversions : BindableObject
{
}