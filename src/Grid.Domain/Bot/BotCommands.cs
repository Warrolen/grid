namespace Grid.Domain.Bot;

public interface IBotCommand : IWithBotId;

public sealed record StartBotCommand(string BotId) : IBotCommand;
public sealed record StopBotCommand(string BotId) : IBotCommand;
