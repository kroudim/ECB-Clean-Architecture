using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECB.Infrastructure.Application
    {
    /// <summary>
    /// POCO class for configuring the ECB rates synchronization options.
    /// </summary>
    public class EcbRatesSyncOptions
        {
        public string ECBRatesConnectionString { get; set; }
        public double ECBRatesInterval { get; set; }
        public string EcbUrl { get; set; }
        public string Redis { get; set; }
        }
    }
