using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECB.Domain
{
    public class CurrencyRate
        {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        public decimal Rate { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public CurrencyRate(string currency, decimal rate, DateTime date)
            {
            Currency = currency;
            Rate = rate;
            Date = date;
            }
        }
     }
