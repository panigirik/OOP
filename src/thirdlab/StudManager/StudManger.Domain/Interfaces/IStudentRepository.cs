using StudManger.Domain.Entities;

namespace StudManger.Domain.Interfaces;

public interface IStudentRepository
{
    Task AddAsync(Student student);
    Task<List<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(Guid id);
    Task UpdateAsync(Student student);
}