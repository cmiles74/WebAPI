using System;
using Microsoft.AspNetCore.Mvc;
using Nervestaple.WebApi.Exceptions;

namespace Nervestaple.WebApi.Controllers {

    /// <summary>
    /// Provides a base controller all other controllers may extend.
    /// </summary>
    [ApiController]
    public class AbstractController : ControllerBase {

        /// <summary>
        /// wraps the provides lambda in some simple exception handling in 
        /// order to return sensible HTTP status codes
        /// </summary>
        protected IActionResult WrapExceptionHandler(Func<IActionResult> lambda) {
            
            try {
                return lambda.Invoke();
            } catch (ParseException exception) {
                Response.StatusCode = 400;
                return Content(exception.Message);
            } catch (Exception exception) {
                Response.StatusCode = 500;
                return Content(exception.Message);
            }
        }
    }
}