using PMS.Models;

namespace PMS.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllUsers();
        Task<AppUser> GetById(string id);
        Task<AppUser> GetByName(string UserName);
        Task<AppUser> GetByIdNoTracking(string id);
        bool Add(AppUser user);
        bool Update(AppUser user);
        bool Delete(AppUser user);
        bool Save();
    }
}
