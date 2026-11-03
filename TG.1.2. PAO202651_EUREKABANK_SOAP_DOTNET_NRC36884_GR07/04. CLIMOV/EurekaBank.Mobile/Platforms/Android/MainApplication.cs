using Android.App;
using Android.Runtime;

namespace EurekaBank.Mobile;

/// <summary>Puente que crea la aplicación MAUI en Android.</summary>
[Application]
public sealed class MainApplication(IntPtr handle, JniHandleOwnership ownership) : MauiApplication(handle, ownership)
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
