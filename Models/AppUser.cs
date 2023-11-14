using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PMS.Models
{
    public class AppUser : IdentityUser
    {
        public string? ProfileImageUrl { get; set; }
        ICollection<Message>? Messages { get; set; }
    }
}
