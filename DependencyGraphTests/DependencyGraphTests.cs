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

    [TestMethod]
    public void DependencyGraph_TestComplexSizeRemoveAndReplace()
    {
        DependencyGraph dg = new();
        // Implement :)

    }
    // Test HasDependents
    // Test HasDependees
    // Test GetDependents
    // Test GetDependees
    // Test AddDependency
    // Test RemoveDependency
    // Test ReplaceDependents
    // Test ReplaceDependees
}