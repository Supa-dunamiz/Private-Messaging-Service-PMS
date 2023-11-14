using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Interfaces;
using PMS.Models;

namespace PMS.Repository
{
    public class LinkRepository : ILinkRepository
    {
        private readonly AppDbContext _context;

        public LinkRepository(AppDbContext context)
        {
            _context = context;
        }
        public bool Add(Link link)
        {
            _context.Links.Add(link);
            return Save();
        }

        public bool Delete(Link link)
        {
            _context.Links.Remove(link);
            return Save();
        }

        public async Task<IEnumerable<Link>> GetAllUserLinks()
        {
            return await _context.Links.ToListAsync();
        }

        public async Task<Link> GetByLinkId(string linkId)
        {
            return await _context.Links.Where(u => u.LinkId == linkId).FirstOrDefaultAsync();
        }

        public async Task<Link> GetByUserId(string userId)
        {
            return await _context.Links.Where(u => u.AppUserId == userId).FirstOrDefaultAsync();
        }

        public bool LinkExists(string userId)
        {
            var linkCheck = _context.Links.Where(u => u.AppUserId == userId).FirstOrDefault();
            if(linkCheck != null)
            {
                return true;
            }
            return false;
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Link link)
        {
            _context.Links.Update(link);
            return Save();
        }
    }
}
