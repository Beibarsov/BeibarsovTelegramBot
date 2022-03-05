

using Telegram.Bot.Types;

class Conversation{
    private Chat telegramChat;
    private List<Message> telegramMessages;

    public Conversation(Chat chat){
        telegramChat = chat;
        telegramMessages = new List<Message>();
    }

    public void AddMessage(Message message){
        telegramMessages.Add(message);
    }
}