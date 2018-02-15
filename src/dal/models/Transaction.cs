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
        Transfer = 4,
        Airdrop = 5,
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
        public int TargetAccountID { get; set; }

        /// <summary>
        /// Source net amount - without fees (in source account currency)
        /// </summary>
        [Required]
        [Display(Name = "Source net amount")]
        public decimal SourceAmount { get; set; }

        /// <summary>
        /// Source Fees (in source account currency)
        /// </summary>
        [Display(Name = "Source fees")]
        public decimal SourceFees { get; set; }

        /// <summary>
        /// Target net amount - without fees (in target account currency)
        /// </summary>
        [Required]
        [Display(Name = "Target net amount")]
        public decimal TargetAmount { get; set; }

        /// <summary>
        /// Target Fees (in target account currency)
        /// </summary>
        [Display(Name = "Target fees")]
        public decimal TargetFees { get; set; }

        public string Caption { get; set; }
    }
}