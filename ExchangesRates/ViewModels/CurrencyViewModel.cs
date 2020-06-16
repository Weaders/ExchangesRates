using ExchangesRates.Domain.Models.CurrencyService;

namespace ExchangesRates.ViewModels
{
    public class CurrencyViewModel
    {

        public CurrencyViewModel(CurrencyData currencyData) {

            Id = currencyData.ID;
            Value = currencyData.Value;
            Name = currencyData.Name;
            Code = currencyData.CharCode;

        }

        public string Id { get; set; }
        public float Value { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
