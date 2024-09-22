// <copyright file="DependencyGraphTests.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// <summary>
// Author:    Alex Lancaster
// Partner:   None
// Date:      13-Sept-2024
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
//      The DependencyGraphTests contains 28 tests including 2 stress tests. This test class contains
//      simple tests such as adding and removing, as well as edge cases and complex graphs. The tests
//      cover the DependencyGraph code 100%.
// </summary>
namespace CS3500.DevelopmentTests;

using CS3500.DependencyGraph;

/// <summary>
///   This is a test class for DependencyGraphTest and is intended
///   to contain all DependencyGraphTest Unit Tests.
/// </summary>
[TestClass]
public class DependencyGraphExampleStressTests
{
    /// <summary>
    ///   This test adds and removes many nodes in a dependency graph to ensure dependency graph is properly working and is efficient.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    public void StressTest()
    {
        DependencyGraph dg = new();

        // A bunch of strings to use
        const int SIZE = 200;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = string.Empty + ((char)('a' + i));
        }

        // The correct answers
        HashSet<string>[] dependents = new HashSet<string>[SIZE];
        HashSet<string>[] dependees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dependents[i] = [];
            dependees[i] = [];
        }

        // Add a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

        // Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 4; j < SIZE; j += 4)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

        // Add some back
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j += 2)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

        // Remove some more
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = i + 3; j < SIZE; j += 3)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

        // Make sure everything is right
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dependents[i].SetEquals(new HashSet<string>(dg.GetDependents(letters[i]))));
            Assert.IsTrue(dependees[i].SetEquals(new HashSet<string>(dg.GetDependees(letters[i]))));
        }
    }

    /// <summary>
    /// This stress test checks that the Replace dependents and Replace dependees are working properly as well as quickly.
    /// This is done by replacing the dependents for even indices and replacing dependees of odd indices.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    public void StressTest2()
    {
        DependencyGraph dg = new();

        // A bunch of strings to use
        const int SIZE = 200;
        string[] letters = new string[SIZE];

        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = string.Empty + ((char)('a' + i));
        }

        // The correct answers
        HashSet<string>[] dependents = new HashSet<string>[SIZE];
        HashSet<string>[] dependees = new HashSet<string>[SIZE];

        for (int i = 0; i < SIZE; i++)
        {
            dependents[i] = [];
            dependees[i] = [];
        }

        // Add initial dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

        // Replace dependents for even indices
        for (int i = 0; i < SIZE; i += 2)
        {
            // Create new dependents
            var newDependents = new HashSet<string>();
            for (int j = i + 1; j < SIZE; j += 2)
            {
                newDependents.Add(letters[j]);
            }

            // Replace old dependents with new dependents
            dg.ReplaceDependents(letters[i], newDependents);
            dependents[i] = newDependents;

            // Check to see if a member is in newDependents, then dependees at the index should have it, otherwise, remove it
            for (int j = i + 1; j < SIZE; j++)
            {
                if (newDependents.Contains(letters[j]))
                {
                    dependees[j].Add(letters[i]);
                }
                else
                {
                    dependees[j].Remove(letters[i]);
                }
            }
        }

        // Replace dependees for odd indices
        for (int i = 1; i < SIZE; i += 2)
        {
            var newDependees = new HashSet<string>();
            for (int j = 0; j < i; j += 2)
            {
                newDependees.Add(letters[j]);
            }

            dg.ReplaceDependees(letters[i], newDependees);
            dependees[i] = newDependees;

            // Only from 0 to i because a letter can only be a dependee of letters that come after it in the array.
            // Check to see if a member is in newDependees, then dependents at the index should have it, otherwise, remove it
            for (int j = 0; j < i; j++)
            {
                if (newDependees.Contains(letters[j]))
                {
                    dependents[j].Add(letters[i]);
                }
                else
                {
                    dependents[j].Remove(letters[i]);
                }
            }
        }

        // Make sure everything is right
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dependents[i].SetEquals(new HashSet<string>(dg.GetDependents(letters[i]))));
            Assert.IsTrue(dependees[i].SetEquals(new HashSet<string>(dg.GetDependees(letters[i]))));
        }

        // Check size
        int expectedSize = dependents.Sum(d => d.Count);
        Assert.AreEqual(expectedSize, dg.Size);
    }

    /// <summary>
    /// This tests creates an empty graph to make sure the graph size is equal to zero.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestSizeZero()
    {
        DependencyGraph dg = new();
        Assert.IsTrue(dg.Size == 0, $"Dependency Graph size is {dg.Size} when it should be 0.");
    }

    /// <summary>
    /// This test checks the .Size method is properly working. This is the simpler test case and doesn't contain any duplicates, removals, or replacements.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestSimpleSize()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        Assert.IsTrue(dg.Size == 2, $"Dependency Graph size is {dg.Size} when it should be 2.");
    }

    /// <summary>
    /// This test checks the .Size method against a more complex graph with duplicates. This test method does not remove or replace any nodes.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestComplexSizeNoRemove()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "b"); // Duplicate node, shouldn't be included in graph
        dg.AddDependency("b", "c");
        dg.AddDependency("b", "c"); // Duplicate node, shouldn't be included in graph
        dg.AddDependency("b", "d");
        dg.AddDependency("e", "c");
        dg.AddDependency("c", "f");
        dg.AddDependency("d", "f");
        dg.AddDependency("d", "g");
        dg.AddDependency("g", "c");
        Assert.IsTrue(dg.Size == 8, $"Dependency Graph size is {dg.Size} when it should be 8.");
    }

    /// <summary>
    /// This test creates a graph while using RemoveDependency, ReplaceDependents, and ReplaceDependees to check that the graph size is accurate.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestComplexSizeRemoveAndReplace()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.RemoveDependency("a", "b");
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        dg.AddDependency("b", "d");
        dg.ReplaceDependents("b", ["e", "f"]);
        dg.ReplaceDependees("b", ["g", "h"]);

        // Final graph is: (g,b), (h,b), (b,e), (b,f)
        Assert.IsTrue(dg.Size == 4, $"Dependency Graph size is {dg.Size} when it should be 4.");
    }

    /// <summary>
    /// This test ensures that two graphs which are formed differently but in the end are equivalent are equal to each other.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestComplexSizeTwoGraphs()
    {
        DependencyGraph dg1 = new();
        dg1.AddDependency("a", "b");
        dg1.RemoveDependency("a", "b");
        dg1.AddDependency("a", "b");
        dg1.AddDependency("b", "c");
        dg1.AddDependency("b", "d");
        dg1.ReplaceDependents("b", ["e", "f"]);
        dg1.ReplaceDependees("b", ["g", "h"]);

        // Final graph is: (g,b), (h,b), (b,e), (b,f)
        DependencyGraph dg2 = new();
        dg2.AddDependency("g", "b");
        dg2.AddDependency("h", "b");
        dg2.AddDependency("b", "e");
        dg2.AddDependency("b", "f");
        CollectionAssert.AreEquivalent(dg1.GetDependees("b").ToList(), dg2.GetDependees("b").ToList());
        CollectionAssert.AreEquivalent(dg1.GetDependents("b").ToList(), dg2.GetDependents("b").ToList());
    }

    /// <summary>
    /// This takes a complex graph and tests that its size is correct, even when multiple nodes are removed.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestComplexGraphSize2()
    {
        DependencyGraph dg = CreateComplexGraph();
        Assert.IsTrue(dg.Size == 11, $"Actual dg.Size is {dg.Size}");
        dg.RemoveDependency("g", "h");
        Assert.IsTrue(dg.Size == 10, $"Actual dg.Size is {dg.Size}");
        dg.RemoveDependency("b", "d");
        Assert.IsTrue(dg.Size == 9, $"Actual dg.Size is {dg.Size}");
    }

    /// <summary>
    /// This test ensures that an empty dependency graph returns false for HasDependents.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestHasDependentsZeroGraph_Invalid()
    {
        DependencyGraph dg = new();
        Assert.IsFalse(dg.HasDependents(string.Empty));
    }

    /// <summary>
    /// This test creates a simple graph and checks that HasDependents returns true.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestHasDependents_Valid()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        Assert.IsTrue(dg.HasDependents("a"));
    }

    /// <summary>
    /// This tests creates a simple graph and checks that a dependee does not return true for the HasDependents method.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestHasDependents_Invalid()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        Assert.IsFalse(dg.HasDependents("b"));
        Assert.IsFalse(dg.HasDependents("c"));
    }

    /// <summary>
    /// This test checks that multiple nodes with dependents in a complex graph all return true.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestHasDependentsComplex_Valid()
    {
        DependencyGraph dg = CreateComplexGraph();
        Assert.IsTrue(dg.HasDependents("b"));
        Assert.IsTrue(dg.HasDependents("d"));
        Assert.IsTrue(dg.HasDependents("h"));
    }

    /// <summary>
    /// This test checks that multiple nodes without dependent nodes in a complex graph all return false.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestHasDependentsComplex_Invalid()
    {
        DependencyGraph dg = CreateComplexGraph();
        Assert.IsFalse(dg.HasDependents("c"));
        Assert.IsFalse(dg.HasDependents("f"));
        Assert.IsFalse(dg.HasDependents("i"));
    }

    /// <summary>
    /// This test ensures that an empty dependency graph returns false for HasDependees.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestHasDependeesZeroGraph_Invalid()
    {
        DependencyGraph dg = new();
        Assert.IsFalse(dg.HasDependees(string.Empty));
    }

    /// <summary>
    /// This test creates a simple graph and checks that HasDependees returns true.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestHasDependees_Valid()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        Assert.IsTrue(dg.HasDependees("b"));
    }

    /// <summary>
    /// This tests creates a simple graph and checks that a dependent node does not return true for the HasDependeee method.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestHasDependees_Invalid()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        Assert.IsFalse(dg.HasDependees("a"));
        Assert.IsFalse(dg.HasDependees("c"));
    }

    /// <summary>
    /// This test checks that multiple nodes with dependees in a complex graph all return true.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestHasDependeesComplex_Valid()
    {
        DependencyGraph dg = CreateComplexGraph();
        Assert.IsTrue(dg.HasDependees("b"));
        Assert.IsTrue(dg.HasDependees("c"));
        Assert.IsTrue(dg.HasDependees("i"));
    }

    /// <summary>
    /// This test checks that nodes without dependees in a complex graph all return false.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestHasDependeesComplex_Invalid()
    {
        DependencyGraph dg = CreateComplexGraph();
        Assert.IsFalse(dg.HasDependees("a"));
        Assert.IsFalse(dg.HasDependees("g"));
    }

    /// <summary>
    /// This test checks that an empty string in an empty graph returns an empty list for GetDependents.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestGetDependentsEmpty()
    {
        DependencyGraph dg = new();
        Assert.AreEqual(dg.GetDependents(string.Empty), [], $"dg.GetDependents returned {dg.GetDependents(string.Empty)} when it should have returned [].");
    }

    /// <summary>
    /// This test ensures that all the dependent nodes are returned for the GetDependents method and that implied relationships are not included. <br/>
    /// e.g. (a,b), (a,c), (b,d); if a is the target node, even through there is a link from a to d through b, their relationship is implied and should not be returned.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestGetDependents()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        dg.AddDependency("b", "d");
        CollectionAssert.AreEquivalent(dg.GetDependents("a").ToList(), new List<string> { "b", "c" }, $"dg.GetDependents returned {dg.GetDependents("a")} when it should have returned [\"b\", \"c\"].");
    }

    /// <summary>
    /// This test ensures that if the GetDependents method is given a node with no dependent nodes, it returns an empty list.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestGetDependentsZero()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        dg.AddDependency("b", "d");
        Assert.AreEqual(dg.GetDependents("d"), [], $"dg.GetDependents returned {dg.GetDependents("d")} when it should have returned [].");
    }

    /// <summary>
    /// This tests ensures that a complex graph returns all its proper dependent nodes.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestGetDependentsComplex()
    {
        DependencyGraph dg = CreateComplexGraph();
        CollectionAssert.AreEquivalent(dg.GetDependents("b").ToList(), new List<string> { "c", "d", "e" });
        CollectionAssert.AreEquivalent(dg.GetDependents("e").ToList(), new List<string> { "d", "f" });
    }

    /// <summary>
    /// This test method ensures that a complex graph with replaced dependent nodes properly returns the replaced nodes.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestGetDependentsComplexReplace()
    {
        DependencyGraph dg = CreateComplexGraph();
        dg.ReplaceDependents("b", ["j", "k", "l", "c"]);
        CollectionAssert.AreEquivalent(dg.GetDependents("b").ToList(), new List<string> { "j", "k", "l", "c" });
    }

    /// <summary>
    /// This test checks that an empty string in an empty graph returns an empty list for GetDependees.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestGetDependeesEmpty()
    {
        DependencyGraph dg = new();
        Assert.AreEqual(dg.GetDependees(string.Empty), [], $"dg.GetDependees returned {dg.GetDependees(string.Empty)} when it should have returned [].");
    }

    /// <summary>
    /// This test checks that a node with two dependees properly returns those two nodes using the GetDependees method.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestGetDependees()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("c", "b");

        // Assert.AreEqual(dg.GetDependees("b"), ["a", "c"]);
        CollectionAssert.AreEquivalent(dg.GetDependees("b").ToList(), new List<string> { "a", "c" }, $"dg.GetDependees returned {dg.GetDependees("b")} when it should have returned [\"a\", \"c\"].");
    }

    /// <summary>
    /// Tests that node with no dependees returns an empty list.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestGetDependeesZero()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        Assert.AreEqual(dg.GetDependees("a"), [], $"dg.GetDependees returned {dg.GetDependees("a")} when it should have returned [].");
    }

    /// <summary>
    /// This test ensures that nodes with dependee nodes properly returns all dependee nodes.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestGetDependeesComplex()
    {
        DependencyGraph dg = CreateComplexGraph();
        CollectionAssert.AreEquivalent(dg.GetDependees("d").ToList(), new List<string> { "b", "e" });
        CollectionAssert.AreEquivalent(dg.GetDependees("c").ToList(), new List<string> { "a", "b", "d" });
    }

    /// <summary>
    /// This test replaces dependees in a graph and verifies that the dependee nodes have been properly replaced.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestGetDependeesComplexReplace()
    {
        DependencyGraph dg = CreateComplexGraph();
        dg.ReplaceDependees("c", ["j", "k", "l", "b", "f"]);
        CollectionAssert.AreEquivalent(dg.GetDependees("c").ToList(), new List<string> { "j", "k", "l", "b", "f" }, $"GetDependees ToList actually {dg.GetDependees("c").ToList()}");
    }

    /// <summary>
    /// This test checks that whenever elements that are not in a graph (or are not a dependent node)
    /// are called to be replaced, no error should be thrown.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestReplaceDependees_Invalid()
    {
        DependencyGraph dg = CreateComplexGraph();
        dg.ReplaceDependees("a", ["j", "k", "l", "b", "f"]); // 'a' is not a dependent node, but should not throw an error
        CollectionAssert.AreEquivalent(dg.GetDependees("a").ToList(), new List<string> { "j", "k", "l", "b", "f" }, $"GetDependees ToList actually {dg.GetDependees("a").ToList()}");

        dg.GetDependees("z"); // 'z' does not exist
        dg.ReplaceDependees("z", ["j", "k", "l", "b", "f"]);
        CollectionAssert.AreEquivalent(dg.GetDependees("z").ToList(), new List<string> { "j", "k", "l", "b", "f" }, $"GetDependees ToList actually {dg.GetDependees("z").ToList()}");
    }

    /// <summary>
    /// This test checks that whenever elements that are not in a graph (or are not a dependee node)
    /// are called to be replaced, no error should be thrown.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestReplaceDependents_Invalid()
    {
        DependencyGraph dg = CreateComplexGraph();
        dg.ReplaceDependents("c", ["j", "k", "l", "b", "f"]); // 'c' is not a dependent node, but should not throw an error
        CollectionAssert.AreEquivalent(dg.GetDependents("c").ToList(), new List<string> { "j", "k", "l", "b", "f" }, $"GetDependees ToList actually {dg.GetDependents("c").ToList()}");

        dg.GetDependents("z"); // 'z' does not exist
        dg.ReplaceDependents("z", ["j", "k", "l", "b", "f"]);
        CollectionAssert.AreEquivalent(dg.GetDependents("z").ToList(), new List<string> { "j", "k", "l", "b", "f" }, $"GetDependees ToList actually {dg.GetDependents("z").ToList()}");
    }

    /// <summary>
    /// This method tests that the replace, remove, and add functions all work together.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestReplaceAndRemove()
    {
        DependencyGraph dg = CreateComplexGraph();
        dg.ReplaceDependees("c", ["j", "k", "l", "b"]);
        dg.RemoveDependency("a", "c");
        dg.RemoveDependency("b", "c");
        dg.RemoveDependency("j", "c");
        dg.AddDependency("a", "c");
        dg.RemoveDependency("c", "p"); // Send invalid node, check that no error is thrown
        CollectionAssert.AreEquivalent(dg.GetDependees("c").ToList(), new List<string> { "a", "k", "l" }, $"GetDependees ToList actually {dg.GetDependees("c").ToList()}");
    }

    /// <summary>
    /// This private function creates a complex graph. This graph has a disconnected part (g, h), (h, i).
    /// </summary>
    /// <returns>A complex dependency graph.</returns>
    private DependencyGraph CreateComplexGraph()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        dg.AddDependency("b", "c");
        dg.AddDependency("b", "d");
        dg.AddDependency("b", "e");
        dg.AddDependency("d", "c");
        dg.AddDependency("d", "f");
        dg.AddDependency("e", "d");
        dg.AddDependency("e", "f");
        dg.AddDependency("g", "h");
        dg.AddDependency("h", "i");

        return dg;
    }
}