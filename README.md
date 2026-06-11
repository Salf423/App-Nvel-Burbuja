# App-Nivel-Burbuja
# Fuciones clave
    ''' <manifest xmlns:android="http://schemas.android.com/apk/res/android" android:versionCode="1" android:versionName="1.0" package="com.companyname.AccelerometerEssential">
    <uses-sdk android:minSdkVersion="21" android:targetSdkVersion="33" />
    <uses-feature android:name="android.hardware.sensor.accelerometer" android:required="true" />
    <application android:label="Bubble Level"></application>
    </manifest>''' XML#
    Como se ve en la funcion anterior donde se define el SDK de android que se va a usar, mas especfifico el SDK 21-33, se coloca que la version minima es la 21, tambien se indican los requerimientos del hardware y el nombre de la aplicacion
# Funcion logica
    '''private void OnAcelerometroCambio(object sender, AccelerometerChangedEventArgs e)
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
        }''' C#
    En el metodo OnAcelerometroCambio se activa cada vez que hay un cambio en las lecturas del acelerómetro. A continuación, se describen las secciones clave del código:
    Lectura de Datos: Se obtienen las lecturas de aceleración en los ejes X, Y y Z.
    Filtrado de Datos: Se aplican formulas de filtrado exponencial para suavizar las lecturas.
    Calculo de Angulos: Se calculan los angulos de pitch y roll a partir de las lecturas filtradas.
    Actualización de la Interfaz: Se ajustan las posiciones de un objeto visual (en este caso, miBurbuja) en funcion de los angulos calculados.