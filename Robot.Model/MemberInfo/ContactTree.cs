using Newtonsoft.Json;
using System.Collections.Generic;

namespace Robot.Model.MemberInfo
{
    public abstract class BaseContactTree<T> where T : BaseContactModel
    {
        protected Dictionary<string, T> contactDic;

        public SyncKeyModel SyncKey { get; set; }

        public abstract List<T> ContactList { get; set; }

        public void Initial()
        {
            if (ContactList?.Count <= 0)
                return;

            contactDic = new Dictionary<string, T>();
            foreach (T contact in ContactList)
            {
                if (contact == null)
                    continue;

                contactDic.Add(contact.UserName, contact);
                contact.Initial();
            }
        }
    }

    public class InitialContactTree : BaseContactTree<InitialContactModel>
    {
        public int Count { get; set; }

        public override List<InitialContactModel> ContactList { get; set; }
    }

    public class MessageContactTree : BaseContactTree<ModContactModel>
    {
        public int AddMsgCount { get; set; }

        public List<MessageModel> AddMsgList { get; set; }

        public int ModContactCount { get; set; }

        [JsonProperty("ModContactList")]
        public override List<ModContactModel> ContactList { get; set; }
    }
}
