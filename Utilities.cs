using System;
using System.Collections.Generic;

namespace utilities_cs
{
    class Program
    {
        static Dictionary<string, Action<string>> commands = new()
        {
            { "cursive", Cursive.cursive }
        };

        static void Main(string[] args)
        {
            var cmd = args[0];
            var f = commands.GetValueOrDefault(cmd, (args) => Utils.Notification(
                    "Welp.",
                    "It seems utilities couldn't understand what command you were trying to use."
                )
            );
            f.Invoke(string.Join(" ", args[1..]));
        }

    }

}
