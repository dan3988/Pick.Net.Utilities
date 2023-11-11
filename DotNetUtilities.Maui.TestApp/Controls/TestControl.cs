﻿using DotNetUtilities.Maui.Helpers;

namespace DotNetUtilities.Maui.TestApp.Controls;

[BindableProperty("Text", typeof(string))]
[BindableProperty("MaxLength", typeof(int))]
[BindableProperty("MinLength", typeof(int))]
[BindableProperty("TransformedText", typeof(string), WriteAccessLevel = PropertyAccessLevel.Private)]
public partial class TestControl : BindableObject
{
}
