using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContract.DTO;
using ServiceContract.Enums;
using ServiceContract;
using BaseProject_DatabaseBeOn.Filters;
using BaseProject_DatabaseBeOn.Filters.ResultFilters;
using BaseProject_DatabaseBeOn.Filters.ResourceFilters;

using BaseProject_DatabaseBeOn.Filters.AttributeAboutFitler;
using Microsoft.AspNetCore.Authorization;

namespace BaseProject_DatabaseBeOn.Controllers
{
    [Route("[controller]")]
    [TypeFilter(typeof(ResponeHeaderActionFilter),Arguments = new object[] {"Controller-Scope-Key", "Controller-Scope-Value",3},Order =3)]
    //[TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonAlwaysRunResultFilter))]
    public class PersonsController : Controller
    {
        //private fields
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        //Log trong controller
        private readonly ILogger<PersonsController> _logger; // Controller nao se generate log, nếu là service thì chỉ cần làm giống controlelr thôi. Vd PersonService => ILogger<PersonService> rồi inject thôi :))
        //constructor
        public PersonsController(IPersonsService personsService, ICountriesService countriesService, ILogger<PersonsController> logger)
        {
            _personsService = personsService;
            _countriesService = countriesService;
            _logger = logger;
        }
         
        //Url: persons/index
        [Route("[action]")]
        [Route("/")]
        [TypeFilter(typeof(PersonsListActionFilter),Order = 4)]
        [TypeFilter(typeof(ResponeHeaderActionFilter),Arguments = new object[] {"M-KeyFrom-Action","Custom-ValueFrom-Action",1},Order =1)] // cach truyen gia tri vao constructor cua filter
        [TypeFilter(typeof(PersonsListResultFilter))]

        [SkipFilter]
        
        //[Authorize] //Trai nguoc voi allowAnonymous
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            _logger.LogInformation("Index action method of personsControlelr");
            _logger.LogDebug($@"Searchby: {searchBy}, SearchString: {searchString}"); // logdebug lay duoc thong tin cac bien
            //Search
      //      ViewBag.SearchFields = new Dictionary<string, string>()
      //{
      //  { nameof(PersonResponse.PersonName), "Person Name" },
      //  { nameof(PersonResponse.Email), "Email" },
      //  { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
      //  { nameof(PersonResponse.Gender), "Gender" },
      //  { nameof(PersonResponse.CountryID), "Country" },
      //  { nameof(PersonResponse.Address), "Address" }
      //}; // Cung o trong Filter luon roi
            List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);
            //ViewBag.CurrentSearchBy = searchBy;
            //ViewBag.CurrentSearchString = searchString;/ 2 thang nay da duoc thuc hien o trong filter

            //Sort
            List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrder);
            //ViewBag.CurrentSortBy = sortBy;
            //ViewBag.CurrentSortOrder = sortOrder.ToString(); // 2 thang nay da duoc thuc hien o trong filter

            return View(sortedPersons); //Views/Persons/Index.cshtml
        }


        //Executes when the user clicks on "Create Person" hyperlink (while opening the create view)
        //Url: persons/create
        [Route("[action]")]
        [HttpGet]
        [TypeFilter(typeof(LearnAsyncFilter), Arguments = new object[] { "M-Custom-Key-Create", "Custom-Value-Create",2},Order =2)] // Co the resusable => cu the la su dung hco nhieu action hoac 1 action co the dung nhieu filter
        [Authorize(Roles ="Admin")] //////////// Không nhất thiết phải dùng Area, dùng thằng này đã có thể đặt điều kiện role nào có thể nào access rồi, area chỉ là giúp cho ta phân chia dễ hơn thôi
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
              new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() }
            );

            //new SelectListItem() { Text="Harsha", Value="1" }
            //<option value="1">Harsha</option>
            return View();
        }


        //Url: persons/create
       
        [Route("[action]")]
        [HttpPost]
        [TypeFilter(typeof(PersonCreatePostActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFilter),Arguments = new object[] { false})]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            //if (!ModelState.IsValid)
            //{
            //    List<CountryResponse> countries = await _countriesService.GetAllCountries();
            //    ViewBag.Countries = countries.Select(temp =>
            //    new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            //    ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            //    return View(personAddRequest);
            //} // Cái này sẽ được thực hiện ở trong Filter - PersonCreatePostActionFilter

            //call the service method
            PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);

            //navigate to Index() action method (it makes another get request to "persons/index"
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")] //Eg: /persons/edit/1
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            return View(personUpdateRequest);
        }


        [HttpPost]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                PersonResponse updatedPerson = await _personsService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
            else
            {
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = countries.Select(temp =>
                new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personResponse.ToPersonUpdateRequest());
            }
        }


        [HttpGet]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);
            if (personResponse == null)
                return RedirectToAction("Index");

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateResult)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personUpdateResult.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index");

            _personsService.DeletePerson(personUpdateResult.PersonID);
            return RedirectToAction("Index");
        }

        
     
    }
}
