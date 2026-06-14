using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;

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
            IniciarSensorAsync();
        }

        private async void IniciarSensorAsync()
        {
            bool permisoConcedido = await RequestSensorPermissionAsync();
            if (!permisoConcedido)
                return;

            try
            {
                if (!Accelerometer.Default.IsMonitoring)
                {
                    Accelerometer.Default.ReadingChanged += OnAcelerometroCambio;
                    Accelerometer.Default.Start(SensorSpeed.UI);
                }
            }
            catch (FeatureNotSupportedException)
            {
                Console.WriteLine("El dispositivo no tiene acelerómetro físico.");
            }
        }

        private async Task<bool> RequestSensorPermissionAsync()
        {
            if (DeviceInfo.Platform != DevicePlatform.Android)
                return true;

            var status = await Permissions.CheckStatusAsync<Permissions.Sensors>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.Sensors>();

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permiso requerido", "La app necesita permiso para acceder a los sensores del dispositivo.", "OK");
                return false;
            }

            return true;
        }

        private void OnAcelerometroCambio(object? sender, AccelerometerChangedEventArgs e)
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

            MainThread.BeginInvokeOnMainThread(() =>
            {
                const double factorEscala = 2.5;
                double desplazamientoX = Math.Max(-100, Math.Min(rollGrados * factorEscala, 100));
                double desplazamientoY = Math.Max(-100, Math.Min(pitchGrados * factorEscala, 100));

                miBurbuja.TranslationX = desplazamientoX;
                miBurbuja.TranslationY = desplazamientoY;
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (Accelerometer.Default.IsMonitoring)
            {
                Accelerometer.Default.ReadingChanged -= OnAcelerometroCambio;
                Accelerometer.Default.Stop();
            }
        }
    }
}
