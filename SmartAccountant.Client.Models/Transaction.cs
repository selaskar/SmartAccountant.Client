using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using SmartAccountant.Client.Models.Resources;

namespace SmartAccountant.Client.Models;

public partial class Transaction : BaseModel
{
    [StringLength(100)]
    [Display(Name = nameof(Strings.Transaction_ReferenceNumber), Description = nameof(Strings.Transaction_ReferenceNumber_Description), ResourceType = typeof(Strings))]
    public string? ReferenceNumber
    {
        get;
        set => SetProperty(ref field, value, validate: true);
    }

    private protected override void CopyValuesFrom([NotNull] BaseModel other)
    {
        base.CopyValuesFrom(other);

        if (other is not Transaction transaction)
            //TODO: localize
            throw new ArgumentException($"{other.GetType().Name} was expected to be a {typeof(Transaction).Name}");

        //AccountId = transaction.AccountId;
        //Account = transaction.Account;
        ReferenceNumber = transaction.ReferenceNumber;
        //Timestamp = transaction.Timestamp;
        //Amount = transaction.Amount;
        //Description = transaction.Description;
        //PersonalNote = transaction.PersonalNote;
        //Category = transaction.Category;
    }
}
