```
Author:     Alex Lancaster
Partner:    None
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  LancasterAlexUofU
Repo:       https://github.com/uofu-cs3500-20-fall2024/spreadsheet-LancasterAlexUofU
Date:       13-Sept-2024
Project:    DependencyGraph
Copyright:  CS 3500 and Alex Lancaster - This work may not be copied for use in Academic Coursework.
```

# Overview of the DependencyGraph Project

The Dependency Graph project determines which cells must be evaluated before other cells in the spreadsheet solution. 
For example, cell A1 could contain formula A2 \* A3 and cell A2 contains B1 + B2. Therefore, B1 and B2 need to be evaluated before A2 \* A3.

The project has the following pubic methods:

**DependencyGraph** - Creates an empty dependency graph.

**Size** - Returns an integer containing the number of ordered pairs in the dependency graph.

**HasDependents(string nodeName)** - Returns a bool that reports whether the given node has dependents.

**HasDependees(string nodeName)** - Returns a bool that reports whether the given node has dependees.

**GetDependents(string nodeName)** - Returns an IEnumerable\<string> that contains the dependents of the given node.

**GetDependees(string nodeName)** - Returns an IEnumerable\<string> that contains the dependees of the given node.

**AddDependency(string dependee, string dependent)** - Adds the ordered pair (dependee, dependent) to the dependency graph, if it doesn't already exist (otherwise nothing happens).

**RemoveDependency(string dependee, string dependent)** - Removes the ordered pair (dependee, dependent) from the dependency graph, if it exists (otherwise nothing happens).

**ReplaceDependents(string nodeName, IEnumerable\<string> newDependents)** - Removes all existing ordered pairs of the form (nodeName, *) and then for each t in newDependents adds the ordered pair (nodeName, t).

**ReplaceDependees(string nodeName, IEnumerable\<string> newDependees)** - Removes all existing ordered pairs of the form (*, nodeName) and then for each t in newDependees, adds the ordered pair (t, nodeName).


# Time Expenditures

| Assignment | Predicted Hours | Actual Hours|
| :---------:| :-------------: | :---------: |
| Assignment 3 | 14 | 16 |


 ## Hour Breakdown

| Task | Number of Hours |
| :--------:| :--------:
| Setting up coding environment, <br /> reading documentation, & research | 2 |
| Writing Dependency Graph Tests | 5.5 |
| Dependency Graph Implementation | 3.5 |
| Debugging | 3 |
| Formatting and Quality Assurance | 2 |

# Comments for Evaluators
The autograder kept throwing errors about different formatting issues, such as
having a space after a bracket (... = [];). If the space was removed, it would 
say it would need a space. Therefore, I added to ignore the errors. 

I apologize about the number of commits but every time I would fix one thing for the auto grader,
another error would pop up (and these warnings/errors never showed up is visual studio).

Currently, everything has "this." so the autograder could compile the code. I plan on talking
with a TA to figure out what is going on and eventually improve this part of the code.

# Consulted Peers
- Eli
- Yen

# References

1) CollectionAssert Class: https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting.collectionassert?view=visualstudiosdk-2022
2) Dictionary Class: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?view=net-8.0
3) Any: https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.any?view=net-8.0
4) LINQ: https://learn.microsoft.com/en-us/dotnet/csharp/linq/get-started/introduction-to-linq-queries