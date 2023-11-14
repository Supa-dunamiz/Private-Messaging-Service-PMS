using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PMS.Data;
using PMS.Interfaces;
using PMS.Models;
using PMS.ViewModels;

namespace PMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IUserRepository userRepository,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _context = context;
        }
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);
            var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                TempData["Error"] = "Wrong Credentials. Password Incorrect!";
                return View(loginVM);
            }
            TempData["Error"] = "Wrong Credentials. User not Found!";
            return View(loginVM);
        }
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);
            var user = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "This Email address is already in use!";
                return View(registerVM);
            }

            var userNameCheck = await _userRepository.GetByName(registerVM.UserName);
            if (userNameCheck != null)
            {
                TempData["Error"] = "Username is already in use! Pick another username";
                return View(registerVM);
            }

            var newUser = new AppUser()
            {
                Email = registerVM.EmailAddress,
                UserName = registerVM.UserName,
                ProfileImageUrl = "https://cdn-icons-png.flaticon.com/512/634/634795.png"
            };
            static bool IsSpecialCharacter(char c)
            {
                string specialCharacters = "@#$%^&*?!*_-~<>\"";
                return specialCharacters.Contains(c);
            }
            bool containsUpper = registerVM.Password.Any(char.IsUpper);
            bool containsLower = registerVM.Password.Any(char.IsLower);
            bool containsDigit = registerVM.Password.Any(char.IsDigit);
            bool containsEightCharacters = registerVM.Password.Length >= 8;
            bool containsSpecialCharacter = registerVM.Password.Any(c => IsSpecialCharacter(c));
            if (containsUpper && containsLower && containsDigit && containsEightCharacters && containsSpecialCharacter)
            {
                var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);
                if (newUserResponse.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                    await _signInManager.PasswordSignInAsync(newUser, registerVM.Password, false, false);
                }
                return RedirectToAction("Index", "Home");
            }
            TempData["Error"] = "Password must be at least 8 characters and should contain one UPPERCASE, one lowercase, a number and a special character like @#$%^&*?!*_-~<>";
            return View(registerVM);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

