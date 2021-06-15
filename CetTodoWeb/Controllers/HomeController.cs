using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CetTodoWeb.Data;
using CetTodoWeb.Models;
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

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var query = dbContext.TodoItems.Include(t => t.Category).Where(t => !t.IsCompleted).OrderBy(t => t.DueDate).Take(3);
            List<TodoItem> result = await query.ToListAsync();
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
