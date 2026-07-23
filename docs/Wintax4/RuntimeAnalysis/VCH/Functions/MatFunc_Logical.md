### Logical functions

Boolean operators can be used to filter and condition mathematical expressions. See also [Bitwise operators](MatFunc_Bitwise.md)

The functions shown below return value 1 if the condition is True, 0 if the condition is False

Example: IFGT(VitVehicule,150)

#### IFBETWEEN(expression, rangeMin, rangeMax)

Returns 1 (TRUE) if the values of expression are included between the rangeMin and rangeMax. Returns 0 (FALSE) if values are external;

rangeMin and rangeMax can be single values or expression.

### 

####  IFEQ(expression1, expression2, range)

Check whether an expression is equal to another; the second expression can be evaluated in a range.

Returns 1 (TRUE) if expressions are equal, 0 (FALSE) if the expressions are different.

Within range, returns true if (expression2-range) <= expression1 <= (expression2+range)

Remarks: If the parameter range is set to zero, the values of expression1 and expression2 are rounded to the number of decimal calculated for the virtual channel during the computation.

### 

####  IFGE(expression1, expression2)

Returns 1 (TRUE) if expression1 is greater or equal than expression2; 0 (FALSE) if expression2 is greater than expression1.

Greater than or equal to, returns true if expression1 >= expression2

####  IFGT(expression1, expression2)

Returns 1 (TRUE) if expression1 is greater than expression2; 0 (FALSE) if expression2 is greater than expression1.

Greater than or equal to, returns true if expression1 >=expression2

### 

####  IFLE(expression1, expression2)

Returns 1 (TRUE) if expression2 is greater or equal than expression1; 0 (FALSE) if expression1 is greater than expression2.

Less than or equal to, returns true if expression1 <= expression2

### 

####  IFLT(expression1, expression2)

Returns 1 (TRUE) if expression2 is greater than expression1; 0 (FALSE) if expression1 is greater than expression2.

Less than, returns true if expression1 <= expression2

### 

####  IFNE(expression1, expression2, range)

Check whether an expression is different from another; the second expression can be evaluated in a range. Returns 1 (TRUE) if expressions are different, 0 (FALSE) if the expressions are equal.

Out of range, returns true if expression1 < (expression2-range); expression1 > (expression2+range)

Remarks: If the parameter range is set to zero, the values of expression1 and expression2 are rounded to the number of decimal calculated for the virtual channel during the computation.

### 

####  NOT

The logical negation operator NOT returns the value 0 if its operand is nonzero and the value 1 if its operand is zero.