using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace dal.models
{
    public enum ETransactionType : byte
    {
        BuySell = 0,
        Transfer = 1,
        Airdrop = 2,
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
        [Display(Name = "Source account")]
        public int SourceAccountID { get; set; }

        public Account TargetAccount { get; set; }

        [Display(Name = "Target account")]
        public int? TargetAccountID { get; set; }

        /// <summary>
        /// Source gross amount
        /// </summary>
        [Required]
        [Display(Name = "Source gross amount")]
        public decimal SourceGrossAmount { get; set; }

        /// <summary>
        /// Source Fees (in source account currency)
        /// </summary>
        [Display(Name = "Source fees")]
        public decimal SourceFees { get; set; }

        /// <summary>
        /// Target net amount - fees deducted (in target account currency)
        /// </summary>
        [Required]
        [Display(Name = "Target net amount")]
        public decimal TargetNetAmount { get; set; }

        /// <summary>
        /// Target Fees (in target account currency)
        /// </summary>
        [Display(Name = "Target fees")]
        public decimal TargetFees { get; set; }

        public string Caption { get; set; }
    }
}