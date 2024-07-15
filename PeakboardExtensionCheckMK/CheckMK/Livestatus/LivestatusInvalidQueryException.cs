using System;

namespace CheckMK.Livestatus
{
    [Serializable]
    public class LivestatusInvalidQueryException : Exception
    {
        public int Line { get; private set; }
        public int Column { get; private set; }
        public string LQLError { get; private set; }

        public LivestatusInvalidQueryException(int line, int column, string message) : base($"Line {line}, Column {column}\n{message}") { 
            Line = line;
            Column = column;    
            LQLError = message;
        }
    }
}
