using System;

namespace WeatherForecast.Util
{
    public class DownloadStringCompletedEventArgs : EventArgs
    {
        public static string NETWORK_UNAVAILABLE = "Network unavailable";
        public static string DOWNLOAD_FAILED = "Download failed";
        public static string DOWNLOAD_EXCEPTION = "Download exception";

        public DownloadStringCompletedEventArgs(string result)
        {
            Result = result;
        }

        public DownloadStringCompletedEventArgs(Exception ex)
        {
            Error = ex;
        }

        public string Result { get; private set; }
        public Exception Error { get; private set; }
    }
}
