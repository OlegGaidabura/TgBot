namespace TelegramTeamprojectBot.Models;

public class TeamprojectResponse<TData>
{
    public bool IsSuccess { get; set; }
    public TData? Data;

    public TeamprojectResponse(bool isSuccess, TData? data)
    {
        IsSuccess = isSuccess;
        Data = data;
    }
}