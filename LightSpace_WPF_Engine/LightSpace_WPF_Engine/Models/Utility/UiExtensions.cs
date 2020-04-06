using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LightSpace_WPF_Engine.Models.Utility
{
    public static class UiExtensions
    {
        private static readonly Action EmptyDelegate = delegate () { };

        // Force refresh on UIElement when used.
        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
