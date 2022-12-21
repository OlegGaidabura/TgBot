namespace TelegramTeamprojectBot.Models.BotMessages;

public class Button
{
    public string Text;
    public string Command;

    public Button(string text, string command)
    {
        Text = text;
        Command = command;
    }
}