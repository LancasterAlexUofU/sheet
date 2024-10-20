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
// PS6 Branch
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
using System.Threading.Channels;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using System.Security;

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
/// <para>
///   Thrown to indicate that a read or write attempt has failed with
///   an expected error message informing the user of what went wrong.
/// </para>
/// </summary>
public class SpreadsheetReadWriteException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpreadsheetReadWriteException"/> class.
    ///   <para>
    ///     Creates the exception with a message defining what went wrong.
    ///   </para>
    /// </summary>
    /// <param name="msg"> An informative message to the user. </param>
    public SpreadsheetReadWriteException(string msg)
    : base(msg)
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
    private string spreadsheetName;

    // Creates a dictionary of cell names and pairs them with their contents / values
    [JsonPropertyName("Cells")]
    private Dictionary<string, Cells> sheet = [];

    private HashSet<string> nonEmptyCells = [];
    private DependencyGraph dg = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
    /// Zero-argument constructor that sets the name of the spreadsheet to default.
    /// </summary>
    public Spreadsheet()
    {
        spreadsheetName = "default";
        Changed = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
    /// One-argument constructor that sets the name of the spreadsheet to a string the user provides.
    /// </summary>
    /// <param name="userEnteredName">A string that represents the name of the spreadsheet provided by the user.</param>
    public Spreadsheet(string userEnteredName)
    {
        spreadsheetName = userEnteredName;
        Changed = false;
    }

    /// <summary>
    /// Gets a value indicating whether if any changes have been made to the spreadsheet.
    /// </summary>=>
    public bool Changed { get; private set; } = false;

    /// <summary>
    ///   <para>
    ///     Shortcut syntax to for getting the value of the cell
    ///     using the [] operator.
    ///   </para>
    ///   <para>
    ///     See: <see cref="GetCellValue(string)"/>.
    ///   </para>
    ///   <para>
    ///     Example Usage:
    ///   </para>
    ///   <code>
    ///      sheet.SetContentsOfCell( "A1", "=5+5" );
    ///
    ///      sheet["A1"] == 10;
    ///      // vs.
    ///      sheet.GetCellValue("A1") == 10;
    ///   </code>
    /// </summary>
    /// <param name="cellName"> Any valid cell name. </param>
    /// <returns>
    ///   Returns the value of a cell.  Note: If the cell is a formula, the value should
    ///   already have been computed.
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///     If the name parameter is invalid, throw an InvalidNameException.
    /// </exception>
    public object this[string cellName]
    {
        get { return this.GetCellValue(cellName); }
    }

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
        IsVar(name);

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

    /// <summary>
    ///   <para>
    ///     Writes the contents of this spreadsheet to the named file using a JSON format.
    ///     If the file already exists, overwrite it.
    ///   </para>
    ///   <para>
    ///     The output JSON should look like the following.
    ///   </para>
    ///   <para>
    ///     For example, consider a spreadsheet that contains a cell "A1"
    ///     with contents being the double 5.0, and a cell "B3" with contents
    ///     being the Formula("A1+2"), and a cell "C4" with the contents "hello".
    ///   </para>
    ///   <para>
    ///      This method would produce the following JSON string:
    ///   </para>
    ///   <code>
    ///   {
    ///     "Cells": {
    ///       "A1": {
    ///         "StringForm": "5"
    ///       },
    ///       "B3": {
    ///         "StringForm": "=A1+2"
    ///       },
    ///       "C4": {
    ///         "StringForm": "hello"
    ///       }
    ///     }
    ///   }
    ///   </code>
    ///   <para>
    ///     You can achieve this by making sure your data structure is a dictionary
    ///     and that the contained objects (Cells) have property named "StringForm"
    ///     (if this name does not match your existing code, use the JsonPropertyName
    ///     attribute).
    ///   </para>
    ///   <para>
    ///     There can be 0 cells in the dictionary, resulting in { "Cells" : {} }.
    ///   </para>
    ///   <para>
    ///     Further, when writing the value of each cell...
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       If the contents is a string, the value of StringForm is that string
    ///     </item>
    ///     <item>
    ///       If the contents is a double d, the value of StringForm is d.ToString()
    ///     </item>
    ///     <item>
    ///       If the contents is a Formula f, the value of StringForm is "=" + f.ToString()
    ///     </item>
    ///   </list>
    ///   <para>
    ///     After saving the file, the spreadsheet is no longer "changed".
    ///   </para>
    /// </summary>
    /// <param name="filename"> The name (with path) of the file to save to.</param>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   If there are any problems opening, writing, or closing the file,
    ///   the method should throw a SpreadsheetReadWriteException with an
    ///   explanatory message.
    /// </exception>
    public void Save(string filename)
    {
        IsValidFile(filename);

        // Adds "Cells" wrapping around JSON data
        var wrapper = new Dictionary<string, Dictionary<string, Cells>>
        {
            { "Cells", sheet },
        };
        string jsonString = JsonSerializer.Serialize(wrapper, new JsonSerializerOptions { WriteIndented = true });

        // Adding false allows file contents to be overwritten
        // StreamWriter will also add new file if it doesn't exist already
        using (StreamWriter writer = new StreamWriter(filename, false))
        {
            writer.Write(jsonString);
            Changed = false;
        }
    }

    /// <summary>
    ///   <para>
    ///     Read the data (JSON) from the file and instantiate the current
    ///     spreadsheet.  See <see cref="Save(string)"/> for expected format.
    ///   </para>
    ///   <para>
    ///     Note: First deletes any current data in the spreadsheet.
    ///   </para>
    ///   <para>
    ///     Loading a spreadsheet should set changed to false.  External
    ///     programs should alert the user before loading over a changed sheet.
    ///   </para>
    /// </summary>
    /// <param name="filename"> The saved file name including the path. </param>
    /// <exception cref="SpreadsheetReadWriteException"> When the file cannot be opened or the json is bad.</exception>
    public void Load(string filename)
    {
        // Have to check first that filename exists so that IsValidFile can be reused.
        try
        {
            // "if the path of the file name does not exist"
            if (!File.Exists(Path.GetFullPath(filename)))
            {
                throw new SpreadsheetReadWriteException($"The file \"{filename}\" does not exist.");
            }
        }

        // Thrown if GetFullPath returns any exceptions
        catch
        {
            throw new SpreadsheetReadWriteException($"The system could not retrieve the absolute path for \"{filename}\"");
        }

        // Mainly checks that it isn't readonly, a system level file, or already opened
        IsValidFile(filename);

        // Attempt to deserialize filename
        string jsonData = File.ReadAllText(filename);

        try
        {
            // First deserialize into the wrapper dictionary
            var wrapper = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Cells>>>(jsonData);

            // Get the actual sheet data from the "Cells" key
            Dictionary<string, Cells> loadedSheet = wrapper?["Cells"] ?? throw new SpreadsheetReadWriteException("Failed to access spreadsheet cells data.");

            // Clear existing data
            sheet.Clear();
            nonEmptyCells.Clear();
            dg = new();

            SetCellContentsInOrder(loadedSheet);
        }
        catch
        {
            throw new SpreadsheetReadWriteException($"The JSON file is invalid.");
        }
    }

    /// <summary>
    ///   <para>
    ///     Return the value of the named cell.
    ///   </para>
    /// </summary>
    /// <param name="cellName"> The cell in question. </param>
    /// <returns>
    ///   Returns the value (as opposed to the contents) of the named cell.  The return
    ///   value's type should be either a string, a double, or a CS3500.Formula.FormulaError.
    ///   If the cell contents are a formula, the value should have already been computed
    ///   at this point.
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object GetCellValue(string cellName)
    {
        cellName = cellName.ToUpper();
        IsVar(cellName);

        if (nonEmptyCells.Contains(cellName))
        {
            return sheet[cellName].Value;
        }

        // If the cell in not in the spreadsheet, automatically return empty string
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    ///   <para>
    ///       Sets the contents of the named cell to the appropriate object
    ///       based on the string in <paramref name="content"/>.
    ///   </para>
    ///   <para>
    ///       First, if the <paramref name="content"/> parses as a double, the contents of the named
    ///       cell becomes that double.
    ///   </para>
    ///   <para>
    ///       Otherwise, if the <paramref name="content"/> begins with the character '=', an attempt is made
    ///       to parse the remainder of content into a Formula.
    ///   </para>
    ///   <para>
    ///       There are then three possible outcomes when a formula is detected:
    ///   </para>
    ///
    ///   <list type="number">
    ///     <item>
    ///       If the remainder of content cannot be parsed into a Formula, a
    ///       FormulaFormatException is thrown.
    ///     </item>
    ///     <item>
    ///       If changing the contents of the named cell to be f
    ///       would cause a circular dependency, a CircularException is thrown,
    ///       and no change is made to the spreadsheet.
    ///     </item>
    ///     <item>
    ///       Otherwise, the contents of the named cell becomes f.
    ///     </item>
    ///   </list>
    ///   <para>
    ///     Finally, if the content is a string that is not a double and does not
    ///     begin with an "=" (equal sign), save the content as a string.
    ///   </para>
    ///   <para>
    ///     On successfully changing the contents of a cell, the spreadsheet will be changed. <see cref="Changed"/>.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell name that is being changed.</param>
    /// <param name="content"> The new content of the cell.</param>
    /// <returns>
    ///   <para>
    ///     This method returns a list consisting of the passed in cell name,
    ///     followed by the names of all other cells whose value depends, directly
    ///     or indirectly, on the named cell. The order of the list MUST BE any
    ///     order such that if cells are re-evaluated in that order, their dependencies
    ///     are satisfied by the time they are evaluated.
    ///   </para>
    ///   <para>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list {A1, B1, C1} is returned.  If the cells are then evaluate din the order:
    ///     A1, then B1, then C1, the integrity of the Spreadsheet is maintained.
    ///   </para>
    /// <para>
    ///   This method returns a list of all cells that have been updated
    ///   (of course including the cell that was just updated).
    /// </para>
    /// <para>
    ///   For example, if cellName is "A1", and B1 contains A1*2, and C1 contains B1+A1, the
    ///   list containing [A1, B1, C1] is returned.
    /// </para>
    /// </returns>
    /// <exception cref="InvalidNameException">sa
    ///   If the name parameter is invalid, throw an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///   If changing the contents of the named cell to be the formula would
    ///   cause a circular dependency, throw a CircularException.
    ///   (NOTE: No change is made to the spreadsheet.)
    /// </exception>
    public IList<string> SetContentsOfCell(string name, string content)
    {
        name = name.ToUpper();
        IsVar(name);
        IList<string> cellsToReEval = [];

        // Removing any old dependencies
        foreach (string dependent in dg.GetDependents(name))
        {
            dg.RemoveDependency(name, dependent);
        }

        // Checks if content is a number
        if (double.TryParse(content, out double value))
        {
            cellsToReEval = SetCellContents(name, value);
        }

        // Checks if contents is not a formula (and is therefore a string)
        // by seeing if first character is not "=".
        else if (content.Equals(string.Empty) || content[0] != '=' )
        {
            cellsToReEval = SetCellContents(name, content);
        }

        // If this else statement is reached, then an "=" must be present.
        // Therefore, treating string as a formula (valid or invalid)

        // This creates a new formula that excludes the "="

        // If content is not a valid formula, a FormulaFormatException will be thrown
        // TODO: If an exception is thrown, do I just contain like nothing happened in the try parse? And like send and empty string?
        else
        {
            Formula formula = new(content.Substring(1));
            cellsToReEval = SetCellContents(name, formula);
        }

        // Reevaluate cells if their contents contain a formula
        foreach(string cell in cellsToReEval)
        {
            // Need to include ContainsKey in-case a cell was removed
            // Without ContainsKey, sheet[cell] can't find cell (obviously)
            if (sheet.ContainsKey(cell) && (sheet[cell].Content is Formula formula))
            {
                sheet[cell].Value = formula.Evaluate(cell => Convert.ToDouble(sheet[cell].Value));
            }
        }

        return cellsToReEval;
    }

    /// <summary>
    /// This method checks that any given file name/path is valid for writing to.
    /// The path may or may not exist.
    /// </summary>
    /// <param name="filename">The filename or path.</param>
    /// <returns>Returns true if filename is in a correct format.</returns>
    /// <exception cref="SpreadsheetReadWriteException">Thrown if filename is not is a correct format, is readonly, or a system level file.</exception>
    private static bool IsValidFile(string filename)
    {
        string fullPath;
        char[] invalidChars = { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
        char[] invalidPathChars = Path.GetInvalidPathChars();

        char[] combinedInvalidChars = invalidChars.Concat(invalidPathChars).Distinct().ToArray();

        // GetFullPath throws an exception if the path formatting is malformed.
        try
        {
            fullPath = Path.GetFullPath(filename);
        }
        catch
        {
            throw new SpreadsheetReadWriteException($"The system could not retrieve the absolute path for \"{filename}\"");
        }

        // If no invalid characters are found, IndexOfAny returns -1
        if (filename.IndexOfAny(combinedInvalidChars) != -1)
        {
            throw new SpreadsheetReadWriteException($"\"{filename}\" contains an invalid filename character.");
        }

        // If the file path doesn't exist but is properly formatted, then it is okay to create.
        // If it does exist, need to check a few more things such as its attributes.
        if (File.Exists(fullPath))
        {
            try
            {
                FileAttributes attributes = File.GetAttributes(fullPath);

                // Creates a bit mask that combines both ReadOnly and System attributes
                // Then checks if the attributes of the file is not equal to the bit mask
                if ((attributes & (FileAttributes.ReadOnly | FileAttributes.System)) != 0)
                {
                    throw new SpreadsheetReadWriteException($"Spreadsheet does not have proper permissions for \"{filename}\"");
                }

                // Check if the file is locked by another process by attempting to open it
                using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    // If no exception is thrown, the file is not locked
                }
            }

            // If the filename is passed (such as "sheet.txt") is not found in the current directory, a FileNotFoundException will be thrown.
            catch (FileNotFoundException)
            {
                throw new SpreadsheetReadWriteException($"The file \"{filename}\" was not found. (Try using absolute path).");
            }

            // IOException is thrown if the file is already open by another process
            catch (IOException)
            {
                throw new SpreadsheetReadWriteException($"The file \"{filename}\" is currently in use by another process.");
            }

            // Can be thrown if GetAttributes has trouble retrieving attributes from file
            catch
            {
                throw new SpreadsheetReadWriteException($"Spreadsheet does not have proper permissions for \"{filename}\"");
            }
        }

        return true;
    }

    /// <summary>
    ///   Reports whether "token" is a variable.  It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="cellName"> A string that may be a valid cellName. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    /// <exception cref="InvalidNameException">Thrown if a cellName does not fit the correct format.</exception>
    private static bool IsVar(string cellName)
    {
        string variableRegexPattern = @"^[a-zA-Z]+\d+$";
        if (Regex.IsMatch(cellName, variableRegexPattern))
        {
            return true;
        }
        else
        {
            throw new InvalidNameException($"The cell name '{cellName}' is invalid.");
        }
    }

    /// <summary>
    /// <para>
    /// This private method calls SetContentsOfCell after loading in a JSON file and ensures that cells that are dependent on each other
    /// are added to the Spreadsheet in the correct order.
    /// </para>
    /// <para>
    /// The algorithm is the following: <br/>
    /// For as many items in the loadedData JSON, try to set as many contents as possible. If at least one Cell was set, progress is being made. <br/>
    /// However, if there was a pass where no cells were processed but there are still items in loadedSheet, then there is a cell that doesn't have a set value. <br/>
    /// This allows for all double values to first be set, then if any cells rely on those value cells, they will be set next (and so on).
    /// </para>
    /// </summary>
    /// <param name="loadedSheet">The unwrapped dictionary given by the JSON deserializer.</param>
    /// <exception cref="SpreadsheetReadWriteException">Thrown if cell(s) were found that don't have any reference to a value (even through other cells) or if not all cells could be processed.</exception>
    private void SetCellContentsInOrder(Dictionary<string, Cells> loadedSheet)
    {
        for (int i = 0; i < loadedSheet.Count; i++)
        {
            bool atLeastOneCellProcessed = false;

            // Process each cell
            foreach (var pair in loadedSheet)
            {
                string cellName = pair.Key;

                // pair.Value gets "Cells" which is a dictionary
                // inside Cells, getting Content
                // Since Content is an object, convert to string
                // ?? string.Empty converts any null values to the empty string
                string cellContent = pair.Value.Content.ToString() ?? string.Empty;

                try
                {
                    SetContentsOfCell(cellName, cellContent);

                    // If nothing is caught, then remove and keep going
                    atLeastOneCellProcessed = true;
                    loadedSheet.Remove(pair.Key);
                }

                // If cell is dependent on another cell that isn't in the spreadsheet yet, KeyNotFoundException will be thrown.
                catch (KeyNotFoundException)
                {
                }
            }

            if (!atLeastOneCellProcessed)
            {
                throw new SpreadsheetReadWriteException("Cell(s) found with unset value dependencies. (e.g. Cell A1 contains \"=A2\", but A2 does not hold any value)");
            }

            // If loadedSheet has been fully processed, end loop
            if (loadedSheet.Count == 0)
            {
                break;
            }
        }

        if (loadedSheet.Count > 0)
        {
            throw new SpreadsheetReadWriteException("Not all Cells were processed");
        }

        Changed = false;
    }

    /// <summary>
    ///   The contents of the named cell becomes the given contents.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="contents"> The new content of the cell. </param>
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
    private IList<string> SetCellContents(string name, object contents)
    {
        if (contents.Equals(string.Empty))
        {
            return RemoveCell(name);
        }
        else
        {
            return AddCell(name, contents);
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
    ///   The same list as defined in <see cref="SetCellContents(string, object)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, Formula formula)
    {
        // Checking to see if formula contains own cell name (e.g. "A1", "=A1").
        // Need to include before lookup as A1 isn't in lookup dictionary and will throw keyNotFound exception instead of CircularException.
        if (formula.GetVariables().Contains(name))
        {
            // "Cell can't reference own cell"
            throw new CircularException();
        }

        // Holds previous value in case CircularException is thrown.
        // If thrown, sheet will be restored to previous form.
        var temp = GetCellContents(name);

        Cells cell = new()
        {
            Content = formula,

            // TODO: Is this how the evaluate method works?
            Value = formula.Evaluate(s => Convert.ToDouble(sheet[s].Value)),
        };

        sheet[name] = cell;
        nonEmptyCells.Add(name);

        // Takes all variables (cells) in formula and adds them to the dependency graph.
        foreach (string dependentCell in formula.GetVariables())
        {
            dg.AddDependency(name, dependentCell);
        }

        try
        {
            return GetCellsToRecalculate(name).ToList();
        }

        // If caught, revert back to previous spreadsheet, no changes made
        catch(CircularException)
        {
            if (temp is Formula)
            {
                SetContentsOfCell(name, "=" + temp.ToString());
            }
            else
            {
                SetContentsOfCell(name, temp.ToString() ?? string.Empty);
            }

            throw new CircularException();
        }
    }

    /// <summary>
    /// This method adds a new cell to the spreadsheet and updates the contents and value of the cell. <br/>
    /// This method only is used for doubles and strings.
    /// </summary>
    /// <param name="name">Name of the cell that will be added.</param>
    /// <param name="newContent">The content that will be set to the Cell content and value parameters.</param>
    /// <returns>
    ///  <para>
    ///     This method returns an ordered list consisting of the passed in name
    ///     followed by the names of all other cells whose value depends, directly
    ///     or indirectly, on the named cell.
    ///   </para>
    /// </returns>
    private IList<string> AddCell(string name, object newContent)
    {
        Cells cell = new()
        {
            Content = newContent,
            Value = newContent,
        };

        sheet[name] = cell;
        nonEmptyCells.Add(name);

        return GetCellsToRecalculate(name).ToList();
    }

    /// <summary>
    /// This method removes a cell from the spreadsheet and dependency graph.
    /// </summary>
    /// <param name="name">Name of the cell to be removed.</param>
    /// <returns>A list containing the single cell name that was removed (for SetCellContents purposes).</returns>
    private IList<string> RemoveCell(string name)
    {
        // Check that the cell actually exists so that removal doesn't cause an error.
        // Even if it isn't in the spreadsheet, it will still return itself since it is
        // technically equal to the empty string already.
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

            Changed = true;
        }

        // Just returning the name of the cell if it contains the empty string.
        return new List<string> { name };
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

        // If Visit found no circular dependencies, then spreadsheet was changed.
        Changed = true;
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

    private object localValue = string.Empty;

    /// <summary>
    /// Gets or sets content.
    /// </summary>
    [JsonIgnore]
    public object Content
    {
        get { return content; }
        set { content = value; }
    }

    /// <summary>
    /// Gets or sets value.
    /// JsonIgnore is included as values can be computed at runtime.
    /// </summary>
    [JsonIgnore]
    public object Value
    {
        get { return localValue; }
        set { localValue = value; }
    }

    /// <summary>
    /// Gets or sets the string form for JSON serialization.
    /// </summary>
    [JsonPropertyName("StringForm")]
    public string StringForm
    {
        get
        {
            if (content is Formula)
            {
                return "=" + content.ToString();
            }

            // Otherwise, return the content as a string
            return content.ToString() ?? string.Empty;
        }

        set
        {
            content = value;
        }
    }
}