```
Author:     Alex Lancaster
Partner:    None
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  LancasterAlexUofU
Repo:       https://github.com/uofu-cs3500-20-fall2024/spreadsheet-LancasterAlexUofU
Date:       21-Sept-2024
Project:    SpreadsheetTests
Copyright:  CS 3500 and Alex Lancaster - This work may not be copied for use in Academic Coursework.
```

# Overview of the SpreadsheetTests Project
The SpreadsheetTests project consists of 22 tests covering all public methods extensively.

The main focus is on the SetCellContents, especially the Formula version. This is because 
circular exceptions are only thrown here and relies on many sub methods, so extensive testing
was done particularly on this method.

This test method also creates a complex cell dependency graph to ensure all cells will be recalculated
even with multiple indirect dependences

# Time Expenditures

| Assignment | Predicted Hours | Actual Hours|
| :---------:| :-------------: | :---------: |
| Assignment 5 | 10 | 12.5 |
| Assignment 6 | 14 | 13 |

(Rest of the hours come from Spreadsheet)

 ## Hour Breakdown

 ### Assignment 5
| Task | Number of Hours |
| :--------:| :--------:
| Implementing Spreadsheet Tests | 4 |
| Debugging | 1 |

### Assignment 6
| Task | Number of Hours |
| :--------:| :--------:
| Implementing Spreadsheet Tests | 7 |

# Comments for Evaluators
I am assuming, following the assignment instructions, that even if a cell is removed
by passing string.Empty, SetCellContents should still return the cell that was passed in
instead of not returning anything. So, in tests, I will be checking that SetCellContents
still returns the passed-in cell as the correct output  even if it was removed.

# Consulted Peers
- Landon

# References
