using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace dal.models
{
    public class Platform
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Account> Accounts { get; set; }

        public List<PlatformRate> PlatformRates { get; set; }

        public List<PlatformCurrencyPair> PlatformCurrencyPairs { get; set; }

        public override string ToString()
        {
            return $"{this.Name} ({this.ID})";
        }
    }
}
