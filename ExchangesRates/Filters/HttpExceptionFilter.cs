using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace ExchangesRates.Filters
{
    public class HttpExceptionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is HttpResponseException exception)
            {
                context.Result = new JsonResult(exception.Message) { 
                    StatusCode = exception.StatusCode
                };
                context.ExceptionHandled = true;
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public class HttpResponseException : Exception
        {
            public HttpResponseException(string msg) : base(msg) { }

            public int StatusCode { get; set; } = 500;
        }
    }
}
