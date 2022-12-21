namespace TelegramTeamprojectBot.DTOs;

public record UserModel(
    int id, 
    string firstName,
    string lastName,
    string? patronymic,
    string user_role,
    int[] projects_id)
{
}