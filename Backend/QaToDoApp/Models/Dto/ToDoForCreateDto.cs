using System.ComponentModel.DataAnnotations;

namespace QaToDoApp.Models.Dto
{
    public class ToDoForCreateDto
    {
        [Required]
        [MaxLength(500)]
        public string Text { get; set; }
        
        public bool Completed { get; set; } 
    }
}