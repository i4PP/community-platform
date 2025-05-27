
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using FluentValidation;
using BMWDomain.Entities;
using BMWDomain.interfaces;
using BMW.ASP.Models;
using BMWDomain.Exceptions;

namespace BMW.ASP.Controllers
{
    public class LoginController : Controller
    {

        private readonly IUserContainer _userContainer;


        public LoginController(IUserContainer userContainer)
        {
            this._userContainer = userContainer;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel userObj)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Login login = new Login(userObj.User!, userObj.Password!, 0);
                    _userContainer.LoginUser(login);
                    HttpContext.Session.SetInt32("UserId", login.Id);
                    HttpContext.Session.SetString("UserName", login.User);
                    return RedirectToAction("Index", "Home");

                }
                catch (ValidationException ex)
                {
                    foreach (var error in ex.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                }
                catch (BllException e)
                {
                    TempData["Error"] = e.Message ;
                }
                catch(DataBaseException e)
                {
                    TempData["Error"] = e.Message;
                }
                catch (Exception)
                {
                    TempData["Error"] = "invalid username or password.";
                }

            }   
            return View();

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel userObj)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    Account account = new Account(0, userObj.UserName!, userObj.Password!, userObj.Email!);


                    _userContainer.RegisterUser(account);

                    return RedirectToAction("Login");
                    
                }
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
            catch (BllException e)
            {
                TempData["Error"] = e.Message ;
            }
            catch(DataBaseException e)
            {
                TempData["Error"] = e.Message ;
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while Registering user.";
            }

            return View();

        }
    }
}
