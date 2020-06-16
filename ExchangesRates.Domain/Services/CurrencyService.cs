using ExchangesRates.Domain.Models.CurrencyService;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExchangesRates.Domain.Services
{
    public interface ICurrencyService
    {
        Task<IEnumerable<CurrencyData>> GetCurrenciesAsync(GetCurrencyOpts opts = null);
        Task<CurrencyData> GetCurrencyAsync(string currencyId);
    }

    public class CurrencyService : ICurrencyService
    {
        private readonly IMemoryCache _memCache;
        private readonly IHttpClientFactory _httpFactory;

        private const string BANK_GET_CURRENCIES_URL = "https://www.cbr-xml-daily.ru/daily_json.js";

        private const string MEMCACHE_CURRENCIES_KEY = "currencies";

        public CurrencyService(IHttpClientFactory httpFactory, IMemoryCache memCache)
        {
            _httpFactory = httpFactory;
            _memCache = memCache;
        }

        public async Task<CurrencyData> GetCurrencyAsync(string currencyId)
            => (await GetCurrenciesAsync()).FirstOrDefault(x => string.Equals(x.ID, currencyId, StringComparison.OrdinalIgnoreCase));

        public async Task<IEnumerable<CurrencyData>> GetCurrenciesAsync(GetCurrencyOpts opts = null)
        {

            if (!_memCache.TryGetValue(MEMCACHE_CURRENCIES_KEY, out IEnumerable<CurrencyData> currencies))
            {
                try
                {

                    using var client = _httpFactory.CreateClient();

                    var data = await client.GetAsync(BANK_GET_CURRENCIES_URL);

                    var resultStr = await data.Content.ReadAsStringAsync();

                    currencies = JsonConvert.DeserializeObject<BankDailyResult>(resultStr).Valute.Values;

                }
                catch (JsonException)
                {
                    throw new Exception("Error while deserialize data, bank maybe change json format");
                }
                catch (HttpRequestException)
                {
                    throw new BankServiceException("Error while make request to bank, maybe he go offline");
                }

                DateTime dateForRefresh = new DateTime(
                    DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 30, 00
                );

                if (DateTime.Now.Hour > 11 || (DateTime.Now.Hour == 11 && DateTime.Now.Minute > 30))
                {
                    dateForRefresh = dateForRefresh.AddDays(1);
                }

                // ЦБ РФ определяет курс в 11:30, так что можем смело кешировать, до этого времени
                _memCache.Set(MEMCACHE_CURRENCIES_KEY, currencies, dateForRefresh);

            }

            if (opts != null)
            {
                currencies = currencies.Skip((opts.Page - 1) * opts.Count).Take(opts.Count);
            }

            return currencies;

        }

    }

}
