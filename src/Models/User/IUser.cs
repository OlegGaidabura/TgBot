using Deployf.Botf;
using TelegramTeamprojectBot.Classes;
using TelegramTeamprojectBot.Models.BotMessages;

namespace TelegramTeamprojectBot.Models.User;

public interface IUser
{
    public long ChatId { get; set; }
    public string TeamprojectId { get; set; }
    public Role GetRole();
    public string ToString();
    Task<HelloMessage> BuildHelloMessage();
    Task<string[]> GetIncomingMessages();
    Task<string[]> GetProjectsList();
    Task<Dictionary<string,string>> GetDescriptionAbout();
    Task DeleteUserMessages();
    Task<Children> GetTreeByRole();
}