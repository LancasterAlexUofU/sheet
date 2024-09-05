// <copyright file="Formula.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
//   <para>
//     This code is provides to start your assignment.  It was written
//     by Profs Joe, Danny, and Jim.  You should keep this attribution
//     at the top of your code where you have your header comment, along
//     with the other required information.
//   </para>
//
// Author:    Alex Lancaster
// Partner:   None
// Date:      30-Aug-2024
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
//
//    [... and of course you should describe the contents of the
//    file in broad terms here ...]
// </summary>
namespace CS3500.Formula;

using System.Text.RegularExpressions;
using System.Collections;

/// <summary>
///   <para>
///     This class represents formulas written in standard infix notation using standard precedence
///     rules.  The allowed symbols are non-negative numbers written using double-precision
///     floating-point syntax; variables that consist of one ore more letters followed by
///     one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///   </para>
///   <para>
///     Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
///     a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
///     and "x 23" consists of a variable "x" and a number "23".  Otherwise, spaces are to be removed.
///   </para>
///   <para>
///     For Assignment Two, you are to implement the following functionality:
///   </para>
///   <list type="bullet">
///     <item>
///        Formula Constructor which checks the syntax of a formula.
///     </item>
///     <item>
///        Get Variables
///     </item>
///     <item>
///        ToString
///     </item>
///   </list>
/// </summary>
public class Formula
{
    private string formulaString;

    /// <summary>
    ///     This regex matches the operands: [+, -, *, /].
    /// </summary>
    private const string OperandRegex = @"[\+\-*/]";

    /// <summary>
    ///     This regex matches the operands: [(, ), +, -, *, /].
    /// </summary>
    private const string OperandParenthesesRegex = @"[\(\)\+\-*/]";

    /// <summary>
    ///     All variables are letters followed by numbers.  This pattern
    ///     represents valid variable name strings. <br />
    ///     Matches one or more letters, upper or lowercase, followed by one or more numbers.
    /// </summary>
    private const string VariableRegexPattern = @"[a-zA-Z]+\d+";

    /// <summary>
    ///     \d+\.\d*: Matches numbers with a decimal point where the digits appear before the decimal, e.g., 123., 123.45. <br />
    ///     \d*\.\d+: Matches numbers with a decimal point where digits must appear after the decimal but may be absent before, e.g., .45, 0.45. <br />
    ///     (?: [eE][\+-]?\d+)?: Matches scientific notation. <br />
    /// </summary>
    private const string NumberRegexPattern = @"(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?";

    /// <summary>
    ///     The only tokens in the expression are (, ), +, -, *, /, valid variables, and valid numbers.
    /// </summary>
    private const string ValidTokens = $"{OperandParenthesesRegex}|{VariableRegexPattern}|{NumberRegexPattern}";

    /// <summary>
    ///     The first token of an expression must be a number, a variable, or an opening parenthesis.
    /// </summary>
    private const string FirstTokenRegex = $@"{NumberRegexPattern}|{VariableRegexPattern}|\(";

    /// <summary>
    ///     The last token of an expression must be a number, a variable, or a closing parenthesis.
    /// </summary>
    private const string LastTokenRegex = $@"{NumberRegexPattern}|{VariableRegexPattern}|\)";

    // Combination of First and Last Token Rules

