using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Interfaces;
using PMS.Models;

namespace PMS.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }
        public bool Add(Message message)
        {
            _context.Messages.Add(message);
            return Save();
        }

        public bool Delete(Message message)
        {
            _context.Messages.Remove(message);
            return Save();
        }

        public async Task<Message> GetMessageByIdAsync(int id)
        {
            return await _context.Messages.Where(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync()
        {
            return await _context.Messages.OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesOfAUser(string id)
        {
            return await _context.Messages.Where(u => u.ReceiverId == id).
                OrderByDescending(d => d.CreatedAt).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Message message)
        {
            _context.Messages.Update(message);
            return Save();
        }
    }
}
