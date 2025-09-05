using WpfSimpleViewManager.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfSimpleViewManager.Extensions
{
    internal static class WindowUtil
    {
        public static Window? GetActiveWindow()
        {
            if (!Application.Current.Dispatcher.CheckAccess())
                return Application.Current.Dispatcher.Invoke(GetActiveWindow);

            // 우리 앱에서 활성인 창
            return Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
                   ?? Application.Current.MainWindow; // 백업
        }
    }
    
}
