// <copyright file="SpreadsheetTests.cs" company="UofU-CS3500">
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
//      This SpreadsheetTests test class covers 100% of Spreadsheet.
//      It tests that double, string, and Formula SetContentsOfCell are
//      working and work when used in combination.
//      This test class also checks multiple times for circular exceptions.
// <summary>
// Ignore Spelling: JSON
namespace CS3500.SpreadsheetTests;

using CS3500.Formula;
using CS3500.Spreadsheet;
using Newtonsoft.Json.Linq;
using System.IO.Enumeration;
using System.Linq.Expressions;

/// <summary>
/// This test class ensures that the Spreadsheet project is properly working but testing all public methods. <br/>
/// Many of the tests look for Invalid name and circular exceptions to be thrown.
/// For SetContentsOfCell, all return types are tested for doubles, strings, and formulas.
/// </summary>
[TestClass]
public class SpreadsheetTests
{
    /// <summary>
    /// Creates an empty spreadsheet to ensure GetNamesOfAllNonemptyCells returns an empty list.
    /// </summary>
    [TestMethod]
    public void GetNames_Empty()
    {
        Spreadsheet sheet = new();
        List<string> empty = [];
        CollectionAssert.AreEquivalent(empty, sheet.GetNamesOfAllNonemptyCells().ToList());
    }

    /// <summary>
    /// Tests that the GetNames method works properly when cells contain only double type contents.
    /// </summary>
    [TestMethod]
    public void GetNames_DoubleOnly()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A1", "1.0");
        sheet.SetContentsOfCell("B1", "0");
        sheet.SetContentsOfCell("A1", "2E1");

