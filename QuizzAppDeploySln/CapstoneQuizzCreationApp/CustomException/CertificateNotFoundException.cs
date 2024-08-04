namespace CapstoneQuizzCreationApp.CustomException
{
    public class CertificateNotFoundException:Exception
    {
        string message;
        public CertificateNotFoundException(string message)
        {
            this.message = message; 
        }
        public override string Message =>message;
    }
}
