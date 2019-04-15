using System;
using System.Runtime.CompilerServices;

namespace SirePluginDemo
{
    public class GameNotStartedException : Exception
    {
        public GameNotStartedException() : 
            base(Resources.GameNotStartedExMsg)
        {}
    }

    public class ReadMemoryException : Exception
    {
        public ReadMemoryException() :
            base(Resources.ReadMemExMsg)
        { }
    }

    public class InvalidCodeInput : Exception
    {
        public InvalidCodeInput(char input) :
            base(Resources.NotHexMsg + input)
        { }
    }
}