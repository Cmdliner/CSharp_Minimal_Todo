using ecommerce_api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));


var app = builder.Build();
var todoRoutes = app.MapGroup("/todos");

app.MapGet("/", () => "Hello, World");

todoRoutes.MapGet("", async (TodoDb db) => await db.Todos.ToListAsync());
todoRoutes.MapGet("/complete", GetCompletedTodos);
todoRoutes.MapGet("/{id:int}", GetTodoById);

todoRoutes.MapPost("/new", CreateTodo);

todoRoutes.MapPut("/{id:int}", EditTodo);

todoRoutes.MapDelete("/{id:int}", DeleteTodo);

app.Run();

static async Task<IResult> GetCompletedTodos(TodoDb db)
{
    var todos = await db.Todos.Where(todo => todo.IsComplete).ToListAsync();
    return todos.Count == 0 ? Results.Ok(todos) : Results.NotFound();
}

static async Task<IResult> GetTodoById(int id, TodoDb db)
{
    var todo = await db.Todos.FindAsync(id);
    return todo is null ? Results.NotFound() : Results.Ok(todo);
}

static async Task<IResult> CreateTodo(Todo todo, TodoDb db)
{
    var todoInDb = await db.Todos.FindAsync(todo.Id);
    var todoExists = todoInDb is not null;
    if (todoExists) return Results.BadRequest("Todo exist in db");

    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created();
}

static async Task<IResult> EditTodo(int id, Todo inputTodo, TodoDb db)
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();
    
    todo.IsComplete = inputTodo.IsComplete;  
    todo.Name = inputTodo.Name;

    await db.SaveChangesAsync();
    return Results.NoContent();
}

static async Task<IResult> DeleteTodo(int id, TodoDb db) 
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();
    db.Todos.Remove(todo);
    await db.SaveChangesAsync();

    return Results.Redirect("/");
}