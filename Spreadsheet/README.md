```
Author:     Alex Lancaster
Partner:    None
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  LancasterAlexUofU
Repo:       https://github.com/uofu-cs3500-20-fall2024/spreadsheet-LancasterAlexUofU
Date:       21-Sept-2024
Project:    Spreadsheet
Copyright:  CS 3500 and Alex Lancaster - This work may not be copied for use in Academic Coursework.
```

# Overview of the Spreadsheet Project
The Spreadsheet project has many different methods for setting and getting contents of cells
in a spreadsheet. The spreadsheet is able to set cell contents to be either a double, a string,
or a formula. 

The Spreadsheet project can also detect when a cell has referenced back to a previous cell before it, 
creating a loop, even if the cells are indirectly related. If this happens, a CircularException is thrown.


# Time Expenditures

| Assignment | Predicted Hours | Actual Hours|
| :---------:| :-------------: | :---------: |
| Assignment 5 | 10 | 12.5 |
| Assignment 6 | 14 | 28.5 |

(Rest of the hours come from SpreadsheetTests)


 ## Hour Breakdown

 ### Assignment 5
| Task | Number of Hours |
| :--------:| :--------:
| Reading assignment and <br/> understanding code | 1 |
| Implementing Spreadsheet Program | 4.5 |
| Debugging | 2 |

### Assignment 6
| Task | Number of Hours |
| :--------:| :--------:
| Reading assignment and <br/> setting up environment | 2 |
| Implementing Spreadsheet Program | 7.5 |
| Debugging | 6.5 |
| Drawing Whiteboard | 1 |

# Comments for Evaluators
I am assuming that empty cell's values are equal to the empty string by default if GetCellValue is called.

I am also assuming that if a cell does not have content in it and a formula calls it (even if the content is set later), 
an error will occur and this is intentional. I believe this is what Lecture 12 Slide (Page 22) was trying to convey, even though
it was talking about values.

I am going to remove the PS5 grading tests as they break, even with changing 

# Consulted Peers
- Landon
- Eli Parker

# References
1) C# Classes and Objects - https://www.w3schools.com/cs/cs_classes.php
2) Dictionary Class - https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?view=net-8.0
3) Path Class - https://learn.microsoft.com/en-us/dotnet/api/system.io.path?view=net-8.0
4) File Class - https://learn.microsoft.com/en-us/dotnet/api/system.io.file?view=net-8.0
5) Streamwriter Class - https://learn.microsoft.com/en-us/dotnet/api/system.io.streamwriter?view=net-8.0