// <copyright file="Spreadsheet.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// <summary>
// Author:    Alex Lancaster
// Partner:   None
// Date:      21-Sept-2024
// Course:    CS 3500, University of Utah, School of Computing
// Copyright: CS 3500 and Alex Lancaster - This work may not
//            be copied for use in Academic Coursework.
//
// I, Alex Lancaster, certify that I wrote this code from scratch and
// did not copy it in part or whole from another source.  All
// references used in the completion of the assignments are cited
// in my README file.
//
// File Contents
//      This class creates the main functions of a spreadsheet.
//      Spreadsheet can set the contents of cells to either be
//      a double, a string, or a formula. Cell contents
//      can be retrieved and spreadsheet also checks
//      for any circular loops for cells calling itself.
// <summary>

// Written by Joe Zachary for CS 3500, September 2013
// Update by Profs Kopta and de St. Germain
//     - Updated return types
//     - Updated documentation
namespace CS3500.Spreadsheet;

using CS3500.Formula;
using CS3500.DependencyGraph;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;
using System.Linq.Expressions;

/// <summary>
///   <para>
///     Thrown to indicate that a change to a cell will cause a circular dependency.
///   </para>
/// </summary>
public class CircularException : Exception
{
}

/// <summary>
///   <para>
///     Thrown to indicate that a name parameter was invalid.
///   </para>
/// </summary>
public class InvalidNameException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidNameException"/> class.
    /// </summary>
    /// <param name="message">Message explaining why exception way thrown.</param>
    internal InvalidNameException(string message)
        : base(message)
    {
    }
}

/// <summary>
///   <para>
///     An Spreadsheet object represents the state of a simple spreadsheet.  A
///     spreadsheet represents an infinite number of named cells.
///   </para>
/// <para>
///     Valid Cell Names: A string is a valid cell name if and only if it is one or
///     more letters followed by one or more numbers, e.g., A5, BC27.
/// </para>
/// <para>
///    Cell names are case insensitive, so "x1" and "X1" are the same cell name.
///    Your code should normalize (uppercased) any stored name but accept either.
/// </para>
/// <para>
///     A spreadsheet represents a cell corresponding to every possible cell name.  (This
///     means that a spreadsheet contains an infinite number of cells.)  In addition to
///     a name, each cell has a contents and a value.  The distinction is important.
/// </para>
/// <para>
///     The <b>contents</b> of a cell can be (1) a string, (2) a double, or (3) a Formula.
///     If the contents of a cell is set to the empty string, the cell is considered empty.
/// </para>
/// <para>
///     By analogy, the contents of a cell in Excel is what is displayed on
///     the editing line when the cell is selected.
/// </para>
/// <para>
///     In a new spreadsheet, the contents of every cell is the empty string. Note:
///     this is by definition (it is IMPLIED, not stored).
/// </para>
/// <para>
///     The <b>value</b> of a cell can be (1) a string, (2) a double, or (3) a FormulaError.
///     (By analogy, the value of an Excel cell is what is displayed in that cell's position
///     in the grid.)
/// </para>
/// <list type="number">
///   <item>If a cell's contents is a string, its value is that string.</item>
///   <item>If a cell's contents is a double, its value is that double.</item>
///   <item>
///     <para>
///       If a cell's contents is a Formula, its value is either a double or a FormulaError,
///       as reported by the Evaluate method of the Formula class.  For this assignment,
///       you are not dealing with values yet.
///     </para>
///   </item>
/// </list>
/// <para>
///     Spreadsheets are never allowed to contain a combination of Formulas that establish
///     a circular dependency.  A circular dependency exists when a cell depends on itself.
///     For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
///     A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
///     dependency.
/// </para>
/// </summary>
public class Spreadsheet
{
    /// <summary>
    ///     All variables are letters followed by numbers.  This pattern
    ///     represents valid variable name strings. <br />
    ///     Matches one or more letters, upper or lowercase, followed by one or more numbers.
    /// </summary>
    private const string VariableRegexPattern = @"[a-zA-Z]+\d+";

    // Creates a dictionary of cell names and pairs them with their contents / values
    private Dictionary<string, Cells> sheet = [];
    private HashSet<string> nonEmptyCells = [];
    private DependencyGraph dg = new();

    /// <summary>
    ///   Provides a copy of the names of all of the cells in the spreadsheet
    ///   that contain information (i.e., not empty cells).
    /// </summary>
    /// <returns>
    ///   A set of the names of all the non-empty cells in the spreadsheet.
    /// </returns>
    public ISet<string> GetNamesOfAllNonemptyCells()
    {
        return nonEmptyCells;
    }

