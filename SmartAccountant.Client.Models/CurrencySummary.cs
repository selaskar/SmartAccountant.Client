using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Client.Models
{
    public class CurrencySummary
    {
        public MonetaryValue RemainingBalancesTotal { get; set; }

        public MonetaryValue OriginalLimitsTotal { get; set; }

        public MonetaryValue IncomeTotal { get; set; }

        public MonetaryValue ExpensesTotal { get; set; }

        public MonetaryValue InterestAndFeesTotal { get; set; }

        public MonetaryValue PlannedExpensesTotal { get; set; }

        public MonetaryValue LoansTotal { get; set; }

        public MonetaryValue SavingsTotal { get; set; }

        public MonetaryValue Net { get; set; }

        public IDictionary<ExpenseSubCategories, MonetaryValue> ExpensesBreakdown { get; } = new Dictionary<ExpenseSubCategories, MonetaryValue>();
    }
}