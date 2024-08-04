namespace CapstoneQuizzCreationApp.CustomException
{
    public class EmptyItemException:Exception
    {
        string message;
        public EmptyItemException(string message)
        {
            this.message = message;

        }
        public override string Message =>message;
    }
}
