namespace WebMicroblogging.Models
{
    public class EditProfileViewModel
    {
        public string UserName { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }
}
