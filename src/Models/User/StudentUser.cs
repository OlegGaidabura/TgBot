using System.Text.Json;
using Deployf.Botf;
using Microsoft.Extensions.Caching.Distributed;
using TelegramTeamprojectBot.Controllers;
using TelegramTeamprojectBot.DTOs;
using TelegramTeamprojectBot.Models.BotMessages;

namespace TelegramTeamprojectBot.Models.User;

public class StudentUser: UserBase
{
    public StudentUser(
        long chatId, 
        string teamprojectId,
        ITeamprojectApiController apiController
    ) : base(
        chatId, 
        teamprojectId,
        Role.Student,
        apiController
    )
    {
    }

    public override async Task<HelloMessage> BuildHelloMessage()
    {
        var userData = await TeamprojectApi.GetUserDataAsync(TeamprojectId);
        if (!userData.IsSuccess) throw new InvalidDataException();
        var userDto = userData.Data!;
        var userModel = userDto.GetModel();
        var projectInfo = await TeamprojectApi.GetProjectDataAsync(userModel.projects_id[0]);
        return new HelloMessage(
            userModel.firstName + ' ' + userModel.lastName + " вы являетесь участником проекта " +
            projectInfo.Data!.title,
            new[]
            {
                new Button("Список сообщений", "/messages"),
                new Button("Подробнее об управлении проектом", "/management_guide"),
                new Button("Перейти в сервис", "/teamproject")
            }
        );
    }
}