    /// <summary>
    ///   Returns the contents (as opposed to the value) of the named cell.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   Thrown if the name is invalid.
    /// </exception>
    ///
    /// <param name="name">The name of the spreadsheet cell to query. </param>
    /// <returns>
    ///   The contents as either a string, a double, or a Formula.
    ///   See the class header summary.
    /// </returns>
    public object GetCellContents(string name)
    {
        name = name.ToUpper();
        if (IsVar(name))
        {
            if (nonEmptyCells.Contains(name))
            {
                return sheet[name].Content;
            }

            // If the cell in not in the spreadsheet, automatically return empty string
            else
            {
                return string.Empty;
            }
        }
        else
        {
            throw new InvalidNameException($"The cell name '{name}' is invalid.");
        }
    }

    /// <summary>
    ///  Set the contents of the named cell to the given number.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    ///
    /// <param name="name"> The name of the cell. </param>
    /// <param name="number"> The new content of the cell. </param>
    /// <returns>
    ///   <para>
    ///     This method returns an ordered list consisting of the passed in name
    ///     followed by the names of all other cells whose value depends, directly
    ///     or indirectly, on the named cell.
    ///   </para>
    ///   <para>
    ///     The order must correspond to a valid dependency ordering for recomputing
    ///     all of the cells, i.e., if you re-evaluate each cell in the order of the list,
    ///     the overall spreadsheet will be correctly updated.
    ///   </para>
    ///   <para>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list [A1, B1, C1] is returned, i.e., A1 was changed, so then A1 must be
    ///     evaluated, followed by B1 re-evaluated, followed by C1 re-evaluated.
    ///   </para>
    /// </returns>
    public IList<string> SetCellContents(string name, double number)
    {
        name = name.ToUpper();
        if (IsVar(name))
        {
            // Removing any old dependencies
            foreach (string dependent in dg.GetDependents(name))
            {
                dg.RemoveDependency(name, dependent);
            }

            Cells cell = new()
            {
                Content = number,
            };

            sheet[name] = cell;
            nonEmptyCells.Add(name);
            return GetCellsToRecalculate(name).ToList();
        }
        else
        {
            throw new InvalidNameException($"The cell name '{name}' is invalid.");
        }
    }

    /// <summary>
    ///   The contents of the named cell becomes the given text.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="text"> The new content of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    public IList<string> SetCellContents(string name, string text)
    {
        name = name.ToUpper();
        if (IsVar(name))
        {
            // Removing any old dependencies
            foreach (string dependent in dg.GetDependents(name))
            {
                dg.RemoveDependency(name, dependent);
            }

            if (text.Equals(string.Empty))
            {
                // Check that the cell actually exists so that removal doesn't cause an error.
                if (sheet.ContainsKey(name))
                {
                    // Remove from spreadsheet dictionary and nonEmptyCell HashSet
                    sheet.Remove(name);
                    nonEmptyCells.Remove(name);

                    // Remove every dependency for cells which relied on name
                    foreach (string dependentCell in dg.GetDependents(name))
                    {
                        dg.RemoveDependency(name, dependentCell);
                    }
                }

                // Just returning the name of the cell if it contains the empty string.
                List<string> emptyCell = [];
                emptyCell.Add(name);
                return emptyCell;
            }
            else
            {
                Cells cell = new()
                {
                    Content = text,
                };

                sheet[name] = cell;
                nonEmptyCells.Add(name);
                return GetCellsToRecalculate(name).ToList();
            }
        }
        else
        {
            throw new InvalidNameException($"The cell name '{name}' is invalid.");
        }
    }

    /// <summary>
    ///   Set the contents of the named cell to the given formula.
    /// </summary>
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///   <para>
    ///     If changing the contents of the named cell to be the formula would
    ///     cause a circular dependency, throw a CircularException.
    ///   </para>
    ///   <para>
    ///     No change is made to the spreadsheet.
    ///   </para>
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="formula"> The new content of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    public IList<string> SetCellContents(string name, Formula formula)
    {
        name = name.ToUpper();
        if (IsVar(name))
        {
            // Removing any old dependencies
            foreach (string dependent in dg.GetDependents(name))
            {
                dg.RemoveDependency(name, dependent);
            }

            Cells cell = new()
            {
                Content = formula,
            };

            sheet[name] = cell;
            nonEmptyCells.Add(name);

            // Takes all variables (cells) in formula and adds them to the dependency graph.
            foreach (string dependentCell in formula.GetVariables())
            {
                dg.AddDependency(name, dependentCell);
            }

            return GetCellsToRecalculate(name).ToList();
        }
        else
        {
            throw new InvalidNameException($"The cell name '{name}' is invalid.");
        }
    }

