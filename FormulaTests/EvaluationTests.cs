// <copyright file="EvaluationTests.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
//
// Author:    Alex Lancaster
// Partner:   None
// Date:      14-Sept-2024
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
//      FIXME: Add file contents!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// </summary>

namespace CS3500.Formula;
using CS3500.Formula;
using System.Text.RegularExpressions;

/// <summary>
/// This test class ensures that formulas are properly evaluated and return mathematically correct results.
/// This class will never expect the Formula class to throw an error.
/// The only time a formula should be syntactically correct but unevaluable is if zero is in the denominator
/// or if there is an unknown variable. In these cases, a FormulaError Object (not an exception, just returned).
/// </summary>
[TestClass]
public class EvaluationTests
{
    private const string NumberRegexPattern = @"(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?";

    private const string ComplexFormulaCanonical = "((AA1+E2)/0)*(0-30)+0.1-50-(5)/(3/AA1)+1.2345678910111213E+20+1E-13";

    private const string ComplexFormulaEquivalentModified1 = "  ((  Aa1+E2) /0.0)*(0.E-0 -3e1)+ 00.10 - 050. - (50E-1)/(3/Aa1) +0000123456789101112131415+0.00000000000010000";
    private const string ComplexFormulaEquivalentModified2 = "((aA1+E2)/0)*(0.0E-00-3e1)+0.1-50-(050.E-1)/(3./Aa1) + 00123456789101112131415.00+00.000000000000100";

    private const string ComplexFormulaEquivalentModifiedNumbersOnly1 = "  ((  10+15) /0.0)*(0.E-0 -3e1)+ 00.10 - 050. - (50E-1)/(3/10) +0000123456789101112131415+0.00000000000010000"; // AA1 = 10, E2 = 15
    private const string ComplexFormulaEquivalentModifiedNumbersOnly2 = "((10+15)/0)*(0.0E-00-3e1)+0.1-50-(050.E-1)/(3./10) + 00123456789101112131415.00+00.000000000000100";

    private const string ComplexFormulaEquivalentModifiedVariablesOnly1 = "  ((  A1+B1) /C1)*(D1 -E1)+ F1 - G1 - (H1)/(I1/A1) +J1+K1";
    private const string ComplexFormulaEquivalentModifiedVariablesOnly2 = "((A1+B1)/C1)*(D1-E1)+F1-G1-(H1)/(I1/A1) + J1+K1";

    /// <summary>
    /// This method adds three numbers together and checks that their sum is correct.
    /// </summary>
    [TestMethod]
    public void Evaluator_NumAddition()
    {
        Formula f = new("5 + 10 + 15");
        Formula.Lookup lookup = (string s) => Convert.ToDouble(s);
        Assert.AreEqual(f.Evaluate(lookup), 30);
    }

    /// <summary>
    /// This method subtracts three numbers, with the result being negative.
    /// </summary>
    [TestMethod]
    public void Evaluator_NumSubtraction_ResultNegative()
    {
        Formula f = new("5 - 10 - 15");
        Formula.Lookup lookup = (string s) => Convert.ToDouble(s);
        Assert.AreEqual(f.Evaluate(lookup), -20);
    }

    /// <summary>
    /// This method subtracts three numbers, with the result being positive.
    /// </summary>
    [TestMethod]
    public void Evaluator_NumSubtraction_ResultPositive()
    {
        Formula f = new("20 - 10 - 5");
        Formula.Lookup lookup = (string s) => Convert.ToDouble(s);
        Assert.AreEqual(f.Evaluate(lookup), 5);
    }

    /// <summary>
    /// This method multiplies three numbers and checks that their product is correct.
    /// </summary>
    [TestMethod]
    public void Evaluator_NumMultiply()
    {
        Formula f = new("5 * 5 * 5");
        Formula.Lookup lookup = (string s) => Convert.ToDouble(s);
        Assert.AreEqual(f.Evaluate(lookup), 125);
    }

    /// <summary>
    /// This method divides three numbers and checks that their quotient is correct.
    /// </summary>
    [TestMethod]
    public void Evaluator_NumDivide()
    {
        Formula f = new("625 / 5 / 5");
        Formula.Lookup lookup = (string s) => Convert.ToDouble(s);
        Assert.AreEqual(f.Evaluate(lookup), 25);
    }

