using System.Threading.Tasks;
using QaToDoApp.Models;

namespace QaToDoApp.Repository;

public interface IToDoItemRepository : IRepository<ToDoItem>
{
}