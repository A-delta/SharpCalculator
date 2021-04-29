using System;

namespace DieseCalcLib.Exceptions
{
    [Serializable]
    public class IllegalVariableNameException : Exception

    {
        public IllegalVariableNameException() : base()
        {
        }

        public IllegalVariableNameException(string message) : base(message)
        {
        }

        public IllegalVariableNameException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}