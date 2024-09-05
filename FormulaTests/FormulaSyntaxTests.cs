// <copyright file="FormulaSyntaxTests.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

/// <summary>
/// Author:    Alex Lancaster
/// Partner:   None
/// Date:      21-Aug-2024
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Alex Lancaster - This work may not
///            be copied for use in Academic Coursework.
///
/// I, Alex Lancaster, certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source.  All
/// references used in the completion of the assignments are cited
/// in my README file.
///
/// File Contents
///
///  FormulaTests runs 28 various tests on Formula.dll to ensure that the formula constructor is properly working.
///  This is done by checking the 10 Formula Syntax and Validation Rules for a formula in infix notation.
///  This test additionally checks for valid variables as well as numbers in proper scientific notation.
/// </summary>

namespace CS3500.Formula;

using CS3500.Formula; // Change this using statement to use different formula implementations.

/// <summary>
///   <para>
///     The following class shows the basics of how to use the MSTest framework,
///     including:
///   </para>
///   <list type="number">
///     <item> How to catch exceptions. </item>
///     <item> How a test of valid code should look. </item>
///   </list>
/// </summary>
[TestClass]
public class FormulaSyntaxTests
{
    // --- Tests for One Token Rule ---

    /// <summary>
    ///   <para>
    ///     This test makes sure the right kind of exception is thrown
    ///     when trying to create a formula with no tokens.
    ///   </para>
    ///   <remarks>
    ///     <list type="bullet">
    ///       <item>
    ///         We use the _ (discard) notation because the formula object
    ///         is not used after that point in the method.  Note: you can also
    ///         use _ when a method must match an interface but does not use
    ///         some of the required arguments to that method.
    ///       </item>
    ///       <item>
    ///         string.Empty is often considered best practice (rather than using "") because it
    ///         is explicit in intent (e.g., perhaps the coder forgot to but something in "").
    ///       </item>
    ///       <item>
    ///         The name of a test method should follow the MS standard:
    ///         https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
    ///       </item>
    ///       <item>
    ///         All methods should be documented, but perhaps not to the same extent
    ///         as this one.  The remarks here are for your educational
    ///         purposes (i.e., a developer would assume another developer would know these
    ///         items) and would be superfluous in your code.
    ///       </item>
    ///       <item>
    ///         Notice the use of the attribute tag [ExpectedException] which tells the test
    ///         that the code should throw an exception, and if it doesn't an error has occurred;
    ///         i.e., the correct implementation of the constructor should result
    ///         in this exception being thrown based on the given poorly formed formula.
    ///       </item>
    ///     </list>
    ///   </remarks>
    ///   <example>
    ///     <code>
    ///        // here is how we call the formula constructor with a string representing the formula
    ///        _ = new Formula( "5+5" );
    ///     </code>
    ///   </example>
    /// </summary>
    [TestMethod]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestNoTokens_Invalid( )
    {
        _ = new Formula(string.Empty);  // note: it is arguable that you should replace "" with string.Empty for readability and clarity of intent (e.g., not a cut and paste error or a "I forgot to put something there" error).
    }

    // --- Tests for Valid Token Rule ---

    // --- Tests for Closing Parenthesis Rule

    // --- Tests for Balanced Parentheses Rule

    // --- Tests for First Token Rule

    /// <summary>
    ///   <para>
    ///     Make sure a simple well formed formula is accepted by the constructor (the constructor
    ///     should not throw an exception).
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is not expected to throw an exception, i.e., it succeeds.
    ///     In other words, the formula "1+1" is a valid formula which should not cause any errors.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenNumber_Valid( )
    {
        _ = new Formula( "1+1" );
    }

    // --- Tests for  Last Token Rule ---

    // --- Tests for Parentheses/Operator Following Rule ---

    // --- Tests for Extra Following Rule ---

    /// <summary>
    ///   <para>
    ///     A simple formula enclosed by open and closed parentheses.
    ///     The constructor should not throw an error.
    ///   </para>
    ///   <remarks>
    ///     This test ensures that a simple equation surrounded by a set of parentheses
    ///     is still considered a valid formula. [ExpectedException] is not used as the test should
    ///     not throw an error.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenParenthesis_Valid()
    {
        _ = new Formula("(1 + 1)");
    }

