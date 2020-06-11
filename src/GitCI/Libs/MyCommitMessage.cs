namespace GitCI.Libs
{
    public class MyCommitMessage
    {
        public string Message { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public static MyCommitMessage Create(string message, string username = null, string email = null)
        {
            return new MyCommitMessage() { Message = message, Username = username, Email = email };
        }
    }
}