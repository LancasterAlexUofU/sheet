// <copyright file="SpreadsheetGUI.razor.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// <summary>
//
// Author:    Alex Lancaster
// Partner:   Landon Huking
// Date:      24-Oct-2024
// Course:    CS 3500, University of Utah, School of Computing
// Copyright: CS 3500 and Alex Lancaster - This work may not
//            be copied for use in Academic Coursework.
//
// I, Alex Lancaster & Landon Huking, certify that I wrote this code from scratch and
// did not copy it in part or whole from another source.  All
// references used in the completion of the assignments are cited
// in my README file.
//
// File Contents
//
// </summary>
// Ignore Spelling: Blazor Interop
namespace SpreadsheetNS;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Diagnostics;
using CS3500.Spreadsheet;
using CS3500.Formula;

/// <summary>
///  <remarks>
///    <para>
///      This is a partial class because class SpreadsheetGUI is also automatically
///      generated from the SpreadsheetGUI.razor file.  Any code in that file, and variable in
///      that file can be referenced here, and vice versa.
///    </para>
///    <para>
///      It is usually better to put the code in a separate CS isolation file so that Visual Studio
///      can use intellisense better.
///    </para>
///    <para>
///      Note: only GUI related information should go in the sheet. All (Model) spreadsheet
///      operations should happen through the Spreadsheet class API.
///    </para>
///    <para>
///      The "backing stores" are strings that are used to affect the content of the GUI
///      display.  When you update the Spreadsheet, you will then have to copy that information
///      into the backing store variable(s).
///    </para>
///  </remarks>
/// </summary>
public partial class SpreadsheetGUI
{
    /// <summary>
    /// Initialize a new spreadsheet model.
    /// </summary>
    private Spreadsheet sheet = new();

    /// <summary>
    /// Bool to store state of chart.
    /// </summary>
    private bool showChart = false;

