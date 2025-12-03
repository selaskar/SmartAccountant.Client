using Syncfusion.Maui.Core;

namespace Nihil.Maui.Controls;

public partial class AdvancedEntry : AdvancedEntryBase
{
    public AdvancedEntry()
    {
        InitializeComponent();
    }

    protected override InputView Entry => txtEntry;

    protected override SfTextInputLayout InputLayout => inputLayout;
}
