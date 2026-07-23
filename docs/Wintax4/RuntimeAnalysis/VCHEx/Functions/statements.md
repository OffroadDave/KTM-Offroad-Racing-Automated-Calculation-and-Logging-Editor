### Statements

## 

#### dim

Declares variables, allocates storage space and optionally initializes it with a specified value; if not specified the variable is initialized by default as NoRx.

Syntax:

dim variable [=number];

A variable declared as local is available only inside the virtual channel and has its scope resolution inside the code block where it is declared.

The last value is maintained for each sample calculated.

Local variables declared with a dim statement has higher precedence than the global variable with the same name declared with static statement.

#### static

Declares variables, allocates storage space and optionally initializes it with a specified value; if not specified the variable is initialized by default as NoRx..

Syntax:

static variable [=number];

A variable declared static retains its state between all the virtual channels in the loaded libraries.

Local variables declared with a dim statement has higher precedence than the global variable with the same name declared with static statement.

#### if, else

Controls conditional branching. If the expression value is nonzero, statement1 is executed. If the optional else is present, statement2 is executed if the value of expression is zero.

Syntax:

If (expression)

{

statement1;

}

else

{

statement2;

}

#### for

Use the for statement to construct loops that must execute a specified number of times. A for loop terminates when a break or return within statement is executed. A continue statement in a for loop terminates only the current iteration.

Syntax:

for ( init-expression ; conditional-expression ; loop-expression )

{

statement;

}

#### while

Executes statement repeatedly until expression evaluates to zero. A while loop can also terminate when a break or return within the statement body is executed. Use continue to terminate the current iteration without exiting the while loop; continue passes control to the next iteration of the while loop.

Syntax:

while ( expression )

{

statement;

}

#### break

The break statement terminates the execution of the nearest enclosing loop or conditional statement in which it appears. Control passes to the statement that follows the terminated statement, if any; break is used with the for and while loop statements.

Syntax:

break;

#### continue

Forces transfer of control to the controlling expression of the smallest enclosing for or while loop. Any remaining statements in the current iteration are not executed. The next iteration of the loop is determined as follows: 1)In a while loop the next iteration starts by reevaluating the controlling expression of the while statement. 2) In a for loop continue causes loop-expression to be executed.

Syntax:

continue;

#### return

Exits from the current function and returns a value from that function. The value of expression is returned to the calling function.

Syntax:

return expression;

#### Switch_case

Allows the selection of multiple code sections based on the value of an expression. The body of the 'switch_case' statement consists of a series of 'case' labels and an optional 'default' label. Each 'case' label expression is compared to the condition for equality. If a match is found, control is passed to the corresponding statement. If no 'case' matches the condition and a 'default' label is present, control is transferred to it.

Syntax:

**switch_case**(expression) { **case** expression:

{ statement; } ... **default:**{ statement; }

}

#### case

The expression in each 'case' label is compared to the 'switch_case' condition for equality. If a match is found, control passes to the first statement following the corresponding 'case' label." 

Syntax:

Case expression: {statement}