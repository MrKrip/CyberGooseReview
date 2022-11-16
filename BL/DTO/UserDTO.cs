namespace BLL.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public DateTime? RegistationDate { get; set; }
        public string Tag { get; set; }
        public byte[] ProfilePicture { get; set; } = null!;
    }
}
