using AutoMapper;
using StudManager.Application.DTOs;
using StudManager.Application.Interfaces;
using StudManager.Application.ValidationInterfaces;
using StudManger.Domain.Interfaces;

namespace StudManager.Application.Commands;

public class EditStudentAsyncCommand: IEditStudentAsyncCommand
{
    private readonly IStudentValidationService _studentValidationService;
    private readonly IMapper _mapper;
    private readonly IStudentRepository _studentRepository;

    public EditStudentAsyncCommand(IStudentValidationService studentValidationService,
        IMapper mapper,
        IStudentRepository studentRepository)
    {
        _studentValidationService = studentValidationService;
        _mapper = mapper;
        _studentRepository = studentRepository;
    }
    
    
    public async Task EditStudentAsync(Guid id, StudentDTO dto)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        await _studentValidationService.ValidateLoginRequestAsync(dto);
        if (student == null)
        {
            throw new InvalidOperationException("Student not found");
        }

        _mapper.Map(dto, student);

        await _studentRepository.UpdateAsync(student);
    }
}