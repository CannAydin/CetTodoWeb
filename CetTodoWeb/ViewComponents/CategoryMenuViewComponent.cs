using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CetTodoWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CetTodoWeb.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext dbContext;
        public CategoryMenuViewComponent(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await dbContext.Categories.ToListAsync();
            return View(items);
        }
    }
}
