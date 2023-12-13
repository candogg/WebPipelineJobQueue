using WebPipelineJobQueue.Common.Enumerations;

namespace WebPipelineJobQueue.Common.Items
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public class CommunicationItem
    {
        public CommunicationItem()
        { }

        public MessageTypesEnum MessageType { get; set; }
        public string AdditionalMessage { get; set; }
    }
}