    /// <summary>
    /// This method uses all numbers and all operations to form a complex formula. <br/>
    /// There is multiplication in the front and back of the equation to check for PEMDAS.
    /// </summary>
    [TestMethod]
    public void Evaluator_NumComplexFormula()
    {
        Formula f = new("(5 * 5) / 5 + 5 - 10 + 5 - (10 - 5) * 5");
        Formula.Lookup lookup = (string s) => Convert.ToDouble(s);
        Assert.AreEqual(f.Evaluate(lookup), -20);
    }

    /// <summary>
    /// This method adds 3 variables to ensures variables are evaluated correctly.
    /// </summary>
    [TestMethod]
    public void Evaluator_VarAddition()
    {
        Formula f = new("A1 + B1 + C1");
        Formula.Lookup lookup = (string s) => 5; // Sets A1, B1, C1 equal to 5.
        Assert.AreEqual(f.Evaluate(lookup), 15);
    }

    /// <summary>
    /// This method subtracts 3 variables to ensures variables are evaluated correctly.
    /// </summary>
    [TestMethod]
    public void Evaluator_VarSubtraction()
    {
        Formula f = new("A1 - B1 - C1");
        Formula.Lookup lookup = (string s) => 5; // Sets A1, B1, C1 equal to 5.
        Assert.AreEqual(f.Evaluate(lookup), -5);
    }

    /// <summary>
    /// This method multiplies 3 variables to ensures variables are evaluated correctly.
    /// </summary>
    [TestMethod]
    public void Evaluator_VarMultiplication()
    {
        Formula f = new("A1 * B1 * C1");
        Formula.Lookup lookup = (string s) => 5; // Sets A1, B1, C1 equal to 5.
        Assert.AreEqual(f.Evaluate(lookup), 125);
    }

    /// <summary>
    /// This method divides 3 variables to ensures variables are evaluated correctly.
    /// </summary>
    [TestMethod]
    public void Evaluator_VarDivision()
    {
        Formula f = new("A1 / B1 / C1");
        Formula.Lookup lookup = (string s) => 5; // Sets A1, B1, C1 equal to 5.
        Assert.AreEqual(f.Evaluate(lookup), 0.2);
    }

    /// <summary>
    /// This method uses all variables and all operations to form a complex formula. <br/>
    /// There is multiplication in the front and back of the equation to check for PEMDAS.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if unknown variable if found in equation.</exception>
    [TestMethod]
    public void Evaluator_VarComplexFormula()
    {
        Formula f = new("(A1 * A1) / A1 + A1 - B1 + A1 - (B1 - A1) * A1");
        Formula.Lookup lookup = (string s) =>
        {
            switch (s)
            {
                case "A1": return 5;
                case "B1": return 10;
                default: throw new ArgumentException($"Unknown variable: {s}");
            }
        };
        Assert.AreEqual(f.Evaluate(lookup), -20);
    }

    /// <summary>
    /// This method adds 2 variables and a number to ensures variables and numbers can both be evaluated correctly.
    /// </summary>
    [TestMethod]
    public void Evaluator_NumVarAddition()
    {
        Formula f = new("A1 + 5 + B1");
        Formula.Lookup lookup = (string s) => 5; // Sets A1, B1 equal to 5.
        Assert.AreEqual(f.Evaluate(lookup), 15);
    }

    /// <summary>
    /// This method subtracts 2 variables and a number to ensures variables and numbers can both be evaluated correctly.
    /// </summary>
    [TestMethod]
    public void Evaluator_NumVarSubtraction()
    {
        Formula f = new("A1 - 5 - B1");
        Formula.Lookup lookup = (string s) => 5; // Sets A1, B1 equal to 5.
        Assert.AreEqual(f.Evaluate(lookup), -5);
    }

    /// <summary>
    /// This method multiplies 2 variables and a number to ensures variables and numbers can both be evaluated correctly.
    /// </summary>
    [TestMethod]
    public void Evaluator_NumVarMultiplication()
    {
        Formula f = new("A1 * 5 * B1");
        Formula.Lookup lookup = (string s) => 5; // Sets A1, B1 equal to 5.
        Assert.AreEqual(f.Evaluate(lookup), 125);
    }

    /// <summary>
    /// This method divides 2 variables and a number to ensures variables and numbers can both be evaluated correctly.
    /// </summary>
    [TestMethod]
    public void Evaluator_NumVarDivision()
    {
        Formula f = new("A1 / 5 / B1");
        Formula.Lookup lookup = (string s) => 5; // Sets A1, B1 equal to 5.
        Assert.AreEqual(f.Evaluate(lookup), 0.2);
    }

