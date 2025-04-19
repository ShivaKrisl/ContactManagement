using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ContactManagement.Controllers
{
    public class HomeController : Controller
    {
        [Route("Error")]
        public IActionResult Error()
        {
            IExceptionHandlerPathFeature? exceptionHandlerPathFeature =  HttpContext.Features.Get<IExceptionHandlerPathFeature>(); // get the error message 

            if(exceptionHandlerPathFeature != null && exceptionHandlerPathFeature.Error != null)
            {
                ViewBag.errorMessage = exceptionHandlerPathFeature?.Error.Message;
            }

            return View();
        }
    }
}
