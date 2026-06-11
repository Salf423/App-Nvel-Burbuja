using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AccelerometerEssential
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            bool primeraVez = Xamarin.Essentials.Preferences.Get("EsPrimeraEjecucion", true);
    
        if (primeraVez)
            {
                Console.WriteLine("Inicializando registros y parámetros de telemetría por defecto.");
                Xamarin.Essentials.Preferences.Set("EsPrimeraEjecucion", false);
            }
        }

        protected override void OnSleep()
        {
        Xamarin.Forms.MessagingCenter.Send<App>(this, "AppEnPausa");}

        protected override void OnResume()
        {
            Xamarin.Forms.MessagingCenter.Send<App>(this, "AppReanudada");}
    }
}