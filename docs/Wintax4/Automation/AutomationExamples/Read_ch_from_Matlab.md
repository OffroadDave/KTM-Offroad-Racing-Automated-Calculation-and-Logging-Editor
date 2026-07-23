### Read WinTAX4 channels from Matlab

The following Matlab routine allows to read WinTAX4 data

%%%%%%%%%Matlab Example used to read Data from WinTAX4%%%%%

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%%%%This example needs to have WinTAX4 open with a loaded lap %%%%%

clear;

%%%%%Create WinTAX4 Server Object..

oWTX4 = actxserver('WinTax4.Application');

oWTX4.visible = 'TRUE';

%%%%%Get the Data From WinTAX4....

%%%%% get the current DataSet Loaded in WinTAX4..

myData = oWTX4.CurrentData;

myDataLoad = invoke(myData, 'Item', 0);

%%%%% LapTime is..

myDataLoadTime = get(myDataLoad, 'Time' );

MyChannels = get(myDataLoad, 'Channels');

%%%%% only for demonstration purpose, get the channel "RPM" ..

MyChannel = invoke(MyChannels, 'Item', 'RPM')

%%%%% "RPM" SampleRate is..

MyChannelFreq = get(MyChannel, 'SampleRate')

%%%%% extract the buffer of samples by "RPM"..

MyChannelValues = invoke(MyChannel, 'Values', 0, myDataLoadTime);

%%%%% only for demonstration purpose, the first value of "RPM" is..

FirstSample = MyChannelValues{1};