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

    Message sendMessage;

    ILogger logger;
    public void Inizalize()
    {

        botClient = new TelegramBotClient(BotToken);

        cts = new CancellationTokenSource();

        receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { } // receive all update types
        };

        logger = new Logger();

        

    }

    public void Start()
    {
        botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token);

        var me = botClient.GetMeAsync().Result;

        logger.Event($"Start listening for @{me.Username}");


        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var chatId = update.Message.Chat.Id;

            try
            {
                if (update.Type != UpdateType.Message)
                    return;
                // Only process text messages
                if (update.Message!.Type != MessageType.Text)
                    throw new Exception($"Неподходящий тип данных в чате {chatId}, пользователь вместо текста прислал {update.Message.Type}");

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
             sendMessage = await botClient.SendTextMessageAsync(
                chatId: chatId, text: $"Произошла ошибка: {ex.Message}", cancellationToken: cancellationToken
            );
                return;
            } // Only process Message updates: https://core.telegram.org/bots/api#message
            
            var messageText = update.Message.Text;

            logger.Event($"Received a '{messageText}' message in chat {chatId}.");

            sendMessage = await botClient.SendTextMessageAsync(
                chatId: chatId, text: $"Сам такой, {messageText}", cancellationToken: cancellationToken
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

            logger.Error(ErrorMessage);
            return Task.CompletedTask;
        }
    }

    public void Stop()
    {
        Console.ReadLine();
        // Send cancellation request to stop bot
        cts.Cancel();
    }
}
