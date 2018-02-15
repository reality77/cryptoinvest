using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace dal.models
{
    public class Account
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int UserID { get; set; }

        public Currency Currency { get; set; }

        [Required]
        public int CurrencyID { get; set; }

        [Required]
        public string Name { get; set; }

        public string Caption { get; set; }

        public List<Transaction> SourceTransactions { get; set; }

        public List<Transaction> TargetTransactions { get; set; }

        public decimal GetBalance()
        {
            return GetCreditSum() - GetDebitSum();
        }

        public decimal GetDebitSum()
        {
            return this.SourceTransactions.Where(t => t.Type == ETransactionType.BuySell || t.Type == ETransactionType.Transfer).Sum(t => t.SourceAmount + t.SourceFees);
        }

        public decimal GetCreditSum()
        {
            var credit = this.SourceTransactions.Where(t => t.Type == ETransactionType.Airdrop).Sum(t => t.SourceAmount + t.SourceFees);
            credit += this.TargetTransactions.Where(t => t.Type == ETransactionType.BuySell || t.Type == ETransactionType.Transfer).Sum(t => t.TargetAmount + t.TargetFees);

            return credit;
        }
    }
}