    /// <summary>
    ///    Gets the alphabet for ease of creating columns.
    /// </summary>
    private static char[ ] Alphabet { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    /// <summary>
    ///   Gets or sets the javascript object for this web page that allows
    ///   you to interact with any javascript in the associated file.
    /// </summary>
    private IJSObjectReference? JSModule { get; set; }

   /// <summary>
    ///   Gets or sets the Filename to be saved.
    /// </summary>
    private string FileSaveName { get; set; } = "Spreadsheet.sprd";

    /// <summary>
    ///   <para> Gets or sets the data for the Tool Bar Cell Contents text area, e.g., =57+A2. </para>
    ///   <remarks>Backing Store for HTML</remarks>
    /// </summary>
    private string ToolBarCellContents { get; set; } = string.Empty;

    /// <summary>
    ///   <para> Gets or sets the data for all of the cells in the spreadsheet GUI. </para>
    ///   <remarks>Backing Store for HTML</remarks>
    /// </summary>
    private string[,] CellsBackingStore { get; set; } = new string[ 99, 26 ];

    /// <summary>
    ///   <para> Gets or sets the html class string for all of the cells in the spreadsheet GUI. </para>
    ///   <remarks>Backing Store for HTML CLASS strings</remarks>
    /// </summary>
    private string[,] CellsClassBackingStore { get; set; } = new string[ 99, 26 ];

    /// <summary>
    ///   Gets or sets a value indicating whether we are showing the save "popup" or not.
    /// </summary>
    private bool SaveGUIView { get; set; }

    /// <summary>
    ///   Query the spreadsheet to see if it has been changed.
    ///   <remarks>
    ///     Any method called from JavaScript must be public
    ///     and JSInvokable!
    ///   </remarks>
    /// </summary>
    /// <returns>
    ///   true if the spreadsheet is changed.
    /// </returns>
    [JSInvokable]
    public bool HasSpreadSheetChanged()
    {
        Debug.WriteLine( $"{"HasSpreadSheetChanged",-30}: {Navigator.Uri}. Remove Me." );

        if (sheet.Changed)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///   Example of how JavaScript can talk "back" to the C# side.
    /// </summary>
    /// <param name="message"> string from javascript side. </param>
    [JSInvokable]
    public void TestBlazorInterop( string message )
    {
        Debug.WriteLine( $"JavaScript has send me a message: {message}" );
    }

    /// <summary>
    ///   Set up initial state and event handlers.
    ///   <remarks>
    ///     This is somewhat like a constructor for a Blazor Web Page (object).
    ///     You probably don't need to do anything here.
    ///   </remarks>
    /// </summary>
    protected override void OnInitialized( )
    {
        Debug.WriteLine( $"{"OnInitialized",-30}: {Navigator.Uri}. Remove Me." );
        CellsClassBackingStore[0, 0] = "selected";
    }

    /// <summary>
    ///   Called anytime in the lifetime of the web page were the page is re-rendered.
    ///   <remarks>
    ///     You probably don't need to do anything in here beyond what is provided.
    ///   </remarks>
    /// </summary>
    /// <param name="firstRender"> true the very first time the page is rendered.</param>
    protected async override void OnAfterRender( bool firstRender )
    {
        base.OnAfterRender( firstRender );

        Debug.WriteLine( $"{"OnAfterRenderStart",-30}: {Navigator.Uri} - first time({firstRender}). Remove Me." );

        if ( firstRender )
        {
            /////////////////////////////////////////////////
            //
            // The following three lines setup and test the
            // ability for Blazor to talk to javascript and vice versa.
            JSModule = await JS.InvokeAsync<IJSObjectReference>( "import", "./Pages/SpreadsheetGUI.razor.js" ); // create/read the javascript
            await JSModule.InvokeVoidAsync( "SetDotNetInterfaceObject", DotNetObjectReference.Create( this ) ); // tell the javascript about us (dot net)
            await JSModule.InvokeVoidAsync( "TestJavaScriptInterop", "Hello JavaScript!" ); // test that it is working.  You could remove this.
            await FormulaContentEditableInput.FocusAsync(); // when we start up, put the focus on the input. you will want to do this anytime a cell is clicked.
        }

        Debug.WriteLine( $"{"OnAfterRender Done",-30}: {Navigator.Uri} - Remove Me." );
    }

    /// <summary>
    ///  cells should be of the form "A5" or "B1".  The matrix of cells (the backing store) is zero
    ///  based but the first row in the spreadsheet is 1.
    /// </summary>
    /// <param name="cellName"> The name of the cell. </param>
    /// <param name="row"> The returned conversion between row and zero based index. </param>
    /// <param name="col"> The returned conversion between column letter and zero based matrix index. </param>
    private static void ConvertCellNameToRowCol( string cellName, out int row, out int col )
    {
        col = Array.IndexOf(Alphabet, cellName[0]);
        int.TryParse(cellName[1].ToString(), out row);
        row -= 1;
    }

    /// <summary>
    ///   Given a row,col such as "(0,0)" turn this into the appropriate
    ///   cell name, such as: "A1".
    /// </summary>
    /// <param name="row"> The row number (0-A, 1-B, ...).</param>
    /// <param name="col"> The column number (0 based).</param>
    /// <returns>A string defining the cell name, where the col is A-Z and row is not zero based.</returns>
    private static string CellNameFromRowCol( int row, int col )
    {
        return $"{Alphabet[col]}{row + 1}";
    }

    /// <summary>
    ///   Called when the input widget (representing the data in a particular cell) is modified.
    /// </summary>
    /// <param name="newInput"> The new value to put at row/col. </param>
    /// <param name="row"> The matrix row identifier. </param>
    /// <param name="col"> The matrix column identifier. </param>
    private async void HandleUpdateCellInSpreadsheet( string newInput, int row, int col )
    {
        InputWidgetBackingStore = $"{row},{col}";
        try
        {
            string cellName = CellNameFromRowCol(row, col);
            var updatedCells = sheet.SetContentsOfCell(cellName, newInput);
            foreach (var cell in updatedCells)
            {
                ConvertCellNameToRowCol(cell, out int theRow, out int theCol);
                object value = sheet.GetCellValue(cell);

                if (value is FormulaError error)
                {
                    CellsBackingStore[theRow, theCol] = error.Reason;
                }
                else
                {
                    CellsBackingStore[theRow, theCol] = value.ToString() ?? string.Empty;
                }
            }
        }
        catch (FormulaFormatException)
        {
            await JS.InvokeVoidAsync("alert", "This formula is invalid. No changes made.");
        }
        catch (CircularException)
        {
            await JS.InvokeVoidAsync("alert", "This causes a circular dependency. No changes made.");
        }
        catch
        {
            // a way to communicate to the user that something went wrong.
            await JS.InvokeVoidAsync( "alert", "Something went wrong." );
        }
    }

    /// <summary>
    ///   <para>
    ///     Using a Web Input ask the user for a file and then process the
    ///     data in the file.
    ///   </para>
    ///   <remarks>
    ///     Unfortunately, this happens after the file is chosen, but we will live with that.
    ///   </remarks>
    /// </summary>
    /// <param name="args"> Information about the file that has been selected. </param>
    private async void HandleLoadFile( EventArgs args )
    {
        try
        {
            if (sheet.Changed)
            {
                bool success = await JS.InvokeAsync<bool>("confirm", "Do this?");

                if (!success)
                {
                    return;    // user canceled the action.
                }
            }

            string fileContent = string.Empty;

            InputFileChangeEventArgs eventArgs = args as InputFileChangeEventArgs ?? throw new Exception("that didn't work");
            if ( eventArgs.FileCount == 1 )
            {
                var file = eventArgs.File;
                if ( file is null )
                {
                    return;
                }

                using var stream = file.OpenReadStream();
                using var reader = new System.IO.StreamReader(stream);
                fileContent = await reader.ReadToEndAsync();

                await JS.InvokeVoidAsync( "alert", fileContent );
                sheet.InstantiateFromJSON(fileContent);

                // Takes all loaded in cells and places them into CellsBackingStore for display
                foreach (var cell in sheet.GetNamesOfAllNonemptyCells())
                {
                    ConvertCellNameToRowCol(cell, out int row, out int col);
                    CellsBackingStore[row, col] = sheet.GetCellValue(cell).ToString() ?? string.Empty;
                }

                StateHasChanged();
            }
        }
        catch ( Exception e )
        {
            Debug.WriteLine( "something went wrong with loading the file..." + e );
        }
    }

    /// <summary>
    ///   Switch between the file save view or main view.
    /// </summary>
    /// <param name="show"> if true, show the file save view. </param>
    private void ShowHideSaveGUI(bool show)
    {
        SaveGUIView = show;
        StateHasChanged();
    }

    /// <summary>
    ///   Call the JavaScript necessary to download the data via the Browser's Download
    ///   Folder.
    /// </summary>
    /// <param name="e"> Ignored. </param>
    private async void HandleSaveFile(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
    {
        // <remarks> this null check is done because Visual Studio doesn't understand
        // the Blazor life cycle and cannot assure of non-null. </remarks>
        if ( JSModule is not null )
        {
            var success = await JSModule.InvokeAsync<bool>( "saveToFile", SaveFileName, sheet.GetJSON() );
            if (success)
            {
                ShowHideSaveGUI( false );
                StateHasChanged();
            }
        }
    }

    /// <summary>
    ///   Clear the spreadsheet if not modified.
    /// </summary>
    /// <param name="e"> Ignored. </param>
    private async void HandleClear(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
    {
        if ( JSModule is not null )
        {
            bool success = true;

            if (sheet.Changed)
            {
                success = await JS.InvokeAsync<bool>("confirm", "Clear the sheet?");
            }

            if (success)
            {
                Array.Clear(CellsBackingStore);
                Array.Clear(CellsClassBackingStore);
                InputWidgetBackingStore = string.Empty;
                ToolBarCellContents = string.Empty;
                sheet = new Spreadsheet();
                showChart = false;
                StateHasChanged();
            }
        }
    }

    /// <summary>
    ///   Helper method to set TollbarCellContents.
    /// </summary>
    /// <param name="value"> The value as an object. </param>
    private void SetToolbarCellContents(object value)
    {
        if (value is Formula)
        {
            ToolBarCellContents = '=' + value.ToString();
        }
        else
        {
            ToolBarCellContents = value.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    /// Builds chart data by iterating through the non empty cells.
    /// </summary>
    private List<ChartDataType> GetChart()
    {
        showChart = true;
        ISet<string> nonEmptyCells = sheet.GetNamesOfAllNonemptyCells();

        foreach(string cell in nonEmptyCells)
        {
            int.TryParse(sheet.GetCellValue(cell).ToString(), out int value);
            ChartDataType item = new() { Value = value, Cell = cell };
            ChartData.Add(item);
        }

        return ChartData;
    }
}
