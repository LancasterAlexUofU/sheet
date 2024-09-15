```
Author:     Alex Lancaster
Partner:    None
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  LancasterAlexUofU
Repo:       https://github.com/uofu-cs3500-20-fall2024/spreadsheet-LancasterAlexUofU
Date:       5-Sept-2024
Project:    Formula
Copyright:  CS 3500 and Alex Lancaster - This work may not be copied for use in Academic Coursework.
```

# Overview of the Formula Project

The Formula class handles formulas in infix formula notation, given as string,
and ensures that the formula is has the proper syntax of a formula.

### Examples of valid formulas are the following:

- 1 + 1
- x1 / 2
- (a1)*(b1)
- 10e2 - 37

### Examples of INVALID  formula are the following:

- 1 + 1 = 2
- 1A1 + 2
- x + y
- 5**2
- 5^2
- -1 + 2
- ((x1)

If a formula does not have proper syntax, a FormulaFormatException is thrown.

The Formula class also normalizes all variables, numbers, and removes spaces,
to make a canonical string representation of the formula.

### Original formula
x1 + x2 + 10e1 + 1.0

### Canonical representation
X1+X2+100+1

The Formula class further includes a GetVariables function, which returns a set
of all variables in a formula, and ToString, which returns a canonical string 
representation of the formula.

# Time Expenditures

| Assignment | Predicted Hours | Actual Hours|
| :---------:| :-------------: | :---------: |
| Assignment 2 | 12 | 17 |
| Assignment 4 | 18 | 1 |


 ## Hour Breakdown
 ### Assignment 2
| Task | Number of Hours |
| :--------:| :--------:
| Setting up coding environment & <br /> reading documentation | 3 |
| Implementing formula syntax logic | 6 |
| Writing Formula Tests | 1 |
| Implementing ToString Function | 2.5 |
| Writing ToString Tests | 0.5 |
| Implementing GetVariables Function | 1 |
| Writing GetVariable Tests | 0.5 |
| Improving readability & <br/> quality of code | 2.5 |

### Assignment 4
| Task | Number of Hours |
| :--------:| :--------:
| Setting up coding environment & <br /> reading documentation | 1 |

# Comments for Evaluators
Work stands on its own.

# Consulted Peers
Eli (unknown last name)

# References
1) Microsoft RegEx Documentation - https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference
2) Best Practices for Exceptions - https://learn.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions
3) RegEx Translator - https://www.regextranslator.com
4) RegEx 101 - https://regex101.com
5) Stack Class - https://learn.microsoft.com/en-us/dotnet/api/system.collections.stack?view=net-8.0
6) this (C# Reference) - https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/this
7) String.Replace Method - https://learn.microsoft.com/en-us/dotnet/api/system.string.replace?view=net-8.0
8) Double.TryParse Method - https://learn.microsoft.com/en-us/dotnet/api/system.double.tryparse?view=net-8.0