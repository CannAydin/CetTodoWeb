using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CetTodoWeb.Data;
using CetTodoWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace CetTodoWeb.Controllers
{
    // Buraya bunu yazarsak hepsi için [Authorize] gerektirir.
    public class TodoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public TodoController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // [AllowAnonymous]
        // Buraya bunu yazarsak yukarda authorize olsa bile yine authorize gerektirmez. 
        // GET: Todo
        public async Task<IActionResult> Index(SearchViewModel searchModel)
        {
            //var applicationDbContext = _context.TodoItems.Include(t => t.Category).Where(t => searchModel.ShowAll || t.IsCompleted == false).OrderBy(t => t.DueDate);


            /* Second Option AsQueryable önemli bir method */
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var query = _context.TodoItems.Include(t => t.Category).Where(t => t.UserId == currentUser.Id);
            var isCompleted = query.Where(t => t.IsCompleted);
            if (searchModel.categoryId != 0)
            {
                query = query.Where(t => t.Category.Id == searchModel.categoryId);
            }
            if (!searchModel.ShowAll)
            {
                query = query.Where(t => !t.IsCompleted);
            }
            if (!String.IsNullOrWhiteSpace(searchModel.SearchText))
            {
                query = query.Where(t => t.Title.Contains(searchModel.SearchText));
            }
            query = query.OrderBy(t => t.DueDate);
            //isCompleted.ForEachAsync(item => query.)

            searchModel.Result = await query.ToListAsync();

            return View(searchModel);
        }

        // GET: Todo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }

        // GET: Todo/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Todo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,IsCompleted,DueDate,CategoryId")] TodoItem todoItem)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            todoItem.UserId = currentUser.Id;
            if (ModelState.IsValid)
            {
                _context.Add(todoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", todoItem.CategoryId);
            return View(todoItem);
        }

        // GET: Todo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems.FindAsync(id);
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if(todoItem.UserId != currentUser.Id)
            {
                return Unauthorized();
            }
            if (todoItem == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", todoItem.CategoryId);
            return View(todoItem);
        }

        // POST: Todo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsCompleted,DueDate,CategoryId,CreatedDate,UserId")] TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldTodo = await _context.TodoItems.FindAsync(id);
                    var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                    if(oldTodo.UserId != currentUser.Id)
                    {
                        return Unauthorized();
                    }
                    oldTodo.Title = todoItem.Title;
                    oldTodo.CompletedDate = todoItem.CompletedDate;
                    oldTodo.CategoryId = todoItem.CategoryId;
                    oldTodo.DueDate = todoItem.DueDate;
                    oldTodo.Description = todoItem.Description;
                    oldTodo.IsCompleted = todoItem.IsCompleted;
                    _context.Update(todoItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoItemExists(todoItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", todoItem.CategoryId);
            return View(todoItem);
        }

        // GET: Todo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }

        // POST: Todo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MakeComplete(int id, bool showall)
        {
            return await ChangeStatus(id, true, showall);
        }

        public async Task<IActionResult> MakeInComplete(int id, bool showall)
        {
            return await ChangeStatus(id, false, showall);
        }

        private async Task<IActionResult> ChangeStatus(int id, bool status, bool showall)
        {
            var todoItemItem = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (todoItemItem == null)
            {
                return NotFound();
            }
            todoItemItem.IsCompleted = status;
            todoItemItem.CompletedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { showall = showall } );
        }

        

        private bool TodoItemExists(int id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
