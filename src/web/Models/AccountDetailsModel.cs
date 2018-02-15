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

    }
}
