using Microsoft.AspNetCore.Mvc;
using NutriTEC.MongoApplication.DTOs.Messages;
using NutriTEC.MongoApplication.Interfaces;

namespace NutriTEC.MongoApi.Controllers;

[ApiController]
[Route("api/messages")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost]
    public async Task<IActionResult> Send(
        SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _messageService.SendAsync(request, cancellationToken);
        return Created($"/api/messages/{request.NutritionistCode}/{request.ClientId}", response);
    }

    [HttpGet("{nutritionistCode:int}/{clientId:int}")]
    public async Task<IActionResult> GetConversation(
        int nutritionistCode,
        int clientId,
        CancellationToken cancellationToken)
    {
        var response = await _messageService.GetConversationAsync(nutritionistCode, clientId, cancellationToken);
        return Ok(response);
    }

    [HttpPatch("{nutritionistCode:int}/{clientId:int}/read")]
    public async Task<IActionResult> MarkAsRead(
        int nutritionistCode,
        int clientId,
        [FromQuery] int readerId,
        CancellationToken cancellationToken)
    {
        await _messageService.MarkAsReadAsync(nutritionistCode, clientId, readerId, cancellationToken);
        return NoContent();
    }
}
