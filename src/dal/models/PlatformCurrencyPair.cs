using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace dal.models
{
    public class PlatformCurrencyPair
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int PlatformID { get; set; }
        public Platform Platform { get; set; }

        [Required]
        public int SourceCurrencyID { get; set; }
        public Currency SourceCurrency { get; set; }

        [Required]
        public int TargetCurrencyID { get; set; }
        public Currency TargetCurrency { get; set; }
    }
}
