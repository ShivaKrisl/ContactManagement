namespace Exceptions
{
    public class PersonNotFoundException : ArgumentException
    {
        public PersonNotFoundException() : base()
        {

        }

        public PersonNotFoundException(string? message) : base(message)
        {

        }

        public PersonNotFoundException(string? message, Exception innerException) : base(message, innerException)
        {

        }

    }
}
