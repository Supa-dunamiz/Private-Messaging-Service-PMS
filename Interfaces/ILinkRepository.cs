using PMS.Models;

namespace PMS.Interfaces
{
    public interface ILinkRepository
    {
        Task<IEnumerable<Link>> GetAllUserLinks();
        Task<Link> GetByUserId(string UserId);
        Task<Link> GetByLinkId(string linkId);
        bool Add(Link link);
        bool Update(Link link);
        bool Delete(Link link);
        bool LinkExists(string userId);
        bool Save();
    }
}