    // --- Tests for Closing Parentheses Rule ---

    // --- Tests for Balanced Parentheses Rule ---

    // --- Tests for  Last Token Rule ---

    // --- Tests for Parentheses/Operator Following Rule ---

    // --- Tests for Extra Following Rule ---

    /// <summary>
    ///   <para>
    ///   This test ensures invalid symbols in a formula throws a FormulaFormatException.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestInvalidTokens_Invalid()
    {
        _ = new Formula("@");
    }

    // --- Tests for Valid Tokens Rule ---

    /// <summary>
    ///   <para>
    ///     This test makes sure that formulas with unbalanced parentheses throws a FormulaFormatException.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestBalancedParentheses_Invalid()
    {
        _ = new Formula("(x1))");
    }

    // --- Tests for Balanced Parentheses Rule ---

    // --- Tests for First Token Rule ---

    // --- Tests for Last Token Rule ---

    // --- Tests for Parenthesis Following Rule ---

    /// <summary>
    ///   <para>
    ///     This test checks that no closing parentheses seen so far be greater than the number of opening parentheses seen so far.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]

    public void FormulaConstructor_TestClosingParentheses_Invalid()
    {
        _ = new Formula("(x1))(((x2))");
    }

    // --- Tests for Closing Parentheses Rule ---

    // --- Tests for Balanced Parentheses Rule ---

    // --- Tests for Parenthesis Following Rule ---

    /// <summary>
    ///   <para>
    ///     This test assures that an operand isn't the first token in a formula. <br/>
    ///     (Side note: an opening parenthesis at the end or a closed parenthesis should be caught by the Closing Parentheses or Balanced Parentheses Test.)
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstToken_Invalid()
    {
        _ = new Formula("+ (x1)");
    }

    // --- Tests for First Token Rule ---

    /// <summary>
    ///   <para>
    ///     This test assures that an operand isn't the last token in a formula. <br/>
    ///     (Side note: an opening parenthesis at the end or a closed parenthesis should be caught by the Closing Parentheses or Balanced Parentheses Test.)
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastToken_Invalid()
    {
        _ = new Formula("(x1) -");
    }

    // --- Tests for Last Token Rule ---

    /// <summary>
    ///   <para>
    ///     This test checks that no operand is followed by another operand.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorFollowing_Invalid()
    {
        _ = new Formula("x1 + + x2");
    }

    // --- Test for First Token Rule ---

    // --- Tests for Last Token Rule ---

    // --- Tests for Operator Following Rule ---

    /// <summary>
    ///   <para>
    ///     This test checks that there is an operand between two variables.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]

    public void FormulaConstructor_TestExtraFollowingVariable_Invalid()
    {
        _ = new Formula("x1 x2");
    }

    // --- Tests for Extra Following Rule ---

    /// <summary>
    ///   <para>
    ///     This test checks that there is an operand between two numbers.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]

    public void FormulaConstructor_TestExtraFollowingNumber_Invalid()
    {
        _ = new Formula("1 1");
    }

    // --- Tests for Extra Following Rule ---


    /// <summary>
    ///     Tests that even though a number in scientific notation has an operand in the number, <br />
    ///     it is still treated as a number and not an operand for the Extra Following Rule.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingScientificNumber_Invalid()
    {
        _ = new Formula("(x1)7e-7");
    }

    // --- Tests for Extra Following Rule

    /// <summary>
    /// Tests that the number zero is a valid formula. [ExpectedException] is not used so the test should not throw an error.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestZero_Valid()
    {
        _ = new Formula("0");
    }

    // --- Tests for One Token Rule ---

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// Tests that a non-zero number is a valid formula. [ExpectedException] is not used so the test should not throw an error.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestInteger_Valid()
    {
        _ = new Formula("7");
    }

    // --- Tests for One Token Rule ---

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// Tests that a decimal is a valid formula.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestDecimal_Valid()
    {
        _ = new Formula("0.1");
    }

