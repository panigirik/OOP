using AutoMapper;
using StudManager.Application.ValidationInterfaces;

namespace StudManager.Application.Creators
{
    using StudManger.Domain.Entities;
    using StudManger.Domain.Interfaces;
    using Interfaces;
    using System;
    using System.Threading.Tasks;

    public class DefaultStudentFactory : StudentCreator
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IQuoteService      _quoteService;
        private readonly IMapper _mapper;

        public DefaultStudentFactory(
            IStudentRepository studentRepository,
            IQuoteService      quoteService,
            IMapper mapper,
            IStudentValidationService studentValidationService): base(studentValidationService, mapper)
        {
            _studentRepository = studentRepository;
            _quoteService      = quoteService;
            
        }

        protected override async Task<Student> FactoryMethodAsync(string name, int grade)
        {
            var student = new Student
            {
                Id = Guid.NewGuid(),
                Name = name,
                Grade = grade,
            };
            
            var quote = await _quoteService.GetMotivationalQuoteAsync();
            if (quote != null)
            {
                student.Quote = quote.Content + " — " + quote.Author;
            }
            
            await _studentRepository.AddAsync(student);
            
            return student;
        }
    }
}