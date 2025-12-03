using System.Globalization;

namespace SmartAccountant.Client.MAUI.Converters;

/// <summary>
/// For scenarios where parameter binding is necessary.
/// </summary>
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

        return value.Equals(Parameter ?? parameter);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool b)
            return Binding.DoNothing;

        if (!b)
            return Binding.DoNothing;

        if ((Parameter ?? parameter) == null)
            return Binding.DoNothing;

        return (Parameter ?? parameter)!; //Certainly not going to be null.
    }
}
