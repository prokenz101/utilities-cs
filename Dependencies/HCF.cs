namespace utilities_cs {
    public class HCF {
        public static System.Numerics.BigInteger hcf_exec(
            System.Numerics.BigInteger a, System.Numerics.BigInteger b
        ) {
            if (a == 0)
                return b;
            return hcf_exec(b % a, a);
        }

        // Function to find gcd of 
        // array of numbers
        public static System.Numerics.BigInteger findGCD(
            System.Numerics.BigInteger[] arr,
            System.Numerics.BigInteger n
        ) {
            System.Numerics.BigInteger result = arr[0];
            for (int i = 1; i < n; i++) {
                result = hcf_exec(arr[i], result);

                if (result == 1) {
                    return 1;
                }
            }

            return result;
        }
    }
}