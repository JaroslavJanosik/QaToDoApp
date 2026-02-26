using AutoMapper;
using QaToDoApp.Models;
using QaToDoApp.Models.Dto;

namespace QaToDoApp;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<ToDoItem, ToDoItemDto>();
        CreateMap<ToDoItemDto,ToDoItem>();
        
        CreateMap<ToDoItem, ToDoForCreateDto>().ReverseMap();
        CreateMap<ToDoItem, ToDoForUpdateDto>().ReverseMap();
    }
}