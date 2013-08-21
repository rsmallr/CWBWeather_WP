using System.Net;
using System.Net.NetworkInformation;
using System;

namespace WeatherForecast.Util
{
    public class ForecastDownloader
    {
        public delegate void DownloadStringCompletedHandler(object sender, DownloadStringCompletedEventArgs e);

        public event DownloadStringCompletedHandler DownloadStringCompleted;

        public ForecastDownloader()
        {
        }

        public void DownloadStringAsync(Uri uri)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                try
                {
                    WebClient downloader = new WebClient();
                    downloader.DownloadStringCompleted += ((s, a) =>
                    {
                        try
                        {
                            if (DownloadStringCompleted != null)
                            {
                                DownloadStringCompletedEventArgs eventArg;
                                if (a.Result == null || a.Error != null)
                                    eventArg = new DownloadStringCompletedEventArgs(new Exception(DownloadStringCompletedEventArgs.DOWNLOAD_FAILED));
                                else
                                    eventArg = new DownloadStringCompletedEventArgs(a.Result);

                                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(delegate()
                                {
                                    DownloadStringCompleted(this, eventArg);
                                });
                            }
                        }
                        catch (WebException ex)
                        {
                            if (DownloadStringCompleted != null)
                            {
                                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(delegate()
                                {
                                    DownloadStringCompleted(this, new DownloadStringCompletedEventArgs(new Exception(DownloadStringCompletedEventArgs.DOWNLOAD_EXCEPTION)));
                                });
                            }
                        }
                    });
                    downloader.DownloadStringAsync(uri);
                }
                catch (Exception ex)
                {
                    if (DownloadStringCompleted != null)
                    {
                        System.Windows.Deployment.Current.Dispatcher.BeginInvoke(delegate()
                        {
                            DownloadStringCompleted(this, new DownloadStringCompletedEventArgs(new Exception(DownloadStringCompletedEventArgs.DOWNLOAD_EXCEPTION)));
                        });
                    }
                }
            }
            else
            {
                if (DownloadStringCompleted != null)
                {
                    System.Windows.Deployment.Current.Dispatcher.BeginInvoke(delegate()
                    {
                        DownloadStringCompleted(this, new DownloadStringCompletedEventArgs(new Exception(DownloadStringCompletedEventArgs.NETWORK_UNAVAILABLE)));
                    });
                }
            }
        }
    }
}
