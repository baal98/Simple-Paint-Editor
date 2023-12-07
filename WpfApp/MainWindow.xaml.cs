using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private TextBlock SelectedTextBlock; // Държи референция към избрания текстов обект

        public MainWindow()
        {
            InitializeComponent();
            sliderThickness.ValueChanged += SliderThickness_ValueChanged;
        }

        private void ChangeInkColor(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var color = (Color)ColorConverter.ConvertFromString(button.Tag.ToString());
            rotatableInkCanvas.DefaultDrawingAttributes.Color = color;
        }

        private void SliderThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (rotatableInkCanvas != null)
            {
                rotatableInkCanvas.DefaultDrawingAttributes.Width = sliderThickness.Value;
                rotatableInkCanvas.DefaultDrawingAttributes.Height = sliderThickness.Value;
            }
        }

        private void SelectMode(object sender, RoutedEventArgs e)
        {
            rotatableInkCanvas.EditingMode = InkCanvasEditingMode.Select;
        }

        private void EraseMode(object sender, RoutedEventArgs e)
        {
            rotatableInkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
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
                var renderTargetBitmap = new RenderTargetBitmap((int)rotatableInkCanvas.ActualWidth, (int)rotatableInkCanvas.ActualHeight, 96d, 96d, PixelFormats.Default);
                renderTargetBitmap.Render(rotatableInkCanvas);
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
            rotatableInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void RoundBrush(object sender, RoutedEventArgs e)
        {
            rotatableInkCanvas.DefaultDrawingAttributes.StylusTip = StylusTip.Ellipse;
        }

        private void RectangleBrush(object sender, RoutedEventArgs e)
        {
            rotatableInkCanvas.DefaultDrawingAttributes.StylusTip = StylusTip.Rectangle;
        }

        private void RotateButton_Click(object sender, RoutedEventArgs e)
        {
            rotatableInkCanvas.RotateSelectedStrokes(15); // Въртене на 15 градуса
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            // Този код изисква допълнителна логика за избиране на конкретен Stroke
            rotatableInkCanvas.SelectStroke(rotatableInkCanvas.Strokes[0]); 
        }

        private void ChangeSelectedStrokesColor(object sender, RoutedEventArgs e)
        {
            if (rotatableInkCanvas.GetSelectedStrokes().Count == 0) return;

            ComboBoxItem selectedColorItem = colorPicker.SelectedItem as ComboBoxItem;
            if (selectedColorItem == null) return;

            string colorName = selectedColorItem.Tag.ToString();
            Color newColor = (Color)ColorConverter.ConvertFromString(colorName);

            foreach (Stroke stroke in rotatableInkCanvas.GetSelectedStrokes())
            {
                stroke.DrawingAttributes.Color = newColor;
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                SelectedTextBlock = textBlock; // Съхраняване на избрания TextBlock
                textToAdd.Text = textBlock.Text; // Зареждане на текста в полето за редактиране
            }
        }

        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            TextBlock newTextBlock = new TextBlock
            {
                Text = textToAdd.Text,
                Foreground = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.Transparent)
            };

            // Задаване на MouseDown обработчика извън инициализатора на обекта
            newTextBlock.MouseDown += TextBlock_MouseDown;

            rotatableInkCanvas.Children.Add(newTextBlock);
            Canvas.SetLeft(newTextBlock, 100); // Примерна позиция
            Canvas.SetTop(newTextBlock, 100);
        }

        private void EditSelectedTextButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTextBlock != null)
            {
                // Промяна на текста
                SelectedTextBlock.Text = textToAdd.Text;

                // Промяна на цвета на текста
                ComboBoxItem selectedTextColorItem = textColorPicker.SelectedItem as ComboBoxItem;
                if (selectedTextColorItem != null)
                {
                    string colorName = selectedTextColorItem.Tag.ToString();
                    Color newColor = (Color)ColorConverter.ConvertFromString(colorName);
                    SelectedTextBlock.Foreground = new SolidColorBrush(newColor);
                }

                // Промяна на размера на текста
                SelectedTextBlock.FontSize = textSizeSlider.Value;
            }
        }

        private void InsertImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
                Title = "Open Image File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(openFileDialog.FileName)),
                    Width = 100, // Избираме подходяща начална ширина
                    Height = 100, // Избираме подходяща начална височина
                    Stretch = Stretch.Uniform
                };

                InkCanvas.SetLeft(image, 0); // Начална позиция X
                InkCanvas.SetTop(image, 0); // Начална позиция Y

                rotatableInkCanvas.Children.Add(image);
            }
        }

        private void InsertImageButton_Click(object sender, RoutedEventArgs e)
        {
            InsertImage();
        }

        private void DeleteImageButton_Click(object sender, RoutedEventArgs e)
        {
            var inkCanvas = rotatableInkCanvas as ManipulatableInkCanvas;
            inkCanvas?.DeleteSelectedElement();
        }


    }
}
