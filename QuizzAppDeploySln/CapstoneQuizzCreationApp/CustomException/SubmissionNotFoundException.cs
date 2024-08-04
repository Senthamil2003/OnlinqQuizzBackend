namespace CapstoneQuizzCreationApp.CustomException
{
    public class SubmissionNotFoundException:Exception
    {
        string message;
        public SubmissionNotFoundException(string message)
        {
            this.message = message; 
        }
        public override string Message => message;
    }
}
