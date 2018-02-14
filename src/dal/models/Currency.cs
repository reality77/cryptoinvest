using System;
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

        public string CssImagePath { get; set; }

        [Required]
        public bool IsFiat { get; set; }
    }
}
