using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Graphics;

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

            // Calculamos la inclinación lateral (eje X)
            double rollRadianes = Math.Atan2(_xFiltrado, Math.Sqrt(_yFiltrado * _yFiltrado + _zFiltrado * _zFiltrado));
            double rollGrados = rollRadianes * (180.0 / Math.PI);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Tubo: 320 px ancho. Burbuja: 60 px ancho.
                // Limite físico de desplazamiento desde el centro = (320 - 60) / 2 = 130
                // Mapeo: Si se inclina 45 grados, llega al límite del tubo
                const double pixelesPorGrado = 130.0 / 45.0; // aprox 2.88
                double desplazamientoX = rollGrados * pixelesPorGrado;
                
                // Restringir el movimiento de la burbuja para que no salga del tubo
                desplazamientoX = Math.Max(-130, Math.Min(desplazamientoX, 130));

                // La burbuja va hacia el lado más alto. 
                miBurbuja.TranslationX = -desplazamientoX;

                // Actualizar etiqueta de grados (con 1 decimal)
                lblGrados.Text = $"{Math.Abs(rollGrados):F1}°";
                
                // Efecto visual: cambiar a verde cuando está perfectamente nivelado (< 1 grado)
                if (Math.Abs(rollGrados) < 1.0)
                {
                    lblGrados.TextColor = Color.FromArgb("#39ff14"); // Verde neón
                }
                else
                {
                    lblGrados.TextColor = Colors.White;
                }
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
