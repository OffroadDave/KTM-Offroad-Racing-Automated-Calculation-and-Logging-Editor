using Autofac;
using KtmDataHandler.Marelli.Wintax;
using KtmDataHandler.Utils;
using KtmSharedUtils.DataType;
using KtmSharedUtils.FileIO;

namespace Ktm.Racing.Oracle;

/// <summary>
/// Factory for KtmDataHandler objects.
/// 
/// KtmDataHandler.dll uses Autofac-based dependency injection.
/// The constructor of <see cref="MarelliDataAccess"/> expects 6 interface dependencies
/// (ITelDataX, ITelDataZTX, ILocalLogHelper, etc.), which are resolved via the DI container.
/// 
/// <c>new MarelliDataAccess()</c> leads to a <see cref="System.NullReferenceException"/>,
/// since all parameters are optional (default null) and the constructor does not handle null references.
/// </summary>
public static class KtmDataHandlerFactory
{
    // volatile: guarantees that the double-checked-lock read outside the lock sees the write.
    private static volatile IContainer? _container;
    private static readonly object _lock = new();

    /// <summary>
    /// Creates and caches the Autofac container with all KtmDataHandler and KtmSharedUtils types.
    /// Thread-safe via lock.
    /// </summary>
    private static IContainer GetContainer()
    {
        if (_container != null) return _container;

        // Make the KtmDataHandler companion/native DLLs discoverable from the suite folder.
        KtmRuntime.EnsureInitialized();

        lock (_lock)
        {
            if (_container != null) return _container;

            var builder = new ContainerBuilder();

            var ktmDataHandlerAssembly = typeof(MarelliDataAccess).Assembly;
            var ktmSharedUtilsAssembly = typeof(IPropertyList).Assembly;

            // Register all public concrete types from KtmDataHandler.dll
            // → resolves the entire DI chain (ITelDataX, ITelDataZTX, ILocalLogHelper, etc.)
            // PublicOnly() excludes non-public nested types (e.g. WintaxGuiTaskHandler+TaskControl_*)
            builder.RegisterAssemblyTypes(ktmDataHandlerAssembly)
                .PublicOnly()
                .Where(t => t.GetConstructors().Length > 0)
                .AsImplementedInterfaces()
                .AsSelf();

            // Register all public concrete types from KtmSharedUtils.dll
            // → resolves dependencies such as IPropertyList, IProperty, IPropertyListArray
            //   that are required by KtmDataHandler types (e.g. TelDataX)
            builder.RegisterAssemblyTypes(ktmSharedUtilsAssembly)
                .PublicOnly()
                .Where(t => t.GetConstructors().Length > 0)
                .AsImplementedInterfaces()
                .AsSelf();

            // Open generic types (RegisterAssemblyTypes does not capture open generics):
            // KtmDataHandler types such as MarelliDataXmlProcessingConstants require IXMLFileProcessor<T>.
            builder.RegisterGeneric(typeof(XMLFileProcessor<>)).As(typeof(IXMLFileProcessor<>));
            builder.RegisterGeneric(typeof(BinaryFileProcessor<>)).As(typeof(IBinaryFileProcessor<>));
            builder.RegisterGeneric(typeof(JsonFileProcessor<>)).As(typeof(IJsonFileProcessor<>));

            // Register ILocalLogHelper explicitly: the parameterless constructor of
            // LocalLogHelper leaves the log4net field (log) as null, while BLogEnable
            // is true by default. KtmDataHandler types (e.g. Channel) log in the constructor
            // and access the null field → NullReferenceException.
            // The last registration wins in Autofac → overrides the assembly registration.
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
    /// Creates an <see cref="IMarelliDataAccessRead"/> instance with correctly resolved dependencies.
    /// Replaces: <c>IMarelliDataAccessRead reader = new MarelliDataAccess();</c>
    /// </summary>
    public static IMarelliDataAccessRead CreateReader()
        => GetContainer().Resolve<IMarelliDataAccessRead>();

    /// <summary>
    /// Creates an <see cref="IMarelliDataAccessWrite"/> instance with correctly resolved dependencies.
    /// Replaces: <c>IMarelliDataAccessWrite writer = new MarelliDataAccess();</c>
    /// </summary>
    public static IMarelliDataAccessWrite CreateWriter()
        => GetContainer().Resolve<IMarelliDataAccessWrite>();

    /// <summary>
    /// Creates a <see cref="MarelliDataAccess"/> instance (implements both read and write).
    /// For cases in which both interfaces or MarelliDataAccess-specific methods 
    /// (e.g. GetLapMarks) must be accessed.
    /// </summary>
    public static MarelliDataAccess CreateAccess()
        => GetContainer().Resolve<MarelliDataAccess>();
}
