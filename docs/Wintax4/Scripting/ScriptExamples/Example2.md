### Calculate the Power Spectral Density in Matlab

This example calculates a PSD spectrum using MATLAB

In order to run this script

  1. Copy the script into menu: General/setup/scripts

  2. Create a text file called "myData.txt” in c:\ containing one column of values, e.g.

  3. Load some Data

  4. Select the script desired into the toolbar “Select Macro”




Sub Matlab_PSD()

Dim Math

Set Math = CreateObject("matlab.application")

Math.Visible = True

Math.Execute("clear all")

Math.Execute("in_file='C:\myData.txt'")

Math.Execute("in_file_p=fopen(in_file,'r')")

Math.Execute("y=fscanf(in_file_p,'%f')")

Math.Execute("fclose(in_file_p)")

Math.Execute("Subplot(2,1,1)")

Math.Execute("plot(y)")

Math.Execute("xlabel('Time [Sec]')")

Math.Execute("ylabel('Amplitude [m/sec^2]')")

Math.Execute("a = fft(y,512)/100")

Math.Execute("Pyy =(a.* conj(a))/512")

Math.Execute("Subplot(2,1,2)")

Math.Execute("plot(10*log10(Pyy))")

Math.Execute("xlabel('Frequency [Hz]')")

Math.Execute("ylabel('Power Spectral Density [dB/Hz]')")

MsgBox "Exit From Matlab"

End Sub