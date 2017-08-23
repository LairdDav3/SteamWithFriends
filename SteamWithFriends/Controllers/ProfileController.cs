using Microsoft.AspNetCore.Mvc;
using SteamWithFriends.Models;
using SteamWithFriends.Services;
using System.Linq;

namespace SteamWithFriends.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ISteamApiService _steamApiService;
        public ProfileController(ISteamApiService steamApiService)
        {
            _steamApiService = steamApiService;
        }

        public IActionResult Index()
        {
            return View(new SteamProfileIndexViewModel());
        }

        [HttpPost]
        public IActionResult Index(SteamProfileIndexViewModel viewModel)
        {
            if (viewModel?.Profiles == null || viewModel.Profiles.All(p => string.IsNullOrWhiteSpace(p)))
                return RedirectToAction("Index");

            var steamProfiles = _steamApiService.GetPlayers(viewModel.Profiles);

            return View(new SteamProfileIndexViewModel
            {
                Results = steamProfiles
            });
        }
    }
}