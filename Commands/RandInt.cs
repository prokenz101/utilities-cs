using System;
using System.Collections.Generic;

namespace utilities_cs {
    public class RandInt {
        public static string? Randint(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) {
                return null;
            }

            string text = string.Join(" ", args[1..]);
            List<int> nums = Utils.RegexFindAllInts(text);

            Random rand = new Random(); 
            int randint = rand.Next(nums[0], nums[1]);

            Utils.CopyCheck(copy, randint.ToString());
            Utils.NotifCheck(notif, new string[] { "Success!", $"The number was: {randint}", "5" });
            return randint.ToString();
        }
    }
}
