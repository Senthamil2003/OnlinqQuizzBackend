namespace CapstoneQuizzCreationApp.CustomException
{
    public class CertificationTestNotFoundException:Exception
    {
        string message;
        public CertificationTestNotFoundException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
