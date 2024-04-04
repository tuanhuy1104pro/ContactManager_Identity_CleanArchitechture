using BaseProject_DatabaseBeOn.Controllers;
using Entities;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContract;
using ServiceContract.DTO;
namespace BaseProject_DatabaseBeOn.Filters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;
        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("Dang o OnActionExecuted");

            //access ViewData or Bag
            PersonsController personsController = (PersonsController)context.Controller;
            IDictionary<string,object?>? parameters = (IDictionary<string,object?>?)context.HttpContext.Items["agruments"];

            personsController.ViewBag.SearchFields = new Dictionary<string, string>()
      {
        { nameof(PersonResponse.PersonName), "Person Name" },
        { nameof(PersonResponse.Email), "Email" },
        { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
        { nameof(PersonResponse.Gender), "Gender" },
        { nameof(PersonResponse.CountryID), "Country" },
        { nameof(PersonResponse.Address), "Address" }
      };

            if (parameters != null)
            {
                if(parameters.ContainsKey("searchBy"))
                {
                    personsController.ViewData["CurrentSearchBy"] = Convert.ToString(parameters["searchBy"]); // lam vay boi vi thang nay khogn the truy cap truc tiep den ActionAgrument duoc //// Luu y khogn convert sang String la loi~ sml nhe
                }

                if(parameters.ContainsKey("searchString"))
                {
                    personsController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["searchString"]);
                }

                if (parameters.ContainsKey("sortOrder"))
                {
                    personsController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["sortOrder"]);
                }
                if (parameters.ContainsKey("sortBy"))
                {
                    personsController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["sortBy"]);
                }
            }

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["agruments"] = context.ActionArguments; // tao cai items de co the giup OnActionExecuted truy cap cac parameter duoc truyen vao action
            _logger.LogInformation("Dang o OnActionExecuting");


            if(context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);
                if(!string.IsNullOrEmpty(searchBy) )
                {
                    var searchByOoptions = new List<string>()
                    { 
                       nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                         nameof(PersonResponse.DateOfBirth),
                          nameof(PersonResponse.Gender),
                          nameof(PersonResponse.CountryID),
                           nameof(PersonResponse.Address)
                    };
                    if(searchByOoptions.Any(temp => temp == searchBy) == false)
                    {
                        //Thang SearchBy ma  trung voi mot trong nhung thang o tren thi cai nay se khong chay => chac chan se khogn roi. Vi seachBy cua Person cung chi co nhieu do
                        _logger.LogInformation($@"searchBy actual value {searchBy}");
                        context.ActionArguments["searchBy"] = nameof(Person.PersonName); // gui ve parameter gia tri 
                        _logger.LogInformation($@"searchBy update value {searchBy}");
                    }
                }
                
            }
            

           
        }
    }
}
