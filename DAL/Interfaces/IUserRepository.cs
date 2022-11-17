using Microsoft.AspNetCore.Identity;

namespace DAL.Interfaces
{
    public interface IUserRepository<T, S> where T : class where S : class
    {
        IEnumerable<S> GetAll();
        S Get(string id);
        IEnumerable<S> Find(Func<T, bool> predicate);
        Task<IdentityResult> Create(T item, UserManager<T> userManager);
        void Update(T item);
        void Delete(string id);
        Task LogIn(T item, SignInManager<T> signInManager);
    }
}
