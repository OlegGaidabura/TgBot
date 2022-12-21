using Deployf.Botf;
using Pipelines.Sockets.Unofficial;
using TelegramTeamprojectBot.Classes;
using TelegramTeamprojectBot.Controllers;
using TelegramTeamprojectBot.DTOs;
using TelegramTeamprojectBot.Models.BotMessages;

namespace TelegramTeamprojectBot.Models.User;

public abstract class UserBase: IUser
{
    protected ITeamprojectApiController TeamprojectApi;
    
    public long ChatId { get; set; }
    public string TeamprojectId { get; set; }
    

    private readonly Role _role;
    public Role GetRole() => _role;

    public UserBase(
        long chatId, 
        string teamprojectId,
        Role role, 
        ITeamprojectApiController teamprojectApi
        )
    {
        ChatId = chatId;
        TeamprojectId = teamprojectId;
        _role = role;
        TeamprojectApi = teamprojectApi;
    }

    public override string ToString() => string.Join(' ', new List<string>{ TeamprojectId, _role.ToString() });

    public abstract Task<HelloMessage> BuildHelloMessage();
    public async Task<string[]> GetIncomingMessages()
    {
        var userModel = (await TeamprojectApi.GetUserDataAsync(TeamprojectId)).Data!.GetModel();
        var messages = await DownloadMessages(userModel.projects_id);
        
        return (from message in messages where message.user_id.ToString() == TeamprojectId select message.project + ": " + message.message).ToArray();
    }

    public async Task<string[]> GetProjectsList()
    {
        var userModel = (await TeamprojectApi.GetUserDataAsync(TeamprojectId)).Data!.GetModel();
        var projectsId = userModel.projects_id;
        var result= new List<string>();
        foreach (var projectId in projectsId)
        {
            var projectInfo = await TeamprojectApi.GetProjectDataAsync(userModel.projects_id[projectId-1]);
            result.Add(projectInfo.Data!.title);
        }

        return result.ToArray();
    }

    public async Task DeleteUserMessages()
    {
        var userModel = (await TeamprojectApi.GetUserDataAsync(TeamprojectId)).Data!.GetModel();
        var messages = await DownloadMessages(userModel.projects_id);
        var messagesId = (from message in messages where message.user_id.ToString() == TeamprojectId select message.id)
            .ToArray();
        foreach (var messageId in messagesId)
        {
            await TeamprojectApi.DeleteProjectMessageAsync(messageId);
        }
    }

    public async Task<Dictionary<string,string>> GetDescriptionAbout()
    {
        var userModel = (await TeamprojectApi.GetUserDataAsync(TeamprojectId)).Data!.GetModel();
        var description = await TeamprojectApi.GetDescription(userModel.user_role);
        var result = new Dictionary<string, string>()
        {
            {"about_roles", description.Data!.about_roles},
            {"about_iters", description.Data!.about_iters},
            {"about_iters_tasks", description.Data!.about_iters_tasks},
            {"about_iters_grade", description.Data!.about_iters_grade},
            {"about_project_docs", description.Data!.about_project_docs}
        };
        return result;
    }

    public async Task<Children> GetTreeByRole()
    {
        var userModel = (await TeamprojectApi.GetUserDataAsync(TeamprojectId)).Data!.GetModel();
        var root = (await TeamprojectApi.GetHelpTreeAsync());
        
        return userModel.user_role switch
        {
            "Student" => root.Data.children[0],
            "Curator" => root.Data.children[1],
            _ => root.Data.children[2]
        };
    } 
    
    protected async Task<MessageDto[]> DownloadMessages(IEnumerable<int> projectsId)
    {
        var result = new List<MessageDto>();
        foreach (var projectId in projectsId)
        {
            result.AddRange((await TeamprojectApi.GetProjectMessagesAsync(projectId)).Data!);
        }

        return result.ToArray();
    }
}