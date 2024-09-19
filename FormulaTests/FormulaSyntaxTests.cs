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
///  FormulaTests runs 42 various tests on Formula.dll to ensure that the formula constructor is properly working.
///  This is done by checking the 8 Formula Syntax and Validation Rules for a formula in infix notation.
///  This test additionally checks for valid variables as well as numbers in proper scientific notation.
/// </summary>
namespace CS3500.Formula;
using CS3500.Formula;

/// <summary>
/// The FormulaSyntaxTest sends a formula, valid or invalid, to the formula constructor.
/// If the formula is valid, then no error is expected to be thrown.
/// If the formula is invalid, then a FormulaFormatException is expected to be thrown.
/// </summary>
[TestClass]
public class FormulaSyntaxTests
{
    /// Complex formulas are meant to rigorously test Formula Class against many edge cases.
    /// That is why the formulas are long and strange looking. <see cref="FormulaConstructor_TestComplexFormula_Valid"/>
    private const string ComplexFormula = "((aA1 + E2)/0)*(0.0E-0-3e1)+ 0.1 - 50 - (50E-1)/(3/Aa1)+123456789101112131415+0.0000000000001";
    private const string ComplexFormulaCanonical = "((AA1+E2)/0)*(0-30)+0.1-50-(5)/(3/AA1)+1.2345678910111213E+20+1E-13";

    private const string ComplexFormulaEquivalentModified1 = "  ((  Aa1+E2) /0.0)*(0.E-0 -3e1)+ 00.10 - 050. - (50E-1)/(3/Aa1) +0000123456789101112131415+0.00000000000010000";
    private const string ComplexFormulaEquivalentModified2 = "((aA1+E2)/0)*(0.0E-00-3e1)+0.1-50-(050.E-1)/(3./Aa1) + 00123456789101112131415.00+00.000000000000100";

    /// <summary>
    /// Tests that an empty string is not accepted as a valid formula.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNoTokens_Invalid()
    {
        _ = new Formula(string.Empty);
    }

    // --- Tests for One Token Rule ---
    // --------------------------------

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
    public void FormulaConstructor_TestFirstTokenNumber_Valid()
    {
        _ = new Formula("1+1");
    }

    // --- Tests for  Last Token Rule ---

    // --- Tests for Parentheses/Operator Following Rule ---

    // --- Tests for Extra Following Rule ---
    // -----------------------------------------------------

    /// <summary>
    ///   <para>
    ///     A simple formula enclosed by open and closed parentheses.
    ///     The constructor should not throw an error.
    ///   </para>
    ///   <remarks>
    ///     This test ensures that a simple formula surrounded by a set of parentheses
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
    // ------------------------------------------------------

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
    // -----------------------------------

    /// <summary>
    ///   <para>
    ///     This test makes sure that formulas with unbalanced parentheses throws a FormulaFormatException.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestUnbalancedParenthesesRight_Invalid()
    {
        _ = new Formula("(x1))");
    }

    // --- Tests for Parenthesis Following Rule ---
    // --------------------------------------------

    /// <summary>
    /// This test ensures that two left unbalanced parentheses throws a FormulaFormatException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestUnbalancedParenthesesLeft_Invalid()
    {
        _ = new Formula("((x1)");
    }

    // --- Tests for Parenthesis Following Rule ---
    // --------------------------------------------

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

    // --- Tests for Parenthesis Following Rule ---
    // --------------------------------------------

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
    // ----------------------------------

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
    // ---------------------------------

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

    // --- Tests for Operator Following Rule ---
    // -----------------------------------------

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
    // --------------------------------------

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

    /// <summary>
    /// Tests that the number zero is a valid formula. [ExpectedException] is not used so the test should not throw an error.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestZero_Valid()
    {
        _ = new Formula("0");
    }

    // --- Tests for Valid Token Rule ---
    // ----------------------------------

    /// <summary>
    /// Tests that a non-zero number is a valid formula. [ExpectedException] is not used so the test should not throw an error.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestInteger_Valid()
    {
        _ = new Formula("7");
    }

    /// <summary>
    /// Tests that a decimal is a valid formula.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestDecimal_Valid()
    {
        _ = new Formula("0.1");
    }

