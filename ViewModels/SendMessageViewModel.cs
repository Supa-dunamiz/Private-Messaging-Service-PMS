using PMS.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.ViewModels
{
    public class SendMessageViewModel
    {
        public int? Id { get; set; }
        public string? Content { get; set; }
        public IFormFile? Image { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public AppUser? Receiver { get; set; }
    }
}
