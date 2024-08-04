namespace ChatAPI.Exceptions
{
    public class AuthenticateException :Exception
    {
        public AuthenticateException(string i_Message = "Invalid password") : base(i_Message) { }
    }
}
