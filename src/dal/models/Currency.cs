using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace dal.models
{
    public class Currency
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Acronym { get; set; }

        public string CurrencySymbol { get; set; }

        [Required]
        public bool IsFiat { get; set; }

        public List<Account> Accounts { get; set; }

        public int? RoundToDecimals { get; set; }

        public List<PlatformRate> SourcePlatformRates { get; set; }
        public List<PlatformRate> TargetPlatformRates { get; set; }

        public List<PlatformCurrencyPair> SourcePlatformCurrencyPairs { get; set; }
        public List<PlatformCurrencyPair> TargetPlatformCurrencyPairs { get; set; }

        public string GetCurrencySymbol()
        {
            if (!string.IsNullOrEmpty(this.CurrencySymbol))
                return this.CurrencySymbol;
            else
                return this.Acronym;
        }

        public override string ToString()
        {
            return $"{this.Acronym} ({this.ID})";
        }

        public string GetAmountString(decimal amount, bool includeCurrency = true)
        {
            decimal amountRounded;

            if (this.RoundToDecimals != null)
                amountRounded = Math.Round(amount, this.RoundToDecimals.Value);
            else
                amountRounded = amount;

            if (includeCurrency)
                return amountRounded.ToString() + " " + this.GetCurrencySymbol();
            else
                return amountRounded.ToString();
        }

    }
}