    /// <summary>
    /// Tests that a number with an extraneous decimal (e.g. 1.) is considered a valid formula.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestExtraneousDecimal_Valid()
    {
        _ = new Formula("1.");
    }

    /// <summary>
    /// Tests that 0 in scientific notation is a valid formula.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestScientificNotationZero_Valid()
    {
        _ = new Formula("0.0e-0");
    }

    /// <summary>
    /// Tests that integers used in writing numbers in scientific notation is a valid formula (e.g. 7e7).
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestScientificNotationInteger_Valid()
    {
        _ = new Formula("7e7");
    }

    /// <summary>
    /// Tests that numbers with decimals that are included in writing numbers in scientific notation are considered valid formulas (e.g. 0.7e7).
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestScientificNotationDecimal_Valid()
    {
        _ = new Formula("0.7e7");
    }

    /// <summary>
    /// Tests that a number in scientific notation with a negative sign in the exponent is considered a valid formula.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestScientificNotationExponentNegative_Valid()
    {
        _ = new Formula("7e-7");
    }

    /// <summary>
    /// Tests that numbers in scientific notation do not contain decimals in their exponent.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestScientificNotationExponentDecimal_Invalid()
    {
        _ = new Formula("7e0.7");
    }

    /// <summary>
    /// This test passes a single letter without a number and should throw a FormulaFormatException.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableWithoutNumber_Invalid()
    {
        _ = new Formula("a");
    }

    /// <summary>
    /// Tests that the formula constructor throws a FormulaFormatException error if a number is seen before a letter (e.g. 1a).
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableNumberBeforeVariable_Invalid()
    {
        _ = new Formula("1a");
    }

    /// <summary>
    /// Tests that a letter does not follow a number in a variable (e.g. a1a).
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableAfterNumber_Invalid()
    {
        _ = new Formula("a1a");
    }

    /// <summary>
    /// Tests that letters followed by a number in the decimal form is not considered a valid formula (e.g. a1.0).
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableWithDecimal_Invalid()
    {
        _ = new Formula("a1.0");
    }

    /// <summary>
    /// Tests that a variable with uppercase lettered mixed with lower case letters is still considered a valid formula.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestVariableUppercaseMixed_Valid()
    {
        _ = new Formula("aAaA1");
    }

    /// <summary>
    /// Tests that valid variables that contain 'E' (the same letter used to denote numbers in scientific notation) <br/>
    /// is counted as a variable instead of a scientific notation formating error (e.g. e1).
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestVariablesContainingE_Valid()
    {
        _ = new Formula("e1");
    }

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

