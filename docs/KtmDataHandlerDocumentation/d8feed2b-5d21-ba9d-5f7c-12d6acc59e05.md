Ktm Data Handler

# MarelliCLXAccess Constructor   
  
---  
  
Constructor of the class. 

**Namespace:** Â [KtmDataHandler.Marelli.Sysma](231941d2-4c65-7ce2-a989-329d2f7fdc2e.md) **Assembly:** Â KtmDataHandler (in KtmDataHandler.dll) Version: 3.5.7.0
    
    
    public MarelliCLXAccess(
    	IObjectProcessor objProcessorObjIn = null,
    	IDataTypeDetails DataTypeDetailsObjIn = null,
    	IGenericSymbolProperties MarelliCLXPropertiesIn = null,
    	IConversionRule ConversionRuleObjIn = null,
    	IPropertyList PropertyListObjIn = null,
    	IMeasurement MeasurementObjIn = null,
    	ICalibration CalibrationObjIn = null,
    	IDataDictionary DataDictionaryObjIn = null,
    	IDataUtils DataUtilsObjIn = null,
    	ILocalLogHelper LocalLogHelperObjIn = null,
    	IApplicationVersions ApplicationVersionsObjIn = null
    )
    
    
    Public Sub New ( 
    	Optional objProcessorObjIn As IObjectProcessor = Nothing,
    	Optional DataTypeDetailsObjIn As IDataTypeDetails = Nothing,
    	Optional MarelliCLXPropertiesIn As IGenericSymbolProperties = Nothing,
    	Optional ConversionRuleObjIn As IConversionRule = Nothing,
    	Optional PropertyListObjIn As IPropertyList = Nothing,
    	Optional MeasurementObjIn As IMeasurement = Nothing,
    	Optional CalibrationObjIn As ICalibration = Nothing,
    	Optional DataDictionaryObjIn As IDataDictionary = Nothing,
    	Optional DataUtilsObjIn As IDataUtils = Nothing,
    	Optional LocalLogHelperObjIn As ILocalLogHelper = Nothing,
    	Optional ApplicationVersionsObjIn As IApplicationVersions = Nothing
    )
    
    
    public:
    MarelliCLXAccess(
    	IObjectProcessor^ objProcessorObjIn = nullptr, 
    	IDataTypeDetails^ DataTypeDetailsObjIn = nullptr, 
    	IGenericSymbolProperties^ MarelliCLXPropertiesIn = nullptr, 
    	IConversionRule^ ConversionRuleObjIn = nullptr, 
    	IPropertyList^ PropertyListObjIn = nullptr, 
    	IMeasurement^ MeasurementObjIn = nullptr, 
    	ICalibration^ CalibrationObjIn = nullptr, 
    	IDataDictionary^ DataDictionaryObjIn = nullptr, 
    	IDataUtils^ DataUtilsObjIn = nullptr, 
    	ILocalLogHelper^ LocalLogHelperObjIn = nullptr, 
    	IApplicationVersions^ ApplicationVersionsObjIn = nullptr
    )
    
    
    new : 
            ?objProcessorObjIn : IObjectProcessor * 
            ?DataTypeDetailsObjIn : IDataTypeDetails * 
            ?MarelliCLXPropertiesIn : IGenericSymbolProperties * 
            ?ConversionRuleObjIn : IConversionRule * 
            ?PropertyListObjIn : IPropertyList * 
            ?MeasurementObjIn : IMeasurement * 
            ?CalibrationObjIn : ICalibration * 
            ?DataDictionaryObjIn : IDataDictionary * 
            ?DataUtilsObjIn : IDataUtils * 
            ?LocalLogHelperObjIn : ILocalLogHelper * 
            ?ApplicationVersionsObjIn : IApplicationVersions 
    (* Defaults:
            let _objProcessorObjIn = defaultArg objProcessorObjIn null
            let _DataTypeDetailsObjIn = defaultArg DataTypeDetailsObjIn null
            let _MarelliCLXPropertiesIn = defaultArg MarelliCLXPropertiesIn null
            let _ConversionRuleObjIn = defaultArg ConversionRuleObjIn null
            let _PropertyListObjIn = defaultArg PropertyListObjIn null
            let _MeasurementObjIn = defaultArg MeasurementObjIn null
            let _CalibrationObjIn = defaultArg CalibrationObjIn null
            let _DataDictionaryObjIn = defaultArg DataDictionaryObjIn null
            let _DataUtilsObjIn = defaultArg DataUtilsObjIn null
            let _LocalLogHelperObjIn = defaultArg LocalLogHelperObjIn null
            let _ApplicationVersionsObjIn = defaultArg ApplicationVersionsObjIn null
    *)
    -> MarelliCLXAccess

#### Parameters

objProcessorObjIn (Optional)
    Type:Â IObjectProcessorObject processor
DataTypeDetailsObjIn (Optional)
    Type:Â IDataTypeDetailsDataTypeDetails object
MarelliCLXPropertiesIn (Optional)
    Type:Â IGenericSymbolPropertiesMarelliCLXProperties object
ConversionRuleObjIn (Optional)
    Type:Â IConversionRuleConversionRule object
PropertyListObjIn (Optional)
    Type:Â IPropertyListProperty List Object
MeasurementObjIn (Optional)
    Type:Â IMeasurementMeasurement object
CalibrationObjIn (Optional)
    Type:Â ICalibrationCalibration object
DataDictionaryObjIn (Optional)
    Type:Â IDataDictionaryDataDictionary object
DataUtilsObjIn (Optional)
    Type:Â IDataUtilsDataUtils object
LocalLogHelperObjIn (Optional)
    Type:Â ILocalLogHelperLocalLogHelper object
ApplicationVersionsObjIn (Optional)
    Type:Â IApplicationVersionsApplicationVersion object

#### Reference

[MarelliCLXAccess Class](89f6891c-0695-8d1c-0b5f-9b59d06a4822.md)

[KtmDataHandler.Marelli.Sysma Namespace](231941d2-4c65-7ce2-a989-329d2f7fdc2e.md)