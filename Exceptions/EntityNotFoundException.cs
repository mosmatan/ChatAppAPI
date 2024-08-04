namespace ChatAPI.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string i_Message ="Entity not found"):base(i_Message) { }
    }
}