        List<string> correctCells = ["A1", "B1"];
        CollectionAssert.AreEquivalent(correctCells, sheet.GetNamesOfAllNonemptyCells().ToList());
    }

    /// <summary>
    /// Tests that the GetNames method works properly when cells contain only string type contents.
    /// </summary>
    [TestMethod]
    public void GetNames_StringOnly()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A1", "A1");
        sheet.SetContentsOfCell("B1", "This is a string");
        sheet.SetContentsOfCell("A1", "A1 + B1");
        sheet.SetContentsOfCell("C1", "1234");
        sheet.SetContentsOfCell("C1", string.Empty);
        sheet.SetContentsOfCell("D1", string.Empty);

        List<string> correctCells = ["A1", "B1"];
        CollectionAssert.AreEquivalent(correctCells, sheet.GetNamesOfAllNonemptyCells().ToList());
    }

    /// <summary>
    /// Tests that the GetNames method works properly when cells contain only Formula type contents.
    /// </summary>
    [TestMethod]
    public void GetNames_FormulaOnly()
    {
        Spreadsheet sheet = new();
        Formula expression1 = new("1 + 1");
        Formula expression2 = new("A1 + C1");
        Formula expression3 = new("1 + C1");
        Formula expression4 = new("2 * 2");

        sheet.SetContentsOfCell("C1", "1");

        sheet.SetContentsOfCell("A1", $"={expression1}");
        sheet.SetContentsOfCell("B1", $"={expression2}");
        sheet.SetContentsOfCell("A1", $"={expression3}");
        sheet.SetContentsOfCell("C1", $"={expression4}");

        List<string> correctCells = ["A1", "B1", "C1"];
        CollectionAssert.AreEquivalent(correctCells, sheet.GetNamesOfAllNonemptyCells().ToList());
    }

    /// <summary>
    /// Tests that the GetNames method works properly when cells contain strings, double, and Formula type contents.
    /// </summary>
    [TestMethod]
    public void GetNames_AllTypes()
    {
        Spreadsheet sheet = new();
        Formula expression1 = new("1 + 1");
        Formula expression2 = new("A1 + C1");

        sheet.SetContentsOfCell("A1", "1");
        sheet.SetContentsOfCell("A1", $"={expression1}"); // Replace with formula
        sheet.SetContentsOfCell("A1", "1 + 1"); // Replace with string
        sheet.SetContentsOfCell("A1", "2"); // Replace with double

        sheet.SetContentsOfCell("C1", "4");
        sheet.SetContentsOfCell("B1", $"={expression2}");
        sheet.SetContentsOfCell("D1", string.Empty);
        List<string> correctCells = ["A1", "B1", "C1"];
        CollectionAssert.AreEquivalent(correctCells, sheet.GetNamesOfAllNonemptyCells().ToList());
    }

    /// <summary>
    /// Tests that an uninitialized cell contents is equal to the empty string.
    /// </summary>
    [TestMethod]
    public void GetCellContents_Empty()
    {
        Spreadsheet sheet = new();
        Assert.AreEqual(string.Empty, sheet.GetCellContents("A1"));
    }

    /// <summary>
    /// This tests that if GetCellContents is called with an invalid cell name, InvalidNameException is thrown.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellContents_InvalidName()
    {
        Spreadsheet sheet = new();
        sheet.GetCellContents("1E1");
    }

    /// <summary>
    /// This tests that GetCellContents properly returns cells contents consisting of only strings.
    /// </summary>
    [TestMethod]
    public void GetCellContents_StringOnly()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A1", "A1");
        sheet.SetContentsOfCell("B1", "This is a string");
        sheet.SetContentsOfCell("A1", "A1 + B1");
        sheet.SetContentsOfCell("C1", "1234");
        sheet.SetContentsOfCell("C1", string.Empty);
        sheet.SetContentsOfCell("D1", string.Empty);

        Assert.AreEqual("A1 + B1", sheet.GetCellContents("A1"));
        Assert.AreEqual("This is a string", sheet.GetCellContents("B1"));
        Assert.AreEqual(string.Empty, sheet.GetCellContents("C1")); // C1 equal to string.Empty or nothing since it doesn't exist?
        Assert.AreEqual(string.Empty, sheet.GetCellContents("D1")); // ditto
    }

    /// <summary>
    /// This tests that GetCellContents properly returns cells contents consisting of only doubles.
    /// </summary>
    [TestMethod]
    public void GetCellContents_DoubleOnly()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A1", "1.0");
        sheet.SetContentsOfCell("B1", "0");
        sheet.SetContentsOfCell("A1", "2E1");

        Assert.AreEqual(2E1, sheet.GetCellContents("A1"));
        Assert.AreEqual(0.0, sheet.GetCellContents("B1"));
    }

    /// <summary>
    /// This tests that GetCellContents properly returns cells contents consisting of only Formulas.
    /// </summary>
    [TestMethod]
    public void GetCellContents_FormulaOnly()
    {
        Spreadsheet sheet = new();
        Formula expression1 = new("1 + 1");
        Formula expression2 = new("A1 + C1");
        Formula expression3 = new("1 + C1");
        Formula expression4 = new("2 * 2");

        sheet.SetContentsOfCell("C1", "1");

        sheet.SetContentsOfCell("A1", $"={expression1}");
        sheet.SetContentsOfCell("B1", $"={expression2}");
        sheet.SetContentsOfCell("A1", $"={expression3}");
        sheet.SetContentsOfCell("C1", $"={expression4}");

        Assert.AreEqual(expression3, sheet.GetCellContents("A1"));
        Assert.AreEqual(expression2, sheet.GetCellContents("B1"));
        Assert.AreEqual(expression4, sheet.GetCellContents("C1"));
    }

    /// <summary>
    /// Tests that a GetCellContents returns with a sheet and cell of various types.
    /// </summary>
    [TestMethod]
    public void GetCellContents_AllTypes()
    {
        Spreadsheet sheet = new();
        Formula expression1 = new("1 + 1");
        Formula expression2 = new("A1 + C1");

        sheet.SetContentsOfCell("A1", $"={expression1}");
        Assert.AreEqual(expression1, sheet.GetCellContents("A1"));

        sheet.SetContentsOfCell("A1", "1 + 1"); // Replace with string
        Assert.AreEqual("1 + 1", sheet.GetCellContents("A1"));

        sheet.SetContentsOfCell("A1", "2"); // Replace with double
        Assert.AreEqual(2.0, sheet.GetCellContents("A1"));

        sheet.SetContentsOfCell("C1", "4");
        sheet.SetContentsOfCell("B1", $"={expression2}");
        sheet.SetContentsOfCell("D1", string.Empty);

        Assert.AreEqual(expression2, sheet.GetCellContents("B1"));
        Assert.AreEqual(4.0, sheet.GetCellContents("C1"));
        Assert.AreEqual(string.Empty, sheet.GetCellContents("D1")); // Might throw error? Is it equal to nothing?
    }

    /// <summary>
    /// This tests that the SetContentsOfCell (double method) returns an InvalidNameException for an invalid cell name.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCell_Double_InvalidName()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("1E1", "2");
    }

    /// <summary>
    /// This tests that the SellCellContents (double method) returns a list of itself.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_Double()
    {
        Spreadsheet sheet = new();

        List<string> values = [];
        values.Add("A1");
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("A1", "2").ToList());

        values.Clear();
        values.Add("A1");
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("A1", "2E1").ToList());

        values.Clear();
        values.Add("B1");
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("B1", "3.0").ToList());
    }

    /// <summary>
    /// This tests that the SetContentsOfCell (string method) returns an InvalidNameException for an invalid cell name.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCell_String_InvalidName()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("1E1", "2");
    }

    /// <summary>
    /// This tests that the SellCellContents (string method) returns a list of itself (cell name).
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_String()
    {
        Spreadsheet sheet = new();

        List<string> values = [];
        values.Add("A1");
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("A1", "2").ToList());

        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("A1", "1 + 1").ToList());

        values.Clear();
        values.Add("B1");
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("B1", "(A1 * 2)").ToList());
    }

    /// <summary>
    /// This method attempts to set a cell's contents in a cell with an invalid name, expected to throw
    /// InvalidNameException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCell_Formula_InvalidName()
    {
        Spreadsheet sheet = new();
        Formula formula = new("1 + 1");
        sheet.SetContentsOfCell("1E1", $"={formula}");
    }

    /// <summary>
    /// This tests that a cell that links back to a previous cell throws a CircularException.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_Formula_InvalidCircular()
    {
        Spreadsheet sheet = new();
        Formula formula1 = new("1 + B1");
        Formula formula2 = new("1 + A1");

        sheet.SetContentsOfCell("B1", "4");
        sheet.SetContentsOfCell("A1", $"={formula1}");

        bool exceptionThrown = false;
        try
        {
            sheet.SetContentsOfCell("B1", $"={formula2}"); // Invalid
        }
        catch (CircularException)
        {
            exceptionThrown = true;
        }

        Assert.IsTrue(exceptionThrown, "CircularException was not thrown as expected.");

        // Check that B1 is same value (the circular reference didn't change the spreadsheet)
        Assert.AreEqual(4.0, sheet.GetCellContents("B1"));

        // Check that A1 still contains formula1
        Assert.AreEqual(formula1, sheet.GetCellContents("A1"));
    }

    /// <summary>
    /// This test sets a cell contents to a formula containing itself, expected to throw CircularException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCell_Formula_InvalidCircularSingleCell()
    {
        Spreadsheet sheet = new();
        Formula formula = new("A1");
        sheet.SetContentsOfCell("A1", $"={formula}");
    }

    /// <summary>
    /// This method tests that SetContentsOfCell are returning correct lists of dependent cells based on only formula inputs.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_FormulaOnly()
    {
        Spreadsheet sheet = new();
        Formula formula1 = new("(B1 * 4) + 2");
        Formula formula2 = new("C1 / 2");
        Formula formula3 = new("70 - 20");
        Formula formula4 = new("(B1 * 4) + 4");
        List<string> values = [];

        sheet.SetContentsOfCell("B1", "4");
        sheet.SetContentsOfCell("C1", "2");

        // It should only return A1 and not A1, B1, since even if things are changed in A1, B1 won't be changed and therefore no recalculations are needed.
        values.Add("A1");
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("A1", $"={formula1}").ToList());

        // Since B1 depends on C1, should return B1, C1
        values.Insert(0, "B1"); // Inserts B1 at the beginning of the list
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("B1", $"={formula2}").ToList());

        values.Insert(0, "C1"); // C1 goes at front of list
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("C1", $"={formula3}").ToList());

        // "Replace" A1, see that it still returns A1 even when linked to other established cells.
        values.Clear();
        values.Add("A1");
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("A1", $"={formula4}").ToList());
    }

    /// <summary>
    /// This test creates a spreadsheet that sets cells of all types and ensures that the returned dependency lists are returning correctly.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_AllTypes()
    {
        Spreadsheet sheet = new();
        Formula formula1 = new("(B1 * 4) + 2");
        Formula formula2 = new("C1 / 2");
        Formula formula3 = new("(D1 * 4) + 4");
        List<string> values = [];

        sheet.SetContentsOfCell("B1", "4");
        sheet.SetContentsOfCell("C1", "2");

        sheet.SetContentsOfCell("A1", $"={formula1}");

        values.Add("B1");
        values.Add("A1");
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("B1", $"={formula2}").ToList());

        values.Insert(0, "C1"); // values = ["C1", "B1", "A1"]
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("C1", "50").ToList());

        values = ["D1"];
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("D1", "4").ToList());

        values = ["E1"]; // values = ["E1"]
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("E1", $"={formula3}").ToList());

        values = ["F1"];
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("F1", "STRING!").ToList());
    }

    /// <summary>
    /// This test add and removes a cell, then checks whether it was removed from the spreadsheet properly.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_Remove()
    {
        Spreadsheet sheet = new();
        Formula formula = new("1");
        List<string> values = [];

        // Even though that cell will be removed, SetContentsOfCell still returns the cell.
        values.Add("A1");
        sheet.SetContentsOfCell("A1", $"={formula}");

        // Setting A1 to string.Empty removes it
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("A1", string.Empty).ToList());

        values.Clear();

        // Ensure GetCellContents and GetNamesOfAllNonemptyCells are also working
        Assert.AreEqual(string.Empty, sheet.GetCellContents("A1"));
        CollectionAssert.AreEquivalent(values, sheet.GetNamesOfAllNonemptyCells().ToList());
    }

    /// <summary>
    /// This test method has cells that depend on cells which depend on multiple cells.
    /// This is to ensure that even with many dependencies, the correct list of dependent cells are returned.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_Formula_Complex()
    {
        Spreadsheet sheet = new();
        List<string> values = [];

        Formula formula1 = new("B1 + D1");
        Formula formula2 = new("2 * C1 * G1");
        Formula formula3 = new("E1 + 1");
        Formula formula4 = new("C1");
        Formula formula5 = new("B1");

        sheet.SetContentsOfCell("B1", "4");
        sheet.SetContentsOfCell("D1", "5");
        sheet.SetContentsOfCell("C1", "6");
        sheet.SetContentsOfCell("G1", "7");
        sheet.SetContentsOfCell("E1", "8");

        sheet.SetContentsOfCell("A1", $"={formula1}");
        sheet.SetContentsOfCell("B1", $"={formula2}");
        sheet.SetContentsOfCell("C1", "5");
        sheet.SetContentsOfCell("D1", $"={formula3}");
        sheet.SetContentsOfCell("E1", "3");
        sheet.SetContentsOfCell("F1", $"={formula4}");
        sheet.SetContentsOfCell("G1", "3");
        sheet.SetContentsOfCell("h1", $"={formula5}"); // Lowercase to make sure it can handle it

        values = ["A1"];
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("A1", $"={formula1}").ToList());

        values = ["B1", "A1", "H1"];
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("B1", $"={formula2}").ToList());

        values = ["C1", "B1", "A1", "H1", "F1"];
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("C1", "5").ToList());

        values = ["D1", "A1"];
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("D1", $"={formula3}").ToList());

        values = ["E1", "D1", "A1"];
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("E1", "3").ToList());

        values = ["F1"];
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("F1", $"={formula4}").ToList());

        values = ["G1", "B1", "A1", "H1"];
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("G1", "3").ToList());

        values = ["H1"];
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("H1", $"={formula5}").ToList());

        values = ["C1"];
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("C1", string.Empty).ToList());

        values = ["B1"];
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("B1", string.Empty).ToList());

        // Removed B1, so G1 should only depend on itself
        values = ["G1"];
        CollectionAssert.AreEquivalent(values, sheet.SetContentsOfCell("G1", "3").ToList());
    }

    /// <summary>
    /// This test checks that the index notation properly returns a double value of a cell.
    /// </summary>
    [TestMethod]
    public void GetValueIndex_Double_Valid()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A1", "10");
        Assert.AreEqual(sheet["A1"], 10.0);
    }

    /// <summary>
    /// This test checks that the index notation properly returns the value of a formula with addition.
    /// </summary>
    [TestMethod]
    public void GetValueIndex_Formula_Valid()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A1", "=5+5");
        Assert.AreEqual(sheet["A1"], 10.0);
    }

    /// <summary>
    /// This test checks that the index notation properly returns the value of a formula that has cell dependencies.
    /// </summary>
    [TestMethod]
    public void GetValueIndex_FormulaDependencies_Valid()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A2", "5");
        sheet.SetContentsOfCell("A3", "5");
        sheet.SetContentsOfCell("A1", "=A2+A3");

        Assert.AreEqual(sheet["A1"], 10.0);
    }

    /// <summary>
    /// Checks to see that an empty cell returns an empty string.
    /// </summary>
    [TestMethod]
    public void GetValueIndex_EmptyCell()
    {
        Spreadsheet sheet = new();
        Assert.AreEqual(sheet["A1"], string.Empty);
    }

    /// <summary>
    /// This test checks that the index notation properly returns the value of a string.
    /// </summary>
    [TestMethod]
    public void GetValueIndex_String_Valid()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A1", "AB");
        Assert.AreEqual(sheet["A1"], "AB");
    }

    /// <summary>
    /// This tests that an invalid cell name for the index notation throws an InvalidNameException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetValueIndex_CellName_Invalid()
    {
        Spreadsheet sheet = new();
        _ = sheet["E1E"];
    }

    /// <summary>
    /// This method tests that the StringForm of a string is saved properly.
    /// </summary>
    [TestMethod]
    public void Save_String_StringForm_Valid()
    {
        string expectedOutput = @"{
  ""Cells"": {
    ""A2"": {
      ""StringForm"": ""hello""
    }
  }
}";
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A2", "hello");
        string filename = "sheet.txt";
        sheet.Save(filename);

        string savedOutput = File.ReadAllText(filename);

        // Asserts that the file contains at least a substring containing the same Cells
        Assert.IsTrue(savedOutput.Contains(expectedOutput));
    }

    /// <summary>
    /// This method tests that the StringForm of a formula is saved properly.
    /// </summary>
    [TestMethod]
    public void Save_Formula_StringForm_Valid()
    {
        // \u002B is '+', will be de serialized into + by load.
        string expectedOutput = @"{
  ""Cells"": {
    ""A2"": {
      ""StringForm"": ""3""
    },
    ""A1"": {
      ""StringForm"": ""=A2\u002B3""
    }
  }
}";

        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A2", "3"); // TODO: ADDED THIS CODE, REST WON'T WORK
        sheet.SetContentsOfCell("A1", "=A2 + 3");
        string filename = "sheet.txt";
        sheet.Save(filename);

        string savedOutput = File.ReadAllText(filename);

        // Asserts that the file contains at least a substring containing the same Cells
        Assert.IsTrue(savedOutput.Contains(expectedOutput));

        sheet.Load(filename);
        Assert.AreEqual(sheet.GetCellContents("A1"), new Formula("A2 + 3"));
        Assert.AreEqual(sheet.GetCellValue("A1"), 6.0);

        Assert.AreEqual(sheet.GetCellContents("A2"), 3.0);
        Assert.AreEqual(sheet.GetCellValue("A2"), 3.0);
    }

    /// <summary>
    /// Tests that multiple sheets with various stages of saves do not affect the Changed variable for each other.
    /// Checks that Changed is false after using save.
    /// </summary>
    [TestMethod]
    public void Save_ChangedIsFalse()
    {
        Spreadsheet sheet = new();
        Spreadsheet sheet2 = new("sheet2");
        sheet.SetContentsOfCell("A1", "10"); // Changed is equal to true
        Assert.IsFalse(sheet2.Changed);

        string filename = "sheet.txt";
        sheet.Save(filename);

        Assert.IsFalse(sheet.Changed);
        Assert.IsFalse(sheet2.Changed);
    }

    /// <summary>
    /// This test checks that an already open file is not able to be written to by the save method.
    /// Should throw SpreadsheetReadWriteException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Save_FileAlreadyOpen_Invalid()
    {
        Spreadsheet sheet = new();
        string filename = "sheet.txt";

        // Open the file and keep it open
        using FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
        sheet.Save(filename);
    }

    /// <summary>
    /// Tests that a filename with invalid characters throws a SpreadsheetReadWriteException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Save_InvalidCharacterInFileName_Invalid()
    {
        Spreadsheet sheet = new();
        string filename = @"sheet/?\<.txt";

        sheet.Save(filename);
    }

    /// <summary>
    /// Tests that a filename which doesn't exist yet is properly created.
    /// </summary>
    [TestMethod]
    public void Save_UninitializedFile()
    {
        Spreadsheet sheet = new();
        string filename = "ThisFileDoesNotExist.txt";

        // Check that file doesn't exist first
        Assert.IsFalse(File.Exists(filename));

        sheet.Save(filename);

        // Ensure that file is created with no errors
        Assert.IsTrue(File.Exists(filename));
        File.Delete(filename);
    }

    /// <summary>
    /// Tests that a cell in which its contents are overwritten has its final contents as the saved version.
    /// </summary>
    [TestMethod]
    public void Save_OverrideFile()
    {
        string expectedOutput = @"{
  ""Cells"": {
    ""A2"": {
      ""StringForm"": ""world""
    }
  }
}";

        Spreadsheet sheet = new();
        string filename = "sheet.txt";

        sheet.SetContentsOfCell("A2", "hello");
        sheet.Save(filename);

        sheet.SetContentsOfCell("A2", "world");
        sheet.Save(filename);

        string savedOutput = File.ReadAllText(filename);

        // Asserts that the file contains at least a substring containing the same Cells
        Assert.IsTrue(savedOutput.Contains(expectedOutput));
    }

    /// <summary>
    /// This test creates a complex spreadsheet and makes sure that its file contents contains the cell values.
    /// </summary>
    [TestMethod]
    public void Save_ComplexSpreadsheet()
    {
        Spreadsheet sheet = new();
        string filename = "sheet.txt";

        string expectedOutput = @"{
  ""Cells"": {
    ""B1"": {
      ""StringForm"": ""=2*C1*G1""
    },
    ""D1"": {
      ""StringForm"": ""=E1\u002B1""
    },
    ""C1"": {
      ""StringForm"": ""5""
    },
    ""G1"": {
      ""StringForm"": ""3""
    },
    ""E1"": {
      ""StringForm"": ""3""
    },
    ""A1"": {
      ""StringForm"": ""=B1\u002BD1""
    },
    ""F1"": {
      ""StringForm"": ""=C1""
    },
    ""H1"": {
      ""StringForm"": ""=B1""
    }
  }
}";

        Formula formula1 = new("B1 + D1");
        Formula formula2 = new("2 * C1 * G1");
        Formula formula3 = new("E1 + 1");
        Formula formula4 = new("C1");
        Formula formula5 = new("B1");

        sheet.SetContentsOfCell("B1", "4");
        sheet.SetContentsOfCell("D1", "5");
        sheet.SetContentsOfCell("C1", "6");
        sheet.SetContentsOfCell("G1", "7");
        sheet.SetContentsOfCell("E1", "8");

        sheet.SetContentsOfCell("A1", $"={formula1}");
        sheet.SetContentsOfCell("B1", $"={formula2}");
        sheet.SetContentsOfCell("C1", "5");
        sheet.Save(filename);

        sheet.SetContentsOfCell("D1", $"={formula3}");
        sheet.SetContentsOfCell("E1", "3");
        sheet.SetContentsOfCell("F1", $"={formula4}");
        sheet.Save(filename);

        sheet.SetContentsOfCell("G1", "3");
        sheet.SetContentsOfCell("h1", $"={formula5}"); // Lowercase to make sure it can handle it

        sheet.Save(filename);

        string savedOutput = File.ReadAllText(filename);

        // Asserts that the file contains at least a substring containing the same Cells
        Assert.IsTrue(savedOutput.Contains(expectedOutput));

        sheet.Load(filename);
        Assert.AreEqual(sheet.GetCellContents("A1"), formula1);
        Assert.AreEqual(sheet.GetCellContents("B1"), formula2);
        Assert.AreEqual(sheet.GetCellContents("C1"), 5.0);
        Assert.AreEqual(sheet.GetCellContents("D1"), formula3);
        Assert.AreEqual(sheet.GetCellContents("E1"), 3.0);
        Assert.AreEqual(sheet.GetCellContents("F1"), formula4);
        Assert.AreEqual(sheet.GetCellContents("G1"), 3.0);
        Assert.AreEqual(sheet.GetCellContents("H1"), formula5);
    }

    /// <summary>
    /// This attempts to save to an invalid file path.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Save_InvalidPaths()
    {
        Spreadsheet sheet = new();
        string filename = "C:\\file*name.txt";  // Contains *
        sheet.Save(filename);
    }

    /// <summary>
    /// This test checks that after an exception is thrown (in this case CircularException), the spreadsheet
    /// remains unchanged.
    /// </summary>
    [TestMethod]
    public void Save_NoChangesIfThingGoWrong()
    {
        Spreadsheet sheet = new();
        string filename = "sheet.txt";

        Formula formula1 = new("A2");
        sheet.SetContentsOfCell("A2", "1");

        sheet.SetContentsOfCell("A1", $"={formula1}");
        Assert.ThrowsException<CircularException>(() => sheet.SetContentsOfCell("A2", "=A1"));
        sheet.Save(filename);
        sheet.Load(filename);

        Assert.AreEqual(sheet.GetCellContents("A2"), 1.0);
    }

    /// <summary>
    /// This test method attempts to load a file that doesn't exist and expects  a SpreadsheetReadWriteException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Load_PathFile_Invalid()
    {
        Spreadsheet sheet = new();
        string filename = @"C:\ThisFolderDoesNotExist\ThisFileDoesNotExist.txt";
        sheet.Load(filename);
    }

    /// <summary>
    /// This method ensures that if the spreadsheet is loaded, that the changed parameter is false.
    /// </summary>
    [TestMethod]
    public void Load_ChangedIsFalse()
    {
        Spreadsheet sheet = new();
        string filename = "sheet.txt";

        sheet.SetContentsOfCell("A1", "5");
        sheet.Save(filename);

        sheet.SetContentsOfCell("A2", "5"); // This shouldn't be included in JSON, so will be deleted when sheet is loaded.
        sheet.Load(filename);

        Assert.IsFalse(sheet.Changed);
    }

    /// <summary>
    /// This test method ensures that a readonly file, when loaded, throws a SpreadsheetReadWriteException.
    /// </summary>
    [TestMethod]
    public void Load_ReadOnlyFile_Invalid()
    {
        Spreadsheet sheet = new();
        string filename = "ReadOnlyFile.txt";

        // if (File.Exists(filename))
        // {
        //    // Sometimes file is still readonly for whatever reason, and fs can't open, so normalizing file
        //    File.SetAttributes(filename, FileAttributes.Normal);
        // }
        using (FileStream fs = File.Create(filename))
        {
            // File stream will be automatically closed here
        }

        File.SetAttributes(filename, FileAttributes.ReadOnly);
        Assert.ThrowsException<SpreadsheetReadWriteException>(() => sheet.Load(filename));

        // Remove the read-only attribute before attempting to delete the file
        File.SetAttributes(filename, FileAttributes.Normal);

        File.Delete(filename);
    }

    /// <summary>
    /// This checks that a spreadsheet which that has unsaved data isn't included when an older save is loaded.
    /// </summary>
    [TestMethod]
    public void Load_RestoreOldSpreadSheet()
    {
        Spreadsheet sheet = new();
        string filename = "sheet.txt";

        sheet.SetContentsOfCell("A1", "5");
        sheet.SetContentsOfCell("C1", "1");
        sheet.SetContentsOfCell("A2", "=C1");
        sheet.SetContentsOfCell("C1", "=3+3");
        sheet.Save(filename);

        sheet.SetContentsOfCell("A1", "10");
        sheet.SetContentsOfCell("D1", "10");
        sheet.SetContentsOfCell("C1", "=6+6");
        sheet.Load(filename);

        List<string> correctCells = ["A1", "A2", "C1"];
        CollectionAssert.AreEquivalent(correctCells, sheet.GetNamesOfAllNonemptyCells().ToList());
        Assert.AreEqual(sheet.GetCellContents("A1"), 5.0);
        Assert.AreEqual(sheet.GetCellValue("A1"), 5.0);

        Formula formula1 = new("C1");
        Assert.AreEqual(sheet.GetCellContents("A2"), formula1);
        Assert.AreEqual(sheet.GetCellValue("A2"), 6.0);

        Assert.AreEqual(sheet.GetCellContents("D1"), string.Empty);
    }

    /// <summary>
    /// This test checks that a cell that contains a string returns its value as the same string.
    /// </summary>
    [TestMethod]
    public void GetCellValue_StringSimple()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A1", "hello");
        Assert.AreEqual(sheet.GetCellValue("A1"), "hello");
    }

    /// <summary>
    /// This test checks that a cell that contains a double returns its value as the same double.
    /// </summary>
    [TestMethod]
    public void GetCellValue_DoubleSimple()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A1", "10");
        Assert.AreEqual(sheet.GetCellValue("A1"), 10.0);
    }

    /// <summary>
    /// This tests that a Formula with no dependencies returns the correct double evaluation.
    /// Asserts that new value is used instead of old value.
    /// </summary>
    [TestMethod]
    public void GetCellValue_FormulaWithNoDependents()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A1", "=5+5");
        Assert.AreEqual(sheet.GetCellValue("A1"), 10.0);

        sheet.SetContentsOfCell("A1", "=10*10");
        Assert.AreEqual(sheet.GetCellValue("A1"), 100.0);
    }

    /// <summary>
    /// This test checks that a formula with dependencies returns the correct double evaluation.
    /// This test nests formulas to ensure the value is properly being computed for complicated relationships.
    /// </summary>
    [TestMethod]
    public void GetCellValue_FormulaWithDependents()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A2", "5");
        sheet.SetContentsOfCell("A4", "5");
        sheet.SetContentsOfCell("A3", "=A4");

        sheet.SetContentsOfCell("A1", "=A2+A3");

        Assert.AreEqual(sheet.GetCellValue("A1"), 10.0);

        sheet.SetContentsOfCell("A4", "10");
        Assert.AreEqual(sheet.GetCellValue("A1"), 15.0);
    }

    /// <summary>
    /// This test ensures that an intended cell name with an invalid name throws an InvalidNameException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellValue_CellName_Invalid()
    {
        Spreadsheet sheet = new();
        sheet.GetCellValue("E1E");
    }

    /// <summary>
    /// This method checks that an empty cell returns an empty string as its value.
    /// </summary>
    [TestMethod]
    public void GetCellValue_CellEmpty()
    {
        Spreadsheet sheet = new();
        Assert.AreEqual(sheet.GetCellValue("A1"), string.Empty);
    }

    /// <summary>
    /// Tests that a doubles (include a double in scientific notation) are parsed as doubles.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_SimpleDouble()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A1", "5E1");
        sheet.SetContentsOfCell("A2", "5");
        Assert.AreEqual(sheet.GetCellContents("A1"), 50.0);
        Assert.AreEqual(sheet.GetCellContents("A2"), 5.0);
    }

    /// <summary>
    /// This test checks that a formula (indicated by the =), which is invalid, throws a FormulaFormatException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void SetContentsOfCell_FormulaFormat_Invalid()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A1", "=1+1 - A1CAT");
    }

    /// <summary>
    /// This test checks that an empty formula throws a FormulaFormatException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void SetContentsOfCell_FormulaFormatEmpty_Invalid()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A1", "=");
    }

    /// <summary>
    /// This method checks that the SetContentsOfCell is setting contents to their correct respective types.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_IsCorrectType()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A3", "3");
        sheet.SetContentsOfCell("A4", "4");
        sheet.SetContentsOfCell("A1", "=5+5");
        sheet.SetContentsOfCell("A2", "=A3+A4");
        sheet.SetContentsOfCell("B1", "5");
        sheet.SetContentsOfCell("C1", "5+5");

        Assert.IsInstanceOfType(sheet.GetCellContents("A1"), typeof(Formula));
        Assert.IsInstanceOfType(sheet.GetCellContents("A2"), typeof(Formula));
        Assert.IsInstanceOfType(sheet.GetCellContents("B1"), typeof(double));
        Assert.IsInstanceOfType(sheet.GetCellContents("C1"), typeof(string));
    }

    /// <summary>
    /// This test checks that changed is set to true whenever SetContentsOfCell is called.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_ChangedIsTrue()
    {
        Spreadsheet sheet = new();
        Spreadsheet sheet2 = new();
        sheet.SetContentsOfCell("A1", "5");
        Assert.IsTrue(sheet.Changed);
    }

    /// <summary>
    /// This method attempts to load an invalid JSON file and expects a SpreadsheetReadWriteException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Load_JSON_Invalid()
    {
        Spreadsheet sheet = new();
        string filename = "Nonsense.txt";
        string filetext = "THIS IS NOT A JSON";

        using (StreamWriter writer = new StreamWriter(filename))
        {
            writer.WriteLine(filetext);
        }

        sheet.Load(filename);

        // File.Delete(filename);
    }

    /// <summary>
    /// This test attempts to load a file that does not exist and looks to catch a SpreadsheetReadWriteException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Load_FileDoesNotExist_Invalid()
    {
        Spreadsheet sheet = new();
        string filename = "ThisFileDoesNotExist.txt";

        sheet.Load(filename);
    }

    /// <summary>
    /// Tests that if JSON file was somehow tampered with, load could still correctly identify that items can not be validly loaded.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Load_JSON_Dependencies_Invalid()
    {
        string expectedOutput = @"{
  ""Cells"": {
    ""A2"": {
      ""StringForm"": ""=A1""
    },
    ""A1"": {
      ""StringForm"": ""=A2""
    }
  }
}";

        Spreadsheet sheet = new();
        string filename = "sheet.txt";
        File.WriteAllText(filename, expectedOutput);
        sheet.Load(filename);
    }

    /// <summary>
    /// This test ensures that a cell with previous data is truly empty.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_Values_To_Empty()
    {
        Spreadsheet sheet = new();
        sheet.SetContentsOfCell("A2", "2");
        sheet.SetContentsOfCell("A3", "3");
        sheet.SetContentsOfCell("A1", "=A2+A3");

        sheet.SetContentsOfCell("A1", string.Empty);
        Assert.AreEqual(sheet.GetCellContents("A1"), string.Empty);
    }
}

// Tests:

// Create stress test

// If it is empty, throw argument exception
// with invalid formula, throw formula error object
// Maybe test save with a filename without an extension
// Make sure error is thrown if string and double are eval?
// Have I tried just passing in a single cell that doesn't exist?
// TODO: UPDATE SO THAT ALL PATHFILES CAN BE ACCEPTED