using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Models
{
    public class TransactionsListModel
    {
        public dal.models.Transaction TransactionToCreate { get; set; }

        public IEnumerable<dal.models.Transaction> Transactions { get; set; }
    }
}
