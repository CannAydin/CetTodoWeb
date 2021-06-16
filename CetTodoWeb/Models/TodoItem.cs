using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CetTodoWeb.Models
{
    public class TodoItem
    {
        public TodoItem()
        {
            CreatedDate = DateTime.Now;
        }
        public int Id { get; set; }
        [Required(ErrorMessage = "Please Enter Title")]
        [MaxLength(200)]
        public string Title { get; set; }
        [MaxLength(1500)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Is Completed")]
        public bool IsCompleted { get; set; }

        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        // Scaffold false => view'e ekleme bu alanları diyoruz :)
        [ScaffoldColumn(false)]
        public DateTime CreatedDate { get; set; }

        [ScaffoldColumn(false)]
        public DateTime CompletedDate { get; set; }

        [Display(Name = "Remaining Hours")]
        public int RemainingHour
        {
            get
            {
                var remainingTime = (DueDate - DateTime.Now);
                return (int)remainingTime.TotalHours;
            }
        }

        [Display(Name = "Category Name")]

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
