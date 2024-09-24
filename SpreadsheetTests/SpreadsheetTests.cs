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
//      TODO: ADD TO ME!!!!!!!!!!
// <summary>
namespace CS3500.SpreadsheetTests;

using CS3500.Formula;
using CS3500.Spreadsheet;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;

/// <summary>
/// This test class ensures that the Spreadsheet project is properly working but testing all public methods. <br/>
/// Many of the tests look for Invalid name and circular exceptions to be thrown.
/// For SetCellContents, all return types are tested for doubles, strings, and formulas.
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
        sheet.SetCellContents("A1", 1.0);
        sheet.SetCellContents("B1", 0);
        sheet.SetCellContents("A1", 2E1);

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
        sheet.SetCellContents("A1", "A1");
        sheet.SetCellContents("B1", "This is a string");
        sheet.SetCellContents("A1", "A1 + B1");
        sheet.SetCellContents("C1", "1234");
        sheet.SetCellContents("C1", string.Empty);
        sheet.SetCellContents("D1", string.Empty);

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

        sheet.SetCellContents("A1", expression1);
        sheet.SetCellContents("B1", expression2);
        sheet.SetCellContents("A1", expression3);
        sheet.SetCellContents("C1", expression4);

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

        sheet.SetCellContents("A1", expression1);
        sheet.SetCellContents("A1", expression1); // Replace with formula
        sheet.SetCellContents("A1", "1 + 1"); // Replace with string
        sheet.SetCellContents("A1", 2); // Replace with double

        sheet.SetCellContents("B1", expression2);
        sheet.SetCellContents("C1", 4);
        sheet.SetCellContents("D1", string.Empty);
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
        sheet.SetCellContents("A1", "A1");
        sheet.SetCellContents("B1", "This is a string");
        sheet.SetCellContents("A1", "A1 + B1");
        sheet.SetCellContents("C1", "1234");
        sheet.SetCellContents("C1", string.Empty);
        sheet.SetCellContents("D1", string.Empty);

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
        sheet.SetCellContents("A1", 1.0);
        sheet.SetCellContents("B1", 0);
        sheet.SetCellContents("A1", 2E1);

        Assert.AreEqual(2E1, sheet.GetCellContents("A1"));
        Assert.AreEqual(0, sheet.GetCellContents("B1"));
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

        sheet.SetCellContents("A1", expression1);
        sheet.SetCellContents("B1", expression2);
        sheet.SetCellContents("A1", expression3);
        sheet.SetCellContents("C1", expression4);

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

        sheet.SetCellContents("A1", expression1);
        Assert.AreEqual(expression1, sheet.GetCellContents("A1"));

        sheet.SetCellContents("A1", "1 + 1"); // Replace with string
        Assert.AreEqual("1+ 1", sheet.GetCellContents("A1"));

        sheet.SetCellContents("A1", 2); // Replace with double
        Assert.AreEqual(2, sheet.GetCellContents("A1"));

        sheet.SetCellContents("B1", expression2);
        sheet.SetCellContents("C1", 4);
        sheet.SetCellContents("D1", string.Empty);

        Assert.AreEqual(expression2, sheet.GetCellContents("B1"));
        Assert.AreEqual(4, sheet.GetCellContents("C1"));
        Assert.AreEqual(string.Empty, sheet.GetCellContents("D1")); // Might throw error? Is it equal to nothing?
    }

    /// <summary>
    /// This tests that the SetCellContents (double method) returns an InvalidNameException for an invalid cell name.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetCellContents_Double_InvalidName()
    {
        Spreadsheet sheet = new();
        sheet.SetCellContents("1E1", 2);
    }

    /// <summary>
    /// This tests that the SellCellContents (double method) returns a list of itself.
    /// </summary>
    [TestMethod]
    public void SetCellContents_Double()
    {
        Spreadsheet sheet = new();

        List<double> values = [];
        values.Add(2);
        CollectionAssert.AreEquivalent(values, sheet.SetCellContents("A1", 2).ToList());

        values.Clear();
        values.Add(2E1);
        CollectionAssert.AreEquivalent(values, sheet.SetCellContents("A1", 2E1).ToList());
    }

    /// <summary>
    /// This tests that the SetCellContents (string method) returns an InvalidNameException for an invalid cell name.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetCellContents_String_InvalidName()
    {
        Spreadsheet sheet = new();
        sheet.SetCellContents("1E1", "2");
    }

    /// <summary>
    /// This tests that the SellCellContents (string method) returns a list of itself (cell name).
    /// </summary>
    [TestMethod]
    public void SetCellContents_String()
    {
        Spreadsheet sheet = new();

        List<string> values = [];
        values.Add("A1");
        CollectionAssert.AreEquivalent(values, sheet.SetCellContents("A1", "2").ToList());

        CollectionAssert.AreEquivalent(values, sheet.SetCellContents("A1", "1 + 1").ToList());

        values.Clear();
        values.Add("B1");
        CollectionAssert.AreEquivalent(values, sheet.SetCellContents("B1", "(A1 * 2)").ToList());
    }

    /// <summary>
    /// This method attempts to set a cell's contents in a cell with an invalid name, expected to throw
    /// InvalidNameException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetCellContents_Formula_InvalidName()
    {
        Spreadsheet sheet = new();
        Formula formula = new("1 + 1");
        sheet.SetCellContents("1E1", formula);
    }

    /// <summary>
    /// This tests that a cell that links back to a previous cell throws a CircularException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetCellContents_Formula_InvalidCircular()
    {
        Spreadsheet sheet = new();
        Formula formula1 = new("1 + B1");
        Formula formula2 = new("1 + A1");
        sheet.SetCellContents("A1", formula1);
        sheet.SetCellContents("B1", formula2);

        Assert.AreEqual(string.Empty, sheet.GetCellContents("B1")); // I don't think this will be executed if formula2 throws circular, so how to check?

        // TODO: Maybe check that it doesn't actually change the spreadsheet
    }

    /// <summary>
    /// This test sets a cell contents to a formula containing itself, expected to throw CircularException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetCellContents_Formula_InvalidCircularSingleCell()
    {
        Spreadsheet sheet = new();
        Formula formula = new("A1");
        sheet.SetCellContents("A1", formula);
    }

    /// <summary>
    /// This method tests that SetCellContents are returning correct lists of dependent cells based on only formula inputs.
    /// </summary>
    [TestMethod]
    public void SetCellContents_FormulaOnly()
    {
        Spreadsheet sheet = new();
        Formula formula1 = new("(B1 * 4) + 2");
        Formula formula2 = new("C1 / 2");
        Formula formula3 = new("70 - 20");
        Formula formula4 = new("(B1 * 4) + 4");
        List<string> values = [];

        // It should only return A1 and not A1, B1, since even if things are changed in A1, B1 won't be changed and therefore no recalculations are needed.
        values.Add("A1");
        CollectionAssert.AreEquivalent(values, sheet.SetCellContents("A1", formula1).ToList());

        // Since B1 depends on C1, should return B1, C1
        values.Insert(0, "B1"); // Inserts B1 at the beginning of the list
        CollectionAssert.AreEquivalent(values, sheet.SetCellContents("B1", formula2).ToList());

        values.Insert(0, "C1"); // C1 goes at front of list
        CollectionAssert.AreEquivalent(values, sheet.SetCellContents("C1", formula3).ToList());

        // "Replace" A1, see that it still returns A1 even when linked to other established cells.
        values.Clear();
        values.Add("A1");
        CollectionAssert.AreEquivalent(values, sheet.SetCellContents("A1", formula4).ToList());
    }

    /// <summary>
    /// This test creates a spreadsheet that sets cells of all types and ensures that the returned dependency lists are returning correctly.
    /// </summary>
    [TestMethod]
    public void SetCellContents_AllTypes()
    {
        Spreadsheet sheet = new();
        Formula formula1 = new("(B1 * 4) + 2");
        Formula formula2 = new("C1 / 2");
        Formula formula3 = new("(D1 * 4) + 4");
        List<string> values = [];

        sheet.SetCellContents("A1", formula1);

        values.Add("B1");
        values.Add("A1");
        CollectionAssert.AreEquivalent(values, sheet.SetCellContents("B1", formula2).ToList());

        values.Insert(0, "C1"); // values = ["C1", "B1", "A1"]
        CollectionAssert.AreEquivalent(values, sheet.SetCellContents("C1", 50).ToList());

        values.Clear();
        values.Add("D1");
        CollectionAssert.AreEquivalent(values, sheet.SetCellContents("D1", "STRING!").ToList());

        values.Insert(0, "E1"); // values = ["E1", "D1"]
        CollectionAssert.AreEquivalent(values, sheet.SetCellContents("E1", formula3).ToList());
    }

    /// <summary>
    /// This test add and removes a cell, then checks whether it was removed from the spreadsheet properly.
    /// </summary>
    [TestMethod]
    public void SetCellContents_Remove()
    {
        Spreadsheet sheet = new();
        Formula formula = new("1");
        List<string> values = [];

        sheet.SetCellContents("A1", formula);

        // Setting A1 to string.Empty removes it
        CollectionAssert.AreEquivalent(values, sheet.SetCellContents("A1", string.Empty).ToList());

        // Ensure GetCellContents and GetNamesOfAllNonemptyCells are also working
        Assert.AreEqual(string.Empty, sheet.GetCellContents("A1"));
        CollectionAssert.AreEquivalent(values, sheet.GetNamesOfAllNonemptyCells().ToList());
    }
}
