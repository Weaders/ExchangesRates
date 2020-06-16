using System;

namespace ExchangesRates.Domain.Models.CurrencyService
{
    public class BankServiceException : Exception
    {
        public BankServiceException(string msg) : base(msg) { }
    }
}
