using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.helpers
{
    [HtmlTargetElement("trxamount")]
    public class TrxAmountTagHelper : TagHelper
    {
        public bool IsGross { get; set; }
        public bool IsSource { get; set; }

        public dal.models.Currency DefaultCurrency { get; set; }
        public dal.models.Transaction Transaction { get; set; }
        public dal.CryptoInvestContext Context { get; set; }

        public TrxAmountTagHelper(dal.CryptoInvestContext context)
        {
            this.Context = context;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string tooltip = null;
            decimal value;
            dal.models.Account account;

            string valueString;
            string currencyString;
            
            if (this.IsSource)
            {
                account = this.Transaction.SourceAccount;

                if (this.IsGross)
                    value = this.Transaction.SourceGrossAmount;
                else
                    value = this.Transaction.SourceGrossAmount - this.Transaction.SourceFees;

                valueString = account.Currency.GetAmountString(value, includeCurrency: false);
            }
            else
            {
                account = this.Transaction.TargetAccount;

                if (this.IsGross)
                    value = this.Transaction.TargetNetAmount + this.Transaction.TargetNetAmount;
                else
                    value = this.Transaction.TargetNetAmount;

                valueString = account.Currency.GetAmountString(value, includeCurrency: false);
            }

            currencyString = account.Currency.GetCurrencySymbol();

            if (!account.Currency.IsFiat)
            {
                if (this.Transaction.Type == dal.models.ETransactionType.BuySell && (this.Transaction.SourceAccount.Currency.IsFiat || this.Transaction.TargetAccount.Currency.IsFiat))
                {
                    //TODO : Use BuySell rate in transaction
                    tooltip = "TODO : Use BuySell rate in transaction";
                }
                else
                {
                    // Looking for the transaction day rate
                    var rate = this.Context.PlatformRates
                        .Where(pr => pr.PlatformID == account.PlatformID)
                        .Where(pr => pr.Date == Transaction.Date.Date)
                        .Where(pr => (pr.SourceCurrencyID == account.CurrencyID && pr.TargetCurrencyID == this.DefaultCurrency.ID) || (pr.SourceCurrencyID == this.DefaultCurrency.ID && pr.TargetCurrencyID == account.CurrencyID))
                        .SingleOrDefault();

                    var rateValue = rate?.GetDayRate();

                    if (rateValue != null)
                    {
                        decimal convertedValue;

                        if (rate.SourceCurrencyID == this.DefaultCurrency.ID)
                            convertedValue = value / rateValue.Value;
                        else
                            convertedValue = value * rateValue.Value;

                        tooltip = $"= {this.DefaultCurrency.GetAmountString(convertedValue)}  (@ {rateValue} {rate.SourceCurrency.GetCurrencySymbol()} / {rate.TargetCurrency.GetCurrencySymbol()})";
                    }
                    else
                        tooltip = "No rate value";
                }
            }

            output.TagName = "span";
            if (!string.IsNullOrEmpty(tooltip))
            {
                output.Attributes.Add("title", tooltip);
                output.Attributes.Add("data-toggle", "tooltip");
            }

            output.Content.SetHtmlContent($@"<span class=""amount value"">{valueString}</span> 
    <span class=""amount currency"">{currencyString}</span>");

            output.Attributes.Add("id", context.UniqueId);
        }
    }
}
