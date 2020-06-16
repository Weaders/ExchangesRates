using System;
using System.Collections.Generic;

namespace ExchangesRates.Domain.Models.CurrencyService
{
    internal class BankDailyResult
    {
        public DateTime Date { get; set; }
        public DateTime PreviousDate { get; set; }
        public string PreviousURL { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, CurrencyData> Valute { get; set; }

    }
}
