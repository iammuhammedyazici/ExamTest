using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        [Route("")]
        [Route("index")]
        [Route("~/")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("login")]
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username != null && password != null)
            {
                using (var ctx = new ApplicationDbContext(new Microsoft.EntityFrameworkCore.DbContextOptions<ApplicationDbContext>()))
                {
                    DataTable dt = new DataTable();
                    using (SQLLiteConnector Connector = new SQLLiteConnector())
                    {
                        dt = Connector.LoadData("Select * from Users where UserName == '"+username+"'");
                        if (dt.Rows==null || dt.Rows.Count == 0)
                        {
                            ViewBag.error = "Invalid Account";
                            return View("Success");
                        }
                        else
                        {
                            User user = new User()
                            {
                                UserName = dt.Rows[0]["UserName"].ToString(),
                                PasswordHash = dt.Rows[0]["PasswordHash"].ToString()
                            };
                            if(user==null || user.PasswordHash != password)
                            {
                                ViewBag.error = "Invalid Account";
                                return View("Success");
                            }
                            else
                            {
                                HttpContext.Session.SetString("username", username);
                                return View("Success");
                            }
                        }
                    }
                }
            }
            else
            {
                ViewBag.error = "Invalid Account";
                return View("Index");
            }
        }

        [Route("logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            return RedirectToAction("Index");
        }
    }
}