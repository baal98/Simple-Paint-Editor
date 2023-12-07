using System;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;

namespace WpfApp
{
    public class RotatableInkCanvas : InkCanvas
    {
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