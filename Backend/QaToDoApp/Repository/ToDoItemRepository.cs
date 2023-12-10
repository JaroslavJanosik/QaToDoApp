using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QaToDoApp.Data;
using QaToDoApp.Models;

namespace QaToDoApp.Repository;

public class ToDoItemRepository : Repository<ToDoItem>, IToDoItemRepository
{
    private readonly ToDoDbContext _db;

    public ToDoItemRepository(ToDoDbContext db) : base(db)
    {
        _db = db;
    }
}