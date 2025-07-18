using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECB.Application
    {
    public class WalletDto
        {
        public long Id { get; set; }
        public decimal Balance { get; set; } 

        public string Currency { get; set; }

        public WalletDto(long id, decimal balance, string currency)
            {
            Id = id;
            Balance = balance;
            Currency = currency;
            }
        }
    }