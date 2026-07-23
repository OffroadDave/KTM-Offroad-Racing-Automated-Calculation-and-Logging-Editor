[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# Sample: Access to Configuration objects

Access to WinTAX configuration setup objects is based on really simple interfaces exposing methods that allow to load, save and navigate some generic objects. Refer to the page [XML - Parameters](xml_doc_parameters.md) and [XML - Virtual Channels](xml_doc_virtualchannels.md) for a documentation of the XML format used in WinTAX configuration objects. The following example demonstrates how to get and parse the WinTAX Parameters configuration. It also implements a basic XML parsing in order to extract single information (channel name and range values) from the configuration. 
    
    
    Sub Show_Parameters_Configuration
    
    Dim oCnf
    Set oCnf = Application.ConfParameters
    
    Dim oParameters
    Set oParameters = oCnf.Objects
    
    Dim oDocXml
    Set oDocXml = CreateObject("MSXML.DOMDocument")
    oDocXml.async = False
    
    Dim oParameter
    For Each oParameter In oParameters
    
      oDocXml.loadXML oParameter.XML
    
      Dim oLabel, oRangeMin, oRangeMax
    
      Set oLabel= oDocXml.getElementsByTagName( "Name" )
      Set oRangeMin = oDocXml.getElementsByTagName( "RangeMin" )
      Set oRangeMax= oDocXml.getElementsByTagName( "RangeMax" )
    
      Application.LogText "Parameter - [" & oLabel(0).Text & "] - Range (" & oRangeMin(0).Text & "," & oRangeMax(0).Text & ")"
    
    Next
    
    End Sub
    

The following example demonstrates a similar script that makes a dump of all Virtual Channel libraries. Note that there is no XML parsing. 
    
    
    Function DumpLibrary( oLib )
    Dim oChan
    Dim oChannels
    Set oChannels = oLib.Objects
    
    For Each oChan In oChannels
            Application.LogText( oChan.XML )
    next
    
    End Function 
    
    
    Sub Dump_VirtualChannels_Libraries
    
    Dim oLibraries
    Dim oLib
    Set oLibraries = Application.ConfVCHLibraries
    
    For Each oLib In oLibraries
     Application.LogText "Library: " &      oLib.Name
     DumpLibrary( oLib )
    Next
    
    End sub
    

The following example changes the Parameters setup by accessing the XML content. 
    
    
    Sub Test_Parameters_Configuration_WRITE
    
    Dim oCnf
    Set oCnf = Application.ConfParameters
    
    Dim oParameters
    Set oParameters = oCnf.Objects
    
    Dim oDocXml
    Set oDocXml = CreateObject("MSXML2.DOMDocument")
    oDocXml.async = False
    
    Dim oParameter
    For Each oParameter In oParameters
    
      oDocXml.loadXML oParameter.XML
    
      Dim oLabels
      Set oLabels = oDocXml.getElementsByTagName( "Name" )
    
            Dim oLabel
            For Each oLabel In oLabels
              oLabel.Text = "-CHANGED-"
            Next
    
            oParameter.XML = oDocXml.XML
    
    Next
    
    End Sub
    