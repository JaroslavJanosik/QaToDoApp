using System.ComponentModel.DataAnnotations;

namespace QaToDoApp.Models.Dto;

public class ToDoForUpdateDto
{
    [Required]
    public int Id { get; set; }
    public string Text { get; set; }
    public bool Completed { get; set; } 
}