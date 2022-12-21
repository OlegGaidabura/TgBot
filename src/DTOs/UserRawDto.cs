namespace TelegramTeamprojectBot.DTOs;

public record UserRawDto(
    int id, 
    string firstName,
    string lastName,
    string? patronymic,
    string user_role,
    int project_id)
{
}