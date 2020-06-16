using ExchangesRates.Domain.Models.CurrencyService;
using ExchangesRates.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
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

                var result = await _currencyService.GetCurrenciesAsync(opts);
                return Ok(result);

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
                var result = await _currencyService.GetCurrencyAsync(currencyId);

                if (result == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(result);
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
