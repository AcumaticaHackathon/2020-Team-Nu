using System;
using PX.Data;

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

        [PXDBGuid]
        public Guid? UserId
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

        [PXDBGuid]
        public Guid? SecondUserId
        {
            get;
            set;
        }
    }
}