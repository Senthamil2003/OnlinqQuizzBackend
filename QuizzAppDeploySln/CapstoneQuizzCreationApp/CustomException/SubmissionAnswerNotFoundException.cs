namespace CapstoneQuizzCreationApp.CustomException
{
    public class SubmissionAnswerNotFoundException:Exception
    {
        string message;
        public SubmissionAnswerNotFoundException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
