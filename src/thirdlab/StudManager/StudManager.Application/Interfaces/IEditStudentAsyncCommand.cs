using StudManager.Application.DTOs;

namespace StudManager.Application.Interfaces;

public interface IEditStudentAsyncCommand
{
    Task EditStudentAsync(Guid id, StudentDTO dto);
}