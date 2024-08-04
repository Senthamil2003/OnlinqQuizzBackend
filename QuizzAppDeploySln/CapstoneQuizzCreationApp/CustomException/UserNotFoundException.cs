namespace CapstoneQuizzCreationApp.CustomException
{
    public class UserNotFoundException:Exception
    {
        string message;
        public UserNotFoundException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
