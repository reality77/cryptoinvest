using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    }
}
