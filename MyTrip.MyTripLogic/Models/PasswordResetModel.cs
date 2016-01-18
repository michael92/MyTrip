using System.ComponentModel.DataAnnotations;

namespace MyTrip.MyTripLogic.Models
{
    public class PasswordResetModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Token { get; set; }
    }
}