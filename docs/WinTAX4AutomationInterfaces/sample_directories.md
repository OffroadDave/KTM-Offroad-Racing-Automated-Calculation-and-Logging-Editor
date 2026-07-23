[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# Sample: Directories

The following example demonstrates how to get WinTAX current directories: 
    
    
    Function LogDir( oDirs )
    For j = LBound(oDirs ) To UBound(oDirs ) 
            Application.LogText ( oDirs(j) )
    Next
    End Function
    
    Sub TestDirectory
     Application.LogText( "Program Directory= " & Application.ProgramDirectory)
     Application.LogText( "User Directory= " & Application.UserDirectory)
     Application.LogText( "Acquisition Directory= " & Application.AcquisitionDirectory)
     Application.LogText( "Log Directory= " & Application.LogDirectory )
     Application.LogText( "Export Directory= " & Application.ExportDirectory )
     Application.LogText( "Libraries Directory= " & Application.LibrariesDirectory )
     Application.LogText( "Circuit Directory= " & Application.CircuitDirectory )
     Application.LogText( "Data directories:" )
     LogDir( Application.DataDirectories )
     Application.LogText( "Host directories:" )
     LogDir( Application.HostDirectories )
     Application.LogText( "ImportLap directories:" )
     LogDir( Application.ImportLapDirectories )
    End Sub
    