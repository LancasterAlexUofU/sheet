// <copyright file="Formula.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
//   <para>
//     This code is provided for you to start your assignment.  It was written
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
//      The Formula class handles formulas in infix formula notation, given as a string,
//      and ensures that the formula has the proper syntax of a formula.
//      If a formula does not have proper syntax, a FormulaFormatException is thrown.
//
//      The Formula class also normalizes all variables, numbers, and removes spaces,
//      to make a canonical string representation of the formula.
//
//      The Formula class further includes a GetVariables function, which returns a set
//      (no duplicates) of all variables in a formula, and ToString, which returns a
//      canonical string representation of the formula.
//
//      The Formula class contains an Evaluator, which takes in syntactically
//      correct formulas and returns a result double based on the operators and numbers.
//      Variable values are found using lookup, and lookup returns a double value.
//
//      There are overrides for ==, !=, .Equals, and GetHashCode so that Formulas can use this syntax.
// </summary>
namespace CS3500.Formula;

using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

/// <summary>
///   <para>
///     This class represents formulas written in standard infix notation using standard precedence
///     rules.  The allowed symbols are non-negative numbers written using double-precision
///     floating-point syntax; variables that consist of one ore more letters followed by
///     one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///   </para>
/// </summary>
public class Formula
{
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

    /// <summary>
    /// Defines the string representation of an operation and the corresponding operation on two numbers.
    /// </summary>
    private Dictionary<string, Func<double, double, double>> operations = new()
    {
        { "+", (a, b) => a + b },
        { "-", (a, b) => a - b },
        { "*", (a, b) => a * b },
        { "/", (a, b) => a / b },
    };

    private List<string> tokens = [];

    private Stack<double> values = [];

    private Stack<string> operators = []; // Includes '(', ')'

    private string formula = string.Empty;

    /// <summary>
    ///   Initializes a new instance of the <see cref="Formula"/> class.
    ///   <para>
    ///     Creates a Formula from a string that consists of an infix expression written as
    ///     described in the class comment.  If the expression is syntactically incorrect,
    ///     throws a FormulaFormatException with an explanatory Message.
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
    /// <param name="inputFormula"> The string representation of the formula to be created.</param>
    public Formula(string inputFormula)
    {
        formula = inputFormula;
        tokens = GetTokens(formula);

        OneTokenRule();
        ValidTokenRule();
        ClosingBalancedParenthesesRule();
        FirstTokenRule();
        LastTokenRule();
        ParenthesisOperatingFollowingRule();
        ExtraFollowingRule();

        NumberParse();
        LetterUpper();
    }

    /// <summary>
    ///   Any method meeting this type signature can be used for
    ///   looking up the value of a variable.  In general the expected behavior is that
    ///   the Lookup method will "know" about all variables in a formula
    ///   and return their appropriate value.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///   If a variable name is provided that is not recognized by the implementing method,
    ///   then the method should throw an ArgumentException.
    /// </exception>
    /// <param name="variableName">
    ///   The name of the variable (e.g., "A1") to lookup.
    /// </param>
    /// <returns> The value of the given variable (if one exists). </returns>
    public delegate double Lookup(string variableName);

