using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartAccountant.Client.Models;

public abstract class BaseModel : ObservableValidator, IEditableObject
{
    [Editable(false)]
    public Guid Id { get; set; }


    #region IEditableObject
    private BaseModel? _backup;

    [MemberNotNullWhen(true, nameof(_backup))]
    public bool IsEditing { get; private set; }

    public void BeginEdit()
    {
        _backup = MemberwiseClone() as BaseModel;
        _backup!.CopyValuesFrom(this);

        IsEditing = true;
    }

    /// <exception cref="InvalidOperationException"/>
    public void CancelEdit()
    {
        if (!IsEditing)
            //TODO: localize
            throw new InvalidOperationException("Object is not in editing mode.");

        CopyValuesFrom(_backup);

        IsEditing = false;
    }

    public void EndEdit()
    {
        _backup = null;
        IsEditing = false;
    }


    /// <summary>Shallow-copies the model.</summary>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    private protected virtual void CopyValuesFrom([NotNull] BaseModel other)
    {
        ArgumentNullException.ThrowIfNull(other);

        Id = other.Id;
    }
    #endregion
}
