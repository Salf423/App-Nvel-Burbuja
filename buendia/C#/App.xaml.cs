using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
            bool primeraVez = Xamarin.Essentials.Preferences.Get(PrimeraEjecucionKey, true);

            if (primeraVez)
            {
                Console.WriteLine("Inicializando registros y parámetros de telemetría por defecto.");
                Xamarin.Essentials.Preferences.Set(PrimeraEjecucionKey, false);
            }
        }

        protected override void OnSleep()
        {
            Xamarin.Forms.MessagingCenter.Send(this, "AppEnPausa");
        }

        protected override void OnResume()
        {
            Xamarin.Forms.MessagingCenter.Send(this, "AppReanudada");
        }
    }
}
