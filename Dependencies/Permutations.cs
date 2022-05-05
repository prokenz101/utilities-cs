namespace utilities_cs {
    /// <summary>
    /// Permutations code copied from stack overflow ¯\_(ツ)_/¯
    /// </summary>
    /// <remarks>https://stackoverflow.com/questions/756055/listing-all-permutations-of-a-string-integer</remarks>
    class Permutations {
        public HashSet<string> Permutation { get; set; } = new();
        private static void Swap(ref char a, ref char b) {
            if (a == b) return;

            var temp = a;
            a = b;
            b = temp;
        }

        public void GetPer(char[] list) {
            HashSet<string> permutations = new();

            int x = list.Length - 1;
            GetPer(list, 0, x);
        }

        private void GetPer(char[] list, int k, int m) {
            if (k == m) {
                this.Permutation.Add(new string(list));
            } else {
                for (int i = k; i <= m; i++) {
                    Swap(ref list[k], ref list[i]);
                    GetPer(list, k + 1, m);
                    Swap(ref list[k], ref list[i]);
                }
            }
        }
    }
}