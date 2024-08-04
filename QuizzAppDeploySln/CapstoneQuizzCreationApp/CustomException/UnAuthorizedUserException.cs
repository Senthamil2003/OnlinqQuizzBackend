namespace CapstoneQuizzCreationApp.CustomException
{
    public class UnAuthorizedUserException:Exception
    {
        string messge;
        public UnAuthorizedUserException(string messge)
        {
            this.messge = messge;
        }
        public override string Message => Message;
    }
}
