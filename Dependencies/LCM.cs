using System.Numerics;

namespace utilities_cs {
    public class lcm_class {
        public static BigInteger lcm_exec(BigInteger[] element_array) {
            BigInteger lcm_of_array_elements = 1;
            int divisor = 2;

            while (true) {
                int counter = 0;
                bool divisible = false;
                for (int i = 0; i < element_array.Length; i++) {

                    if (element_array[i] == 0) {
                        return 0;
                    } else if (element_array[i] < 0) {
                        element_array[i] = element_array[i] * (-1);
                    }
                    if (element_array[i] == 1) {
                        counter++;
                    }

                    if (element_array[i] % divisor == 0) {
                        divisible = true;
                        element_array[i] = element_array[i] / divisor;
                    }
                }

                if (divisible) {
                    lcm_of_array_elements = lcm_of_array_elements * divisor;
                } else {
                    divisor++;
                }

                if (counter == element_array.Length) {
                    return lcm_of_array_elements;
                }
            }
        }
    }
}