namespace ExchangesRates.Domain.Models.CurrencyService
{
    public class GetCurrencyOpts
    {
        public GetCurrencyOpts(int page, int count) {
            Page = page;
            Count = count;
        }

        public int Page { get; set; } = 1;
        public int Count { get; set; } = 10;

    }
}
