namespace SocleRHHBESSAIH.CustomExceptions
{
    public class InvalidExpenseException : Exception
    {
        public InvalidExpenseException()
        { }

        public InvalidExpenseException(string message)
            : base(message)
        { }

        public InvalidExpenseException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
