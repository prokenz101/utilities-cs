namespace utilities_cs {
    public class Shuffle {
        public static HashSet<string> permutations = new();
        public static void swap(ref char a, ref char b) {
            if (a == b) return;

            var temp = a;
            a = b;
            b = temp;
        }

        public static void getPer(char[] list) {
            int x = list.Length - 1;
            getPer(list, 0, x);
        }

        public static void getPer(char[] list, int k, int m) {
            if (k == m) {
                permutations.Add(string.Join("", list));
            } else {
                for (int i = k; i <= m; i++) {
                    swap(ref list[k], ref list[i]);
                    getPer(list, k + 1, m);
                    swap(ref list[k], ref list[i]);
                }
            }
        }
    }
}
