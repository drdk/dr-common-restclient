using System;
using System.Reflection;

namespace DR.Common.RESTClient
{
    public class ConsolePrinter : IConsolePrinter
    {
        ConsoleColor ForegroundDefault = ConsoleColor.DarkGreen;
        ConsoleColor BackgroundDefault = ConsoleColor.Black;

        public ConsolePrinting Status { get; set; }

        bool Enabled { get { return Status == ConsolePrinting.Enabled; } }

        public ConsolePrinter(ConsolePrinting printing)
        {
            Status = printing;

            if (Enabled)
            {
                Console.ForegroundColor = ForegroundDefault;
                Console.BackgroundColor = BackgroundDefault;
                Console.Clear();
            }
        }

        public void PrettyPrint<T>(T param)
        {
            PrettyPrint(param, ForegroundDefault, BackgroundDefault);
        }

        public void PrettyPrint<T>(T param, ConsoleColor foreground)
        {
            PrettyPrint(param, foreground, BackgroundDefault);
        }

        public void PrettyPrint<T>(T param, ConsoleColor foreground, ConsoleColor background)
        {
            if (Enabled)
            {
                Console.ForegroundColor = foreground;
                Console.BackgroundColor = background;
                Write(param);
                Console.ForegroundColor = ForegroundDefault;
                Console.BackgroundColor = BackgroundDefault;
            }
        }

        public void WriteLine<T>(T param)
        {
            if (Enabled)
            {
                callMethod("WriteLine", param);
            }
        }

        public void WriteLine()
        {
            if (Enabled) { Console.WriteLine(); }
        }

        public void WriteLine(string format, object arg0)
        {
            WriteLine(format, new object[] { arg0 });
        }

        public void WriteLine(string format, object[] args)
        {
            if (Enabled) { Console.WriteLine(format, args); }
        }

        public void Write<T>(T param)
        {
            if (Enabled)
            {
                callMethod("Write", param);
            }
        }

        public void Write(string format, object arg0)
        {
            Write(format, new object[] { arg0 });
        }

        public void Write(string format, params object[] args)
        {
            if (Enabled) { Console.Write(format, args); }
        }

        public void Clear()
        {
            Console.Clear();
        }

        private void callMethod<T>(string methodname, T param)
        {
            var method = typeof(Console).GetMethod(methodname,
                BindingFlags.Static | BindingFlags.Public,
                null,
                CallingConventions.Any,
                new Type[] { typeof(T) },
                new ParameterModifier[] { new ParameterModifier(1) }
            );

            method.Invoke(null, new object[] { param });
        }
    }
}
