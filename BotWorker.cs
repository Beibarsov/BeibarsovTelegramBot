using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


class BotWorker
{
     string BotToken = "5158490376:AAEMo4M6didw6xBPdJ99xNz1SERGRim-sE8";
     TelegramBotClient botClient;
     CancellationTokenSource cts;
    ReceiverOptions receiverOptions;
    public void Inizalize()
    {
       
       botClient = new TelegramBotClient(BotToken);

     cts = new CancellationTokenSource();

        receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { } // receive all update types
        };
        

    }

     public void Start()
    {
        botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token);

        var me =  botClient.GetMeAsync().Result;

        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();

        // Send cancellation request to stop bot
        cts.Cancel();

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Type != UpdateType.Message)
                return;
            // Only process text messages
            if (update.Message!.Type != MessageType.Text)
                return;

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            Message sendMessage = await botClient.SendTextMessageAsync(
                chatId: chatId, text:$"Сам такой, {messageText}", cancellationToken: cancellationToken
            );
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
