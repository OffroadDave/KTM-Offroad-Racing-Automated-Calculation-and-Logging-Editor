### Numerical functions

#### ceil(expression)

Ceiling. Returns the value of expression rounded to the nearest higher integer.

#### ceileven(expression)

Ceiling. Returns the value of expression rounded to the nearest higher even integer.

#### ceilodd(expression)

Ceiling. Returns the value of expression rounded to the nearest higher odd integer.

#### div(expression1, expression2)

The result is the integer quotient of expression1 divided by expression2. The integer quotient discards any remainder and retains only the integer portion.

#### fact(expression)

Returns the factorial of a number. The factorial of a number is equal to 1*2*3*...* number. If number is not an integer, it is truncated; if the number is negative, returns NoRx.

#### factdouble(expression)

Returns the double factorial of a number. The double factorial of an odd number is equal to 1*3*5*7...*number; the double factorial of an even number is equal to (2*4*6*8...*number). If number is not an integer, it is truncated; if the number is negative, returns NoRx.

#### floor(expression)

Returns the value of expression rounded to the nearest lower integer.

#### flooreven(expression)

Returns the value of expression rounded to the nearest lower even integer.

#### floorodd(expression)

Returns the value of expression rounded to the nearest lower odd integer.

#### fmod(expression)

Returns the floating-point remainder of numerator/denominator. Numerator and denominator can be values or expressions.

#### frac(expression)

Returns the fractional part of a number. The argument can be a value or an expression.

#### gcd(N0, N1, N2, N3, N4, N5, N6, N7, N8, N9, N10, N11, N12, N13, N14, N15)

Returns the greatest common divisor of two or more integers. The greatest common divisor is the largest integer that divides the numbers without a remainder. The function accepts 16 optionally input values. If you need more, you can call recursively the function gcd as a parameter of the same function.

#### iseven(expression)

Returns 1 if the number is even, and 0 otherwise.

#### isnan(expression)

Returns TRUE (1) if expression is a NaN (Not a Number) or NoRx.

#### isnum(expression)

Returns TRUE (1) if expression gets a valid number.

#### isodd(expression)

Returns 1 if the number is odd, and 0 otherwise.

#### isprime(expression)

Returns 1 if the number is prime, and 0 otherwise.

####  lcm(N0, N1, N2, N3, N4, N5, N6, N7, N8, N9, N10, N11, N12, N13, N14, N15)

Returns the least common multiple of numbers. The least common multiple is the smallest positive integer that is a multiple of all integer arguments number1, number2, and so on. If number is not an integer, it is truncated. The function accepts 16 optionally input values. If you need more, you can call recursively the function lcm as a parameter of the same function.

#### mod(numerator, denominator)

Remainder or modulo

Returns the integer remainder of numerator/denominator. Numerator and denominator can be values or expressions.

Example:

to get minutes and seconds from a time counter

minutes -> time /60 seconds -> MOD(time,60)

#### nk(n, k)

Returns the binomial coefficient n!/((n–k)! k!) where n and k are non negative integers. This is the number of combinations of n things taken k at a time.

#### rand()

Returns a random number between 0 and 1.

#### randbetween()

Returns a random number between the numbers you specify.

#### round(expression, n)

Rounds expression to a precision given by n.

#### roundn(expression)

Rounds expression to the nearest integer value.

#### sign(expression)

Determines the sign of a number. Returns 1 if the number is positive, zero (0) if the number is 0, and -1 if the number is negative.