namespace SmushMySite
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    public static class ExtensionMethods
    {
        private static readonly Action EmptyDelegate = delegate() { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
