namespace CapstoneQuizzCreationApp.CustomException
{
    public class QuestionNotFoundException:Exception
    {
        string message;
        public QuestionNotFoundException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