            // Expected exception
            catch (FormulaFormatException)
            {
            }
        }
    }

    /// <summary>
    /// This test passes a complex formula to the formula constructor that uses all operations, variables with differing attributes, <br/>
    /// integers, numbers with decimals, varying numbers in scientific notation, and parentheses throughout. <br/> <br/>
    /// The purpose of this test is to ensure that if a complex formula with a blanket of almost all interactions <br/>
    /// found in formulas passes, then simpler formula should also pass.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestComplexFormula_Valid()
    {
        _ = new Formula($"{ComplexFormula}");
    }

    // --- Tests for Parenthesis/Operator Following Rule ---

    // --- Tests for Extra Following Rule ---
    // ------------------------------------------------------

    /// <summary>
    /// This is a simple test which validates that the ToString method uppercases a variable correctly and returns it as a string.
    /// </summary>
    [TestMethod]
    public void ToString_TestVariable_Valid()
    {
        Formula formula = new("x1");
        Assert.AreEqual(formula.ToString(), "X1");
    }

    /// <summary>
    /// Test that ensures the ToString method correctly returns a formula with an operand and two variables.
    /// </summary>
    [TestMethod]
    public void ToString_TestFormulaOperation_Valid()
    {
        Formula formula = new("x1 + x2");
        Assert.AreEqual(formula.ToString(), "X1+X2");
    }

    /// <summary>
    /// Test which ensures the ToString method can properly handle parentheses with varying spaces between them.
    /// </summary>
    [TestMethod]
    public void ToString_TestParentheses_Valid()
    {
        Formula formula = new(" (  (x1+ 2  ) )");
        Assert.AreEqual(formula.ToString(), "((X1+2))");
    }

    /// <summary>
    /// Test that ensures the ToString method works properly on numbers in scientific notation by removing any unnecessary zeros after the decimal and capitalizing 'e'.
    /// </summary>
    [TestMethod]
    public void ToString_TestScientificNotation_Valid()
    {
        Formula formula = new("0.0e-0");
        Assert.AreEqual(formula.ToString(), "0");
    }

    /// <summary>
    /// This test checks that the ToString can properly recognize and eliminate any extraneous zeros in a number.
    /// </summary>
    [TestMethod]
    public void ToString_TestExtraneousZeros_Valid()
    {
        Formula formula = new("05 + 05.00 + 0.050 + 05.00e-0001");
        Assert.AreEqual(formula.ToString(), "5+5+0.05+0.5");
    }

    /// <summary>
    /// This test ensures that a number which contains a decimal point and no number to the right of the decimal point is properly handled.
    /// </summary>
    [TestMethod]
    public void ToString_TestExtraneousDecimalPoint_Valid()
    {
        Formula formula = new("01. + 1.");
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
        Formula formula = new($"{ComplexFormula}");
        Assert.AreEqual(formula.ToString(), $"{ComplexFormulaCanonical}");
    }

    /// <summary>
    /// This test ensures that two equivalent canonical string representations of a formula, which have been modified <br/>
    /// with extra spaces, different capitalizations, extraneous decimal points, and extraneous zeros are still canonically the same.
    /// </summary>
    [TestMethod]
    public void ToString_TestComplexFormulaEquivalent_Valid()
    {
        Formula equivalentFormula1 = new($"{ComplexFormulaEquivalentModified1}");
        Formula equivalentFormula2 = new($"{ComplexFormulaEquivalentModified2}");

        string equivalentFormula1ToString = equivalentFormula1.ToString();
        string equivalentFormula2ToString = equivalentFormula2.ToString();

        Assert.AreEqual(equivalentFormula1ToString, $"{ComplexFormulaCanonical}");
        Assert.AreEqual(equivalentFormula2ToString, $"{ComplexFormulaCanonical}");
    }

    /// <summary>
    /// Simple test that ensures the GetVariables method properly returns a single variable.
    /// </summary>
    [TestMethod]
    public void GetVariables_TestVariable_Valid()
    {
        Formula formula = new("x1");
        HashSet<string> variables = (HashSet<string>)formula.GetVariables();
        string variablesString = string.Join(",", variables);
        Assert.AreEqual(variablesString, "X1");
    }

    /// <summary>
    /// Test that ensures a formula with only numbers does not return any variables. <br/>
    /// Tests numbers that start out in scientific notation as well a number that will be converted into scientific notation via double.TryParse() (Which contains 'E').
    /// </summary>
    [TestMethod]
    public void GetVariables_TestNoVariables_Valid()
    {
        Formula formula = new("10E1 + 20 + 1.1 + 100000000000000000000000");
        HashSet<string> variables = (HashSet<string>)formula.GetVariables();
        string variablesString = string.Join(",", variables);
        Assert.AreEqual(variablesString, string.Empty);
    }

    /// <summary>
    /// Test that ensures no duplicates are returned by GetVariables.
    /// </summary>
    [TestMethod]
    public void GetVariables_TestNoDuplicates_Valid()
    {
        Formula formula = new("x2 + x1 + x1 + x2");
        HashSet<string> variables = (HashSet<string>)formula.GetVariables();
        string variablesString = string.Join(",", variables);
        Assert.AreEqual(variablesString, "X2,X1");
    }

    /// <summary>
    /// Test that ensures GetVariables can handle complex formulas with many different interactions between variables, numbers, operands, and parentheses.
    /// </summary>
    [TestMethod]
    public void GetVariables_TestComplexFormula_Valid()
    {
        Formula formula = new($"{ComplexFormula}");
        HashSet<string> variables = (HashSet<string>)formula.GetVariables();
        string variablesString = string.Join(",", variables);
        Assert.AreEqual(variablesString, "AA1,E2");
    }
}