    /// <summary>
    ///   Reports whether "token" is a variable.  It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="cellName"> A string that may be a valid cellName. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar(string cellName)
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable
        string standaloneVarPattern = $"^{VariableRegexPattern}$";
        return Regex.IsMatch(cellName, standaloneVarPattern);
    }

    /// <summary>
    ///   Returns an enumeration, without duplicates, of the names of all cells whose
    ///   values depend directly on the value of the named cell.
    /// </summary>
    /// <param name="name"> This <b>MUST</b> be a valid name.  </param>
    /// <returns>
    ///   <para>
    ///     Returns an enumeration, without duplicates, of the names of all cells
    ///     that contain formulas containing name.
    ///   </para>
    ///   <para>For example, suppose that: </para>
    ///   <list type="bullet">
    ///      <item>A1 contains 3</item>
    ///      <item>B1 contains the formula A1 * A1</item>
    ///      <item>C1 contains the formula B1 + A1</item>
    ///      <item>D1 contains the formula B1 - C1</item>
    ///   </list>
    ///   <para> The direct dependents of A1 are B1 and C1. </para>
    /// </returns>
    private IEnumerable<string> GetDirectDependents(string name)
    {
        // Name will be valid as name is checked in GetCellContents
        return dg.GetDependees(name);
    }

    /// <summary>
    ///   <para>
    ///     This method is implemented for you, but makes use of your GetDirectDependents.
    ///   </para>
    ///   <para>
    ///     Returns an enumeration of the names of all cells whose values must
    ///     be recalculated, assuming that the contents of the cell referred
    ///     to by name has changed.  The cell names are enumerated in an order
    ///     in which the calculations should be done.
    ///   </para>
    ///   <exception cref="CircularException">
    ///     If the cell referred to by name is involved in a circular dependency,
    ///     throws a CircularException.
    ///   </exception>
    ///   <para>
    ///     For example, suppose that:
    ///   </para>
    ///   <list type="number">
    ///     <item>
    ///       A1 contains 5
    ///     </item>
    ///     <item>
    ///       B1 contains the formula A1 + 2.
    ///     </item>
    ///     <item>
    ///       C1 contains the formula A1 + B1.
    ///     </item>
    ///     <item>
    ///       D1 contains the formula A1 * 7.
    ///     </item>
    ///     <item>
    ///       E1 contains 15
    ///     </item>
    ///   </list>
    ///   <para>
    ///     If A1 has changed, then A1, B1, C1, and D1 must be recalculated,
    ///     and they must be recalculated in an order which has A1 first, and B1 before C1
    ///     (there are multiple such valid orders).
    ///     The method will produce one of those enumerations.
    ///   </para>
    ///   <para>
    ///      PLEASE NOTE THAT THIS METHOD DEPENDS ON THE METHOD GetDirectDependents.
    ///      IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
    ///   </para>
    /// </summary>
    /// <param name="name"> The name of the cell.  Requires that name be a valid cell name.</param>
    /// <returns>
    ///    Returns an enumeration of the names of all cells whose values must
    ///    be recalculated.
    /// </returns>
    private IEnumerable<string> GetCellsToRecalculate(string name)
    {
        LinkedList<string> changed = new();
        HashSet<string> visited = [];
        Visit(name, name, visited, changed);
        return changed;
    }

    /// <summary>
    /// <para>
    /// This function visits all the dependents of the dependents. <br/>
    /// For example, if <br/>
    /// A1 -> 3 <br/>
    /// B1 -> A1 * 2 <br/>
    /// C1 -> B1 + 7, <br/>
    /// C1 not only relies on B1 but indirectly relies on A1 as well. <br/>
    /// This method recursively finds all values which indirectly depend on one another. <br/>
    /// </para>
    /// </summary>
    /// <param name="start">This is the starting cell (to check for a circular exception).</param>
    /// <param name="name">This is the cell that is currently being processed.</param>
    /// <param name="visited">Creates a set of all visited cells so that if a cell hasn't been
    /// visited, then it can be explored and the recursive method doesn't repeat itself.</param>
    /// <param name="changed">If a cell is determined to be directly or indirectly dependent,
    /// it will be added to a list changed.
    /// </param>
    /// <exception cref="CircularException">If the current dependent cell is equal to the starting cell,
    /// the dependent cell has looped back to the start which will cause an infinite recursion
    /// so a CircularException is thrown.
    /// </exception>
    private void Visit(string start, string name, ISet<string> visited, LinkedList<string> changed)
    {
        visited.Add(name);
        foreach (string dependent in GetDirectDependents(name))
        {
            if (dependent.Equals(start))
            {
                throw new CircularException();
            }
            else if (!visited.Contains(dependent))
            {
                Visit(start, dependent, visited, changed);
            }
        }

        changed.AddFirst(name);
    }
}

/// <summary>
/// This class creates the structure of each cell, which holds its content and value.
/// </summary>
internal class Cells
{
    private object content = string.Empty;

    // private double localValue = 0.0;

    /// <summary>
    /// Gets or sets content.
    /// </summary>
    public object Content
    {
        get { return content; }
        set { content = value; }
    }

    ///// <summary>
    ///// Gets or sets value.
    ///// </summary>
    // public double Value
    // {
    //    get { return localValue; }
    //    set { localValue = value; }
    // }
}