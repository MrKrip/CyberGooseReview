namespace BLL.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string UserNick { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public DateTime? RegistationDate { get; set; }
        public string Tag { get; set; }
        public byte[] ProfilePicture { get; set; } = null!;
    }
}
