using BikeTheft.Application.Models;
using BikeTheft.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BikeTheft.Application.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBikeTheftService _bikeTheftService;

        public HomeController(IBikeTheftService bikeTheftService)
        {
            _bikeTheftService = bikeTheftService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetData(string cityName)
        {
            var result = await _bikeTheftService.GetStolenBikesCountByCity(cityName);

            return new JsonResult(result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}