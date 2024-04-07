using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.DTO
{
    public class RegisterDTO
    {
        //Note về view. => Khi dùng model của class nào thì khi ta sử dụng thuộc tính asp-for => nếu model đó không có propterty đó thì sẽ lỗi, nghĩa là property trong asp-for phải giống với những property trong class
        [Required(ErrorMessage = "Ten khong duoc de trong")]
        public string PersonName { get; set; }


        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email should be in a proper email address format")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Phone can't be blank")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number should contain numbers only")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Password can't be blank")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessage = "Confirm Password can't be blank")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }
}
