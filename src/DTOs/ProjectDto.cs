namespace TelegramTeamprojectBot.DTOs;

public record ProjectDto(
    int id,
    string title,
    string description,
    int instance_number
    )
{
}