    /// <summary>
    /// This method uses variables, numbers, and all operations to form a complex formula. <br/>
    /// There is multiplication in the front and back of the equation to check for PEMDAS.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if unknown variable if found in equation.</exception>
    [TestMethod]
    public void Evaluator_NumVarComplexFormula()
    {
        Formula f = new("(A1 * 5) / A1 + 5 - B1 + 5 - (10 - A1) * A1");
        Formula.Lookup lookup = (string s) =>
        {
            // Check for a number first before a variable so default switch case isn't triggered
            if (Regex.IsMatch(NumberRegexPattern, s))
            {
                return Convert.ToDouble(s);
            }

            switch (s)
            {
                case "A1": return 5;
                case "B1": return 10;
                default: throw new ArgumentException($"Unknown variable: {s}");
            }
        };
        Assert.AreEqual(f.Evaluate(lookup), -20);
    }

    /// <summary>
    /// This method takes two equivalent complex formulas comprised of only numbers, and checks whether they are equal. <br/>
    /// Each formula has an equivalent canonical formula, but the string representation is not equal (extra spaces, more leading zeros, etc.).
    /// </summary>
    [TestMethod]
    public void Equals_NumComplexFormula()
    {
        Formula equivalentFormula1 = new($"{ComplexFormulaEquivalentModifiedNumbersOnly1}");
        Formula equivalentFormula2 = new($"{ComplexFormulaEquivalentModifiedNumbersOnly2}");
        Assert.IsTrue(equivalentFormula1 == equivalentFormula2);
    }

    /// <summary>
    /// This method takes two equivalent complex formulas comprised of only variables, and checks whether they are equal. <br/>
    /// Each formula has an equivalent canonical formula, but the string representation is not equal (extra spacing).
    /// </summary>
    [TestMethod]
    public void Equals_VarComplexFormula()
    {
        Formula equivalentFormula1 = new($"{ComplexFormulaEquivalentModifiedVariablesOnly1}");
        Formula equivalentFormula2 = new($"{ComplexFormulaEquivalentModifiedVariablesOnly2}");
        Assert.IsTrue(equivalentFormula1 == equivalentFormula2);
    }

    /// <summary>
    /// This method takes two equivalent complex formulas comprised of numbers and variables, and checks whether they are equal. <br/>
    /// Each formula has an equivalent canonical formula, but the string representation is not equal (extra spaces, more leading zeros, etc.).
    /// </summary>
    [TestMethod]
    public void Equals_NumVarComplexFormula()
    {
        Formula equivalentFormula1 = new($"{ComplexFormulaEquivalentModified1}");
        Formula equivalentFormula2 = new($"{ComplexFormulaEquivalentModified2}");
        Assert.IsTrue(equivalentFormula1 == equivalentFormula2);
    }

    /// <summary>
    /// This method is nearly the same as Equals_NumComplexFormula, but checks that != returns false for a true statement for a number-only formula.
    /// </summary>
    [TestMethod]
    public void NotEquals_NumComplexFormula_False()
    {
        Formula equivalentFormula1 = new($"{ComplexFormulaEquivalentModifiedNumbersOnly1}");
        Formula equivalentFormula2 = new($"{ComplexFormulaEquivalentModifiedNumbersOnly2}");
        Assert.IsFalse(equivalentFormula1 != equivalentFormula2);
    }

    /// <summary>
    /// This method is nearly the same as Equals_VarComplexFormula, but checks that != returns false for a true statement for a variable-only formula.
    /// </summary>
    [TestMethod]
    public void NotEquals_VarComplexFormula_False()
    {
        Formula equivalentFormula1 = new($"{ComplexFormulaEquivalentModifiedVariablesOnly1}");
        Formula equivalentFormula2 = new($"{ComplexFormulaEquivalentModifiedVariablesOnly2}");
        Assert.IsFalse(equivalentFormula1 != equivalentFormula2);
    }

    /// <summary>
    /// This method is nearly the same as Equals_NumVarComplexFormula, but checks that != returns false for a true statement for a number-variable formula.
    /// </summary>
    [TestMethod]
    public void NotEquals_NumVarComplexFormula_False()
    {
        Formula equivalentFormula1 = new($"{ComplexFormulaEquivalentModified1}");
        Formula equivalentFormula2 = new($"{ComplexFormulaEquivalentModified2}");
        Assert.IsFalse(equivalentFormula1 != equivalentFormula2);
    }

