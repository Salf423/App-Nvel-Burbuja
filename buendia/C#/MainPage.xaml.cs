using System;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace AccelerometerEssential
{
    public partial class MainPage : ContentPage
    {
        private const double Alpha = 0.15;
        private double _xFiltrado;
        private double _yFiltrado;
        private double _zFiltrado;

        public MainPage()
        {
            InitializeComponent();
            IniciarSensor();
        }

        private void IniciarSensor()
        {
            try
            {
                if (!Accelerometer.IsMonitoring)
                {
                    Accelerometer.Start(SensorSpeed.UI);
                    Accelerometer.ReadingChanged += OnAcelerometroCambio;
                }
            }
            catch (FeatureNotSupportedException)
            {
                Console.WriteLine("El dispositivo no tiene acelerómetro físico.");
            }
        }

        private void OnAcelerometroCambio(object sender, AccelerometerChangedEventArgs e)
        {
            double xPuro = e.Reading.Acceleration.X;
            double yPuro = e.Reading.Acceleration.Y;
            double zPuro = e.Reading.Acceleration.Z;

            _xFiltrado = (xPuro * Alpha) + (_xFiltrado * (1.0 - Alpha));
            _yFiltrado = (yPuro * Alpha) + (_yFiltrado * (1.0 - Alpha));
            _zFiltrado = (zPuro * Alpha) + (_zFiltrado * (1.0 - Alpha));

            double pitchRadianes = Math.Atan2(-_xFiltrado, Math.Sqrt(_yFiltrado * _yFiltrado + _zFiltrado * _zFiltrado));
            double pitchGrados = pitchRadianes * (180.0 / Math.PI);
            double rollRadianes = Math.Atan2(_yFiltrado, _zFiltrado);
            double rollGrados = rollRadianes * (180.0 / Math.PI);

            Device.BeginInvokeOnMainThread(() =>
            {
                double factorEscala = 2.5;
                double desplazamientoX = rollGrados * factorEscala;
                double desplazamientoY = pitchGrados * factorEscala;

                desplazamientoX = Math.Max(-100, Math.Min(desplazamientoX, 100));
                desplazamientoY = Math.Max(-100, Math.Min(desplazamientoY, 100));

                miBurbuja.TranslationX = desplazamientoX;
                miBurbuja.TranslationY = desplazamientoY;
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (Accelerometer.IsMonitoring)
            {
                Accelerometer.ReadingChanged -= OnAcelerometroCambio;
                Accelerometer.Stop();
            }
        }
    }
}