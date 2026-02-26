using QaToDoApp.Data;
using QaToDoApp.Models;

namespace QaToDoApp.Repository;

public class ToDoItemRepository : Repository<ToDoItem>, IToDoItemRepository
{
    public ToDoItemRepository(ToDoDbContext db) : base(db)
    {
    }
}