    /// <summary>
    ///   Initializes a new instance of the <see cref="Formula"/> class.
    ///   <para>
    ///     Creates a Formula from a string that consists of an infix expression written as
    ///     described in the class comment.  If the expression is syntactically incorrect,
    ///     throws a FormulaFormatException with an explanatory Message.  See the assignment
    ///     specifications for the syntax rules you are to implement.
    ///   </para>
    ///   <para>
    ///     Non Exhaustive Example Errors:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///        Invalid variable name, e.g., x, x1x  (Note: x1 is valid, but would be normalized to X1)
    ///     </item>
    ///     <item>
    ///        Empty formula, e.g., string.Empty
    ///     </item>
    ///     <item>
    ///        Mismatched Parentheses, e.g., "(("
    ///     </item>
    ///     <item>
    ///        Invalid Following Rule, e.g., "2x+5"
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="formula"> The string representation of the formula to be created.</param>
    public Formula(string formula)
    {
        // --- Tests for One Token Rule ---
        if (formula == string.Empty)
        {
            throw new FormulaFormatException("Formula must not be empty");
        }

        // --- End of One Token Rule Test ---
        // ----------------------------------

        // --- Tests for Valid Token Rule ---
        List<string> tokens = GetTokens(formula);
        foreach (string token in tokens)
        {
            if (!Regex.IsMatch(token, ValidTokens))
            {
                throw new FormulaFormatException($"Formula may only contain (, ), +, -, *, /, valid variables, and valid numbers. {token} is not valid.");
            }
        }

        // --- End of Valid Token Rule ---
        // -------------------------------

        // --- Tests for Closing Parentheses, Balanced Parentheses Rule---
        // When an open parenthesis is seen, it is pushed to a stack.
        // Whenever a closed parenthesis is seen (assuming the stack is not empty), one parenthesis is popped from the stack.
        // If the stack of parentheses is empty after processing the formula, then the opening and closing parentheses are balanced.
        Stack parenthesesStack = new ();
        foreach (string token in tokens)
        {
            if (token == "(")
            {
                parenthesesStack.Push(token);
            }

            // If '(' is pushed on an empty stack, then the number of closing parentheses is greater than opening parenthesis at one point.
            if (token == ")" & parenthesesStack.Count == 0)
            {
                throw new FormulaFormatException("The number of closing parentheses is greater at one point than the number of opening parentheses.");
            }
            else if (token == ")")
            {
                parenthesesStack.Pop();
            }
        }

        if (parenthesesStack.Count != 0)
        {
            throw new FormulaFormatException("Formula has unbalanced number of parentheses.");
        }

        // --- End of Closing Parentheses, Balanced Parentheses Test ---
        // -------------------------------------------------------------

        // --- Test for First Token Rule
        if (!Regex.IsMatch(tokens[0], FirstTokenRegex))
        {
            throw new FormulaFormatException($"Formula is invalid with '{tokens[0]}' as the first character");
        }

        // --- End of First Token Test ---
        // -------------------------------

        // --- Test for Last Token Rule ---
        if (!Regex.IsMatch(tokens[^1], LastTokenRegex))
        {
            throw new FormulaFormatException($"Formula is invalid with '{tokens.Count - 1}' as the last character");
        }

        // --- End of Last Token Test ---
        // ------------------------------

        // --- Test for Parenthesis/Operator Following Rule ---
        for (int i = 0; i < tokens.Count - 1; i++)
        {
            // Statement checks for an opening parenthesis or a single operand.
            // If "^ $" is not included for OperandRegex, numbers in scientific notation would be mistakingly passed as an operand.
            if (tokens[i] == "(" || Regex.IsMatch(tokens[i], $"^{OperandRegex}$"))
            {
                if (!Regex.IsMatch(tokens[i + 1], FirstTokenRegex))
                {
                    throw new FormulaFormatException($"Formula is invalid with '{tokens[i + 1]}' following '{tokens[i]}.' Formula fails to meet Parenthesis/Operator Following Rule.");
                }
            }
        }

        // --- End of Parenthesis/Operator Following Test ---
        // --------------------------------------------------

        // --- Test for Extra Following Rule ---
        for (int i = 0; i < tokens.Count - 1; i++)
        {
            if (Regex.IsMatch(tokens[i], LastTokenRegex))
            {
                // Statement checks for the absence of a closing parenthesis or absence of an operand to make formula invalid.
                // Must check that OperandRegex is just one character using "^ $" as numbers in scientific notation would mistakingly be passed as an operand.
                if (tokens[i + 1] != ")" && !Regex.IsMatch(tokens[i + 1], $"^{OperandRegex}$"))
                {
                    throw new FormulaFormatException($"Formula is invalid with '{tokens[i + 1]}' following '{tokens[i]}.' Formula fails to meet Extra Following Rule.");
                }
            }
        }

        // --- End of Extra Following Rule Test ---
        // ----------------------------------------

        formulaString = formula;
    }

