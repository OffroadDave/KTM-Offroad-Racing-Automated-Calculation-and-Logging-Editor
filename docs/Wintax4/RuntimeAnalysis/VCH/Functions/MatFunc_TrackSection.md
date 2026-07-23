### Track Section Function

Note: The section indices in the functions of map management are '0 based'.

####  deltasplittime(section)

Delta section split time: Returns the difference in the time used to run the section in input, between the current comparison and the base comparison. The argument is the index of the section and can be a value or an expression.

####  deltalapsplittime(section)

Delta lap split time: Returns the difference in the time elapsed from the beginning of the lap to the section in input, between the current comparison and the base comparison group. The argument is the index of the section and can be a value or an expression.

####  splittime(section)

Section split time: Returns time used to run the section in input. The argument is the index of the section and can be a value or an expression.

####  tracksection(time)

Returns the index of the map section corresponding to time.

####  tracksectionend(section)

Returns the distance corresponding to the end of section.

####  tracksectionendtime(section)

Returns the time corresponding to the end of section.

####  tracksectionstart(section)

Returns the distance corresponding to the beginning of section.

####  tracksectionstarttime(section)

Returns the time corresponding to the beginning of section.