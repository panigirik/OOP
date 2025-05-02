using StudManager.Application.DTOs;

namespace StudManager.Application.Interfaces;

public interface IAddStudentAsyncCommand
{
    Task AddStudentAsync(StudentDTO dto);
}