    // --- Tests for One Token Rule ---

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// Tests that a number with an extraneous decimal (e.g. 1.) is considered a valid formula.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestExtraneousDecimal_Valid()
    {
        _ = new Formula("1.");
    }

    // --- Tests for One Token Rule ---

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// Tests that 0 in scientific notation is a valid formula.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestScientificNotationZero_Valid()
    {
        _ = new Formula("0.0e-0");
    }

    // --- Tests for One Token Rule ---

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// Tests that integers used in writing numbers in scientific notation is a valid formula (e.g. 7e7).
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestScientificNotationInteger_Valid()
    {
        _ = new Formula("7e7");
    }

    // --- Tests for One Token Rule ---

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// Tests that numbers with decimals that are included in writing numbers in scientific notation are considered valid formulas (e.g. 0.7e7).
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestScientificNotationDecimal_Valid()
    {
        _ = new Formula("0.7e7");
    }

    // --- Tests for One Token Rule ---

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// Tests that a number in scientific notation with a negative sign in the exponent is considered a valid formula.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestScientificNotationExponentNegative_Valid()
    {
        _ = new Formula("7e-7");
    }

    // --- Tests for One Token Rule ---

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// Tests that numbers in scientific notation do not contain decimals in their exponent.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestScientificNotationExponentDecimal_Invalid()
    {
        _ = new Formula("7e0.7");
    }

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// This test passes a single letter without a number and should throw a FormulaFormatException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableWithoutNumber_Invalid()
    {
        _ = new Formula("a");
    }

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// Tests that the formula constructor throws a FormulaFormatException error if a number is seen before a letter (e.g. 1a).
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableNumberBeforeVariable_Invalid()
    {
        _ = new Formula("1a");
    }

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// Tests that a letter does not follow a number in a variable (e.g. a1a).
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableAfterNumber_Invalid()
    {
        _ = new Formula("a1a");
    }

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// Tests that letters followed by a number in the decimal form is not considered a valid formula (e.g. a1.0).
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableWithDecimal_Invalid()
    {
        _ = new Formula("a1.0");
    }

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// Tests that a variable with uppercase lettered mixed with lower case letters is still considered a valid formula.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestVariableUppercaseMixed_Valid()
    {
        _ = new Formula("aAaA1");
    }

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// Tests that valid variables that contain 'E' (the same letter used to denote numbers in scientific notation) <br/>
    /// is counted as a variable instead of a scientific notation formating error (e.g. e1).
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestVariablesContainingE_Valid()
    {
        _ = new Formula("e1");
    }

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// Runs a suit of the previous invalid tests, except the letters are replaced with 'E'. <br/>
    /// This is to make sure any invalid variable containing 'E' do not count as valid numbers in scientific notation. <br/>
    /// If this test doesn't return an exception for most cases but the other variable invalid tests return exceptions, it is highly likely there is an issue with a scientific notation processor.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestVariablesContainingE_Invalid()
    {
        string[] testCases = ["e", "e1e", "1e", "1ee"];

        foreach (string testCase in testCases)
        {
            try
            {
                _ = new Formula(testCase);
                Assert.Fail($"No exception thrown for case: {testCase}");
            }
            catch (FormulaFormatException) // Expected exception
            {
            }
        }
    }

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// This test passes a complex formula to the formula constructor that uses all operations, variables with differing attributes, <br/>
    /// integers, numbers with decimals, varying numbers in scientific notation, and parentheses throughout. <br/> <br/>
    /// The purpose of this test is to ensure that if a complex formula with a blanket of almost all interactions <br/>
    /// found in formulas passes, then simpler equations should also pass.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestComplexEquation_Valid()
    {
        _ = new Formula("((aA1 + E2)/0)*(0.0E-0-3e1)+ 0.1 - 50 - (50E-1)/(3/Aa1)");
    }

    // --- Tests for Balanced Parentheses Rule ---

    // --- Tests for First Token Rule ---

    // --- Tests for Last Token Rule ---

    // --- Tests for Parenthesis/Operator Following Rule ---

    // --- Tests for Extra Following Rule ---

