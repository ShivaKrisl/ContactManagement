using Entities_DTO;
using Microsoft.AspNetCore.Mvc;
using Service_Contracts;

namespace ContactManagement.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesAdderService _countriesAdderService;

        public CountriesController(ICountriesAdderService countriesService)
        {
            _countriesAdderService = countriesService;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Add(CountryRequest countryRequest)
        {
            if (!ModelState.IsValid)
            {
                List<string> errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
                ViewBag.errors = errors;
                return RedirectToAction("Add", "Countries");   
            }

            CountryResponse countryResponse = await _countriesAdderService.AddCountry(countryRequest);

            return RedirectToAction("Index", "Persons");
        }

    }
}
