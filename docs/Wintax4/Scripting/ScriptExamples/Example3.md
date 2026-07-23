### JAVA Script Example

This example returns the position of the cursor using Java as language for scripting

In order to run this script:

  1. Set the Script Language as Java Script in WinTAX4 menu: General Setup/General Option/Script language

  2. Restart WinTAX4

  3. Copy the script for example in a Graph window

  4. Load Data

  5. Move the cursor

  6. 


function Window::OnCursorPosChange(TimePos)

{

var oData;

var oChan;

var iValues;

oData = Application.CurrentData(0);

oChan = oData.Channels("rpm");

iValues = oChan.Value( TimePos );

Application.LogText( iValues );

}