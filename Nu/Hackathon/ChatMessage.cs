using PX.Data;

namespace Hackathon
{
    public class ChatMessage : IBqlTable
    {
        [PXDBIdentity]
        public int? ChatMessageId
        {
            get;
            set;
        }

        [PXDBInt]
        public int? ConversationId
        {
            get;
            set;
        }

        [PXDBString]
        public string Message
        {
            get;
            set;
        }
    }
}