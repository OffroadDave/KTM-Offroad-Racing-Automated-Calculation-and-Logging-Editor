[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# Sample: Layouts

Changes current WinTAX layout removing all opened windows and opening two existing graphs window tiled vertically 
    
    
    Sub ChangeCurrentLayout
    
    Dim oLay
    Dim oDisplay
    
    Set oLay = Application.Layouts( Application.CurrentLayout )
    
    for each oDisplay in oLay.Displays
            oLay.Displays.Remove( 0 )
    next
    
    Set oDisplay = oLay.Displays.Add( "W1.gra" )
    oDisplay.Top            = 0
    oDisplay.Bottom         = 10000
    oDisplay.Left           = 0
    oDisplay.Right  = 5000
    
    
    Set oDisplay = oLay.Displays.Add( "W2.gra" )
    oDisplay.Top            = 0
    oDisplay.Bottom         = 10000
    oDisplay.Left           = 5000
    oDisplay.Right  = 10000
    
    Application.CurrentLayout = oLay.Name
    
    End Sub
    