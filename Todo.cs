namespace ecommerce_api;

public record Todo
{
    public int Id { get; init; }
    public required string Name { get; set; } 
    public bool IsComplete { get; set; }
}