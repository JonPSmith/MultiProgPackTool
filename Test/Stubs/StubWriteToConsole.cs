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
        private readonly bool _throwExceptionOnErrors;
        private readonly LogLevel _minLevel = LogLevel.Debug;

        public StubWriteToConsole(ITestOutputHelper output = null, bool throwExceptionOnErrors = true)
        {
            _output = output;
            _throwExceptionOnErrors = throwExceptionOnErrors;
        }

        public string LastMessage { get; private set; }
        public LogLevel HighestLogLevel { get; private set; }

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

            if (level > HighestLogLevel)
                HighestLogLevel = level;

            if (level >= _minLevel)
                _output?.WriteLine($"{level}: {message}");
            LastMessage = message;

            if (level >= LogLevel.Error && _throwExceptionOnErrors)
                throw new Exception("ERROR: " + message);
        }
    }
}