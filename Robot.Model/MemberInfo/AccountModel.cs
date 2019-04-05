using System.Collections.Generic;
using System.Threading;

namespace Robot.Model.MemberInfo
{
    public class AccountModel
    {
        private Dictionary<string, BaseContactModel> contactDic;

        public SyncKeyModel SyncKey { get; set; }

        private static AccountModel single;

        public static AccountModel Instance
        {
            get
            {
                if (single != null)
                    return single;

                AccountModel temp = new AccountModel();
                Interlocked.CompareExchange(ref single, temp, null);

                return temp;
            }
        }

        public BaseContactModel this[string userName]
        {
            get
            {
                if (contactDic != null && contactDic.ContainsKey(userName))
                    return contactDic[userName];

                return null;
            }
        }

        private AccountModel()
        {
            contactDic = new Dictionary<string, BaseContactModel>();
        }

        public void SetContact<T>(List<T> contactList) where T : BaseContactModel
        {
            foreach (T contact in contactList)
            {
                if (!contactDic.ContainsKey(contact.UserName))
                {
                    contactDic.Add(contact.UserName, contact);
                }
                else
                {
                    if (contact.MemberCount > 0)
                    {
                        contactDic[contact.UserName].SetMember(contact.MemberList);
                    }
                }
            }
        }
    }
}
