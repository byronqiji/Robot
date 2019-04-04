using Newtonsoft.Json;

namespace Robot.Model.MemberInfo
{
    public class InitialContactModel : BaseContactModel
    {     
        public override int ContactFlag { get; set; } //0,
    }

    public class ModContactModel : BaseContactModel
    {
        [JsonProperty("ContactType")]
        public override int ContactFlag { get; set; } //0,
    }
}
