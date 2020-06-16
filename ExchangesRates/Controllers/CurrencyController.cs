using ExchangesRates.Domain.Models.CurrencyService;
using ExchangesRates.Domain.Services;
using ExchangesRates.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ExchangesRates.Filters.HttpExceptionFilter;

namespace ExchangesRates.Controllers
{
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [Route("/currencies/{page?}")]
        [HttpGet]
        public async Task<IActionResult> AllCurrencies(int? page, [FromQuery] int? count)
        {

            try
            {
                GetCurrencyOpts opts = new GetCurrencyOpts(page ?? 1, count ?? 5);

                var currencies = await _currencyService.GetCurrenciesAsync(opts);

                var viewModels = currencies.Select(x => new CurrencyViewModel(x));

                return Ok(viewModels);

            }
            catch (Exception e)
            {
                return ProcessError(e);
            }

        }

        [Route("/currency/{currencyId}")]
        [HttpGet]
        public async Task<IActionResult> GetCurreny(string currencyId)
        {
            try
            {
                var currencyData = await _currencyService.GetCurrencyAsync(currencyId);

                if (currencyData == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(new CurrencyViewModel(currencyData));
                }

            }
            catch (Exception e)
            {
                return ProcessError(e);
            }

        }

        private IActionResult ProcessError(Exception e)
        {

            if (e is BankServiceException)
            {
                throw new HttpResponseException("Error on bank side") { StatusCode = 503 };
            }
            else
            {
                throw new HttpResponseException("All is down, faster call programmers!");
            }

        }

    }
}
