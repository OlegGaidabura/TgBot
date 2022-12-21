namespace TelegramTeamprojectBot.DTOs;

public record MessageDto(
    int id,
    string message,
    string project,
    int project_id,
    string user,
    int user_id
    )
{
    
}