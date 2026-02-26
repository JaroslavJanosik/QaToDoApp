using System.ComponentModel.DataAnnotations;

namespace QaToDoApp.Models.Dto;

public class ToDoItemDto
{
    public int Id { get; set; }
    [Required]
    public string Text { get; set; }
    public bool Completed { get; set; } 
}