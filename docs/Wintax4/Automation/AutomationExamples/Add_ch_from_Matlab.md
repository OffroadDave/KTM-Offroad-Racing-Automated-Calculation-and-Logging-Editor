### Add WinTAX4 channels from MATLAB

The following MATLAB routine allows to put data in WinTAX4 via OLE Channels

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%%%%%Matlab Example used to put Data to WinTAX4 from Matlab%%%%

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%%%%This example needs to have WinTAX4 open with a loaded lap %%%%

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

clear;

%%%%%Create WinTAX4 Server Object..

oWTX4 = actxserver('WinTax4.Application');

oWTX4.visible = 'TRUE';

%%%%% get the current DataSet Loaded in WinTAX4..

myData = oWTX4.CurrentData;

myDataLoad = invoke(myData, 'Item', 0);

%%%%% LapTime is..

myDataLoadTime = get(myDataLoad, 'Time' );

MyChannels = get(myDataLoad, 'Channels');

%%%%% My OLE Channel Name is..

MyChannelMatlab = invoke(MyChannels, 'Add', 'OleChannelTest');

MyChannel = invoke(MyChannels, 'Item', 'OleChannelTest');

%%%%% My OLE Channel SampleRate = 1 HZ

set(MyChannel, 'SampleRate', 1);

start = 0;

finish = myDataLoadTime;

%%%%% create a ramp vector with length = LapTime

for i = 1: myDataLoadTime

vector(i) = i;

end

%%%%% Put the vector in WinTAX4

set(MyChannel, 'Values', 0, finish, vector);