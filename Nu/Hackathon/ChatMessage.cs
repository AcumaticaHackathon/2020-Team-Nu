using PX.Data;
using PX.Data.BQL;

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

        public abstract class chatMessageId : BqlInt.Field<chatMessageId>
        {
        }

        public abstract class conversationId : BqlInt.Field<conversationId>
        {
        }

        public abstract class message : BqlString.Field<message>
        {
        }
    }
}