    /// <summary>
    /// This method checks that 2 number-only formulas that are not equal returns true for the not equal method.
    /// </summary>
    [TestMethod]
    public void NotEquals_NumComplexFormula_True()
    {
        Formula equivalentFormula1 = new($"{ComplexFormulaEquivalentModifiedNumbersOnly1}+1");
        Formula equivalentFormula2 = new($"{ComplexFormulaEquivalentModifiedNumbersOnly1}");
        Assert.IsTrue(equivalentFormula1 != equivalentFormula2);
    }

    /// <summary>
    /// This method checks that 2 variable-only formulas that are not equal returns true for the not equal method.
    /// </summary>
    [TestMethod]
    public void NotEquals_VarComplexFormula_True()
    {
        Formula equivalentFormula1 = new($"{ComplexFormulaEquivalentModifiedVariablesOnly1}+A1");
        Formula equivalentFormula2 = new($"{ComplexFormulaEquivalentModifiedVariablesOnly1}");
        Assert.IsTrue(equivalentFormula1 != equivalentFormula2);
    }

    /// <summary>
    /// This method checks that 2 number-variable formulas that are not equal returns true for the not equal method.
    /// </summary>
    [TestMethod]
    public void NotEquals_NumVarComplexFormula_True()
    {
        Formula equivalentFormula1 = new($"{ComplexFormulaEquivalentModified1}+1");
        Formula equivalentFormula2 = new($"{ComplexFormulaEquivalentModified1}");
        Assert.IsTrue(equivalentFormula1 != equivalentFormula2);
    }

    /// <summary>
    /// This test checks that two negative numbers multiplied together equals the correct positive number.
    /// </summary>
    [TestMethod]
    public void Evaluator_NegativeNumberMultiply()
    {
        Formula f = new("(0-10) * (0-10)");
        Formula.Lookup lookup = (string s) => Convert.ToDouble(s);
        Assert.AreEqual(f.Evaluate(lookup), 100);
    }

    /// <summary>
    /// This test checks that a FormulaError is thrown if a division by 0 occurs in the denominator.
    /// </summary>
    [TestMethod]
    public void Evaluator_DivideByZero_Invalid()
    {
        Formula f = new("1/0");
        Formula.Lookup lookup = (string s) => Convert.ToDouble(s);
        Assert.IsInstanceOfType(f.Evaluate(lookup), typeof(FormulaError));
    }

    /// <summary>
    /// Tests that two strings, even with different string representations of the same formula, have the same hash code.
    /// </summary>
    [TestMethod]
    public void GetHashCode_HashEqual_Valid()
    {
        Formula equivalentFormula1 = new($"{ComplexFormulaEquivalentModified1}");
        Formula equivalentFormula2 = new($"{ComplexFormulaEquivalentModified2}");
        Assert.IsTrue(equivalentFormula1 == equivalentFormula2);
        Assert.IsTrue(equivalentFormula1.GetHashCode() == equivalentFormula2.GetHashCode());
    }

    /// <summary>
    /// This tests that two distinctly different formulas return false for the equals method.
    /// </summary>
    [TestMethod]
    public void GetHashCode_HashEqual_Invalid()
    {
        Formula equivalentFormula1 = new($"{ComplexFormulaEquivalentModified1}");
        Formula equivalentFormula2 = new($"{ComplexFormulaEquivalentModified2}+1");
        Assert.IsFalse(equivalentFormula1 == equivalentFormula2);
        Assert.IsFalse(equivalentFormula1.GetHashCode() == equivalentFormula2.GetHashCode());
    }

    /// <summary>
    /// Tests that two distinctly different formulas do not return the same hash code.
    /// </summary>
    [TestMethod]
    public void GetHashCode_HashNotEqual_Valid()
    {
        Formula equivalentFormula1 = new($"{ComplexFormulaEquivalentModified1}");
        Formula equivalentFormula2 = new($"{ComplexFormulaEquivalentModified2}+1");
        Assert.IsTrue(equivalentFormula1 != equivalentFormula2);
        Assert.IsTrue(equivalentFormula1.GetHashCode() != equivalentFormula2.GetHashCode());
    }

    /// <summary>
    /// Tests that two identical formulas return false for the not equals method.
    /// </summary>
    [TestMethod]
    public void GetHashCode_HashNotEqual_Invalid()
    {
        Formula equivalentFormula1 = new($"{ComplexFormulaEquivalentModified1}");
        Formula equivalentFormula2 = new($"{ComplexFormulaEquivalentModified2}");
        Assert.IsFalse(equivalentFormula1 != equivalentFormula2);
        Assert.IsFalse(equivalentFormula1.GetHashCode() != equivalentFormula2.GetHashCode());
    }
}