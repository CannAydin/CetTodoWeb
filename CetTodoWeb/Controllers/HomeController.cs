using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CetTodoWeb.Data;
using CetTodoWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CetTodoWeb.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Bu Deneme amaçlı olan bir şey
        /// </summary>
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<User> _userManager; 

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            _logger = logger;
            this.dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<TodoItem> result;
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                var query = dbContext.TodoItems.Include(t => t.Category).Where(t => t.UserId == currentUser.Id && !t.IsCompleted).OrderBy(t => t.DueDate).Take(3);
                result = await query.ToListAsync();
            }
            else result = new List<TodoItem>();
            
            return View(result);
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
