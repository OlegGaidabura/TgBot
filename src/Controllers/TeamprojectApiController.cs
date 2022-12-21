using System.Text.Json;
using TelegramTeamprojectBot.Classes;
using TelegramTeamprojectBot.DTOs;
using TelegramTeamprojectBot.Models;

namespace TelegramTeamprojectBot.Controllers;

public interface ITeamprojectApiController
{
    public Task<TeamprojectResponse<string>> RegisterUserAsync(string token);
    public Task<TeamprojectResponse<UserDto>> GetUserDataAsync(string userId);
    public Task<TeamprojectResponse<ProjectDto>> GetProjectDataAsync(int projectId);
    public Task<TeamprojectResponse<string>> GetProjectRopIdAsync(string projectId);
    public Task<TeamprojectResponse<string>> GetProjectCuratorIdAsync(string projectId);
    public Task<TeamprojectResponse<string>> GetProjectStudentsIdAsync(string projectId);
    public Task<TeamprojectResponse<MessageDto[]>> GetProjectMessagesAsync(int projectId);
    public Task DeleteProjectMessageAsync(int messageId);
    public Task<TeamprojectResponse<DescriptionDto>> GetDescription(string userRole);
    public Task<TeamprojectResponse<Children>> GetHelpTreeAsync();
}

public class TeamprojectApiController: ITeamprojectApiController
{
    private readonly HttpClient _httpClient;
    
    public TeamprojectApiController(string baseUri)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(baseUri);
    }

    public async Task<TeamprojectResponse<string>> RegisterUserAsync(string token)
    {
        var result = await _httpClient.GetAsync("api/register?token=" + token);
        return !result.IsSuccessStatusCode
            ? new TeamprojectResponse<string>(false, null)
            : new TeamprojectResponse<string>(true, await result.Content.ReadAsStringAsync());
    }

    public async Task<TeamprojectResponse<UserDto>> GetUserDataAsync(string userId)
    {
        var result = await _httpClient.GetAsync("api/user?user_id=" + userId);
        if (!result.IsSuccessStatusCode) return new TeamprojectResponse<UserDto>(false, null);
        var userRawDtos = JsonSerializer.Deserialize<UserRawDto[]>(await result.Content.ReadAsStringAsync());
        return new TeamprojectResponse<UserDto>(true, new UserDto(userRawDtos!));
    }

    public async Task<TeamprojectResponse<ProjectDto>> GetProjectDataAsync(int projectId)
    {
        var result = await _httpClient.GetAsync("/api/project?project_id=" + projectId);
        if (!result.IsSuccessStatusCode) return new TeamprojectResponse<ProjectDto>(false, null);
        var projectDto = JsonSerializer.Deserialize<ProjectDto[]>(await result.Content.ReadAsStringAsync());
        return new TeamprojectResponse<ProjectDto>(true, projectDto![0]);
    }
    
    public async Task<TeamprojectResponse<string>> GetProjectCuratorIdAsync(string projectId)
    {
        var result = await _httpClient.GetAsync("/api/project/curator?project_id=" + projectId);
        return !result.IsSuccessStatusCode
            ? new TeamprojectResponse<string>(false, null)
            : new TeamprojectResponse<string>(true, await result.Content.ReadAsStringAsync());
    }

    public async Task<TeamprojectResponse<string>> GetProjectRopIdAsync(string projectId)
    {
        var result = await _httpClient.GetAsync("/api/project/rop?project_id=" + projectId);
        return !result.IsSuccessStatusCode
            ? new TeamprojectResponse<string>(false, null)
            : new TeamprojectResponse<string>(true, await result.Content.ReadAsStringAsync());
    }

    public async Task<TeamprojectResponse<string>> GetProjectStudentsIdAsync(string projectId)
    {
        var result = await _httpClient.GetAsync("/api/project/students?project_id=" + projectId);
        return !result.IsSuccessStatusCode
            ? new TeamprojectResponse<string>(false, null)
            : new TeamprojectResponse<string>(true, await result.Content.ReadAsStringAsync());
    }

    public async Task<TeamprojectResponse<MessageDto[]>> GetProjectMessagesAsync(int projectId)
    {
        var result = await _httpClient.GetAsync("/api/project/messages?project_id=" + projectId);
        if(!result.IsSuccessStatusCode)
            return new TeamprojectResponse<MessageDto[]>(false, null);
        var messages = JsonSerializer.Deserialize<MessageDto[]>(await result.Content.ReadAsStringAsync())!;
        return new TeamprojectResponse<MessageDto[]>(true, messages);
    }
    
    public async Task DeleteProjectMessageAsync(int messageId)
    {
        var result = await _httpClient.DeleteAsync("/api/project/message?msg_id=" + messageId);
    }
    
    public async Task<TeamprojectResponse<DescriptionDto>> GetDescription(string userRole)
    {
        var result = await _httpClient.GetAsync("/api/info?user_role=" + userRole);
        if (!result.IsSuccessStatusCode) return new TeamprojectResponse<DescriptionDto>(false, null);
        var descriptionDto = JsonSerializer.Deserialize<DescriptionDto[]>(await result.Content.ReadAsStringAsync());
        return new TeamprojectResponse<DescriptionDto>(true, descriptionDto![0]);
    }

    public async Task<TeamprojectResponse<Children>> GetHelpTreeAsync()
    {
        var result = await _httpClient.GetAsync("/emulator/api/help/tree");
        var jsonContent = result.Content.ReadAsStringAsync().Result;
        jsonContent = jsonContent.Substring(1, jsonContent.Length - 3);
        if (!result.IsSuccessStatusCode)
            return new TeamprojectResponse<Children>(false, null);
        var root = JsonSerializer.Deserialize<Children>(jsonContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        return new TeamprojectResponse<Children>(true, root);
    }
}