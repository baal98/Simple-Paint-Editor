using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            sliderThickness.ValueChanged += SliderThickness_ValueChanged;
        }

        private void ChangeInkColor(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var color = (Color)ColorConverter.ConvertFromString(button.Tag.ToString());
            inkCanvas.DefaultDrawingAttributes.Color = color;
        }

        private void SliderThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inkCanvas != null)
            {
                inkCanvas.DefaultDrawingAttributes.Width = sliderThickness.Value;
                inkCanvas.DefaultDrawingAttributes.Height = sliderThickness.Value;
            }
        }

        private void SelectMode(object sender, RoutedEventArgs e)
        {
            inkCanvas.EditingMode = InkCanvasEditingMode.Select;
        }

        private void EraseMode(object sender, RoutedEventArgs e)
        {
            inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        private void SaveInk(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                FileName = "MyDrawing",
                DefaultExt = ".jpg",
                Filter = "JPEG Image (.jpg)|*.jpg"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var renderTargetBitmap = new RenderTargetBitmap((int)inkCanvas.ActualWidth, (int)inkCanvas.ActualHeight, 96d, 96d, PixelFormats.Default);
                renderTargetBitmap.Render(inkCanvas);
                var jpegBitmapEncoder = new JpegBitmapEncoder();
                jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    jpegBitmapEncoder.Save(fileStream);
                }
            }
        }

        private void DrawingMode(object sender, RoutedEventArgs e)
        {
            inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
        }
    }
}