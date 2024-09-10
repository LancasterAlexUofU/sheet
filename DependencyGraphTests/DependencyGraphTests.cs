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
        Assert.AreEqual(dg.GetDependents("a"), ["b", "c"], $"dg.GetDependents returned {dg.GetDependents("a")} when it should have returned [\"b\", \"c\"].");
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

    // Test GetDependees
    // Test AddDependency
    // Test RemoveDependency
    // Test ReplaceDependents
    // Test ReplaceDependees
}