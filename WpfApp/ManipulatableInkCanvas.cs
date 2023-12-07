using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System;
using System.Windows.Ink;

namespace WpfApp
{
    public class ManipulatableInkCanvas : InkCanvas
    {
        private UIElement selectedElement;
        private Point previousMousePosition;

        public ManipulatableInkCanvas() : base()
        {
            this.MouseLeftButtonDown += ManipulatableInkCanvas_MouseLeftButtonDown;
            this.MouseMove += ManipulatableInkCanvas_MouseMove;
            this.MouseLeftButtonUp += ManipulatableInkCanvas_MouseLeftButtonUp;
        }

        private void ManipulatableInkCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var hitTestResult = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (hitTestResult != null && hitTestResult.VisualHit is UIElement element)
            {
                selectedElement = element;
                previousMousePosition = e.GetPosition(this);
                element.CaptureMouse();
            }
        }

        private void ManipulatableInkCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && selectedElement != null)
            {
                var currentMousePosition = e.GetPosition(this);
                var angle = GetAngle(previousMousePosition, currentMousePosition);

                RotateTransform rotateTransform = selectedElement.RenderTransform as RotateTransform ?? new RotateTransform();
                rotateTransform.Angle += angle;
                rotateTransform.CenterX = selectedElement.RenderSize.Width / 2;
                rotateTransform.CenterY = selectedElement.RenderSize.Height / 2;
                selectedElement.RenderTransform = rotateTransform;

                previousMousePosition = currentMousePosition;
            }
        }

        private void ManipulatableInkCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedElement != null)
            {
                selectedElement.ReleaseMouseCapture();
                selectedElement = null;
            }
        }

        private double GetAngle(Point previous, Point current)
        {
            // Изчисляване на ъгъла за въртене, базиран на позициите на мишката
            // Това е упрощен пример и може да изисква допълнителна логика
            return Math.Atan2(current.Y - previous.Y, current.X - previous.X) * (180 / Math.PI);
        }

        public void DeleteSelectedElement()
        {
            if (selectedElement != null)
            {
                this.Children.Remove(selectedElement);
                selectedElement = null;
            }
        }



        protected override void OnStrokeCollected(InkCanvasStrokeCollectedEventArgs e)
        {
            base.OnStrokeCollected(e);
            // Тук може да добавим логика, ако е нужно да променим новосъздадения Stroke
        }

        public void RotateSelectedStrokes(double angle)
        {
            var selectedStrokes = this.GetSelectedStrokes();

            if (selectedStrokes.Count == 0) return;

            // Намиране на центъра на селектирания обект/обекти
            Rect bounds = selectedStrokes.GetBounds();
            Point center = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);

            foreach (var stroke in selectedStrokes)
            {
                // Въртене на всяка точка в чертата
                for (int i = 0; i < stroke.StylusPoints.Count; i++)
                {
                    var point = stroke.StylusPoints[i];
                    var rotatedPoint = RotatePoint(point, center, angle);
                    stroke.StylusPoints[i] = new StylusPoint(rotatedPoint.X, rotatedPoint.Y);
                }
            }
        }

        private Point RotatePoint(StylusPoint point, Point center, double angle)
        {
            double radAngle = (Math.PI / 180) * angle;
            double cosTheta = Math.Cos(radAngle);
            double sinTheta = Math.Sin(radAngle);

            var x = cosTheta * (point.X - center.X) - sinTheta * (point.Y - center.Y) + center.X;
            var y = sinTheta * (point.X - center.X) + cosTheta * (point.Y - center.Y) + center.Y;

            return new Point(x, y);
        }

        public void SelectStroke(Stroke stroke)
        {
            // Пример за селектиране на Stroke
            this.Select(new StrokeCollection { stroke });
        }

        private void RotateStroke(Stroke stroke, double angle)
        {
            // Примерна логика за ротация на черта
            Matrix rotationMatrix = new Matrix();
            Rect strokeBounds = stroke.GetBounds();
            Point center = new Point(strokeBounds.X + strokeBounds.Width / 2, strokeBounds.Y + strokeBounds.Height / 2);

            rotationMatrix.RotateAt(angle, center.X, center.Y);

            // Променяме на трансформацията към точките на чертата
            stroke.Transform(rotationMatrix, false);
        }
    }
}
