using Entities_DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service_Contracts;

namespace ContactManagement.Controllers
{
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        private readonly ICountriesService _countriesService;
        private readonly IPersonsService _personsService;

        public PersonsController(ICountriesService countriesService, IPersonsService personsService)
        {
            _countriesService = countriesService;
            _personsService = personsService;
        }

        [Route("[action]")]
        [Route("/")]
        public async Task<IActionResult> Index(string? searchBy, string? searchString, string sortBy = nameof(PersonRequest.FirstName), SortOrderEnum sortOrder = SortOrderEnum.ASCENDING)
        {
            List<PersonResponse>? personResponses = await _personsService.GetFilteredPersons(searchBy, searchString);

            List<PersonResponse>? sortedPersonResponses = await _personsService.SortPersons(personResponses, sortBy, sortOrder);

            ViewBag.searchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.FirstName), "First Name" },
                {nameof(PersonResponse.LastName), "Last Name" },
                {nameof(PersonResponse.Email), "Email Id" },
                {nameof(PersonResponse.PhoneNumber), "Mobile Number" }
            };

            ViewBag.currentSearchBy = searchBy;
            ViewBag.currentSearchString = searchString; // to persist the search field with last searched one
            ViewBag.currentSortBy = sortBy;
            ViewBag.currentSortOrder = sortOrder;
            return View(viewName:"Index",model:sortedPersonResponses);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse>? countryResponses = await _countriesService.GetAllCountries();
            ViewBag.countries = countryResponses?.Select(c => new SelectListItem()
            {
                Text = c.CountryName,
                Value = c.CountryId.ToString()
            });
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Create(PersonRequest personRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse>? countryResponses = await _countriesService.GetAllCountries();
                ViewBag.countries = countryResponses;

                ViewBag.errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

                return View(viewName : "Create");
            }
            PersonResponse personResponse = await _personsService.AddPerson(personRequest);
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]/{personId:Guid}")] 
        [HttpGet]
        public async Task<IActionResult> Edit(Guid personId)
        {
            PersonResponse? personResponse = await _personsService.GetPersonById(personId);
            if(personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            List<CountryResponse>? countryResponses = await _countriesService.GetAllCountries();
            ViewBag.countries = countryResponses?.Select(c => new SelectListItem()
            {
                Text = c.CountryName,
                Value = c.CountryId.ToString()
            });
            ViewBag.personId = personId;    

            PersonRequest personRequest = personResponse.ToPersonRequest();
            return View(personRequest);
        }

        [HttpPost]
        [Route("[action]/{personId:Guid}")]
        public async Task<IActionResult> Edit(Guid personId, PersonRequest personRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse>? countryResponses = await _countriesService.GetAllCountries();

                ViewBag.countries = countryResponses;
                ViewBag.personId = personId;

                ViewBag.errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }

            PersonResponse? personResponse = await _personsService.UpdatePerson(personId, personRequest);
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]/{personId:Guid}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid personId)
        {
            PersonResponse? personResponse = await _personsService.GetPersonById(personId);
            if (personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personId:Guid}")]
        public async Task<IActionResult> DeleteConfirm(Guid personId)
        {
            bool isDeleted = await _personsService.DeletePerson(personId);
            if (isDeleted) {
                TempData["Success"] = "Person Deleted Successfully";
            }
            else
            {
                TempData["Error"] = "Person Not Found";
            }
            return RedirectToAction("Index", "Persons");    
        }
    }
}
