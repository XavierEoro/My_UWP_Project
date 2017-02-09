using System;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Coding4Fun
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ink.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Touch;

            InkDrawingAttributes drawingAttributes = new InkDrawingAttributes();
            drawingAttributes.Color = Windows.UI.Colors.Black;
            drawingAttributes.IgnorePressure = false;
            drawingAttributes.FitToCurve = true;
            ink.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
        }

        private string[] CapsAlpha = new string[] { "Ant", "ant", "Bear", "bear", "Cat", "cat", "Dog", "dog", "Elephant", "elephant", "Frog", "frog", "Giraffe", "giraffe", "Hippo", "hippo", "Ice-Cream", "ice-cream", "Jellyfish", "jellyfish", "Kite", "kite", "Lion", "lion", "Mouse", "mouse", "Nest", "nest", "Owl", "owl",
            "Parrot", "parrot", "Queen", "queen", "Rabbit", "rabbit", "Sheep", "sheep", "Toy", "toy", "Umbrella", "umbrella", "Vulture", "vulture", "Watch", "watch", "X-ray", "x-ray", "Yak", "yak", "Zebra", "zebra" };

        private int currentIndex = 0;

        private void WPrevTap(object sender, TappedRoutedEventArgs e)
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = 0;
                return;
            }

            Trans_Tex.Source = new BitmapImage(new Uri("ms-appx:///Assets/1.png"));
            Alph_cap.Source = new BitmapImage(new Uri("ms-appx:///Assets/2.jpg"));
            CapAlphaName.Text = CapsAlpha[currentIndex];
            //var speechText = this.CapAlphaName.Text;
        }

        private void W_FwdTap(object sender, TappedRoutedEventArgs e)
        {
            currentIndex++;
            if (currentIndex > 51)
            {
                currentIndex = 25;
                return;
            }
            Trans_Tex.Source = new BitmapImage(new Uri("ms-appx:///Assets/1.png"));
            Alph_cap.Source = new BitmapImage(new Uri("ms-appx:///Assets/2.jpg"));
            CapAlphaName.Text = CapsAlpha[currentIndex];
        }

        private void W_MactiveTap(object sender, TappedRoutedEventArgs e)
        {
            if ((W_Mactive.Source as BitmapImage).UriSource == new Uri("ms-appx:///Assets/2.jpg", UriKind.Absolute))
            {
                W_Mactive.Source = new BitmapImage(new Uri("ms-appx:///Assets/2.jpg"));
                mediaWorkbook.Stop();
            }
            else
            {
                W_Mactive.Source = new BitmapImage(new Uri("ms-appx:///Assets/2.jpg"));
                mediaWorkbook.Play();
            }
        }

        private void W_Paintt(object sender, TappedRoutedEventArgs e)
        {
            W_Paints.Visibility = W_Paints.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CPtest_ColorChanged(object sender, Windows.UI.Color color)
        {
            ink.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Touch;

            // Set initial ink stroke attributes.
            InkDrawingAttributes drawingAttributes = new InkDrawingAttributes();
            drawingAttributes.Color = color;
            drawingAttributes.IgnorePressure = false;
            drawingAttributes.FitToCurve = true;
            ink.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);

            W_Paints.Visibility = W_Paints.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            this.ink.InkPresenter.InputProcessingConfiguration.Mode = InkInputProcessingMode.Inking;
        }

        private void W_ClearTap(object sender, TappedRoutedEventArgs e)
        {
            this.ink.InkPresenter.InputProcessingConfiguration.Mode = InkInputProcessingMode.Erasing;
            //ink.InkPresenter.Strokes.Clear();
            //Windows.UI.Input.Inking.InkInputProcessingMode.Erasing;
            //this.ink.Strokes.Remove();
        }

        private void HomePage(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}