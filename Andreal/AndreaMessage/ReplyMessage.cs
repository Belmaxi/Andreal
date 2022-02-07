namespace AndrealClient.AndreaMessage;

[Serializable]
public class ReplyMessage : IMessage
{
    public readonly int MessageId;
    
    internal ReplyMessage(int messageId) { MessageId = messageId; }
}
