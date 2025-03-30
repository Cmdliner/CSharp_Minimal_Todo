using Microsoft.EntityFrameworkCore;

namespace ecommerce_api;

public class TodoDb(DbContextOptions<TodoDb> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();
}