namespace CyberGooseReview.Models
{
    public class UserRolesModel
    {
        public string Id { get; set; }
        public string UserNick { get; set; }
        public string Email { get; set; }

        public DateTime? RegistationDate { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
