## $AbsTime in Excel

Copying data to clipboard when the Absolute (Date/Time) option is selected in a Graph window will give a timestamp expressed in seconds from 1/1/1970, e.g.

Time &#9;rpm 1102063392.436&#9;16742 1102063392.446&#9;16793 1102063392.456&#9;16799 1102063392.466&#9;16762 1102063392.476&#9;16772

When using an Excel sheet, convert the time vector before it can be formatted as a date/time.

To do this:

  1. convert from seconds to days (i.e. divide by 24x60x60=86,400)

  2. add the date value of 1/1/1970 = DATE(1970,1,1) = 25569

  3. format as required, e.g. d/m/y h:mm:ss.000

  4. thus 1102063392.456 becomes 3/12/04 8:43:12.456


