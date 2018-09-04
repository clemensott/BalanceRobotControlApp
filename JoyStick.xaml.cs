using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Benutzersteuerelement" ist unter http://go.microsoft.com/fwlink/?LinkId=234236 dokumentiert.

namespace BalanceRobotControlApp
{
    public sealed partial class JoyStick : UserControl
    {
        private const double maxValueX = 1, maxValueY = 0.7;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(Point), typeof(JoyStick),
                new PropertyMetadata(new Point(), new PropertyChangedCallback(OnValuePropertyChanged)));

        private static void OnValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (JoyStick)sender;
            var value = (Point)e.NewValue;

            s.SetStickPosition();
        }

        private PointerPoint firstPp;
        private List<PointerPoint> pps;

        public Point Value
        {
            get { return (Point)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public JoyStick()
        {
            this.InitializeComponent();

            pps = new List<PointerPoint>();
        }

        private void control_Loaded(object sender, RoutedEventArgs e)
        {
            SetStickPosition();
        }

        private void control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetStickPosition();
        }

        private void SetValue(Point rawPosition)
        {
            double halfStickWidth = elpStick.ActualWidth / 2.0;
            double x = (rawPosition.X / ActualWidth - 0.5) * 2 * maxValueX;
            double y = (rawPosition.Y / ActualHeight - 0.5) * 2 * -maxValueY;

            Value = new Point(x, y);
        }

        private void SetStickPosition()
        {
            double halfStickWidth = elpStick.ActualWidth / 2.0;
            double left = ActualWidth / 2 * (1 + Value.X / maxValueX) - halfStickWidth;
            double top = ActualHeight / 2 * (1 + Value.Y / -maxValueY) - halfStickWidth;

            elpStick.SetValue(Canvas.LeftProperty, left);
            elpStick.SetValue(Canvas.TopProperty, top);
        }

        private void control_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pp = e.GetCurrentPoint(this);

            if (firstPp == null)
            {
                firstPp = pp;
                pps.Add(pp);
                SetValue(pp.Position);
                return;
            }
            else if (firstPp.PointerId == pp.PointerId)
            {
                pps[0] = firstPp = pp;
                SetValue(pp.Position);
                return;
            }

            int i = 0;
            foreach (PointerPoint item in pps.Skip(1))
            {
                i++;
                if (item.PointerId != pp.PointerId) continue;

                pps[i] = pp;
                return;
            }

            pps.Add(pp);
        }

        private void control_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pp = e.GetCurrentPoint(this);

            if (firstPp == null)
            {
                firstPp = pp;
                pps.Add(pp);
                SetValue(pp.Position);
                return;
            }
            else if (firstPp.PointerId == pp.PointerId)
            {
                pps[0] = firstPp = pp;
                SetValue(pp.Position);
                return;
            }

            int i = 0;
            foreach (PointerPoint item in pps.Skip(1))
            {
                i++;
                if (item.PointerId != pp.PointerId) continue;

                pps[i] = pp;
                return;
            }
        }

        private void control_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pp = e.GetCurrentPoint(this);

            pps.RemoveAll(p => p.PointerId == pp.PointerId);
            firstPp = pps.FirstOrDefault();

            if (firstPp != null) SetValue(firstPp.Position);
            else Value = new Point();
        }
    }
}
