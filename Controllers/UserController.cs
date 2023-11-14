using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PMS.Interfaces;
using PMS.Models;
using PMS.ViewModels;

namespace PMS.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPhotoService _photoService;
        private readonly IMessageRepository _messageRepository;
        private readonly ILinkRepository _linkRepository;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public UserController(IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            IPhotoService photoService,
            IMessageRepository messageRepository,
            ILinkRepository linkRepository,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _photoService = photoService;
            _messageRepository = messageRepository;
            _linkRepository = linkRepository;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        private void MapUserEdit(AppUser user, 
            EditProfileViewModel editVM, 
            ImageUploadResult photoResult)
        {
            user.Id = editVM.Id;
            user.UserName = editVM.Username;
            user.ProfileImageUrl = photoResult.Url.ToString();
        }
        public async Task<IActionResult> AnonymousUsers()
        {
            var users = await _userRepository.GetAllUsers();
            List<UserViewModel> result = new List<UserViewModel>();
            foreach (var user in users)
            {
                var userVM = new UserViewModel()
                {
                    Id = user.Id,
                    Username = user.UserName,
                    EmailAddress = user.Email,
                    ProfileImageUrl = user.ProfileImageUrl,
                };
                result.Add(userVM);
            }
            return View(result);
        }
        public async Task<IActionResult> UserProfile(string id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View("Index", "Home");
            }
            var userVM = new UserViewModel()
            {
                Id = user.Id,
                Username = user.UserName,
                EmailAddress = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
            };
            return View(userVM);
        }
        public async Task<IActionResult> Profile()
        {
            var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.GetById(curUserId);
            if(user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View("Index", "Home");
            }
            var userVM = new UserViewModel()
            {
                Id = user.Id,
                Username = user.UserName,
                EmailAddress = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
            };
            return View(userVM);
        }
        public async Task<IActionResult> EditUserProfile(string id)
        {
            var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.GetById(curUserId);
            if (user == null) return View("Error");
            var editVM = new EditProfileViewModel()
            {
                Id = curUserId,
                ProfileImageUrl = user.ProfileImageUrl,
                Username = user.UserName

            };
            return View(editVM);
        }
        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditProfileViewModel editVM)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View("EditUserProfile", editVM);
            }
            var user = await _userRepository.GetByIdNoTracking(editVM.Id);
            if(editVM.Image == null)
            {
                var userNameCheck = await _userRepository.GetByName(editVM.Username);
                if (userNameCheck != null)
                {
                    if(editVM.Username == user.UserName)
                    {
                        TempData["Error"] = "Pick a username different from the old one!";
                        return View(editVM);
                    }
                    TempData["Error"] = "Username is already in use!";
                    return View(editVM);
                }
                user.UserName = editVM.Username;
                _userRepository.Update(user);
                return RedirectToAction("Profile", "User");
            }
            if (user.ProfileImageUrl == null || user.ProfileImageUrl == "") 
            {
                if(user.UserName == editVM.Username)
                {
                    var photo = await _photoService.AddPhotoAsync(editVM.Image);
                    MapUserEdit(user, editVM, photo);
                    _userRepository.Update(user);
                    return RedirectToAction("Profile", "User");
                }
                var userNameCheck = await _userRepository.GetByName(editVM.Username);
                if (userNameCheck != null)
                {
                    TempData["Error"] = "Username is already in use!";
                    return View(editVM);
                }
                var photoResult = await _photoService.AddPhotoAsync(editVM.Image);
                MapUserEdit(user, editVM, photoResult);
                _userRepository.Update(user);
                return RedirectToAction("Profile", "User");
            }
            else
            {
                if(editVM.Username == user.UserName)
                {
                    try
                    {
                        await _photoService.DeletePhotoAsync(user.ProfileImageUrl);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Could not delete photo");
                    }
                    var photo = await _photoService.AddPhotoAsync(editVM.Image);
                    MapUserEdit(user, editVM, photo);
                    _userRepository.Update(user);
                    return RedirectToAction("Profile", "User");
                }
                var userNameCheck = await _userRepository.GetByName(editVM.Username);
                if (userNameCheck != null)
                {
                    TempData["Error"] = "UserName is already in use!";
                    return View(editVM);
                }
                try
                {
                    await _photoService.DeletePhotoAsync(user.ProfileImageUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                }
                var photoResult = await _photoService.AddPhotoAsync(editVM.Image);
                MapUserEdit(user, editVM, photoResult);
                _userRepository.Update(user);
                return RedirectToAction("Profile", "User");
            }
        }
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
            {
                return View("Error");
            }
            return View(user);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userRepository.GetByIdNoTracking(id);
            if(user == null)
            {
                return View("Error");
            }
            var userLink = await _linkRepository.GetByUserId(id);
            if (userLink != null)
            {
                _linkRepository.Delete(userLink);
            }
            var userMessages = await _messageRepository.GetMessagesOfAUser(id);
            if(userMessages.Count() == 0)
            {
                _userRepository.Delete(user);
                return RedirectToAction("AnonymousUsers", "User");
            }
            foreach(var message in userMessages)
            {
                _messageRepository.Delete(message);
            }
            _userRepository.Delete(user);
            return RedirectToAction("AnonymousUsers", "User");
        }

    }
}
