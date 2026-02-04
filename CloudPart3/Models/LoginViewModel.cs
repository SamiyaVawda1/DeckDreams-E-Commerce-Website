using System.ComponentModel.DataAnnotations;

namespace CloudPart3.Models
{
    public class LoginViewModel
    {
        //this class is used for login
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
