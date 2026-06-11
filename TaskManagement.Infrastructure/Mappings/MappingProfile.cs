using AutoMapper;
using TaskManagement.Core.DTOs.Comment;
using TaskManagement.Core.DTOs.Project;
using TaskManagement.Core.DTOs.Task;
using TaskManagement.Core.DTOs.User;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();

        CreateMap<Project, ProjectDto>()
            .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => src.Tasks.Count));
        CreateMap<CreateProjectDto, Project>();
        CreateMap<UpdateProjectDto, Project>();

        CreateMap<TaskItem, TaskItemDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
            .ForMember(dest => dest.AssignedUserName, opt => opt.MapFrom(src =>
                src.AssignedUser != null
                    ? $"{src.AssignedUser.FirstName} {src.AssignedUser.LastName}"
                    : null));
        CreateMap<CreateTaskItemDto, TaskItem>();
        CreateMap<UpdateTaskItemDto, TaskItem>();

        CreateMap<Comment, CommentDto>();
        CreateMap<CreateCommentDto, Comment>();
        CreateMap<UpdateCommentDto, Comment>();
    }
}
