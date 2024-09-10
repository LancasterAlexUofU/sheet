namespace CS3500.DevelopmentTests;

using CS3500.DependencyGraph;

/// <summary>
///   This is a test class for DependencyGraphTest and is intended
///   to contain all DependencyGraphTest Unit Tests
/// </summary>
[TestClass]
public class DependencyGraphExampleStressTests
{
    /// <summary>
    ///   FIXME: Explain carefully what this code tests.
    ///          Also, update in-line comments as appropriate.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]  // 2 second run time limit <-- remove this comment
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
        Assert.AreEqual(dg1.GetDependees("b"), dg2.GetDependees("b"));
        Assert.AreEqual(dg1.GetDependents("b"), dg2.GetDependents("b"));
    }


    /// <summary>
    /// This takes a complex graph and tests that its size is correct, even when multiple nodes are removed.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestComplexGraphSize2()
    {
        DependencyGraph dg = createComplexGraph();
        Assert.IsTrue(dg.Size == 11);
        dg.RemoveDependency("g", "h");
        Assert.IsTrue(dg.Size == 9);
        dg.RemoveDependency("d", "b");
        Assert.IsTrue(dg.Size == 8); // Just ("a", "c"), ("e", "f") should be left.
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
    }

    /// <summary>
    /// This test checks that multiple nodes with dependents in a complex graph all return true.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestHasDependentsComplex_Valid()
    {
        DependencyGraph dg = createComplexGraph();
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
        DependencyGraph dg = createComplexGraph();
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
        Assert.IsTrue(dg.HasDependents("b"));
    }

    /// <summary>
    /// This tests creates a simple graph and checks that a dependent node does not return true for the HasDependeee method.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestHasDependees_Invalid()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        Assert.IsFalse(dg.HasDependents("a"));
    }


    /// <summary>
    /// This test checks that multiple nodes with dependees in a complex graph all return true.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestHasDependeesComplex_Valid()
    {
        DependencyGraph dg = createComplexGraph();
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
        DependencyGraph dg = createComplexGraph();
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
        CollectionAssert.AreEquivalent((System.Collections.ICollection)dg.GetDependents("a"), new List<string> { "b", "c" },
            $"dg.GetDependents returned {dg.GetDependents("a")} when it should have returned [\"b\", \"c\"].");
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
        DependencyGraph dg = createComplexGraph();
        CollectionAssert.AreEquivalent((System.Collections.ICollection)dg.GetDependents("b"), new List<string> { "c", "d", "e" });
        CollectionAssert.AreEquivalent((System.Collections.ICollection)dg.GetDependents("e"), new List<string> { "d", "f" });
    }

    /// <summary>
    /// This test method ensures that a complex graph with replaced dependent nodes properly returns the replaced nodes.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestGetDependentsComplexReplace()
    {
        DependencyGraph dg = createComplexGraph();
        dg.ReplaceDependents("b", ["j", "k", "l", "c"]);
        CollectionAssert.AreEquivalent((System.Collections.ICollection)dg.GetDependents("b"), new List<string> { "j", "k", "l", "c" });
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
        Assert.AreEqual(dg.GetDependees("b"), [], $"dg.GetDependees returned {dg.GetDependees("b")} when it should have returned [\"a\", \"c\"]."); 
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
        DependencyGraph dg = createComplexGraph();
        CollectionAssert.AreEquivalent((System.Collections.ICollection)dg.GetDependees("d"), new List<string> { "b", "e" });
        CollectionAssert.AreEquivalent((System.Collections.ICollection)dg.GetDependees("c"), new List<string> { "a", "b", "d" });
    }

    /// <summary>
    /// This test replaces dependees in a graph and verifies that the dependee nodes have been properly replaced.
    /// </summary>
    [TestMethod]
    public void DependencyGraph_TestGetDependeesComplexReplace()
    {
        DependencyGraph dg = createComplexGraph();
        dg.ReplaceDependees("c", ["j", "k", "l", "b"]);
        CollectionAssert.AreEquivalent((System.Collections.ICollection)dg.GetDependees("c"), new List<string> { "j", "k", "l", "b" });
    }

    /// <summary>
    /// This private function creates a complex graph. This graph has a disconnected part (g, h), (h, i).
    /// </summary>
    /// <returns>A complex dependency graph.</returns>
    private DependencyGraph createComplexGraph()
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