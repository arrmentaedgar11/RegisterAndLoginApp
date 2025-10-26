using System.ComponentModel.DataAnnotations;


namespace RegisterAndLoginApp.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;


    }
}
