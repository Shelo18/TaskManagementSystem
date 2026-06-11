using AutoMapper;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.DTOs.Project;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.Infrastructure.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public ProjectService(IProjectRepository projectRepository, IMapper mapper)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<ProjectDto>> GetAllAsync(QueryParameters parameters)
    {
        var result = await _projectRepository.GetPagedAsync(parameters);
        return new PagedResult<ProjectDto>
        {
            Items = _mapper.Map<IEnumerable<ProjectDto>>(result.Items),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    public async Task<ProjectDto?> GetByIdAsync(int id)
    {
        var project = await _projectRepository.GetWithTasksAsync(id);
        return project is null ? null : _mapper.Map<ProjectDto>(project);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
    {
        var project = _mapper.Map<Project>(dto);
        var created = await _projectRepository.AddAsync(project);
        return _mapper.Map<ProjectDto>(created);
    }

    public async Task<ProjectDto?> UpdateAsync(int id, UpdateProjectDto dto)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project is null) return null;

        _mapper.Map(dto, project);
        await _projectRepository.UpdateAsync(project);
        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project is null) return false;

        await _projectRepository.DeleteAsync(project);
        return true;
    }
}
