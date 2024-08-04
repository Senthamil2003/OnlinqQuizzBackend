namespace CapstoneQuizzCreationApp.CustomException
{
    public class TestHistoryNotFoundException:Exception
    {
        string message;
        public TestHistoryNotFoundException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
