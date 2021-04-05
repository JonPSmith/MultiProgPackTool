// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;
using MultiProjPackTool.HelperExtensions;

namespace Test.Stubs
{
    public class StubWriteToConsole : IWriteToConsole
    {
        public string LastMessage { get; private set; }
        public LogLevel LastLogLevel { get; private set; }
        public LogLevel HighestLogLevel { get; private set; }

        public LogLevel DefaultLogLevel { get; set; }
        public void LogMessage(string message, LogLevel level)
        {
            LastMessage = message;
            LastLogLevel = level;
            if (level >= LogLevel.Error)
                throw new Exception("ERROR: " + message);
        }
    }
}