namespace StudManager.Application.DTOs;

public class StudentDTO
{
    public Guid StudentId { get; set; } 
    
    public string Name { get; set; } = default!;
    public int Grade { get; set; }
    
    public string Quote { get; set; } 
}
