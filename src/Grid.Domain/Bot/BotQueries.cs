namespace Grid.Domain.Bot;

public interface IBotQuery : IWithBotId;

public sealed record FetchBot(string BotId) : IBotQuery;
