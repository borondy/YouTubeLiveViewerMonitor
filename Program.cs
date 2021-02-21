using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace YoutubeViewerMonitor
{
    class Program
    {
        public const string apiKey = "";
        public static DateTime endingTime = new DateTime(2020, 2, 21, 20, 00, 00);
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No video id provided.");
                return;
            }

            var videoId = args[0];
            while (DateTime.Now > endingTime)
            {
                await Log((await GetViewersCount(videoId)).ToString());
                await Task.Delay(10000);
            }
            var viewersCount = await GetViewersCount(videoId);

        }

        static async Task<int> GetViewersCount(string videoId)
        {

            HttpClient client = new HttpClient();
            try
            {
                var response = await client.GetFromJsonAsync<Response>($"https://www.googleapis.com/youtube/v3/videos?part=liveStreamingDetails&id={videoId}&fields=items%2FliveStreamingDetails%2FconcurrentViewers&key={apiKey}");
                var res = response.Items[0].LiveStreamingDetails.ConcurrentViewers;
                System.Console.WriteLine(BuildLogMessage(res.ToString()));
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return -1;
        }

        static async Task Log(string message)
        {
            using (var sw = new StreamWriter($"log.txt", true))
            {
                await sw.WriteLineAsync(BuildLogMessage(message));
            }
        }

        static string BuildLogMessage(string message) => $"{DateTime.Now}; {message}";

    }
    public class Response
    {
        public ResponseItem[] Items { get; set; }
    }

    public class ResponseItem
    {
        public LiveStreamingDetails LiveStreamingDetails { get; set; }
    }

    public class LiveStreamingDetails
    {
        public int ConcurrentViewers { get; set; }
    }
}
