using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace test_app2.AttachedProperties
{
    public static class HorizontalScrolling
    {
        public static readonly DependencyProperty UseHorizontalScrollingProperty =
            DependencyProperty.RegisterAttached(
                "UseHorizontalScrolling",
                typeof(bool),
                typeof(HorizontalScrolling),
                new PropertyMetadata(
                    new PropertyChangedCallback(OnHorizontalScrollingValueChanged)));

        public static bool GetUseHorizontalScrollingValue(DependencyObject d)
        {
            return (bool)d.GetValue(UseHorizontalScrollingProperty);
        }

        public static void SetUseHorizontalScrollingValue(DependencyObject d, bool value)
        {
            d.SetValue(UseHorizontalScrollingProperty, value);
        }

        public static void OnHorizontalScrollingValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is UIElement element)
            {
                element.PreviewMouseWheel -= OnPreviewMouseWheel;
                element.PreviewMouseWheel += OnPreviewMouseWheel;
            }
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs args)
        {
            int horizontalScrollingAmount = 3;

            if (sender is UIElement element)
            {
                DependencyObject senderDp = sender as DependencyObject;
                ScrollViewer scrollViewer = FindDescendant<ScrollViewer>(element);

                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) || Mouse.MiddleButton == MouseButtonState.Pressed)
                {
                    if (scrollViewer == null)
                        return;

                    if (args.Delta < 0)
                        for (int i = 0; i < horizontalScrollingAmount; i++)
                            scrollViewer.LineRight();
                    else
                        for (int i = 0; i < horizontalScrollingAmount; i++)
                            scrollViewer.LineLeft();

                    args.Handled = true;
                }
            }
        }

        private static T FindDescendant<T>(DependencyObject d) where T : DependencyObject
        {
            if (d == null)
                return null;

            int childCount = VisualTreeHelper.GetChildrenCount(d);

            for (var i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(d, i);
                T result = child as T ?? FindDescendant<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
