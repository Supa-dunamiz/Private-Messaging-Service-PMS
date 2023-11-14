using PMS.Models;

namespace PMS.Interfaces
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetMessagesAsync();
        Task<Message> GetMessageByIdAsync(int id);
        Task<IEnumerable<Message>> GetMessagesOfAUser(string id);
        bool Add(Message message);
        bool Update(Message message);
        bool Delete(Message message);
        bool Save();
    }
}
