using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;

namespace BGMChart
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        // OnStartup 메서드를 오버라이드
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var settings = new CefSettings();

            // 자동재생 정책을 무시하도록 설정
            settings.CefCommandLineArgs.Add("autoplay-policy", "no-user-gesture-required");

            Cef.Initialize(settings);

            CefSharpSettings.WcfEnabled = true; // 동기식 JS 바인딩 활성화
        }
    }
}
