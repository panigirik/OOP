
using AutoMapper;
using StudManager.Application.DTOs;
using StudManager.Application.ValidationInterfaces;
using StudManger.Domain.Entities;

namespace StudManager.Application.Creators
{
    public abstract class StudentCreator
    {
        private readonly IStudentValidationService _validationService;
        private readonly IMapper _mapper;
        
        protected StudentCreator(
            IStudentValidationService validationService,
            IMapper mapper)
        {
            _validationService = validationService;
            _mapper = mapper;
        }
        
        public async Task<Student> CreateAsync(StudentDTO studentDto)
        {
            await _validationService.ValidateLoginRequestAsync(studentDto);
            
            if (string.IsNullOrWhiteSpace(studentDto.Name))
                throw new ArgumentException("Name cannot be empty", nameof(studentDto.Name));
            if (studentDto.Grade < 0)
                throw new ArgumentOutOfRangeException(nameof(studentDto.Grade), "Grade must be non‑negative");
            
            var student = await FactoryMethodAsync(studentDto.Name, studentDto.Grade);
            
            _mapper.Map(studentDto, student);
            
            return student;
        }

      
        protected abstract Task<Student> FactoryMethodAsync(string name, int grade);
    }
}
