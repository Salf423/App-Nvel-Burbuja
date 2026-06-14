# Cambios realizados — Corrección de compilación APK

Este documento describe los problemas encontrados en el proyecto **App-Nivel-Burbuja** (Xamarin.Forms + Android) y las correcciones aplicadas para que pueda generarse el APK.

---

## Resumen

El proyecto no compilaba por una combinación de **configuración de SDK incompatible**, **dependencias NuGet incorrectas**, **manifest no enlazado** y **errores lógicos en el código C#**. Se actualizó la configuración a `net9.0-android` y se alinearon los archivos de build entre sí.

---

## 1. SDK y workload de Android

### Problema
- `global.json` exigía **SDK 8.0.100**, que no estaba instalado (solo había **10.0.108**).
- El target `net6.0-android` está obsoleto y, sin el workload Android, el SDK lo interpretaba como `net6.0` plano, provocando errores de compatibilidad con paquetes Xamarin.
- El workload **android** no estaba instalado en el entorno local.

### Corrección
- `global.json` fija **SDK 9.0.200** con `rollForward: latestPatch` para evitar que CI use .NET 10 sin el workload Android instalado.
- Target framework actualizado de `net6.0-android` → **`net9.0-android`**.
- Eliminadas propiedades obsoletas: `CheckEolWorkloads`, `MonoAndroidVersion`, `SkipAndroidXMigration`, etc.

### Corrección CI (GitHub Actions)
El error `Failed to restore ... exit code 1` ocurría porque:
1. Los runners de GitHub usan **.NET 10** por defecto.
2. `global.json` tenía `rollForward: latestMajor`, así que `dotnet restore` usaba SDK 10.
3. El workload `android` se instalaba para SDK 9 (`dotnet-version: 9.0.x`), pero restore corría con SDK 10 → **NETSDK1147**.

Solución aplicada en `.github/workflows/build-android.yml`:
- `global-json-file: global.json` fuerza SDK 9.0.x en todos los pasos.
- `dotnet workload install android` se ejecuta **después** del setup del SDK correcto.
- Paso de verificación con `dotnet --info` y `dotnet workload list`.

### Acción requerida en tu máquina
Instala **SDK 9.0.x** (el proyecto lo fija en `global.json`) y el workload Android:

```bash
# Instalar SDK 9 si solo tienes .NET 10
dotnet --list-sdks

# Workload Android (requiere permisos de admin en Linux)
sudo dotnet workload install android
```

Verifica:

```bash
dotnet workload list
# Debe aparecer "android" en la lista
```

---

## 2. Proyecto Android (`AccelerometerEssential.Android.csproj`)

### Problemas
| Error | Detalle |
|-------|---------|
| Falta `Xamarin.Forms` | `MainActivity` hereda de `FormsApplicationActivity` pero el paquete no estaba referenciado |
| Paquete incorrecto | `Xamarin.AndroidX.RecyclerView` sin contexto y sin Forms |
| Sin `ApplicationId` | Android no podía generar el paquete correctamente |
| Manifest huérfano | `AndroidManifest.xml` estaba en `Propiedaes/` pero no se incluía en el build |
| CI inconsistente | GitHub Actions compilaba con `net6.0` y publicaba con `net9.0-android` |

### Corrección
```xml
<TargetFramework>net9.0-android</TargetFramework>
<ApplicationId>com.companyname.AccelerometerEssential</ApplicationId>
<PackageReference Include="Xamarin.Forms" Version="5.0.0.2662" />
<AndroidManifest Include="..\Propiedaes\AndroidManifest.xml" Link="AndroidManifest.xml" />
```

Se eliminó la referencia innecesaria a `Xamarin.AndroidX.RecyclerView`.

---

## 3. Proyecto compartido (`AccelerometerEssential.csproj`)

### Problema
- Usaba `net6.0` en lugar de **`netstandard2.0`**, que es el estándar para bibliotecas compartidas de Xamarin.Forms.
- Versiones antiguas de paquetes (`Xamarin.Forms 5.0.0.2196`, `Xamarin.Essentials 1.7.3`).

### Corrección
- Target: **`netstandard2.0`**
- Paquetes actualizados a `Xamarin.Forms 5.0.0.2662` y `Xamarin.Essentials 1.8.1`
- Archivos `.cs` enlazados con `Link=` para mejor organización en el IDE

