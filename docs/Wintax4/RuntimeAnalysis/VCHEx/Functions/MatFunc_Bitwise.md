###  Bitwise operators

Bit-wise operators can be used to analyze the status of individual bits in diagnostic channels by using the AND operator to mask the values of other bits.

The bitwise logical operators available are AND, OR, XOR, bitwiseNOT, << (left shift) and >> (right shift) as shown below. The operators can also be written in C format: & (AND), | (OR), ~ (NOT).

####  AND ( expression1 AND expression2 )

The bitwise AND looks at the binary representation of the values of two expressions and performs the logical AND operation on each pair of corresponding bits. In each pair, the result is 1 if both bits are 1. Otherwise, the result is 0.

| expression1 AND expression2 | expression1 |
| --- | --- |
| 0 | 1 |
| expression2 | 0 | 0 | 0 |
| 1 | 0 | 1 |
  
#### & ( expression1 & expression2 )

The bitwise & looks at the binary representation of the values of two expressions and performs the logical AND operation on each pair of corresponding bits. In each pair, the result is 1 if both bits are 1. Otherwise, the result is 0. This operator is the same of bitwise AND written in C Format.

####  OR ( expression1 OR expression2 )

The bitwise OR looks at the binary representation of the values of two expressions and performs the logical OR inclusive operation on each pair of corresponding bits. In each pair, the result is 0 if both bits are 0. Otherwise, the result is 1.

| expression1 OR expression2 | expression1 |
| --- | --- |
| 0 | 1 |
| expression2 | 0 | 0 | 1 |
| 1 | 1 | 1 |
  
#### | ( expression1 | expression2 )

The bitwise | looks at the binary representation of the values of two expressions and performs the logical OR inclusive operation on each pair of corresponding bits. In each pair, the result is 0 if both bits are 0. Otherwise, the result is 1. This operator is the same of bitwise OR written in C Format.

####  XOR ( expression1 XOR expression2 )

The bitwise XOR looks at the binary representation of the values of two expressions and performs the logical OR exclusive operation on each pair of corresponding bits. In each pair, the result is 1 if the two bits are different and 0 if they are the same.

| expression1 XOR expression2 | expression1 |
| --- | --- |
| 0 | 1 |
| expression2 | 0 | 0 | 1 |
| 1 | 1 | 0 |
  
####  bitwiseNOT(expression)

The bitwise NOT looks at the binary representation of the value and performs the logical NOT on each bit, forming the one's complement of the given binary value. Digits which were 0 become 1, and vice versa.

|  | expression |
| --- | --- |
| 0 | 1 |
| bitwiseNOT (expression) | 1 | 0 |
  
Example: Using bitwise logic to check individual bits

As an example application, the graph above shows a diagnostic channel, DIAG , where each one of the 16 bits represents the status of a particular device or process (0=OK; 1=NOK). The combination of all the status bits results in a number (shown in hexadecimal in this case) which is not easy to be interpreted.

The two math channels, BIT5 and BIT6 use the AND operator to isolate the values of bits 5 and 6 as shown below.

#### ~ (expression)

The bitwise ~ looks at the binary representation of the value and performs the logical NOT on each bit, forming the one's complement of the given binary value. Digits which were 0 become 1, and vice versa. This operator is the same of bitwiseNOT written in C Format.

#### << ( expression1 << expression2 )

The left shift operator causes the bit pattern in the first operand (expression1) to be shifted left the number of bits specified by the second operand (expression2). Bits vacated by the shift operation are zero-filled. This operator is the same of [shiftl](../../VCH/Functions/VCH_functions.md#vch_Shiftl) written in C Format.

#### >> ( expression1 << expression2 )

The right shift operator causes the bit pattern in the first operand (expression1) to be shifted right the number of bits specified by the second operand (expression2). Bits vacated by the shift operation are zero-filled for unsigned quantities. For signed quantities, the sign bit is propagated into the vacated bit positions. This operator is the same of [shiftr](../../VCH/Functions/VCH_functions.md#vch_Shiftr) written in C Format.