    /// <summary>
    ///   <para>
    ///     Reports whether f1 == f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are the same.</returns>
    public static bool operator ==(Formula f1, Formula f2)
    {
        if (f1.Equals(f2))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///   <para>
    ///     Reports whether f1 != f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are not equal to each other.</returns>
    public static bool operator !=(Formula f1, Formula f2)
    {
        if (f1 == f2)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///   <para>
    ///     Determines if two formula objects represent the same formula.
    ///   </para>
    ///   <para>
    ///     By definition, if the parameter is null or does not reference
    ///     a Formula Object then return false.
    ///   </para>
    ///   <para>
    ///     Two Formulas are considered equal if their canonical string representations
    ///     (as defined by ToString) are equal.
    ///   </para>
    /// </summary>
    /// <param name="obj"> The other object.</param>
    /// <returns>
    ///   True if the two objects represent the same formula.
    /// </returns>
    public override bool Equals(object? obj)
    {
        // If the canonical version of the strings are equal, then the formulas are equal.
        if (this.ToString().Equals(obj?.ToString()))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///   <para>
    ///     Evaluates this Formula, using the lookup delegate to determine the values of
    ///     variables.
    ///   </para>
    ///   <remarks>
    ///     When the lookup method is called, it will always be passed a Normalized (capitalized)
    ///     variable name.  The lookup method will throw an ArgumentException if there is
    ///     not a definition for that variable token.
    ///   </remarks>
    ///   <para>
    ///     If no undefined variables or divisions by zero are encountered when evaluating
    ///     this Formula, the numeric value of the formula is returned.  Otherwise, a
    ///     FormulaError is returned (with a meaningful explanation as the Reason property).
    ///   </para>
    ///   <para>
    ///     This method should never throw an exception.
    ///   </para>
    /// </summary>
    /// <param name="lookup">
    ///   <para>
    ///     Given a variable symbol as its parameter, lookup returns the variable's (double) value
    ///     (if it has one) or throws an ArgumentException (otherwise).  This method should expect
    ///     variable names to be capitalized.
    ///   </para>
    /// </param>
    /// <returns> Either a double or a formula error, based on evaluating the formula.</returns>
    public object Evaluate(Lookup lookup)
    {
        Formula formula = this;
        List<string> tokens = this.tokens;

        for (int i = 0; i < tokens.Count; i++)
        {
            string token = tokens[i];

            switch (token)
            {
                // Checks if token is a number
                case string when IsNum(token):

                    double tokenDub = Convert.ToDouble(token);

                    // Detects whether an FormulaError object was returned. If so, return it in evaluation.
                    object valueOrError = TokenNumberStackEvaluation(tokenDub);
                    if (valueOrError.GetType() == typeof(FormulaError))
                    {
                        return valueOrError;
                    }

                    break;

                // Checks if token is a variable
                case string when IsVar(token):

                    double value = lookup(token);
                    TokenNumberStackEvaluation(value);
                    break;

                // Checks if token is '+' or '-'
                case string when IsPlusOrMinus(token):

                    TokenPlusOrMinusEvaluation(token);
                    break;

                // Checks if token is '*' or '/'
                case string when IsMultOrDiv(token):

                    operators.Push(token);
                    break;

                // Checks if token is '('
                case string when IsOpenParen(token):

                    operators.Push(token);
                    break;

                // Checks if token is ')'
                case string when IsClosedParen(token):

                    // Detects whether an FormulaError object was returned. If so, return it in evaluation.
                    valueOrError = TokenClosedParenEvaluation();
                    if (valueOrError.GetType() == typeof(FormulaError))
                    {
                        return valueOrError;
                    }

                    break;
            }
        }

        // If operators stack is not empty when the last token has been processed,
        // it will be either a '+' or '-'. Do the following.
        if (operators.TryPeek(out string? resultPlusMin))
        {
            if (IsPlusOrMinus(resultPlusMin))
            {
                double topValue = values.Pop();
                double secondTopValue = values.Pop();
                string op = operators.Pop();

                return operations[op](secondTopValue, topValue);
            }
        }

        // Otherwise, there will only be a single value which is the result.
        return values.Pop();
    }

    /// <summary>
    ///   <para>
    ///     Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    ///     case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
    ///     randomly-generated unequal Formulas have the same hash code should be extremely small.
    ///   </para>
    /// </summary>
    /// <returns> The hashcode for the object. </returns>
    public override int GetHashCode()
    {
        // string canonicalFormula = formula.ToString();
        return this.ToString().GetHashCode();
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

        foreach (string token in tokens)
        {
            if (IsVar(token))
            {
                variables.Add(token);
            }
        }

        return variables;
    }

    /// <summary>
    ///   <para>
    ///     Returns a string representation of a canonical form of the formula.
    ///   </para>
    ///   <para>
    ///     The string will contain no spaces.
    ///   </para>
    ///   <para>
    ///     All of the variables in the string will be normalized. This
    ///     means capital letters.
    ///   </para>
    ///   <para>
    ///       For example:
    ///   </para>
    ///   <code>
    ///       new("x1 + y1").ToString() should return "X1+Y1"
    ///       new("X1 + 5.0000").ToString() should return "X1+5".
    ///   </code>
    /// </summary>
    /// <returns>
    ///   A canonical version (string) of the formula. All "equal" formulas
    ///   should have the same value here.
    /// </returns>
    public override string ToString()
    {
        return string.Join(string.Empty, tokens);
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
    /// Reports whether "token" is a number. This can be a number with or without decimals
    /// and numbers in scientific notation.
    /// </summary>
    /// <param name="token">A token that may be a number.</param>
    /// <returns>true if the string matches the requirement for a number.</returns>
    private static bool IsNum(string token)
    {
        return Regex.IsMatch(token, $@"^{NumberRegexPattern}$");
    }

    /// <summary>
    /// Reports whether token is either a '+' or '-'.
    /// </summary>
    /// <param name="token">A token that may be a '+' or '-'.</param>
    /// <returns>True if the string contains a single '+' or '-'.</returns>
    private static bool IsPlusOrMinus(string token)
    {
        return Regex.IsMatch(token, @"^[\+\-]$");
    }

    /// <summary>
    /// Reports whether token is either a '*' or '/'.
    /// </summary>
    /// <param name="token">A token that may be a '*' or '/'.</param>
    /// <returns>True if the string contains a single '*' or '/'.</returns>
    private static bool IsMultOrDiv(string token)
    {
        return Regex.IsMatch(token, @"^[*/]$");
    }

    /// <summary>
    /// Reports whether token is '('.
    /// </summary>
    /// <param name="token">A token that may be '('.</param>
    /// <returns>True if the string contains a single '('.</returns>
    private static bool IsOpenParen(string token)
    {
        return Regex.IsMatch(token, @"^\($");
    }

    /// <summary>
    /// Reports whether token is ')'.
    /// </summary>
    /// <param name="token">A token that may be ')'.</param>
    /// <returns>True if the string contains a single ')'.</returns>
    private static bool IsClosedParen(string token)
    {
        return Regex.IsMatch(token, @"^\)$");
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

    /// <summary>
    /// This method computes the logic for pushing numbers onto the stack for evaluator. <br/>
    /// The logic is the following: <br/>
    /// If * or / is at the top of the operator stack, pop the value stack, pop the operator stack, <br/>
    /// and apply the popped operator to the popped number and t. <br/>
    /// Push the result onto the value stack. Otherwise, push t onto the value stack.
    /// </summary>
    /// <param name="token">A double that may have operations applied to it.</param>
    /// <returns>The top value of stack values in all cases except division by zero, which returns a FormulaError Object.</returns>
    private object TokenNumberStackEvaluation(double token)
    {
        if (operators.TryPeek(out string? resultMultDiv))
        {
            if (IsMultOrDiv(resultMultDiv))
            {
                // Pops the top value, operand, applies the operation on token and topValue, and pushes the result to values
                double topValue = values.Pop();
                string op = operators.Pop();

                // Return FormulaError if division by zero is caught.
                if (token == 0 && op == "/")
                {
                    return new FormulaError("Division by zero is not allowed.");
                }

                values.Push(operations[op](topValue, token));
            }

            // Needed as if the first if statement is true, the other "outside" else statement wouldn't execute
            else
            {
                values.Push(token);
            }
        }

        // If there is no '*' or '/', push token onto value stack
        else
        {
            values.Push(token);
        }

        // Added so all paths return
        return values.Peek();
    }

    /// <summary>
    /// This method computes the logic for pushing operands '+' and '-' onto the stack for evaluation. <br/>
    /// The logic is the following: <br/>
    /// If + or - is at the top of the operator stack, pop the value stack twice and the operator stack once, <br/>
    /// then apply the popped operator to the popped numbers, then push the result onto the value stack. <br/>
    /// Push t onto the operator stack.
    /// </summary>
    /// <param name="token">A string that is either '+' or '-'.</param>
    private void TokenPlusOrMinusEvaluation(string token)
    {
        if(operators.TryPeek(out string? resultPlusMin))
        {
            if (IsPlusOrMinus(resultPlusMin))
            {
                double topValue = values.Pop();
                double secondTopValue = values.Pop();
                string op = operators.Pop();

                // Second value first so that subtraction is in the correct order.
                values.Push(operations[op](secondTopValue, topValue));
            }
        }

        operators.Push(token);
    }

    /// <summary>
    /// This method computes the logic for pushing ')' onto the stack for evaluation. <br/> <br/>
    /// The logic is the following, in order: <br/>
    /// (1) If '+' or '-' is at the top of the operator stack, pop the value stack twice and the operator stack once. <br/>
    /// Apply the popped operator to the popped numbers. Push the result onto the value stack. <br/> <br/>
    /// (2) The top of the operator stack should be a '('. Pop it. <br/> <br/>
    /// (3) If '*' or '/' is at the top of the operator stack, pop the value stack twice and the operator stack once. <br/>
    /// Apply the popped operator to the popped numbers. Push the result onto the value stack. <br/>
    /// </summary>
    /// <param name="token">A single closed parenthesis ')'.</param>
    /// <returns>The top value of stack values in all cases except division by zero, which returns a FormulaError Object.</returns>
    private object TokenClosedParenEvaluation()
    {
        // Checks for '+' or '-'
        if (operators.TryPeek(out string? resultPlusMin))
        {
            if (IsPlusOrMinus(resultPlusMin))
            {
                double topValue = values.Pop();
                double secondTopValue = values.Pop();
                string op = operators.Pop();

                values.Push(operations[op](secondTopValue, topValue));
            }
        }

        operators.Pop(); // Should be a '('

        // Checks for '*' or '/'
        if (operators.TryPeek(out string? resultMultDiv))
        {
            if (IsMultOrDiv(resultMultDiv))
            {
                double topValue = values.Pop();
                double secondTopValue = values.Pop();
                string op = operators.Pop();

                // Checks for division by zero
                if (topValue == 0 && op == "/")
                {
                    return new FormulaError("Division by zero is not allowed.");
                }

                // Second is first so that order of division is correct
                values.Push(operations[op](secondTopValue, topValue));
            }
        }

        return values.Peek();
    }

    /// <summary>
    /// Tests for the One Token Rule: <br/>
    /// There must be at least one token.
    /// </summary>
    /// <exception cref="FormulaFormatException">Exception throw if formula is empty.</exception>
    private void OneTokenRule()
    {
        // Checks for zero or more spaces
        if (Regex.IsMatch(formula, @"^\s*$"))
        {
            throw new FormulaFormatException("Formula must not be empty");
        }
    }

    /// <summary>
    /// Tests for the Valid Token Rule: <br/>
    /// The only tokens in the expression are (, ), +, -, *, /, variables, and numbers.
    /// </summary>
    /// <exception cref="FormulaFormatException">Exception is thrown if a token is not a valid token.</exception>
    private void ValidTokenRule()
    {
        // --- Tests for Valid Token Rule ---
        foreach (string token in tokens)
        {
            if (!Regex.IsMatch(token, ValidTokens))
            {
                throw new FormulaFormatException($"Formula may only contain (, ), +, -, *, /, valid variables, and valid numbers. {token} is not valid.");
            }
        }
    }

    /// <summary>
    /// Tests for the Closing Parentheses and Balanced Parentheses Rule: <br/> <br/>
    /// Closed Parentheses Rule: <br/>
    /// When reading tokens from left to right, at no point should the number of closing parentheses seen so far <br/>
    /// be greater than the number of opening parentheses seen so far. <br/> <br/>
    /// Balanced Parentheses Rule: <br/>
    /// The total number of opening parentheses must equal the total number of closing parentheses.
    /// </summary>
    /// <exception cref="FormulaFormatException">
    /// Exception is thrown if The number of closing parentheses is greater at one point than the number of opening parentheses or if <br/>
    /// the Formula has an unbalanced number of parentheses.
    /// </exception>
    private void ClosingBalancedParenthesesRule()
    {
        // When an open parenthesis is seen, it is pushed to a stack.
        // Whenever a closed parenthesis is seen (assuming the stack is not empty), one parenthesis is popped from the stack.
        // If the stack of parentheses is empty after processing the formula, then the opening and closing parentheses are balanced.
        Stack parenthesesStack = new();
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
    }

    /// <summary>
    /// This tests the First Token Rule: <br/>
    /// The first token of an expression must be a number, a variable, or an opening parenthesis.
    /// </summary>
    /// <exception cref="FormulaFormatException">Exception is thrown whenever the first character is not a number, a variable, or an opening parenthesis.</exception>
    private void FirstTokenRule()
    {
        if (!Regex.IsMatch(tokens[0], FirstTokenRegex))
        {
            throw new FormulaFormatException($"Formula is invalid with '{tokens[0]}' as the first character");
        }
    }

    /// <summary>
    /// This tests the Last Token Rule: <br/>
    /// The last token of an expression must be a number, a variable, or a closing parenthesis.
    /// </summary>
    /// <exception cref="FormulaFormatException">Exception is thrown when the last character is not a number, a variable, or a closing parenthesis.</exception>
    private void LastTokenRule()
    {
        if (!Regex.IsMatch(tokens[^1], LastTokenRegex))
        {
            throw new FormulaFormatException($"Formula is invalid with '{tokens.Count - 1}' as the last character");
        }
    }

    /// <summary>
    /// Tests for the Parenthesis/Operator Following Rule: <br/>
    /// Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis.
    /// </summary>
    /// <exception cref="FormulaFormatException">Exception is thrown if a number, a variable, or an opening parenthesis is not seen after an opening parenthesis or an operator.</exception>
    private void ParenthesisOperatingFollowingRule()
    {
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
    }

    /// <summary>
    /// Tests for the Extra Following Rule: <br/>
    /// Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis.
    /// </summary>
    /// <exception cref="FormulaFormatException">Exception is thrown if a number, a variable, or a closing parenthesis is not followed by an operator or a closing parenthesis. </exception>
    private void ExtraFollowingRule()
    {
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
    }

    /// <summary>
    /// This method ensures that numbers are in canonical form (e.g. 3.0 -> 3).
    /// </summary>
    private void NumberParse()
    {
        for (int i = 0; i < tokens.Count; i++)
        {
            if (double.TryParse(tokens[i], out double tokenDouble))
            {
                // If the token is a number, it is converted and parsed as a double, converted back to a string, and placed back in tokens.
                tokens[i] = tokenDouble.ToString();
            }
        }
    }

    /// <summary>
    /// This method uppercases all tokens and stores them in the tokens list.
    /// </summary>
    private void LetterUpper()
    {
        for (int i = 0; i < tokens.Count; i++)
        {
            if (Regex.IsMatch(tokens[i], @"[a-z]"))
            {
                tokens[i] = tokens[i].ToUpper();
            }
        }
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

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public class FormulaError
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaError"/> class.
    ///   <para>
    ///     Constructs a FormulaError containing the explanatory reason.
    ///   </para>
    /// </summary>
    /// <param name="message"> Contains a message for why the error occurred.</param>
    public FormulaError(string message)
    {
        Reason = message;
    }

    /// <summary>
    ///  Gets the reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}
