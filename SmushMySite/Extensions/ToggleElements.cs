using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace SmushMySite.Extensions
{
    /// <summary>
    /// This is used to keep the WPF elements snappy. I dont like
    /// the way this is being used, but for now I cant think of a better way of doing this.
    /// </summary>
    public static class ToggleElements
    {
        /// <summary>
        /// Switches a label on or off depending on params passed in.
        /// Will also update the content if there is any passed in.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="isVisible"></param>
        /// <param name="content"></param>
        public static void ToggleLabel(this Label label, bool isVisible, string content = "")
        {
            // Should we show the label or hide it
            Visibility visibility = isVisible ? Visibility.Visible : Visibility.Hidden;

            if (!label.Dispatcher.CheckAccess())
            {
                label.Dispatcher.Invoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(
                        delegate()
                            {
                                // is there any content?
                                if (string.IsNullOrEmpty(content))
                                {
                                    label.Content = content;
                                }

                                label.Visibility = visibility;

                            }
                        ));
            }
            else
            {
                // is there any content?
                if (string.IsNullOrEmpty(content))
                {
                    label.Content = content;
                }

                label.Visibility = visibility;
            }
        }

        /// <summary>
        /// Toggles the progress ring on/off.
        /// </summary>
        public static void ToggleProgressRing(this ProgressRing progressRing, bool isVisible)
        {
            if (!progressRing.Dispatcher.CheckAccess())
            {
                progressRing.Dispatcher.Invoke(
                  System.Windows.Threading.DispatcherPriority.Normal,
                  new Action(
                    delegate()
                    {
                        progressRing.IsActive = isVisible;
                    }
                ));
            }
            else
            {
                progressRing.IsActive = isVisible;
            }
        }

        /// <summary>
        /// Hides/Shows a button depending on the params passed in.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="isVisible"></param>
        public static void ToggleButton(this Button button, bool isVisible)
        {
            // Should we show the label or hide it
            Visibility visibility = isVisible ? Visibility.Visible : Visibility.Hidden;

            if (!button.Dispatcher.CheckAccess())
            {
                button.Dispatcher.Invoke(
                  System.Windows.Threading.DispatcherPriority.Normal,
                  new Action(
                    delegate()
                    {
                        button.Visibility = visibility;
                    }
                ));
            }
            else
            {
                button.Visibility = visibility;
            }
        }

        /// <summary>
        /// Hides/Shows an image depending on the params passed in.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="isVisible"></param>
        public static void ToggleImage(this Rectangle image, bool isVisible)
        {
            // Should we show the label or hide it
            Visibility visibility = isVisible ? Visibility.Visible : Visibility.Hidden;

            if (!image.Dispatcher.CheckAccess())
            {
                image.Dispatcher.Invoke(
                  System.Windows.Threading.DispatcherPriority.Normal,
                  new Action(
                    delegate()
                    {
                        image.Visibility = visibility;
                    }
                ));
            }
            else
            {
                image.Visibility = visibility;
            }
        }
    }
}
