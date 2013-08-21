#define DEBUG_AGENT

using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System.Linq;
using System.IO.IsolatedStorage;
using CWBWeather.Model;
using Microsoft.Phone.Info;
using System;
using WeatherForecast;

namespace WeatherScheduledTaskAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent 建構函式，會初始化 UnhandledException 處理常式
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // 訂閱 Managed 例外狀況處理常式
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// 發生未處理的例外狀況時要執行的程式碼
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 發生未處理的例外狀況; 切換到偵錯工具
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// 執行排程工作的代理程式
        /// </summary>
        /// <param name="task">
        /// 叫用的工作
        /// </param>
        /// <remarks>
        /// 這個方法的呼叫時機為叫用週期性或耗用大量資料的工作時
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            TileUpdateWorker tileUpdateWorker = new TileUpdateWorker();
            tileUpdateWorker.UpdateCompleted += ((sender) =>
            {
                NotifyComplete();
            });
            tileUpdateWorker.Update();
        }
    }
}