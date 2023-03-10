using System.Text.Json;
using TelegramTeamprojectBot.Controllers;
using TelegramTeamprojectBot.DTOs;
using TelegramTeamprojectBot.Models.BotMessages;

namespace TelegramTeamprojectBot.Models.User;

public class CuratorUser: UserBase 
{
    public CuratorUser(
        long chatId, 
        string teamprojectId,
        ITeamprojectApiController apiController
    ) : base(
        chatId, 
        teamprojectId,
        Role.Curator,
        apiController
    )
    {
    }

    public override async Task<HelloMessage> BuildHelloMessage()
    {
        var userData = await TeamprojectApi.GetUserDataAsync(TeamprojectId);
        if (!userData.IsSuccess) throw new InvalidDataException();
        var userDto = userData.Data!;
        var userModel = userDto!.GetModel();
        return new HelloMessage(
            userModel!.firstName + ' ' + userModel.lastName+ ", вы назначены куратором студенческих команд.",
            new []
            {
                new Button("Список команд", "/show_teams"),
                new Button("Список сообщений", "/messages"),
                new Button("Подробнее об управлении проектом", "/management_guide"),
                new Button("Перейти в сервис", "/teamproject")
            }
        );
    }
}