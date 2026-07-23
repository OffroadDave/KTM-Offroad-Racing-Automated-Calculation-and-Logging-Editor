### Export a list of channels to ASCII file

In order to run this script

  1. Create a file called Channel.Export containing the following lines:




2

rpm

Time

  2. Copy the contents of the script into menu: General/setup/scripts

  3. Copy the file "Channel.Export" in C:\WinTAX4\ or modify the script with the path desired

  4. Modify the configuration file "Channel.Export" according to the own requirements:




The first row represents the number of channel exported

In the successive rows enter the name of the exported channels

  4. Load some Data

  5. Select this script in the toolbar "Select Macro"




Sub Export_Ascii()

Dim fso, f, r, rs, oNumChannels, oLen, oSpace, oSpaceSamples, oTime, oFreq, oRow, oRowName, oLenValue, oLenSamples, oFileName, oMaxLen

Const ForReading = 1

Const ForWriting = 2

oFreq = InputBox ("Select the export frequency (Hz)")

oFileName = InputBox ("Select the export file name")

Set oData = Application.CurrentData(0)

oTime = Application.CurrentData(0).Time

Set fso = CreateObject("Scripting.FileSystemObject")

Set r = fso.OpenTextFile("Channels.Export", ForReading, True)

Set f = fso.OpenTextFile(oFileName & ".txt", ForWriting, True)

oNumChannels = r.readLine

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Dim oLabel(75)

oMaxLen = 1

For i = 1 To oNumChannels

oLabel(i) = r.Readline

oLen =Len(oLabel(i))

If Len(oLabel(i)) > oMaxLen Then

oMaxLen = Len(oLabel(i))

End If

Next

If oMaxLen < 7 Then

oMaxLen = 10

End If

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

r.close

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

oSpace = ""

For ioLen = 0 To oMaxLen

oSpace = oSpace + " "

Next

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

oRowName =""

For h = 1 To oNumChannels

oRowName = oRowName & oLabel(h) & oSpace

Next

f.Write(oRowName)

f.WriteLine

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

For j = 0 To oTime Step (1/oFreq)

oRow = ""

For k = 1 To oNumChannels 

Set oChan = oData.Channels(oLabel(k))

iValue = oChan.Value(j)

If iValue = "-1,#QNAN" Then

iValue = "NoRx"

Else If oChan.Value(j) = "-1,#IND" Then

iValue = "NoRx"

End If

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''' 

oLenValue = Len(iValue)

oLen = Len(oChan.Name)

oLenSamples = oLen + oMaxLen - oLenValue

oSpaceSamples = ""

For ioLen = 0 to oLenSamples

oSpaceSamples = oSpaceSamples + " "

Next

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

oRow = oRow & Replace(iValue, ",", ".") & oSpaceSamples

Next

f.Writeline oRow

Next

MsgBox "Export Finished"

End Sub