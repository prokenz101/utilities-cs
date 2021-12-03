using System.Diagnostics;

namespace utilities_cs {
    public class BrowserSearch {
        public static void GoogleSearch(string[] args) {
            string search_query = string.Join("+", args[1..]);
            Process.Start(new ProcessStartInfo(
                "cmd", $"/c start https://google.com/search?q={search_query}"
            ) { CreateNoWindow = true });
        }

        public static void YouTubeSearch(string[] args) {
            string search_query = string.Join("+", args[1..]);
            Process.Start(new ProcessStartInfo(
                "cmd", $"/c start https://youtube.com/results?search_query={search_query}"
            ) { CreateNoWindow = true });
        }

        public static void ImageSearch(string[] args) {
            string search_query = string.Join("+", args[1..]);
            Process.Start(new ProcessStartInfo(
                "cmd", $"/c start https://www.google.com/search?q={search_query}^&safe=strict^&tbm=isch^&sxsrf=ALeKk029ouHDkHfq3RFVc8WpFzOvZZ8s4g%3A1624376552976^&source=hp^&biw=1536^&bih=763^&ei=6ATSYIOrOduJhbIPzda7yAs^&oq=hello^&gs_lcp=CgNpbWcQAzIFCAAQsQMyBQgAELEDMgIIADICCAAyAggAMgIIADICCAAyBQgAELEDMgUIABCxAzICCAA6BwgjEOoCECc6BAgjECc6CAgAELEDEIMBUNIGWKcJYLELaABwAHgAgAGPAogByAqSAQUwLjEuNZgBAKABAaoBC2d3cy13aXotaW1nsAEK^&sclient=img^&ved=0ahUKEwiDv62byqvxAhXbREEAHU3rDrkQ4dUDCAc^&uact=5"
            ) { CreateNoWindow = true });
        }
    }
}