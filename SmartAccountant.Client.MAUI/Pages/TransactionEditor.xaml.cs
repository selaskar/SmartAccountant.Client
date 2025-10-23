using SmartAccountant.Client.Models;
using SmartAccountant.Client.ViewModels;

namespace SmartAccountant.Client.MAUI.Pages;

public partial class TransactionEditor : ContentView
{
    public TransactionEditor()
    {
        InitializeComponent();

        BindingContext = new TransactionEditorViewModel();
    }

    public double LabelColumnWidth
    {
        get;
        set
        {
            field = value;
            labelColumn.Width = new GridLength(value, GridUnitType.Absolute);
        }
    }

    public static readonly BindableProperty TransactionProperty = BindableProperty.Create(nameof(Transaction), typeof(Transaction), typeof(TransactionEditor),
        defaultValue: null,
        defaultBindingMode: BindingMode.OneWay,
        propertyChanged: TransactionChanged);

    public Transaction Transaction
    {
        get => (Transaction)GetValue(TransactionProperty);
        set => SetValue(TransactionProperty, value);
    }

    public static void TransactionChanged(BindableObject bindable, object _, object newValue)
    {
        if (bindable is not TransactionEditor view
            || view.BindingContext is not TransactionEditorViewModel viewModel)
            return;

        viewModel.Transaction = newValue as Transaction;
    }
}