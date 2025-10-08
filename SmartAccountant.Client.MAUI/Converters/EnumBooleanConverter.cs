﻿using System.Globalization;

namespace SmartAccountant.Client.MAUI.Converters;

internal partial class EnumBooleanConverter : BindableObject, IValueConverter
{
    public static readonly BindableProperty ParameterProperty = BindableProperty.Create(nameof(Parameter), typeof(Enum), typeof(EnumBooleanConverter));

    public Enum? Parameter
    {
        get => (Enum?)GetValue(ParameterProperty);
        set => SetValue(ParameterProperty, value);
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return Binding.DoNothing;

        if (Enum.IsDefined(value.GetType(), value) == false)
            return Binding.DoNothing;

        return value.Equals(Parameter);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool b)
            return Binding.DoNothing;

        if (!b)
            return Binding.DoNothing;

        if (Parameter == null)
            return Binding.DoNothing;

        return Parameter;
    }
}
