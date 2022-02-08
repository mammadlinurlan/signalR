using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalR_task.Hubs;
using SignalR_task.Models;
using SignalR_task.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR_task.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IHubContext<ChatHub> _hubContext;

        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IHubContext<ChatHub> hubContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hubContext = hubContext;
        }



       

        public IActionResult Index()
        {
            return View();
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



        public IActionResult TestUser()
        {
            return Json(User.Identity.Name);
        }

        public IActionResult Chat()
        {
            List<AppUser> users = _userManager.Users.ToList();

            return View(users);

        }

        public IActionResult CreateUser()
        {
            AppUser user1 = new AppUser { Fullname = "user1 name", UserName = "user1" };
            AppUser user2 = new AppUser { Fullname = "user2 name", UserName = "user2" };
            AppUser user3 = new AppUser { Fullname = "user3 name", UserName = "user3" };


            var result = _userManager.CreateAsync(user1, "Nurlan123@").Result;
            var result2 = _userManager.CreateAsync(user2, "Nurlan123@").Result;
            var result3 = _userManager.CreateAsync(user3, "Nurlan123@").Result;



            return Content("ok");
        }

       

         public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM login)
    {
        if (!ModelState.IsValid) return View();

        AppUser user = await _userManager.FindByNameAsync(login.UserName);

        if (user == null)
        {
            ModelState.AddModelError("", "Username or password is incorrect!");
            return View();
        }

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user.UserName, login.Password, true, true);

            if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Username or password is incorrect!");
            return View();
        }


        return RedirectToAction(nameof(Chat));
    }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> PrivateChat(string id)
        {
            AppUser user =await _userManager.FindByIdAsync(id);
            if (user==null)
            {
                return NotFound();
            }

          await  _hubContext.Clients.Client(user.ConnectionId).SendAsync("PrivateMessage");

            return RedirectToAction(nameof(Chat));

        }

        //public IActionResult CreateUser()
        //{
        //    AppUser user1 = new AppUser {  UserName = "Vusal" };
        //    AppUser user2 = new AppUser {  UserName = "Hasan" };
        //    AppUser user3 = new AppUser {  UserName = "Lala" };
        //    AppUser user4 = new AppUser {  UserName = "Ehed" };

        //    var result1 = _userManager.CreateAsync(user1, "User123").Result;
        //    var result2 = _userManager.CreateAsync(user2, "User@123").Result;
        //    var result3 = _userManager.CreateAsync(user3, "User@123").Result;
        //    var result4 = _userManager.CreateAsync(user4, "User@123").Result;


        //    return Content("Created");
        //}



    }
}
