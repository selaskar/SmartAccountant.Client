using System.ComponentModel.DataAnnotations;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Client.Core.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class MonetaryValueAttribute(double minimum, double maximum) : RangeAttribute(minimum, maximum)
{
    public override bool IsValid(object? value)
    {
        if (value is not MonetaryValue monetaryValue)
            return false;

        return base.IsValid(monetaryValue.Amount);
    }
}
