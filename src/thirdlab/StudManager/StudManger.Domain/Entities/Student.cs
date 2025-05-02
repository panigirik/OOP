namespace StudManger.Domain.Entities;

public class Student
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "samenick";
    public double Grade { get; set; }
    
    public string Quote { get; set; } 
}