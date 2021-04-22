using System;

namespace SharpCalculatorLib.Exceptions
{
    [Serializable]
    public  class ParenthesisNotMatchingException : Exception
    {
        public ParenthesisNotMatchingException() : base()
        {
        }

        public ParenthesisNotMatchingException(string message) : base(message)
        {
        }

        public ParenthesisNotMatchingException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}