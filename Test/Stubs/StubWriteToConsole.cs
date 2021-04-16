// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;
using MultiProjPackTool.HelperExtensions;
using Xunit.Abstractions;

namespace Test.Stubs
{
    public class StubWriteToConsole : IWriteToConsole
    {
        private readonly ITestOutputHelper _output;
        private readonly LogLevel _minLevel;

        public StubWriteToConsole(ITestOutputHelper output = null, LogLevel minLevel = LogLevel.Debug)
        {
            _output = output;
            _minLevel = minLevel;
        }

        public string LastMessage { get; private set; }
        public LogLevel LastLogLevel { get; private set; }

        public LogLevel DefaultLogLevel { get; set; }
        public int NumWarnings { get; private set; }
        public void OutputErrorIfAnyWarnings()
        {
            if (NumWarnings > 0)
                LogMessage($"There were {NumWarnings} warnings. The process will not continue.", LogLevel.Error);
        }

        public void LogMessage(string message, LogLevel level, bool warningDoesNotStop = false)
        {
            if (level == LogLevel.Warning && !warningDoesNotStop)
                NumWarnings++;

            if (level >= _minLevel)
                _output?.WriteLine($"{level}: {message}");
            LastMessage = message;
            LastLogLevel = level;
            if (level >= LogLevel.Error)
                throw new Exception("ERROR: " + message);
        }
    }
}