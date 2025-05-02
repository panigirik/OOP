using AutoMapper;
using StudManager.Application.DTOs;
using StudManager.Application.Interfaces;
using StudManger.Domain.Interfaces;

namespace StudManager.Application.Commands;

public class GetAllStudentsAsyncCommand: IGetAllStudentsAsyncCommand
{
    
    private readonly IMapper _mapper;
    private readonly IStudentRepository _studentRepository;

    public GetAllStudentsAsyncCommand(IMapper mapper,
        IStudentRepository studentRepository)
    {
        _mapper = mapper;
        _studentRepository = studentRepository;
    }
    
    public async Task<List<StudentDTO>> GetAllStudentsAsync()
    {
        var students = await _studentRepository.GetAllAsync();
        return _mapper.Map<List<StudentDTO>>(students);
    }
}