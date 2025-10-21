using Syncfusion.Maui.Core;

namespace Nihil.Maui.Controls;

public partial class AdvancedEditor : AdvancedEntryBase
{
    public AdvancedEditor()
    {
        InitializeComponent();
    }

    protected override InputView Entry => txtEntry;

    protected override SfTextInputLayout InputLayout => inputLayout;
}
