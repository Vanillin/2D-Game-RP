using System;

namespace TwoD_Game_RP
{
    internal class CustomException : Exception
    {
        public CustomException() : base() { }
        public CustomException(string message) : base(message) { }
    }
}
