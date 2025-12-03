using CommunityToolkit.Mvvm.Input;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Client.MAUI.Pages;

public partial class MonetaryAmountEditor : ContentView
{
    public MonetaryAmountEditor()
    {
        InitializeComponent();
    }

    public IEnumerable<Currency> Currencies { get; } = Enum.GetValues<Currency>();


    public static readonly BindableProperty AmountProperty = BindableProperty.Create(nameof(Amount), typeof(MonetaryValue), typeof(MonetaryAmountEditor),
        defaultValue: new MonetaryValue(0, Currency.USD),
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: AmountChanged);

    public MonetaryValue Amount
    {
        get => (MonetaryValue)GetValue(AmountProperty);
        set => SetValue(AmountProperty, value);
    }

    public static void AmountChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not MonetaryAmountEditor instance)
            return;

        var newMonetaryValue = (MonetaryValue)newValue;

        string newAmountString = newMonetaryValue.Amount.ToString("N2");

        if (instance.txtAmount.Text != newAmountString)
            instance.txtAmount.Text = newAmountString;

        if (instance.currencyPicker.SelectedItem is not Currency currency || currency != newMonetaryValue.Currency)
            instance.currencyPicker.SelectedItem = newMonetaryValue.Currency;
    }

    [RelayCommand]
    public void SetAmount()
    {
        if (!decimal.TryParse(txtAmount.Text, out decimal amount))
            return;

        if (Amount.Amount == amount)
            return;

        Amount = new MonetaryValue(amount, Amount.Currency);
    }


    private void currencyPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (currencyPicker.SelectedIndex == -1)
            return;

        Amount = new MonetaryValue(Amount.Amount, (Currency)currencyPicker.SelectedItem);
    }
}