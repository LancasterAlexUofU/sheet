```
Author:      Alex Lancaster
Partner:     Landon Huking
Start Date:  27-Aug-2024
Course:      CS 3500, University of Utah, School of Computing
GitHub ID:   LancasterAlexUofU
Repo:        https://github.com/uofu-cs3500-20-fall2024/spreadsheet-LancasterAlexUofU
Commit Date: 27-Sept-2024
Solution:    Spreadsheet
Copyright:   CS 3500 and Alex Lancaster & Landon Huking - This work may not be copied for use in Academic Coursework.
```

# Overview of the Spreadsheet Functionality

The Spreadsheet program is currently capable of:

- Validating the syntax of infix formula notation
- Returning a list of all variables contained in a formula
- Retuning a canonical string representation of a formula
- Determining which cells must be evaluated before other cells
- Evaluating a formula with numbers and variables
- Getting and Setting Cell Contents in a spreadsheet
- Detecting loops for cell dependency values
- Setting contents of a cell
- Automatically calculate cell values
- GUI interface to visualize spreadsheet model

Future extensions are:

- Small GUI improvements

# Launching
To launch from source:
- Open up Visual Studio
- Set the startup project to GUI
- GUI will launch on localhost browser

# Time Expenditures

| Assignment | Predicted Hours | Actual Hours|
| :---------:| :-------------: | :---------: |
| Assignment 1 | 12 | 8|
| Assignment 2 | 12 | 17 |
| Assignment 3 | 14 | 16 |
| Assignment 4 | 18 | 11 |
| Assignment 5 | 10 | 12.5 |
| Assignment 6 | 14 | 30.5 |
| Assignment 7 | 15 | 11.5 |

# Comments for Evaluators
Read FormulaTests README for comment about code coverage.

Read DependencyGraph README about autograder and reason for excessive commit history.

Read Formula README about very minor code coverage comment.

Read SpreadsheetTests about cell removal assumption.

For assignment 7 README requirements, we put them in the GUI README.

# Consulted Peers
- Landon Huking
- Eli
- Yen

# Examples of Good Software Practices (GSP)

### - Separation of concerns
Many functions that were complex were broken down into smaller helper methods. For example, in formula evaluation,
each algorithm check was put into a switch case and each check was its own separate functions. This is done all throughout
the projects in this solution and makes complex functions easier to understand.

### - Well Commented and Short Methods
All of the project methods are well commented in detail. All test cases have comments that tell exactly what they do
and complex RegEx variables in Formula explicitly explain what they take in. All methods and helper methods have be 
purposely shortened so that they can fit on a single page for better readability.

### - Testing strategies
Many testing strategies were employed so that the project's code would be properly working. For example, in FormulaTests,
many complex formulas were created with many different types of interactions between variables, parentheses, operands, and numbers.
This was done so that many small tedious tests weren't needed and a large complex blanket test would cover almost all scenarios.
This was done multiple times throughout the different test classes.

## More Examples of Good Software Practices

### - DRY
There were many methods in the spreadsheet project that fit the DRY aspect of GSP. For one, I combined the private SetCellContents method for double and string
into one method so that there was no repeated code for a very similar function. I also removed any dependencies in SetContentOfCell instead of each private
method and did the same thing for re-evaluation.

## - Regression Testing
The spreadsheet project implemented regression testing to ensure that old tests (updated to the new format) would still pass even with major changes
to the spreadsheet project. This let me continuously test while developing to ensure small changes over time did not break any old functionality. Many bugs
were caught using this method (such as the spreadsheet changing during a circularException) and allowed me to quickly revert and fix any errors.

## - Abstraction
Many of the functions are complex in nature, so I broke down the function into different smaller parts. This allowed me to abstract them away so that anyone working
on the main function can easily look at abstracted function and understand its purpose. Such examples are checking that a cellName is 
valid using IsVar(name), or by using AddCell or RemoveCell in SetCellContents.


# Time Management and Estimation Skills

There are a few things I have learned about my time management and estimation skills. First off, I have learned I need to estimate
more time when I am learning something new. By this, I mean something that I will need to learn and understand through the growing
pains of understanding how it works. For me in the spreadsheet assignment, it was learning how to save, load, know what a 
properly formatted file path looks like, and how to check for it (as you can see from the awful amount of time I spent on this
assignment). For my time management skills, I have learned the earlier I start on something, the easier it makes life. If I wait to start after
the weekend, the projects can pile up and overlap. Overall though, I think my time management skills have been fairly on point.


# References
1) Markdown Cheat Sheet - https://www.markdownguide.org/cheat-sheet/
2) Markdown Tables - https://www.codecademy.com/resources/docs/markdown/tables
3) Microsoft RegEx Documentation - https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference
4) Best Practices for Exceptions - https://learn.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions
5) RegEx Translator - https://www.regextranslator.com
6) RegEx 101 - https://regex101.com
7) Stack Class - https://learn.microsoft.com/en-us/dotnet/api/system.collections.stack?view=net-8.0
8) this (C# Reference) - https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/this
9) String.Replace Method - https://learn.microsoft.com/en-us/dotnet/api/system.string.replace?view=net-8.0
10) Double.TryParse Method - https://learn.microsoft.com/en-us/dotnet/api/system.double.tryparse?view=net-8.0
11) CollectionAssert Class - https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting.collectionassert?view=visualstudiosdk-2022
12) Dictionary Class - https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?view=net-8.0
13) Any - https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.any?view=net-8.0
14) LINQ - https://learn.microsoft.com/en-us/dotnet/csharp/linq/get-started/introduction-to-linq-queries
15) Asset Class - https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting.assert?view=visualstudiosdk-2022
16) C# Classes and Objects - https://www.w3schools.com/cs/cs_classes.php
17) Path Class - https://learn.microsoft.com/en-us/dotnet/api/system.io.path?view=net-8.0
18) File Class - https://learn.microsoft.com/en-us/dotnet/api/system.io.file?view=net-8.0
19) Streamwriter Class - https://learn.microsoft.com/en-us/dotnet/api/system.io.streamwriter?view=net-8.0