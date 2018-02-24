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

        public decimal GetNetBalance() => GetNetCreditSum() - GetNetDebitSum();

        public decimal GetGrossBalance() => GetGrossCreditSum() - GetGrossDebitSum();

        public decimal GetGrossDebitSum() => GetDebitSum(includeSourceFees: true);

        public decimal GetNetDebitSum() => GetDebitSum(includeSourceFees: false);

        public decimal GetGrossCreditSum() => GetCreditSum(includeSourceFees: true, includeTargetFees: true);

        public decimal GetNetCreditSum() => GetCreditSum(includeSourceFees: false, includeTargetFees: false);

        public decimal GetDebitSum(bool includeSourceFees)
        {
            if(includeSourceFees)
                return this.SourceTransactions.Where(t => t.Type == ETransactionType.BuySell || t.Type == ETransactionType.Transfer).Sum(t => t.SourceAmount + t.SourceFees);
            else
                return this.SourceTransactions.Where(t => t.Type == ETransactionType.BuySell || t.Type == ETransactionType.Transfer).Sum(t => t.SourceAmount);
        }

        public decimal GetCreditSum(bool includeSourceFees, bool includeTargetFees)
        {
            decimal credit = 0;

            if(includeSourceFees)
                credit += this.SourceTransactions.Where(t => t.Type == ETransactionType.Airdrop).Sum(t => t.SourceAmount + t.SourceFees);
            else
                credit += this.SourceTransactions.Where(t => t.Type == ETransactionType.Airdrop).Sum(t => t.SourceAmount);

            if(includeTargetFees)
                credit += this.TargetTransactions.Where(t => t.Type == ETransactionType.BuySell || t.Type == ETransactionType.Transfer).Sum(t => t.TargetAmount + t.TargetFees);
            else
                credit += this.TargetTransactions.Where(t => t.Type == ETransactionType.BuySell || t.Type == ETransactionType.Transfer).Sum(t => t.TargetAmount);

            return credit;
        }

        public string GetAmountString(decimal amount, bool includeCurrency = true)
        {
            decimal amountRounded;

            if (this.Currency.RoundToDecimals != null)
                amountRounded = Math.Round(amount, this.Currency.RoundToDecimals.Value);
            else
                amountRounded = amount;

            if (includeCurrency)
                return amountRounded.ToString() + " " + this.Currency.GetCurrencySymbol();
            else
                return amountRounded.ToString();
        }
    }
}
