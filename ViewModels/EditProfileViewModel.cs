namespace PMS.ViewModels
{
    public class EditProfileViewModel
    {
        public string Username { get; set; }
        public string? Id { get; set; }
        public string? ProfileImageUrl { get; set; }
        public IFormFile? Image { get; set; }
    }
}
