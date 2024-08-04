namespace CapstoneQuizzCreationApp.CustomException
{
    public class UserCredentialNotFoundException:Exception
    {
        string message;
        public UserCredentialNotFoundException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
