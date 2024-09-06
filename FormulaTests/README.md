```
Author:     Alex Lancaster
Partner:    None
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  LancasterAlexUofU
Repo:       https://github.com/uofu-cs3500-20-fall2024/spreadsheet-LancasterAlexUofU
Date:       30-Aug-2024
Project:    FormulaTests
Copyright:  CS 3500 and Alex Lancaster - This work may not be copied for use in Academic Coursework.
```

# Overview of the FormulaTests Project
FormulaTests runs 27 various tests on Formula.dll to ensure that the formula constructor is properly working.

This is done by checking the 10 Formula Syntax and Validation Rules for a formula in infix notation.

This test additionally checks for valid variables as well as numbers in proper scientific notation.
# Time Expenditures

| Assignment | Predicted Hours | Actual Hours|
| :---------:| :-------------: | :---------: |
| Assignment 1 | 12 | 8|

# Comments for Evaluators
Comment about code coverage:

With test methods that have "[ExpectedException(typeof(FormulaFormatException))]", the last curly brace is marked as unexplored.
This is okay and we went over it in class.

For Test Method FormulaConstructor_TestVariablesContainingE_Invalid, the same thing happens on two lines, 
as Assert.Fail isn't called (as it is catched by FormulaFormatException). It is marked as red, but is okay as
it means the test itself isn't failing.

# Consulted Peers
- Landon H.
- Eli

# References
1. Markdown Cheat Sheet - https://www.markdownguide.org/cheat-sheet/
2. Markdown Tables - https://www.codecademy.com/resources/docs/markdown/tables