using System;
using PX.Data;
using PX.Data.BQL;

namespace Hackathon
{
    public class Conversation : IBqlTable
    {
        [PXDBIdentity]
        public int? ConversationId
        {
            get;
            set;
        }

        [PXDBString]
        public Guid? UserName
        {
            get;
            set;
        }

        [PXDBGuid]
        public Guid? RelatedEntityNoteId
        {
            get;
            set;
        }

        [PXDBString]
        public Guid? SecondUserName
        {
            get;
            set;
        }

        public abstract class conversationId : BqlInt.Field<conversationId>
        {
        }

        public abstract class userName : BqlString.Field<userName>
        {
        }

        public abstract class secondUserName : BqlString.Field<secondUserName>
        {
        }

        public abstract class relatedEntityNoteId : BqlGuid.Field<relatedEntityNoteId>
        {
        }
    }
}