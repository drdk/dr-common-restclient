using System;

namespace DR.Common.RESTClient
{
    public enum ConsolePrinting { Enabled, Disabled }

    public interface IConsolePrinter
    {
        ConsolePrinting Status { get; set; }

        void PrettyPrint<T>(T param);
        void PrettyPrint<T>(T param, ConsoleColor foreground);
        void PrettyPrint<T>(T param, ConsoleColor foreground, ConsoleColor background);

        void Write<T>(T param);
        void Write(string format, object arg0);
        void Write(string format, params object[] args);

        void WriteLine();
        void WriteLine<T>(T param);
        void WriteLine(string format, object arg0);
        void WriteLine(string format, params object[] args);

        void Clear();
    }

}
