using AutoMapper;
using StudManager.Application.DTOs;
using StudManger.Domain.Entities;

namespace StudManager.Application.Mappings;

public class StudentMappingProfile: Profile
{
    public StudentMappingProfile()
    {
        CreateMap<Student, StudentDTO>().ReverseMap();
    }
}