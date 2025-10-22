﻿namespace SmartAccountant.Client.Models;

public class CreditCard : AbstractCreditCard
{
    public IList<CreditCardLimit> Limits { get; private init; } = [];

    public CreditCardLimit? GetLimit(DateTimeOffset asOf)
    {
        return Limits.SingleOrDefault(l => l.Period.Overlaps(asOf));
    }
}
