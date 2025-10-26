
using Microsoft.AspNetCore.Mvc;
using RegisterAndLoginApp.Models;
using RegisterAndLoginApp.Filters;
using ServiceStack.Text;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace RegisterAndLoginApp.Controllers
{
    public class UserController : Controller
    {
      
        private static readonly UsersDAO users = new UsersDAO();

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessLogin(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", loginViewModel);
            }

            var resultId = users.CheckCredentials(loginViewModel.Username, loginViewModel.Password);
            if (resultId > 0)
            {
                var user = users.GetUserById(resultId);

                var userJson = JsonSerializer.SerializeToString(user);
                HttpContext.Session.SetString("User", userJson);

                return View("LoginSuccess", user);
            }

            return View("LoginFailure");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessRegister(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", vm);
            }

            var existing = users
                .GetAllUsers()
                .FirstOrDefault(u => string.Equals(u.Username?.Trim(),
                                                   (vm.Username ?? string.Empty).Trim(),
                                                   System.StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                ModelState.AddModelError(nameof(vm.Username), "That username is already taken.");
                return View("Register", vm);
            }

            var newUser = new UserModel
            {
                Username = vm.Username.Trim(),
                Groups = string.IsNullOrWhiteSpace(vm.Groups) ? "User" : vm.Groups.Trim()
            };
            newUser.SetPassword(vm.Password);

            var newId = users.AddUser(newUser);
            newUser.Id = newId;

            return View("RegisterSuccess", newUser);
        }

        [SessionCheckFilter]
        public IActionResult MembersOnly()
        {
            return View();
        }


        [AdminCheckFilter]
        public IActionResult AdminOnly()
        {
            return View()

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User");
            return RedirectToAction("Index");
        }
    }
}
