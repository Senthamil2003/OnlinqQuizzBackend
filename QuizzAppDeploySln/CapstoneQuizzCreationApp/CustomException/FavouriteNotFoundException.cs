namespace CapstoneQuizzCreationApp.CustomException
{
    public class FavouriteNotFoundException:Exception
    {
        string message;
        public FavouriteNotFoundException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
