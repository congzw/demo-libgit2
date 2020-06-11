namespace GitCI.Libs
{
    public class MessageResult
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public object Data { get; set; }

        public MessageResult WithData(object data)
        {
            Data = data;
            return this;
        }

        public static MessageResult CreateSuccess(string message)
        {
            return new MessageResult() { Success = true, Message = message };
        }
        public static MessageResult CreateFail(string message)
        {
            return new MessageResult() { Success = false, Message = message };
        }
    }
}