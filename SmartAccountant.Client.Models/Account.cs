using System.ComponentModel.DataAnnotations;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Client.Models
{
    public abstract class Account : BaseModel
    {
        public Guid HolderId { get; init; }

        public Bank Bank { get; init; }

        [StringLength(50)] //TODO: localize
        public string? FriendlyName { get; init; }

        public abstract BalanceType NormalBalance { get; }
    }
}
