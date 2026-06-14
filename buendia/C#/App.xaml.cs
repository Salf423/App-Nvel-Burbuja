using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace AccelerometerEssential
{
    public partial class App : Application
    {
        private const string PrimeraEjecucionKey = "PrimeraEjecucion";

        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            bool primeraVez = Preferences.Get(PrimeraEjecucionKey, true);

            if (primeraVez)
            {
                Console.WriteLine("Inicializando registros y parámetros de telemetría por defecto.");
                Preferences.Set(PrimeraEjecucionKey, false);
            }
        }

        protected override void OnSleep()
        {
            System.Diagnostics.Debug.WriteLine("App en pausa.");
        }

        protected override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine("App reanudada.");
        }
    }
}
