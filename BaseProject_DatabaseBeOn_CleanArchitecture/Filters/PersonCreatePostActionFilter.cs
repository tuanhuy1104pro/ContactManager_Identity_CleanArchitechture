using BaseProject_DatabaseBeOn.Controllers;
using BaseProjectServices;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContract;
using ServiceContract.DTO;

namespace BaseProject_DatabaseBeOn.Filters
{
    public class PersonCreatePostActionFilter : IAsyncActionFilter
    {
        //Đối với edit tương tự
        private readonly ICountriesService _countriesService;
        public PersonCreatePostActionFilter(ICountriesService coutrysv)
        {
            _countriesService = coutrysv;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            
            if(context.Controller is PersonsController personsController)
            {
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> countries = await _countriesService.GetAllCountries();
                   personsController.ViewBag.Countries = countries.Select(temp =>
                    new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

                    personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    context.Result = personsController.View(context.ActionArguments["personAddRequest"]); //View này sẽ được thực thi và gửi tới browser, lúc này action method tương ứng sẽ bị skip (Short-circuits) và sẽ skip đi hết thảy các filter còn lại luôn bởi vì nó đã thông tin về cho browser rồi
                }
                else
                {
                    //Tương tự cái ở dưới ngoài if else
                    await next(); // Nếu Valid thì sẽ tới filter kế tiếp và vẫn thực hiện Action ở trong controller
                }
            }

                    //ActionExcuting scope => giả sử nếu có dòng dưới
               // await next(); //calls the subsequent filter or action method
               
                    /// ActionExcuted scope
        
        }
    }
}
