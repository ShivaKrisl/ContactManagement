using Entities_DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service_Contracts;

namespace ContactManagement.Controllers
{
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        private readonly ICountriesGetterService _countriesGetterService;

        private readonly IPersonsAdderService _personsAdderService;

        private readonly IPersonsDeleterService _personsDeleteService;

        private readonly IPersonsGetterService _personsGetterService;

        private readonly IPersonsSorterService _personsSorterService;

        private readonly IPersonsUpdaterService _personsUpdaterService;

        // logger for PersonsController logging
        private readonly ILogger<PersonsController> _logger;   

        public PersonsController(ICountriesGetterService countriesService, ILogger<PersonsController> logger, IPersonsDeleterService personsDeleterService, IPersonsAdderService personsAdderService, IPersonsGetterService personsGetterService, IPersonsSorterService personsSorterService, IPersonsUpdaterService personsUpdaterService)
        {
            _countriesGetterService = countriesService;
            _logger = logger;
            _personsAdderService = personsAdderService;
            _personsDeleteService = personsDeleterService;
            _personsGetterService = personsGetterService;
            _personsSorterService = personsSorterService;
            _personsUpdaterService = personsUpdaterService;
        }

        [Route("[action]")]
        [Route("/")]
        public async Task<IActionResult> Index(string? searchBy, string? searchString, string sortBy = nameof(PersonRequest.FirstName), SortOrderEnum sortOrder = SortOrderEnum.ASCENDING)
        {

            _logger.LogInformation("PersonsController: Index action called");

            _logger.LogDebug($"SearchBy: {searchBy}, SearchString: {searchString}, SortBy: {sortBy}, SortOrder: {sortOrder}");



            List<PersonResponse>? personResponses = await _personsGetterService.GetFilteredPersons(searchBy, searchString);

            List<PersonResponse>? sortedPersonResponses = await _personsSorterService.SortPersons(personResponses, sortBy, sortOrder);

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
            List<CountryResponse>? countryResponses = await _countriesGetterService.GetAllCountries();
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
                List<CountryResponse>? countryResponses = await _countriesGetterService.GetAllCountries();
                ViewBag.countries = countryResponses;

                ViewBag.errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

                return View(viewName : "Create");
            }
            PersonResponse personResponse = await _personsAdderService.AddPerson(personRequest);
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]/{personId:Guid}")] 
        [HttpGet]
        public async Task<IActionResult> Edit(Guid personId)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonById(personId);
            if(personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            List<CountryResponse>? countryResponses = await _countriesGetterService.GetAllCountries();
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
                List<CountryResponse>? countryResponses = await _countriesGetterService.GetAllCountries();

                ViewBag.countries = countryResponses;
                ViewBag.personId = personId;

                ViewBag.errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }

            PersonResponse? personResponse = await _personsUpdaterService.UpdatePerson(personId, personRequest);
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]/{personId:Guid}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid personId)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonById(personId);
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
            bool isDeleted = await _personsDeleteService.DeletePerson(personId);
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
