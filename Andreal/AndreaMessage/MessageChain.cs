namespace AndrealClient.AndreaMessage;

public class MessageChain
{
    private IEnumerable<IMessage> _messages;

    private MessageChain(params IMessage[] messages) { _messages = messages.ToList(); }

    private MessageChain(IEnumerable<IMessage> messages) { _messages = messages; }

    internal MessageChain Append(IMessage? message)
    {
        if (message is not null) _messages = _messages.Append(message);
        return this;
    }

    internal MessageChain Append(string? message)
    {
        if (message is not null) _messages = _messages.Append((TextMessage)message);
        return this;
    }

    internal MessageChain Prepend(IMessage message)
    {
        _messages = _messages.Prepend(message);
        return this;
    }

    internal IEnumerable<IMessage> ToArray() => _messages;
   
    public static implicit operator MessageChain(string value) => new((TextMessage)value);

    public static implicit operator MessageChain(TextMessage value) => new(value);

    public static implicit operator MessageChain(ImageMessage value) => new(value);
}
