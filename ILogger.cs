internal interface ILogger
    {
        void Event(string message);
        void Error(string message);

    }