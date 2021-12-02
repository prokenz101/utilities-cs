using System;
using System.Collections.Generic;

namespace utilities_cs
{
    class Program
    {
        static Dictionary<string, Action<string[]>> commands = new()
        {
            { "cursive", Cursive.cursive }, { "copypaste", Copypaste.cp }, { "cp", Copypaste.cp },
            { "upper", Upper.Uppercase }, { "lower", Lower.Lowercase }, { "sarcasm", Sarcasm.Sarcasm_ }
        };

        static void Main(string[] args)
        {
            var cmd = args[0].ToLower();
            var f = commands.GetValueOrDefault(cmd, (args) => Utils.Notification(
                    "Welp.",
                    "It seems utilities couldn't understand what command you were trying to use.",
                    6
                )
            );
            f.Invoke(args);
        }

    }

}
