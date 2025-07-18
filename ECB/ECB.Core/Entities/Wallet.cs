using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECB.Domain
  {
    public class Wallet
        {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public decimal Balance { get; set; }

        [Required]
        public string Currency { get; set; }

        public Wallet(long id, string currency, decimal balance)
            {
            Id = id;
            Currency = currency;
            Balance = balance;
            }

        public Wallet()
            {

            }
        }
    }
