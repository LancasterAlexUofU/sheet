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
//      FIXME: ADD TO ME!!!!!!!!!!
// <summary>
namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        /*
         * GetNamesOfAllNonemptyCells()
         * - Empty
         * - Double only
         * - String only
         * - Formula only
         * - Double, String, and Formula
         * 
         * GetCellContents(string name)
         * Empty Cell (Cell that has not been initialized)
         * Invalid cell name
         * String
         * Double
         * Formula
         * 
         * SetCellContents Num
         * Invalid cell name
         * SetCellContents Num + SetCellContents Num
         * 
         * SetCellContents String
         * Invalid cell name 
         * SetCellContents string + SetCellContents string + SetCellContents string
         * 
         * SetCellContents Formula
         * Invalid cell name
         * Circular Exception ( may be thrown by private method?)
         * SetCellContents Formula + SetCellContents Formula + SetCellContents Formula
         * 
         * 
         * SetCellContents All
         * SetCellContents Num + SetCellContents String + SetCellContents Formula
         * (Num, string, and formula all return correctly)
         * 
         */
    }
}