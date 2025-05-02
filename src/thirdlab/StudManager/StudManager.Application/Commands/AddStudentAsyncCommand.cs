using AutoMapper;
using StudManager.Application.DTOs;
using StudManager.Application.Interfaces;
using StudManager.Application.ValidationInterfaces;
using StudManger.Domain.Entities;
using StudManger.Domain.Interfaces;

namespace StudManager.Application.Commands;

public class AddStudentAsyncCommand: IAddStudentAsyncCommand
{
    private readonly IQuoteService _quoteService;
    private readonly IStudentValidationService _studentValidationService;
    private readonly IMapper _mapper;
    private readonly IStudentRepository _studentRepository;

    public AddStudentAsyncCommand(IQuoteService quoteService,
        IStudentValidationService studentValidationService,
        IMapper mapper,
        IStudentRepository studentRepository)
    {
        _quoteService = quoteService;
        _studentValidationService = studentValidationService;
        _mapper = mapper;
        _studentRepository = studentRepository;
    }


    public async Task AddStudentAsync(StudentDTO dto)
    {
        var student = _mapper.Map<Student>(dto);
        student.Id = Guid.NewGuid();
        await _studentValidationService.ValidateLoginRequestAsync(dto);
        var quote = await _quoteService.GetMotivationalQuoteAsync();
        if (quote != null)
        {
            student.Quote = $"{quote.Content} - {quote.Author}"; 
        }

        await _studentRepository.AddAsync(student);
    }
}