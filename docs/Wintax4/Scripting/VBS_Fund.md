## VBScript Fundamentals

### VB Script Data Types

VBScript has only one data type called Variant . A Variant is a special kind of data type that can contain different kinds of information, depending on how it is used. Because Variant is the only data type in VBScript, it is also the data type returned by all functions in VBScript.

### VB Script Variables

Variables are explicitly indicated in the script using the Dim statement:

Dim myVariable

Multiple variables are indicated by separating each variable name with a comma:

Dim MyVar1, MyVar2, MyVar3

### Assigning Values to Variables

Values are assigned to variables creating an expression as follows: the variable is on the left side of the expression and the value to be assigned to the variable is on the right. For example:

MyVar1 = 100

### Array Variables

Array variables and scalar variables are declared in the same way, except that the declaration of an array variable uses parentheses () after the variable name. In the following example, a single-dimension array containing 11 elements is declared:

MyVar1(10)

Arrays aren't limited to a single dimension

MyTable variable is a two-dimensional array consisting of 6 rows and 5 columns:

MyTable(5,6)

### Conditional & Looping Statement

The flow of the script can be controlled with conditional statements and looping statements 

  * If...Then...Else statement

  * Do...Loop: Loops while or until a condition is True.

  * While...Wend: Loops while a condition is True.

  * For...Next: Uses a counter to run statements for a specified number of times.

  * For Each...Next: Repeats a group of statements for each item in a collection or each element of an array.




### VBScript Procedures

In VBScript, there are two kinds of procedures; the Sub procedure and the Function procedure.

### Sub Procedures

A Sub procedure is a series of VBScript statements (enclosed by Sub and End Sub statements) that perform actions but don't return a value. A Sub procedure can accept topics (constants, variables, or expressions that are passed by a calling procedure). If a Sub procedure has no topics, its Sub statement must include an empty set of parentheses ().

My First Script

The following Sub procedure uses two intrinsic VBScript functions, MsgBox and InputBox to prompt a user for information. It then displays the results of a calculation based on that information.

Sub MyFisrtScript()

Dim A 

A = InputBox( "Please enter a Number" ) 

MsgBox "Your Number is " & A 

End Sub

### VBScript Comments

The comments are indicated by the character: '

Sub MyTest()

Dim A 'declaration of my variable

A = InputBox( "Please enter a Number" ) 'prompt to users

MsgBox "Your Number is " & A 'print the result

End Sub

### Function Procedures

A Function procedure is a series of VBScript statements enclosed by the Function and End Function statements. A Function procedure is similar to a Sub procedure, but it can also return a value. A Function procedure can take topics (constants, variables, or expressions that are passed to it by a calling procedure). If a Function procedure has no topics, its Function statement must include an empty set of parentheses. A Function returns a value by assigning a value to its name in one or more statements of the procedure. The return type of a Function is always a Variant.

In the following example, the Celsius function calculates degrees Celsius from degrees Fahrenheit. When the function is called from the ConvertTemp Sub procedure, a variable containing the topic value is passed to the function. The result of the calculation is returned to the calling procedure and displayed in a message box.

Sub ConvertTemp()

Dim temp

temp = InputBox( "Please enter the temperature in degrees F.", 1)

MsgBox "The temperature is " & Celsius(temp) & " degrees C."

End Sub

Function Celsius(fDegrees)

Celsius = (fDegrees - 32) * 5 / 9

End Function

### Getting Data into and out of Procedures

Each piece of data is passed into the user's procedures using an argument . Arguments serve as placeholders for the data to be passed into the user's procedure. Name topics with any valid variable name. When a procedure is created using either the Sub statement or the Function statement, parentheses must be included after the name of the procedure. Any topic placed inside these parentheses, separated by commas. For example, in the following example, fDegrees is a placeholder for the value being passed into the Celsius function for conversion.

Function Celsius(fDegrees)

Celsius = (fDegrees - 32) * 5 / 9

End Function

To get data out of a procedure, use a Function. Remember, a Function procedure can return a value; a Sub procedure can't.

Using Sub and Function Procedures in Code:

A Function in the code must always be used on the right side of a variable assignment or in an expression. For example:

Temp = Celsius(fDegrees)

-or-

MsgBox "The Celsius temperature is " & Celsius(fDegrees) & " degrees."

To call a Sub procedure from another procedure, type the name of the procedure along with values for any required arguments, each separated by a comma. The Call statement is not required, but used, enclose any topic in parentheses.

The following example shows two calls to the MyProc procedure. One uses the Call statement in the code; the other doesn't. Both do exactly the same thing.

Call MyProc(firstarg, secondarg)

MyProc firstarg, secondarg

Notice that the parentheses are omitted in the call when the Call statement isn't used.

### Set Statement

Assign an object reference to a variable

Set objectvar = { objectexpression | New classname }

#### Parameters

objectvar

Required. Name of the variable or property; follows standard variable naming conventions.

objectexpression

Optional. Expression consisting of the name of an object, another declared variable of the same object type, or a function or method that returns an object of the same object type.

New

Keyword used to create a new instance of a class. If objectvar contains a reference to an object, that reference is released when the new one is assigned. The New keyword can only be used to create an instance of a class.

classname

Optional. Name of the class being created. A class and its members are defined using the Class statement.

Examples:

Dim oExcel

Set oExcel = CreateObject( "Excel.Application" )

### CreateObject Function

Creates and returns a reference to a COM object. CreateObject cannot be used to create instances of classes in VBscript unless those classes are explicitly exposed as COM components.

#### Parameters

ProgId

Required, String. The program ID of the object to create.

In order to know the program ID, use a Object Browser tools

For example OLE/COM Object Viewer of C++ tools or the VBA environment into Microsoft Excel

Sub Test_Call ()

Dim WshShell, oExec

Set WshShell = CreateObject( "WScript.Shell" )

Set oExec = WshShell.Exec(calc)

End Sub

Dim oMatlab

Set oMatlab = CreateObject( "Matlab.Application" )

###  JScript Fundamentals

All WinTAX4 functions available in VBScript mode are also available in JScript. There are no differences of performance between the two languages, the customers can therefore select the language based on their preferences. Many examples described have been resumed from the official documentation that Microsoft has available at the following address:

VBScript language: [https://msdn.microsoft.com/en-us/library/d1wf56tt(v=vs.84).aspx](https://msdn.microsoft.com/en-us/library/d1wf56tt\(v%3dvs.84\).aspx)

Jscript language: [https://msdn.microsoft.com/en-us/library/6974wx4d(v=vs.94).aspx](https://msdn.microsoft.com/en-us/library/6974wx4d\(v%3dvs.94\).aspx)