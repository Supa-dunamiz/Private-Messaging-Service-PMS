using Microsoft.AspNetCore.Mvc;
using PMS.Interfaces;
using PMS.Models;

namespace PMS.Controllers
{
    public class LinkController : Controller
    {
        private readonly ILinkRepository _linkRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LinkController(ILinkRepository linkRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _linkRepository = linkRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> GetLink()
        {
            var curUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
            var linkCheck = _linkRepository.LinkExists(curUserId);
            if (linkCheck)
            {
                var UserLink = await _linkRepository.GetByUserId(curUserId);
                return View(UserLink);
            }
            else
            {
                string guid = Guid.NewGuid().ToString();
                string linkId = guid.Substring(0 , 8);
                var UserLink = new Link()
                {
                    LinkId = linkId,
                    AppUserId = curUserId,
                };
                _linkRepository.Add(UserLink);
                return View(UserLink);
            }

        }
    }
}
