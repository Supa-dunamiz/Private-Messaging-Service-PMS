using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string? Content { get; set; }
        public string? Image { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsOpened { get; set; } 
        [ForeignKey("AppUser")]
        public string? SenderId { get; set; }
        public AppUser? Sender { get; set; }
        [ForeignKey("AppUser")]
        public string? ReceiverId { get; set; }
        public AppUser? Receiver { get; set;}
    }
}
