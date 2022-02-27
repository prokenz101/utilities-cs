namespace utilities_cs {
    public class Shuffle {
        public static void swap(ref char a, ref char b) {
            if (a == b) return;

            var temp = a;
            a = b;
            b = temp;
        }

        public static HashSet<string> getPer(char[] list) {
            int x = list.Length - 1;
            HashSet<string> permutations = getPer(list, 0, x);
            
            return permutations;
        }

        public static HashSet<string> getPer(char[] list, int k, int m) {
            HashSet<string> permutations = new();
            if (k == m) {
                permutations.Add(string.Join("", list));
            } else {
                for (int i = k; i <= m; i++) {
                    swap(ref list[k], ref list[i]);
                    getPer(list, k + 1, m);
                    swap(ref list[k], ref list[i]);
                }
            }
            
            return permutations;
        }
    }
}
