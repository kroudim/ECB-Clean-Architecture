using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECB.Application
    {
    public class CurrencyRateDto
        {
        public string Currency { get; set; }
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }

        public CurrencyRateDto(string currency, decimal rate, DateTime date)
            {
            Currency = currency;
            Rate = rate;
            Date = date;
            }
        }
    }
