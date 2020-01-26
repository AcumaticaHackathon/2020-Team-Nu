using System.Collections;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.SM;

namespace Hackathon
{
    public class ChatMaint : PXGraph<ChatMaint>
    {
        public SelectFrom<Conversation>.View Conversation;

        public SelectFrom<ChatMessage>
            .Where<ChatMessage.conversationId.IsEqual<Conversation.conversationId.FromCurrent>>.View Messages;

        public SelectFrom<CRActivity>.View Activities;

        public SelectFrom<Users>.View Users;

        public IEnumerable CreateActivity(PXAdapter adapter)
        {
            var activity = CreateChat();
            Activities.Insert(activity);
            Activities.Cache.PersistInserted(activity);
            return adapter.Get();
        }

        private CRActivity CreateChat()
        {
            return new CRActivity
            {
                Type = "C",
                RefNoteID = Conversation.Current.RelatedEntityNoteId,
                Body = Messages.Current.Message
            };
        }
    }
}