    /// <summary>
    /// This is a simple test which validates that the ToString method uppercases a variable correctly and returns it as a string.
    /// </summary>
    [TestMethod]
    public void ToString_TestVariable_Valid()
    {
        Formula formula = new Formula("x1");
        Assert.AreEqual(formula.ToString(), "X1");
    }

    /// <summary>
    /// Test that ensures the ToString method correctly returns a formula with an operand and two variables.
    /// </summary>
    [TestMethod]
    public void ToString_TestFormulaOperation_Valid()
    {
        Formula formula = new Formula("x1 + x2");
        Assert.AreEqual(formula.ToString(), "X1+X2");
    }

    /// <summary>
    /// Test which ensures the ToString method can properly handle parentheses with varying spaces between them.
    /// </summary>
    [TestMethod]
    public void ToString_TestParentheses_Valid()
    {
        Formula formula = new Formula(" (  (x1+ 2  ) )");
        Assert.AreEqual(formula.ToString(), "((X1+2))");
    }

    /// <summary>
    /// Test that ensures the ToString method works properly on numbers in scientific notation by removing any unnecessary zeros after the decimal and capitalizing 'e'.
    /// </summary>
    [TestMethod]
    public void ToString_TestScientificNotation_Valid()
    {
        Formula formula = new Formula("0.0e-0");
        Assert.AreEqual(formula.ToString(), "0E-0");
    }


    /// <summary>
    /// This test checks that the ToString can properly recognize and eliminate any extraneous zeros in a number.
    /// </summary>
    [TestMethod]
    public void ToString_TestExtraneousZeros_Valid()
    {
        Formula formula = new Formula("05 + 05.00 + 0.050 + 05.00e-0001");
        Assert.AreEqual(formula.ToString(), "5+5+0.05+5E-1");
    }

    /// <summary>
    /// This test ensures that a number which contains a decimal point and no number to the right of the decimal point is properly handled.
    /// </summary>
    [TestMethod]
    public void ToString_TestExtraneousDecimalPoint_Valid()
    {
        Formula formula = new Formula("01. + 1.");
        Assert.AreEqual(formula.ToString(), "1+1");
    }

    /// <summary>
    /// This tests sends a formula to the ToString method that uses all operations, variables with differing attributes, <br/>
    /// integers, numbers with decimals, varying numbers in scientific notation, and parentheses throughout to ensure that <br/>
    /// all interactions in a formula are accounted for and tested.
    /// </summary>
    [TestMethod]
    public void ToString_TestComplexFormula_Valid()
    {
        Formula formula = new Formula("((aA1 + E2)/0)*(0.0E-0-3e1)+ 0.1 - 50 - (50E-1)/(3/Aa1)");
        Assert.AreEqual(formula.ToString(), "((AA1+E2)/0)*(0E-0-3E1)+0.1-50-(50E-1)/(3/AA1)");
    }

    /// <summary>
    /// This test ensures that two equivalent canonical string representations of a formula, which have been modified <br/>
    /// with extra spaces, different capitalizations, extraneous decimal points, and extraneous zeros are still canonically the same.
    /// </summary>
    [TestMethod]
    public void ToString_TestComplexFormulaEquivalent_Valid()
    {
        Formula EquivalentFormula1 = new Formula("  ((  Aa1+E2) /0.0)*(0.E-0 -3e1)+ 00.10 - 050. - (50E-1)/(3/Aa1)");
        Formula EquivalentFormula2 = new Formula("((aA1+E02)/0)*(0.0E-00-3e1)+0.1-50-(050.E-1)/(3./Aa1)");

        string EquivalentFormula1ToString = EquivalentFormula1.ToString();
        string EquivalentFormula2ToString = EquivalentFormula2.ToString();

        Assert.AreEqual(EquivalentFormula1ToString, "((AA1+E2)/0)*(0E-0-3E1)+0.1-50-(50E-1)/(3/AA1)");
        Assert.AreEqual(EquivalentFormula2ToString, "((AA1+E2)/0)*(0E-0-3E1)+0.1-50-(50E-1)/(3/AA1)");
    }

}