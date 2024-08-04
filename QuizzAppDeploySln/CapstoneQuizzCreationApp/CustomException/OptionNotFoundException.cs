namespace CapstoneQuizzCreationApp.CustomException
{
    public class OptionNotFoundException:Exception
    {
        string message;
        public OptionNotFoundException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
