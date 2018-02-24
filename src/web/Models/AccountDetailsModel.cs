using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Models
{
    public enum EDirection
    {
        To = 0,
        From = 1,
    }

    public class AccountDetailsModel
    {
        public decimal TotalNetDebit { get; set; }
        public decimal TotalNetCredit { get; set; }

        public decimal TotalGrossDebit { get; set; }
        public decimal TotalGrossCredit { get; set; }

        public decimal GrossBalance { get; set; }

        public dal.models.Account Account { get; set; }
        public IEnumerable<TransactionWithDirection> AllTransactions { get; set; }
    }

    public class TransactionWithDirection
    {
        public string CurrencyAcronym { get; set; }
        public EDirection Direction { get; set; }
        public dal.models.Account OtherAccount { get; set; }
        public string OtherCurrencyAcronym { get; set; }
        public dal.models.Transaction Transaction { get; set; }

        public string GetCssClass()
        {
            if (this.Transaction.Type == dal.models.ETransactionType.Airdrop)
                return "success";

            if (this.Direction == EDirection.From)
                return "success";
            else
                return "danger";
        }
    }
}
