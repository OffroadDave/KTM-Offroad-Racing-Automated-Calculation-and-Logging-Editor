### Logical functions

Boolean operators can be used to filter and condition mathematical expressions. See also [Bitwise operators](MatFunc_Bitwise.md)

The functions shown below return value 1 if the condition is True, 0 if the condition is False

Example: IFGT(VitVehicule,150)

#### IFBETWEEN(expression, rangeMin, rangeMax)

Returns 1 (TRUE) if the values of expression are included between the rangeMin and rangeMax. Returns 0 (FALSE) if values are external.

RangeMin and rangeMax can be single values or expression.

### 

####  IFEQ(expression1, expression2, range)

Check whether an expression is equal to another; the second expression can be evaluated in a range.

Returns 1 (TRUE) if expressions are equal, 0 (FALSE) if the expressions are different.

Within range, returns true if (expression2-range) <= expression1 <= (expression2+range)

####  == (expression1 == expression2)

Check whether an expression is equal to another. Returns 1 (TRUE) if expressions are equal, 0 (FALSE) if the expressions are different. This operator is the same of IFEQ written in C Format.

####  IFGE(expression1, expression2)

Returns 1 (TRUE) if expression1 is greater or equal than expression2; 0 (FALSE) if expression2 is greater than expression1.

Greater than or equal to, returns true if expression1 >= expression2

####  >= (expression1 >= expression2)

Returns 1 (TRUE) if expression1 is greater or equal than expression2; 0 (FALSE) if expression2 is greater than expression1. This operator is the same of IFGE written in C Format.

####  IFGT(expression1, expression2)

Returns 1 (TRUE) if expression1 is greater than expression2; 0 (FALSE) if expression2 is greater than expression1.

Greater than or equal to, returns true if expression1 >=expression2

####  > (expression1 > expression2)

Returns 1 (TRUE) if expression1 is greater than expression2; 0 (FALSE) if expression2 is greater than expression1. This operator is the same of IFGT written in C Format.

####  IFLE(expression1, expression2)

Returns 1 (TRUE) if expression2 is greater or equal than expression1; 0 (FALSE) if expression1 is greater than expression2.

Less than or equal to, returns true if expression1 <= expression2

#### <= (expression1 <= expression2 )

Returns 1 (TRUE) if expression2 is greater or equal than expression1; 0 (FALSE) if expression1 is greater than expression2. This operator is the same of IFLE written in C Format.

### 

####  IFLT(expression1, expression2)

Returns 1 (TRUE) if expression2 is greater than expression1; 0 (FALSE) if expression1 is greater than expression2.

Less than, returns true if expression1 <= expression2

#### < (expression1 < expression2 )

Returns 1 (TRUE) if expression2 is greater than expression1; 0 (FALSE) if expression1 is greater than expression2. This operator is the same of IFLT written in C Format.

### 

####  IFNE(expression1, expression2, range)

Check whether an expression is different from another; the second expression can be evaluated in a range. Returns 1 (TRUE) if expressions are different, 0 (FALSE) if the expressions are equal.

Out of range, returns true if expression1 < (expression2-range); expression1 > (expression2+range)

#### != (expression1 != expression2 )

Check whether an expression is different from another. Returns 1 (TRUE) if expressions are different, 0 (FALSE) if the expressions are equal. This operator is the same of IFNE written in C Format.

####  NOT

The logical negation operator NOT returns the value 0 if its operand is nonzero and the value 1 if its operand is zero.

#### ! expression

The logical negation operator ! returns the value 0 if its operand is nonzero and the value 1 if its operand is zero. This operator is the same of NOT operator written in C Format.

#### && (expression1 && expression2 )

Logical AND. Returns 1 (TRUE) if both expressions are TRUE, otherwise return FALSE.

#### || (expression1 || expression2 )

Logical OR. Returns 1 (TRUE) if at least one of the two expressions is TRUE, otherwise return FALSE.

#### choose (condition, expression1, expression2 )

Select between two different results based on a logical expression. The arguments can be a channel or an expression.

#### flipflop(set, reset )

Returns true when the first expression sets to true and false when the second expression sets to true. The arguments can be a channel or an expression.