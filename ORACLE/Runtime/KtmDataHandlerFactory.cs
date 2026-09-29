using Autofac;
using KtmDataHandler.Marelli.Wintax;
using KtmDataHandler.Utils;
using KtmSharedUtils.DataType;
using KtmSharedUtils.FileIO;

namespace Ktm.Racing.Oracle;

/// <summary>
/// Factory für KtmDataHandler-Objekte.
/// 
/// KtmDataHandler.dll verwendet Autofac-basierte Dependency Injection.
/// Der Konstruktor von <see cref="MarelliDataAccess"/> erwartet 6 Interface-Abhängigkeiten
/// (ITelDataX, ITelDataZTX, ILocalLogHelper, etc.), die über den DI-Container aufgelöst werden.
/// 
/// <c>new MarelliDataAccess()</c> führt zu <see cref="System.NullReferenceException"/>,
/// da alle Parameter optional sind (default null) und der Konstruktor null-Referenzen nicht abfängt.
/// </summary>
public static class KtmDataHandlerFactory
{
    private static IContainer? _container;
    private static readonly object _lock = new();

    /// <summary>
    /// Erstellt und cached den Autofac-Container mit allen KtmDataHandler- und KtmSharedUtils-Typen.
    /// Thread-safe durch lock.
    /// </summary>
    private static IContainer GetContainer()
    {
        if (_container != null) return _container;

        // KtmDataHandler-Companion-/Native-DLLs aus dem Suite-Ordner auffindbar machen.
        KtmRuntime.EnsureInitialized();

        lock (_lock)
        {
            if (_container != null) return _container;

            var builder = new ContainerBuilder();

            var ktmDataHandlerAssembly = typeof(MarelliDataAccess).Assembly;
            var ktmSharedUtilsAssembly = typeof(IPropertyList).Assembly;

            // Alle öffentlichen konkreten Typen aus KtmDataHandler.dll registrieren
            // → löst die gesamte DI-Kette auf (ITelDataX, ITelDataZTX, ILocalLogHelper, etc.)
            // PublicOnly() schließt nicht-öffentliche verschachtelte Typen aus (z.B. WintaxGuiTaskHandler+TaskControl_*)
            builder.RegisterAssemblyTypes(ktmDataHandlerAssembly)
                .PublicOnly()
                .Where(t => t.GetConstructors().Length > 0)
                .AsImplementedInterfaces()
                .AsSelf();

            // Alle öffentlichen konkreten Typen aus KtmSharedUtils.dll registrieren
            // → löst Abhängigkeiten wie IPropertyList, IProperty, IPropertyListArray auf,
            //   die von KtmDataHandler-Typen (z.B. TelDataX) benötigt werden
            builder.RegisterAssemblyTypes(ktmSharedUtilsAssembly)
                .PublicOnly()
                .Where(t => t.GetConstructors().Length > 0)
                .AsImplementedInterfaces()
                .AsSelf();

            // Offene generische Typen (RegisterAssemblyTypes erfasst keine open generics):
            // KtmDataHandler-Typen wie MarelliDataXmlProcessingConstants benötigen IXMLFileProcessor<T>.
            builder.RegisterGeneric(typeof(XMLFileProcessor<>)).As(typeof(IXMLFileProcessor<>));
            builder.RegisterGeneric(typeof(BinaryFileProcessor<>)).As(typeof(IBinaryFileProcessor<>));
            builder.RegisterGeneric(typeof(JsonFileProcessor<>)).As(typeof(IJsonFileProcessor<>));

            // ILocalLogHelper explizit registrieren: Der parameterlose Konstruktor von
            // LocalLogHelper hinterlässt das log4net-Feld (log) als null, während BLogEnable
            // standardmäßig true ist. KtmDataHandler-Typen (z.B. Channel) loggen im Konstruktor
            // und greifen auf das null-Feld zu → NullReferenceException.
            // Letzte Registrierung gewinnt in Autofac → überschreibt die Assembly-Registrierung.
            builder.Register(_ =>
            {
                var logger = new LocalLogHelper();
                logger.BLogEnable = false;
                logger.BDeepLogEnable = false;
                logger.BEnableTraceView = false;
                return logger;
            }).As<ILocalLogHelper>().AsSelf().SingleInstance();

            _container = builder.Build();
        }

        return _container;
    }

    /// <summary>
    /// Erstellt eine <see cref="IMarelliDataAccessRead"/>-Instanz mit korrekt aufgelösten Abhängigkeiten.
    /// Ersetzt: <c>IMarelliDataAccessRead reader = new MarelliDataAccess();</c>
    /// </summary>
    public static IMarelliDataAccessRead CreateReader()
        => GetContainer().Resolve<IMarelliDataAccessRead>();

    /// <summary>
    /// Erstellt eine <see cref="IMarelliDataAccessWrite"/>-Instanz mit korrekt aufgelösten Abhängigkeiten.
    /// Ersetzt: <c>IMarelliDataAccessWrite writer = new MarelliDataAccess();</c>
    /// </summary>
    public static IMarelliDataAccessWrite CreateWriter()
        => GetContainer().Resolve<IMarelliDataAccessWrite>();

    /// <summary>
    /// Erstellt eine <see cref="MarelliDataAccess"/>-Instanz (implementiert sowohl Read als auch Write).
    /// Für Fälle, in denen auf beide Interfaces oder auf MarelliDataAccess-spezifische Methoden 
    /// (z.B. GetLapMarks) zugegriffen werden muss.
    /// </summary>
    public static MarelliDataAccess CreateAccess()
        => GetContainer().Resolve<MarelliDataAccess>();
}