---

## 4. Errores de código C#

### 4.1 Bug en `App.xaml.cs` — claves de Preferences inconsistentes

**Antes (bug):**
```csharp
// Get usaba una clave larga, Set usaba otra distinta → siempre leía true
Preferences.Get("Hola, esta app es desarrollada por Adrian...", true);
Preferences.Set("Hola, esta app es desarrollada por Adrian...", false);
```

**Después:**
```csharp
private const string PrimeraEjecucionKey = "PrimeraEjecucion";
Preferences.Get(PrimeraEjecucionKey, true);
Preferences.Set(PrimeraEjecucionKey, false);
```

### 4.2 Formato en `App.xaml.cs`
- Corregidas llaves mal colocadas en `OnSleep()` y `OnResume()`.
- Eliminado tipo genérico innecesario en `MessagingCenter.Send<App>`.

---

## 5. GitHub Actions (`.github/workflows/build-android.yml`)

### Problemas
- `dotnet build` y `dotnet publish` usaban frameworks distintos.
- `publish` apuntaba a `net9.0-android` mientras el `.csproj` tenía `net6.0-android`.
- Paso de debug innecesario (`find . -name "*.csproj"`).
- `AndroidGenerateNoSignatureApk=true` generaba APK sin firmar de forma confusa.

### Corrección
- Un solo paso `dotnet publish` con `-f net9.0-android`.
- SDK .NET **9.0.x** en CI (compatible con el TFM del proyecto).
- Firma automática de debug/release con `AndroidGeneratePackageSigningKey=true`.

---

## 6. VS Code tasks

Se añadió la tarea **`build-android-release-apk`** para generar el APK desde el editor:

```
Ctrl+Shift+P → Run Task → build-android-release-apk
```

El APK se genera en la carpeta `publish/`.

---

## 7. `Directory-Build.props`

Se eliminaron propiedades legacy de Xamarin.Android clásico (`MonoAndroidVersion`, `AndroidXValidateXamarinAndroid`) que no aplican a proyectos SDK-style con `net9.0-android`.

---

## Cómo compilar el APK

### Requisitos previos
1. .NET SDK 9 o superior (`dotnet --version`)
2. Workload Android instalado
3. Java JDK 17
4. Android SDK (API 33+)

### Comandos

```bash
cd App-Nvel-Burbuja

# Restaurar paquetes
dotnet restore buendia/C#/AccelerometerEssential.Android.csproj

# Compilar debug
dotnet build buendia/C#/AccelerometerEssential.Android.csproj -c Debug

# Generar APK release
dotnet publish buendia/C#/AccelerometerEssential.Android.csproj \
  -c Release \
  -f net9.0-android \
  -o ./publish \
  /p:AndroidBuildApplicationPackage=true \
  /p:AndroidGeneratePackageSigningKey=true
```

El APK estará en `./publish/` con un nombre similar a:
`com.companyname.AccelerometerEssential-Signed.apk`

---

## Notas adicionales

- La carpeta **`Propiedaes/`** tiene un typo (debería ser `Propiedades/`); se mantuvo el nombre para no romper rutas existentes, pero el manifest ya está correctamente enlazado.
- La carpeta **`C#/`** con el carácter `#` puede causar problemas en algunos scripts de Windows; en Linux funciona correctamente.
- Para distribución en Play Store, considera migrar a **.NET MAUI** a largo plazo, ya que Xamarin.Forms está en modo mantenimiento.

---

## Archivos modificados

| Archivo | Tipo de cambio |
|---------|----------------|
| `global.json` | SDK flexible con rollForward |
| `Directory-Build.props` | Limpieza de props obsoletas |
| `buendia/C#/AccelerometerEssential.Android.csproj` | Reescritura completa |
| `buendia/XAML/AccelerometerEssential.csproj` | TFM + paquetes |
| `buendia/C#/App.xaml.cs` | Bug Preferences + formato |
| `buendia/C#/MainPage.xaml.cs` | Limpieza menor |
| `.github/workflows/build-android.yml` | Pipeline unificado |
| `.vscode/tasks.json` | Tarea release APK |
