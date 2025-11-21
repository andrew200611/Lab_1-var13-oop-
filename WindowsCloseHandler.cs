using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.LifecycleEvents;

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using WinRT.Interop;
#endif

namespace MauiApp1
{
    public static class WindowsCloseHandler
    {
        public static void Init()
        {
#if WINDOWS
            Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
            {
                var nativeWindow = handler.PlatformView;
                var windowHandle = WindowNative.GetWindowHandle(nativeWindow);
                var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
                var appWindow = AppWindow.GetFromWindowId(windowId);

                if (appWindow != null)
                {
                    appWindow.Closing += AppWindow_Closing;
                }
            });
#endif
        }

#if WINDOWS
        private static async void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
        {
           
            args.Cancel = true;

           
            var mainPage = Application.Current?.MainPage;
            if (mainPage == null) return;

           
            bool answer = await mainPage.DisplayAlert(
                "Підтвердження", 
                "Чи дійсно ви хочете вийти?", 
                "Так", 
                "Ні");

           
            if (answer)
            {
                Application.Current?.Quit();
            }
        }
#endif
    }
}