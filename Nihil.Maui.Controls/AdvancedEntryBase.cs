using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Syncfusion.Maui.Core;

namespace Nihil.Maui.Controls;

public abstract partial class AdvancedEntryBase : ContentView
{
    protected abstract InputView Entry { get; }

    protected abstract SfTextInputLayout InputLayout { get; }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(AdvancedEntryBase),
        defaultValue: string.Empty,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: TextChanged);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static void TextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not AdvancedEntryBase instance)
            return;

        instance.Entry.Text = (string)newValue;
    }

    //TODO: suppress warning
    protected void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        Text = e.NewTextValue;
    }

    public static readonly BindableProperty ObjectProperty = BindableProperty.Create(nameof(Object), typeof(INotifyDataErrorInfo), typeof(AdvancedEntryBase),
        defaultValue: null,
        defaultBindingMode: BindingMode.OneWay,
        propertyChanged: ObjectChanged);

    public INotifyDataErrorInfo Object
    {
        get => (INotifyDataErrorInfo)GetValue(ObjectProperty);
        set => SetValue(ObjectProperty, value);
    }

    public static void ObjectChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not AdvancedEntryBase instance
            || newValue is not INotifyDataErrorInfo obj)
            return;

        obj.ErrorsChanged += instance.Object_ErrorsChanged;

        instance.ReadPropertyInfo();

        //TODO: detach old
    }

    private void Object_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        if (e.PropertyName != PropertyName)
            return;

        if (sender is not INotifyDataErrorInfo obj)
            return;

        IEnumerable<ValidationResult> errors = obj.GetErrors(e.PropertyName).OfType<ValidationResult>();
        InputLayout.HasError = errors.Any();
        InputLayout.ErrorText = string.Join(Environment.NewLine, errors.Select(err => err.ErrorMessage));
    }


    public static readonly BindablePropertyKey PropertyNameProperty = BindableProperty.CreateReadOnly(nameof(PropertyName), typeof(string), typeof(AdvancedEntryBase),
        defaultValue: null,
        defaultBindingMode: BindingMode.OneWay);

    public string PropertyName
    {
        get => (string)GetValue(PropertyNameProperty.BindableProperty);
        set => SetValue(PropertyNameProperty, value);
    }


    private void ReadPropertyInfo()
    {
        if (Object == null || PropertyName == null)
            return;

        PropertyInfo propertyInfo = Object.GetType().GetProperty(PropertyName)
            ?? throw new ArgumentException($"Couldn't find property ({PropertyName}) in type ({Object.GetType().Name}).");

        var displayAttribute = propertyInfo.GetCustomAttribute<DisplayAttribute>();

        if (displayAttribute != null)
        {
            InputLayout.Hint = displayAttribute.GetName() ?? string.Empty;
            InputLayout.HelperText = displayAttribute.GetDescription() ?? string.Empty;
        }

        var stringLengthAttribute = propertyInfo.GetCustomAttribute<StringLengthAttribute>();

        if (stringLengthAttribute != null)
            Entry.MaxLength = stringLengthAttribute.MaximumLength + 1;
    }
}