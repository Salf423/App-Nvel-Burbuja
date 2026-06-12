# Guía de Debug en Dispositivo Android Físico

## Requisitos Previos
1. **Teléfono Android conectado por USB** con Android 5.0+ (SDK 21+)
2. **Depuración USB habilitada** en el teléfono:
   - Ve a Configuración > Opciones para desarrolladores
   - Activa "Depuración USB"
   - Acepta el certificado del equipo cuando se pida
3. **Xamarin Essentials 1.7.3** instalado (ya está en el proyecto)
4. **Permisos de sensores configurados** (ya añadidos)

## Estructura del Proyecto
```
buendia/
├── C#/
│   ├── AccelerometerEssential.Android.csproj  ← Proyecto Android (PRINCIPAL)
│   ├── MainActivity.cs
│   └── MainPage.xaml.cs
├── XAML/
│   └── AccelerometerEssential.csproj          ← Proyecto compartido
└── Propiedaes/
    └── AndroidManifest.xml
```

## Pasos para Debuguear

### 1. Verificar Conexión ADB
```bash
adb devices
# Debería mostrarte tu dispositivo conectado
```

### 2. En VS Code
- Abre `.vscode/launch.json` - ya está configurado
- Abre `.vscode/tasks.json` - ya está configurado

### 3. Compilar el Proyecto
```bash
# Opción A: Usar VS Code
Ctrl+Shift+P → Run Task → build-android-debug

# Opción B: Terminal
dotnet restore buendia/C#/AccelerometerEssential.Android.csproj
dotnet build buendia/C#/AccelerometerEssential.Android.csproj -c Debug
```

### 4. Desplegar en Dispositivo
```bash
# Opción A: Usar VS Code
Ctrl+Shift+P → Run Task → deploy-android

# Opción B: Terminal (instala la app automáticamente)
dotnet build buendia/C#/AccelerometerEssential.Android.csproj -c Debug -t Install
```

### 5. Iniciar Debug
```bash
# En VS Code: 
F5 → Selecciona "Android: Debug en Dispositivo Físico"

# O desde Terminal:
dotnet build buendia/C#/AccelerometerEssential.Android.csproj -c Debug -t Install
# Luego abre la app en tu teléfono
```

## Solución de Problemas

### Error: "No device found"
- Reconecta el cable USB
- Ejecuta: `adb kill-server && adb start-server`
- Reinicia el teléfono

### Error: "Permission denied"
- Acepta los permisos cuando te los pida el teléfono
- La app pedirá "Acceso a sensores" al iniciarse

### Error de compilación
- Ejecuta: `dotnet restore buendia/C#/AccelerometerEssential.Android.csproj`
- Verifica tener .NET SDK instalado: `dotnet --version`

### No aparece la burbuja
- Verifica que el acelerómetro esté activo (inclina el teléfono)
- Revisa la consola de debug en VS Code (Output)

## Checklist Antes de Deployar
- ✅ Teléfono conectado por USB y visible en `adb devices`
- ✅ Depuración USB habilitada
- ✅ AccelerometerEssential.Android.csproj puede compilar sin errores
- ✅ AndroidManifest.xml incluye `<uses-permission android:name="android.permission.BODY_SENSORS" />`
- ✅ MainPage.xaml.cs incluye `RequestSensorPermissionAsync()`
- ✅ Modo Debug seleccionado en la compilación

## Próximos Pasos Después del Debug
1. Para una versión final, compilar en `Release`:
   ```bash
   dotnet build buendia/C#/AccelerometerEssential.Android.csproj -c Release -t Install
   ```
2. Generar APK o AAB para distribución si lo necesitas

---
¡Listo para debuguear! 🚀
