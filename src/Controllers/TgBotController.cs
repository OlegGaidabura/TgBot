using Deployf.Botf;
using Pipelines.Sockets.Unofficial;
using TelegramTeamprojectBot.Classes;
using TelegramTeamprojectBot.Models.User;

namespace TelegramTeamprojectBot.Controllers;

public class TgBotController : BotController
{
    private readonly UserController _userController;
    public bool Flag = true;
    public TgBotController(UserController userController, ITeamprojectApiController teamprojectApiController)
    {
        _userController = userController;
    }
    
    [Action("/start", "Подключить TeamProject")]
    public void Start()
    {
        var user = _userController.GetUserAsync(FromId.ToString()).Result;
        if (EnsureSuccessGetUser(user))
        {
            var message = user!.BuildHelloMessage().Result;
            PushL(message.Text);
            foreach (var button in message.Buttons)
            {
                RowButton(button.Text, button.Command);
            }
        }
        else
        {
            PushL("Для старта работы, вам нужно подключить TeamProject");
            RowButton("Подключить TeamProject", Q(EnterLogPass));
        }
        if (Flag) 
            RunInBackground(TimeSpan.FromSeconds(25), () => Console.WriteLine("Sending"));
    }
    
    async Task RunInBackground(TimeSpan timeSpan, Action action)
    {
        var periodicTimer = new PeriodicTimer(timeSpan);
        while (await periodicTimer.WaitForNextTickAsync())
        {
            action();
            Flag = false;
            var user = await _userController.GetUserAsync(FromId.ToString());
            var messages = await user!.GetIncomingMessages();
            if (messages.Length != 0)
            {
                foreach (var message in messages)
                {
                    Push(message);
                }
                await Send();
                await user!.DeleteUserMessages();
            }
        }
    }
    
    [Action("/logout","Выйти из профиля")]
    public async Task LogOut()
    {
        _userController.RemoveUser(ChatId);
        PushL("Вы вышли из профиля");
    }

    [Action("/messages", "Входящие события Teamproject")]
    public async Task MessageTypeList()
    {
        //TODO: сделать красивое удаление сообщения со стороны пользователя
        var user = await _userController.GetUserAsync(FromId.ToString());
        var messages = await user!.GetIncomingMessages();
        if (messages.Length == 0)
            PushL("У вас нет уведомлений");
        else
        {
            foreach (var message in messages)
            {
                Push(message);
            }

            await Send();
            await user!.DeleteUserMessages();
        }
    
        RowButton("Назад",Q(Start));
    }
    
    [Action("/teamproject", "Перейти на сайт Teamproject")]
    public async Task OpenTeamProject()
    {
        Push("https://teamproject.urfu.ru/#/");
        await Send();
    }
    
    [Action("/management_guide", "Подробнее об управлении проектами")]
    public void AboutProjectManagement()
    {
        var user = _userController.GetUserAsync(FromId.ToString()).Result;
        var tree = user!.GetTreeByRole();
        
        ShowInfo(tree.Result);
        RowButton("Назад",Q(Start));
    }

    [Action]
    public async Task ShowInfo(Children? button) //показать содержимое кнопки (строка или другие кнопки)
    {
        if (button.is_folder == 0)
            PushL(button.content);
        else
        {
            PushL("Menu:");
            foreach (var buttonChild in button.children)
            {
                RowButton(buttonChild.name, Q(ShowInfo, buttonChild));
            }
        }
        if (button.parent_id!=0)
            RowButton("Назад",Q(AboutProjectManagement));
    }
    
    [Action("/show_teams", "Показать список команд")]
    public async Task ShowTeamList()
    {
        var user = await _userController.GetUserAsync(FromId.ToString());
        var projects = await user!.GetProjectsList();

        foreach (var project in projects)
            PushL(project);

        RowButton("Назад", Q(Start));
    }

    [Action]
    public async Task EnterLogPass()
    {
        await Send("Введите ваш токен");
        var token = await AwaitText().ConfigureAwait(false);
        var user = await _userController.RegisterUserAsync(FromId, token);

        if (!EnsureSuccessGetUser(user))
        {
            PushL("Неверный токен");
            RowButton("Попробовать еще раз", Q(EnterLogPass));
            await Send();
        }
        else
        {
            PushL("Успешно!");
            Start();
            await Send();
        }
    }

    [On(Handle.Unknown)]
    public void Unknown()
    {
        PushL("Команда не распознана");
    }
    
    private static bool EnsureSuccessGetUser(IUser? user) => user is not null;
}
