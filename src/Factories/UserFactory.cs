using System.Text.Json;
using TelegramTeamprojectBot.Controllers;
using TelegramTeamprojectBot.DTOs;
using TelegramTeamprojectBot.Models.RedisData;
using TelegramTeamprojectBot.Models.User;

namespace TelegramTeamprojectBot.Factories;

public interface IUserFactory
{

    public IUser CreateUser(long chatId, UserDto dto, ITeamprojectApiController apiController);
    
    public IUser CreateUser(string jsonData, ITeamprojectApiController apiController);
}

public class StudentUserFactory : IUserFactory
{
    public IUser CreateUser(long chatId, UserDto dto, ITeamprojectApiController apiController) => new StudentUser(
        chatId,
        dto.User[0].id.ToString(),
        apiController
    );

    public IUser CreateUser(string jsonData, ITeamprojectApiController apiController)
    {
        var data = JsonSerializer.Deserialize<UserData>(jsonData);
        return new StudentUser(data.ChatId, data.TeamprojectId, apiController);
    }
}

public class RopUserFactory : IUserFactory
{
    public IUser CreateUser(long chatId, UserDto dto, ITeamprojectApiController apiController) => new RopUser(
        chatId,
        dto.User[0].id.ToString(),
        apiController
    );

    public IUser CreateUser(string jsonData, ITeamprojectApiController apiController)
    {
        var data = JsonSerializer.Deserialize<UserData>(jsonData);
        return new RopUser(data.ChatId, data.TeamprojectId, apiController);
    }
}

public class CuratorUserFactory : IUserFactory
{
    public IUser CreateUser(long chatId, UserDto dto, ITeamprojectApiController apiController) => new CuratorUser(
        chatId,
        dto.User[0].id.ToString(),
        apiController
    );

    public IUser CreateUser(string jsonData, ITeamprojectApiController apiController)
    {
        var data = JsonSerializer.Deserialize<UserData>(jsonData);
        return new CuratorUser(data.ChatId, data.TeamprojectId, apiController);
    }
}