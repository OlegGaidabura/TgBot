namespace TelegramTeamprojectBot.Models.BotMessages;

public class HelloMessage
{
    public string Text;
    public Button[] Buttons;

    public HelloMessage(string text, Button[] buttons)
    {
        Text = text;
        Buttons = buttons;
    }
}