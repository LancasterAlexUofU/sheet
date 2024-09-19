```
Author:     Alex Lancaster
Partner:    None
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  LancasterAlexUofU
Repo:       https://github.com/uofu-cs3500-20-fall2024/spreadsheet-LancasterAlexUofU
Date:       18-Sept-2024
Project:    FormulaTests
Copyright:  CS 3500 and Alex Lancaster - This work may not be copied for use in Academic Coursework.
```

# Overview of the FormulaTests Project
### FormulaSyntaxTests

FormulaSyntaxTests runs 42 various tests on Formula.dll to ensure that the formula constructor is properly working.

This is done by checking the 8 Formula Syntax and Validation Rules for a formula in infix notation.

This test additionally checks for valid variables as well as numbers in proper scientific notation.

### GradingTests

This test is provided by Joe Zachary, Daniel Kopta, and Jim de St. Germain. It contains additional tests
on top of what already is tested by FormulaSyntaxTests.

### EvaluationTests

EvaluationTests runs 35 various tests to ensure that formulas are properly being evaluated and
formula equality methods are returning correctly.

# Time Expenditures

| Assignment | Predicted Hours | Actual Hours|
| :---------:| :-------------: | :---------: |
| Assignment 1 | 12 | 8 |
| Assignment 4 | 18 | 11 |


(Includes 7.5 hours from Formula Project)

# Time Breakdown
### Assignment 4
| Task | Number of Hours |
| :--------:| :--------:
| Implemented Evaluation Test Methods | 3.5

# Comments for Evaluators

### Comment about code coverage for FormulaSyntaxTests:

With test methods that have "[ExpectedException(typeof(FormulaFormatException))]", the last curly brace is marked as unexplored.
This is okay and we went over it in class.

For Test Method FormulaConstructor_TestVariablesContainingE_Invalid, the same thing happens on two lines, 
as Assert.Fail isn't called (as it is catched by FormulaFormatException). It is marked as red, but is okay as
it means the test itself isn't failing.

### Comment about code coverage for EvaluationTests
For the Evaluator_VarComplexFormula and Evaluator_NumVarComplexFormula methods, I use a switch statement to assign
different values to different variables. The lambda expression will throw error CS1643: Not all code paths return a 
value in method of type 'Formula.Lookup' if I do not add a default path. However, since it is just a default path and
the formula will always be the same, it is never triggered. This leads to the code not being covered. However, the
program won't compile if I remove the default statement, so it must be left this way.

Also, the last curly brace in the program is not counted as covered.



# Consulted Peers
- Landon H.
- Eli

# References
1) Markdown Cheat Sheet - https://www.markdownguide.org/cheat-sheet/
2) Markdown Tables - https://www.codecademy.com/resources/docs/markdown/tables
3) Asset Class - https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting.assert?view=visualstudiosdk-2022