using Akka.Actor;
using Akka.Hosting;
using Grid.App.Actors;
using Microsoft.AspNetCore.Mvc;
using Grid.Domain.Bot;

namespace Grid.App.Controllers;

[ApiController]
[Route("[controller]")]
public class BotController(IRequiredActor<BotActor> botActor) : ControllerBase
{
    private readonly IActorRef _botActor = botActor.ActorRef;

    [HttpPost("start/{botId}")]
    public async Task<IActionResult> Start(string botId)
    {
        var response = await _botActor.Ask<BotOperationResponse>(new StartBotCommand(botId), TimeSpan.FromSeconds(5));
        return response.Success ? Ok(response) : BadRequest(response.Message);
    }

    [HttpPost("stop/{botId}")]
    public async Task<IActionResult> Stop(string botId)
    {
        var response = await _botActor.Ask<BotOperationResponse>(new StopBotCommand(botId), TimeSpan.FromSeconds(5));
        return response.Success ? Ok(response) : BadRequest(response.Message);
    }

    // TODO: An endpoint to fetch all bots and their states.
    // TODO: An endpoint to fetch a specific bot.
}
