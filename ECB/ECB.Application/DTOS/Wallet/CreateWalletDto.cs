using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECB.Application
    {
    public class CreateWalletDto
        {
        public decimal Balance { get; set; } 

        public string Currency { get; set; }

        public CreateWalletDto(decimal balance, string currency)
            {
            Balance = balance;
            Currency = currency;
            }
        }
    }