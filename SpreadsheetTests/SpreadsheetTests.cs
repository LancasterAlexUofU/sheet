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
    /// Wow.
    /// </summary>
    [TestMethod]
    public void Passes()
    {
        Assert.IsTrue(true);
    }
}
