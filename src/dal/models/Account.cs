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

        [Required]
        public int CurrencyID { get; set; }
        public Currency Currency { get; set; }

        public int? PlatformID { get; set; }
        public Platform Platform { get; set; }

        [Required]
        public string Name { get; set; }

        public string Caption { get; set; }

        public List<Transaction> SourceTransactions { get; set; }

        public List<Transaction> TargetTransactions { get; set; }

        public decimal GetDebitSum(bool netAmount = true)
        {
            if(netAmount)
                return this.SourceTransactions.Where(t => t.Type == ETransactionType.BuySell || t.Type == ETransactionType.Transfer).Sum(t => t.SourceGrossAmount - t.SourceFees);
            else
                return this.SourceTransactions.Where(t => t.Type == ETransactionType.BuySell || t.Type == ETransactionType.Transfer).Sum(t => t.SourceGrossAmount);
        }

        public decimal GetCreditSum(bool netAmount = true)
        {
            decimal credit = 0;

            if(netAmount)
                credit += this.SourceTransactions.Where(t => t.Type == ETransactionType.Airdrop).Sum(t => t.SourceGrossAmount - t.SourceFees);
            else
                credit += this.SourceTransactions.Where(t => t.Type == ETransactionType.Airdrop).Sum(t => t.SourceGrossAmount);

            if(netAmount)
                credit += this.TargetTransactions.Where(t => t.Type == ETransactionType.BuySell || t.Type == ETransactionType.Transfer).Sum(t => t.TargetNetAmount);
            else
                credit += this.TargetTransactions.Where(t => t.Type == ETransactionType.BuySell || t.Type == ETransactionType.Transfer).Sum(t => t.TargetNetAmount + t.TargetFees);

            return credit;
        }

        public decimal GetBalance(bool netAmount = false) => GetCreditSum(netAmount) - GetDebitSum(netAmount);

        public override string ToString()
        {
            return $"{this.Name} ({this.ID})";
        }
    }
}
