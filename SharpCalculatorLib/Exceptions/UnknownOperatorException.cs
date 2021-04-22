using System;

namespace SharpCalculatorLib.Exceptions
{
    [Serializable]
    public class UnknownOperatorException : Exception
    {
        public UnknownOperatorException() : base()
        {
        }

        public UnknownOperatorException(string message) : base(message)
        {
        }

        public UnknownOperatorException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}