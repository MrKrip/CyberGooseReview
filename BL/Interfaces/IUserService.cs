using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        public void CreateUser(UserDTO user);
        public void UpdateUser(UserDTO user);
        public UserDataDTO GetUserById(int id);
        public void DeleteUser(int id);
        public bool IsUserHasveRole(string role, int id);
        public void LogIn(UserDTO user);
    }
}
