using System.Collections.Generic;

namespace Robot.Model.RequestModel
{
    public class BatchRequestModel
    {
        public BaseRequestModel BaseRequest { get; set; }

        public int Count { get; set; }

        public LinkedList<BatchItem> List { get; set; }

        public static BatchRequestModel Create()
        {
            BatchRequestModel model = new BatchRequestModel()
            {
                BaseRequest = BaseRequestModel.Create(),
                List = new LinkedList<BatchItem>()
            };

            return model;
        }
    }

    public class BatchItem
    {
        public string EncryChatRoomId { get; set; } = string.Empty;

        public string UserName { get; set; }
    }
}
