using Microsoft.AspNetCore.Mvc;
using WebPipelineJobQueue.Common.Enumerations;
using WebPipelineJobQueue.Common.Extensions;
using WebPipelineJobQueue.Common.Helpers;
using WebPipelineJobQueue.Common.Items;

namespace WebPipelineJobQueue.WebApi.Controllers
{
    [Route("api/WebPipelineJobQueue/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        [HttpPost("SendMessageToPipeline")]
        public async Task<ActionResult> SendMessageToPipeline(ServerMessageItem serverMessageItem)
        {
            if (serverMessageItem == null || serverMessageItem.ServerMessage.IsNullOrEmpty())
            {
                return await Task.FromResult(BadRequest(new ServerResponseItem { Message = "Invalid parameter!" }));
            }

            var result = PipelineHelper.Instance.SendSync(new CommunicationItem { MessageType = MessageTypesEnum.Test, AdditionalMessage = serverMessageItem.ServerMessage }, "CanoTestPipe2");

            if (result == null || result.AdditionalMessage.IsNullOrEmpty())
            {
                return await Task.FromResult(BadRequest(new ServerResponseItem { Message = "Invalid pipeline response!" }));
            }

            return await Task.FromResult(Ok(new ServerResponseItem { Message = result.AdditionalMessage }));
        }
    }
}
