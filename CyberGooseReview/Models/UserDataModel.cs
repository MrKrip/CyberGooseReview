using BLL.Interfaces;

namespace CyberGooseReview.Models
{
    public class UserDataModel
    {
        public string Id { get; set; }
        public string UserNick { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public int ReviewCount { get; set; }
        public DateTime? RegistationDate { get; set; }
        public byte[] ProfilePicture { get; set; } = null!;
        public UserDataModel(string id, IUserService userService, bool CriticCheck = false)
        {
            var user = userService.GetUserById(id);
            Id = user.Id;
            UserNick = user.UserNick;
            Email = user.Email;
            ProfilePicture = user.ProfilePicture;
            if (!CriticCheck)
            {
                Roles = userService.GetUserRoles(id).Result;
                RegistationDate = user.RegistationDate;
            }
        }
    }
}
