using System.ComponentModel.DataAnnotations;

namespace API.DTOS
{
    public class loginDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}