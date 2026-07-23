### Example: User Records in math expression

Example of how to use the User Records in math expression. In principle the content of the User Records can be used in Virtual Channels by means of @ operator. For example the User can define a virtual channel as following 

MyVCH = RPM + @demo where demo is a “User Records” where you can define a number or a formula 

| demo = 1000 | => | MyVCH = RPM + 1000 |
| --- | --- | --- |
| demo = Integ(Speed)/3.14 | => | MyVCH = RPM + Integ(Speed)/3.14 |