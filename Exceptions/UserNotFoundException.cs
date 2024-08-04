namespace ChatAPI.Exceptions
{
    public class UserNotFoundException: EntityNotFoundException
    {
        public UserNotFoundException(string i_Message = "User not found"):base(i_Message) { }

        public UserNotFoundException(int i_UserId):base($"User with id {i_UserId} not found") { }
    }
}
