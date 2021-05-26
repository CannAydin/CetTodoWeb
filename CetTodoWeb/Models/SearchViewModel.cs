using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CetTodoWeb.Models
{
    public class SearchViewModel
    {
        public string SearchText { get; set; }
        public bool ShowAll { get; set; }

        public bool ShowHobbies { get; set; }
        public bool ShowWorks { get; set; }
        public bool ShowCourses { get; set; }

        public List<TodoItem> Result { get; set; }
    }
}
