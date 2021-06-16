using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CetTodoWeb.Data;
using CetTodoWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CetTodoWeb.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<User> _userManager;


        public CategoryMenuViewComponent(ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            this.dbContext = dbContext;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var items = await dbContext.Categories.Where(c => c.UserId == currentUser.Id).ToListAsync();
            return View(items);
        }
    }
}
