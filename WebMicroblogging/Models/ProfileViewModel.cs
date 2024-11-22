namespace WebMicroblogging.Models
{
    public class ProfileViewModel
    {
        public string UserName { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }
}
