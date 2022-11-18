namespace BLL.DTO
{
    public class UserDataDTO
    {
        public string Id { get; set; }
        public string UserNick { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Salt { get; set; }
        public string Tag { get; set; }
        public DateTime? RegistationDate { get; set; }
        public byte[] ProfilePicture { get; set; } = null!;
    }
}
