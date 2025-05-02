using StudManager.Application.DTOs;

namespace StudManager.Application.Interfaces;

public interface IGetAllStudentsAsyncCommand
{
    Task<List<StudentDTO>> GetAllStudentsAsync();
}