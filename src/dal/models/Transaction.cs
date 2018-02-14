using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace dal.models
{
    public enum ETransactionType : byte
    {
        Deposit = 0,
        Withdrawal = 1,
        Buy = 2,
        Sell = 3,
    }

    public class Transaction
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public ETransactionType Type { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public Account SourceAccount { get; set; }

        [Required]
        public int SourceAccountID { get; set; }

        public Account TargetAccount { get; set; }

        public int TargetAccountID { get; set; }

        /// <summary>
        /// Amount (in source account currency)
        /// </summary>
        [Required]
        public decimal Amount { get; set; }

        public decimal? Rate { get; set; }

        /// <summary>
        /// Source Fees (in source account currency)
        /// </summary>
        public decimal SourceFees { get; set; }

        /// <summary>
        /// Target Fees (in target account currency)
        /// </summary>
        public decimal TargetFees { get; set; }

        public string Caption { get; set; }
    }
}