    /// <summary>
    ///   <para>
    ///     Returns a set of all the variables in the formula.
    ///   </para>
    ///   <remarks>
    ///     Important: no variable may appear more than once in the returned set, even
    ///     if it is used more than once in the Formula.
    ///   </remarks>
    ///   <para>
    ///     For example, if N is a method that converts all the letters in a string to upper case:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>new("x1+y1*z1").GetVariables() should enumerate "X1", "Y1", and "Z1".</item>
    ///     <item>new("x1+X1"   ).GetVariables() should enumerate "X1".</item>
    ///   </list>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables()
    {
        HashSet<string> variables = [];
        foreach (string variable in variables)
        {
            if (Regex.IsMatch(variable, VariableRegexPattern))
            {
                _ = variable.ToUpper();
                variables.Add(variable);
            }
        }

        return new HashSet<string>();
    }

    /// <summary>
    ///   <para>
    ///     Returns a string representation of a canonical form of the formula.
    ///   </para>
    ///   <para>
    ///     The string will contain no spaces.
    ///   </para>
    ///   <para>
    ///     If the string is passed to the Formula constructor, the new Formula f
    ///     will be such that this.ToString() == f.ToString().
    ///   </para>
    ///   <para>
    ///     All of the variables in the string will be normalized.  This
    ///     means capital letters.
    ///   </para>
    ///   <para>
    ///       For example:
    ///   </para>
    ///   <code>
    ///       new("x1 + y1").ToString() should return "X1+Y1"
    ///       new("X1 + 5.0000").ToString() should return "X1+5".
    ///   </code>
    ///   <para>
    ///     This code should execute in O(1) time.
    ///   <para>
    /// </summary>
    /// <returns>
    ///   A canonical version (string) of the formula. All "equal" formulas
    ///   should have the same value here.
    /// </returns>
    public override string ToString()
    {
        string formulaCanonicalString = this.formulaString;
        formulaCanonicalString = formulaCanonicalString.ToUpper();

        formulaCanonicalString.Replace(" ", string.Empty);

        // Leading Zeros
        string leadingZerosRegexPattern = @"(?<=\D|^)0+(?=\d)";
        // STILL EFFECTS TRAILING ZEROS, LOOK INTO
        Regex.Replace(formulaCanonicalString, leadingZerosRegexPattern, string.Empty);

        //Trailing zeros

        //extraneous decimal point


        return formulaCanonicalString;
    }

    /// <summary>
    ///   Reports whether "token" is a variable.  It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar(string token)
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable
        string standaloneVarPattern = $"^{VariableRegexPattern}$";
        return Regex.IsMatch(token, standaloneVarPattern);
    }

    /// <summary>
    ///   <para>
    ///     Given an expression, enumerates the tokens that compose it.
    ///   </para>
    ///   <para>
    ///     Tokens returned are:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>left paren</item>
    ///     <item>right paren</item>
    ///     <item>one of the four operator symbols</item>
    ///     <item>a string consisting of one or more letters followed by one or more numbers</item>
    ///     <item>a double literal</item>
    ///     <item>and anything that doesn't match one of the above patterns</item>
    ///   </list>
    ///   <para>
    ///     There are no empty tokens; white space is ignored (except to separate other tokens).
    ///   </para>
    /// </summary>
    /// <param name="formula"> A string representing an infix formula such as 1*B1/3.0. </param>
    /// <returns> The ordered list of tokens in the formula. </returns>
    private static List<string> GetTokens(string formula)
    {
        List<string> results = [];

        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format(
                                        "({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                        lpPattern,
                                        rpPattern,
                                        OperandRegex,
                                        VariableRegexPattern,
                                        NumberRegexPattern,
                                        spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                results.Add(s);
            }
        }

        return results;
    }
}

/// <summary>
///   Used to report syntax errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaFormatException"/> class.
    ///   <para>
    ///      Constructs a FormulaFormatException containing the explanatory message.
    ///   </para>
    /// </summary>
    /// <param name="message"> A developer defined message describing why the exception occurred.</param>
    public FormulaFormatException(string message)
        : base(message)
    {
        // All this does is call the base constructor. No extra code needed.
    }
}
