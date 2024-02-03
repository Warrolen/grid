using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using Grid.Domain.Bot;

namespace Grid.App.Actors;

public class BotActor : ReceivePersistentActor
{
    private readonly HashSet<IActorRef> _subscribers = [];
    private BotState _botState;
    private readonly ILoggingAdapter _log = Context.GetLogger();

    public BotActor(string botId)
    {
        PersistenceId = $"Bot_{botId}";
        _botState = new BotState(botId, false);

        Recover<BotStarted>(@event =>
        {
            _botState = _botState with { IsRunning = true };
        });

        Recover<BotStopped>(@event =>
        {
            _botState = _botState with { IsRunning = false };
        });

        Command<StartBotCommand>(cmd =>
        {
            if (!_botState.IsRunning)
            {
                var @event = new BotStarted(cmd.BotId);
                Persist(@event, e =>
                {
                    _botState = _botState with { IsRunning = true };
                    _log.Info("Bot started: {0}", cmd.BotId);
                    NotifySubscribers(@event);
                    Sender.Tell(new BotOperationResponse(cmd.BotId, true, "Bot started successfully"));
                });
            }
            else
            {
                Sender.Tell(new BotOperationResponse(cmd.BotId, false, "Bot is already running"));
            }
        });

        Command<StopBotCommand>(cmd =>
        {
            if (_botState.IsRunning)
            {
                var @event = new BotStopped(cmd.BotId);
                Persist(@event, e =>
                {
                    _botState = _botState with { IsRunning = false };
                    _log.Info("Bot stopped: {0}", cmd.BotId);
                    NotifySubscribers(@event);
                    Sender.Tell(new BotOperationResponse(cmd.BotId, true, "Bot stopped successfully"));
                });
            }
            else
            {
                Sender.Tell(new BotOperationResponse(cmd.BotId, false, "Bot is not running"));
            }
        });
    }

    private void NotifySubscribers(IBotEvent @event)
    {
        foreach (var subscriber in _subscribers)
        {
            subscriber.Tell(@event);
        }
    }

    public override string PersistenceId { get; }
}

public record BotState(string BotId, bool IsRunning);

public record BotOperationResponse(string BotId, bool Success, string Message);
