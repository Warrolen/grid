namespace Grid.Domain.Bot;

public interface IBotEvent : IWithBotId;

public sealed record BotStarted(string BotId) : IBotEvent;
public sealed record BotStopped(string BotId) : IBotEvent;
