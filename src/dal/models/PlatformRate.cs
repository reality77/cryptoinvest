using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace dal.models
{
    public class PlatformRate
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

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        /// <summary>
        /// Rate set for the day (can be set manually) [default value = Average(Open, Close)]
        /// </summary>
        public decimal? RateSet { get; set; }

        public decimal? Low { get; set; }

        public decimal? High { get; set; }

        public decimal? Open { get; set; }

        public decimal? Close { get; set; }

        public decimal? Volume { get; set; }
    }
}
