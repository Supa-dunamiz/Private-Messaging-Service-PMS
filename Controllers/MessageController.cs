using Microsoft.AspNetCore.Mvc;
using PMS.Data;
using PMS.Interfaces;
using PMS.Models;
using PMS.ViewModels;

namespace PMS.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILinkRepository _linkRepository;
        private readonly IUserRepository _userRepository;

        public MessageController(IMessageRepository messageRepository,
            IPhotoService photoService,
            IHttpContextAccessor httpContextAccessor,
            ILinkRepository linkRepository,
            IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
            _linkRepository = linkRepository;
            _userRepository = userRepository;
        }
        public async Task<IActionResult> AllReceivedMessages()
        {
            var messages = await _messageRepository.GetMessagesAsync();
            return View(messages);
        }
        public async Task<IActionResult> UserMessage()
        {
            var curUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
            var messages = await _messageRepository.GetMessagesOfAUser(curUserId); 
            return View(messages);
        }
        public async Task<IActionResult> Detail(int id) 
        {
            var message = await _messageRepository.GetMessageByIdAsync(id);
            var curUserId = _httpContextAccessor?.HttpContext?.User.GetUserId();
            var curUser = await _userRepository.GetByIdNoTracking(curUserId);
            if(message.ReceiverId != curUserId)
            {
                if (User.IsInRole("admin"))
                {
                    return View(message);
                }
                TempData["Error"] = "Message not found";
                return RedirectToAction("Index", "Home");
            }
            if (message != null)
            {
                if (message.IsOpened)
                {
                    return View(message);
                }
                message.IsOpened = true;
                _messageRepository.Update(message);
                return View(message);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<IActionResult> Send(string token) 
        {
            var link = await _linkRepository.GetByLinkId(token);
            if (link == null)
            {
                ModelState.AddModelError("", "Invalid link");
                return RedirectToAction("AllReceivedMessages", "Home");
            }
            var messageVM = new SendMessageViewModel()
            {
                ReceiverId = link.AppUserId,
                Receiver = await _userRepository.GetById(link.AppUserId),
            };
            return View(messageVM);   
        }
        [HttpPost]
        public async Task<IActionResult> Send(SendMessageViewModel messageVM)
        {
            if (ModelState.IsValid)
            {
                if (messageVM.Image == null)
                {
                    var message = new Message()
                    {
                        Content = messageVM.Content,
                        CreatedAt = DateTime.Now,
                        ReceiverId = messageVM.ReceiverId,
                        Receiver = messageVM.Receiver,
                        IsOpened = false,
                        Image = ""
                    };
                    _messageRepository.Add(message);
                    TempData["Message"] = "Message sent successfully! Now it's your turn.";
                    return RedirectToAction("Index", "Home");   
                }
                else
                {
                    var result = await _photoService.AddPhotoAsync(messageVM.Image);

                    var message = new Message()
                    {
                        Content = messageVM.Content,
                        CreatedAt = DateTime.Now,
                        ReceiverId = messageVM.ReceiverId,
                        Receiver = messageVM.Receiver,
                        Image = result.Url.ToString()
                    };
                    _messageRepository.Add(message);
                    TempData["Message"] = "Message sent successfully! Now it's your turn.";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Failed to send message.");
            }
            TempData["Message"] = "Message not sent! Now it's your turn.";
            return View(messageVM);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var messageDetails = await _messageRepository.GetMessageByIdAsync(id);
            if(messageDetails == null)
            {
                return View("Error");
            }
            return View(messageDetails);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var messageDetails = await _messageRepository.GetMessageByIdAsync(id);
            if (messageDetails == null)
            {
                return View("Error");   
            }
            _messageRepository.Delete(messageDetails);
            TempData["Message"] = "Message deleted successfully!";
            return RedirectToAction("UserMessage");
        }
    }
}
