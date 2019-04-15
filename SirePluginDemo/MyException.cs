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

    public class InvalidHexException : Exception
    {
        public InvalidHexException(char input) :
            base(Resources.InvalidHexExMsg + input)
        { }
    }

    public class InvalidScriptException : Exception
    {
        public InvalidScriptException(string scriptName) :
            base(Resources.InvalidHexExMsg + scriptName)
        { }
    }
}