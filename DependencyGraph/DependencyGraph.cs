// <copyright file="DependencyGraph.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
//
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
//      The DependencyGraph class takes in two elements (in this case cells), and determine
//      which cells must be evaluated before other cells. This is done by storing
//      the dependee and dependent cells in separate dictionaries so that dependent cells
//      point to dependee cells and vice versa. This creates a unique dependency graph.
//      Cells can be added, removed, and replaced.
// </summary>

// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta
// Version 1.3 - H. James de St. Germain Fall 2024
// (Clarified meaning of dependent and dependee.)
// (Clarified names in solution/project structure.)
namespace CS3500.DependencyGraph;
using System.Collections;

/// <summary>
///   <para>
///     (s1,t1) is an ordered pair of strings, meaning t1 depends on s1.
///     (in other words: s1 must be evaluated before t1.)
///   </para>
///   <para>
///     A DependencyGraph can be modeled as a set of ordered pairs of strings.
///     Two ordered pairs (s1,t1) and (s2,t2) are considered equal if and only
///     if s1 equals s2 and t1 equals t2.
///   </para>
///   <remarks>
///     Recall that sets never contain duplicates.
///     If an attempt is made to add an element to a set, and the element is already
///     in the set, the set remains unchanged.
///   </remarks>
///   <para>
///     Given a DependencyGraph DG:
///   </para>
///   <list type="number">
///     <item>
///       If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
///       (The set of things that depend on s.)
///     </item>
///     <item>
///       If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
///       (The set of things that s depends on.)
///     </item>
///   </list>
///   <para>
///      For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}.
///   </para>
///   <code>
///     dependents("a") = {"b", "c"}
///     dependents("b") = {"d"}
///     dependents("c") = {}
///     dependents("d") = {"d"}
///     dependees("a")  = {}
///     dependees("b")  = {"a"}
///     dependees("c")  = {"a"}
///     dependees("d")  = {"b", "d"}
///   </code>
/// </summary>
public class DependencyGraph
{
    private Dictionary<string, HashSet<string>> dependees;
    private Dictionary<string, HashSet<string>> dependents;

    private int size;

    /// <summary>
    ///   Initializes a new instance of the <see cref="DependencyGraph"/> class.
    ///   The initial DependencyGraph is empty.
    /// </summary>
    public DependencyGraph()
    {
        this.dependees = [];
        this.dependents = [];
    }

    /// <summary>
    /// Gets the number of ordered pairs in the DependencyGraph.
    /// </summary>
    public int Size
    {
        // This counts all the elements in each list and then sums each count.
        // get { return graph.Values.Sum(set => set.Count); }
        get { return this.size; }
    }

/// <summary>
///   Reports whether the given node has dependents (i.e., other nodes depend on it).
/// </summary>
/// <param name="nodeName"> The name of the node.</param>
/// <returns> true if the node has dependents. </returns>
    public bool HasDependents(string nodeName)
    {
        // If the node is a key in dependees, then that means it has at least one dependent node
        return this.dependees.ContainsKey(nodeName);
    }

    /// <summary>
    ///   Reports whether the given node has dependees (i.e., depends on one or more other nodes).
    /// </summary>
    /// <returns> true if the node has dependees.</returns>
    /// <param name="nodeName">The name of the node.</param>
    public bool HasDependees(string nodeName)
    {
        // If the node is a key in dependents, then that means it has at least one dependee node
        return this.dependents.ContainsKey(nodeName);
    }

    /// <summary>
    ///   <para>
    ///     Returns the dependents of the node with the given name.
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependents of nodeName. </returns>
    public IEnumerable<string> GetDependents(string nodeName)
    {
        // Checks that dependees contains nodeName and if so, stores values in set values
        if (this.dependees.TryGetValue(nodeName, out HashSet<string>? values))
        {
            return values;
        }

        // Returns empty if nodeName is not found
        return [];
    }

    /// <summary>
    ///   <para>
    ///     Returns the dependees of the node with the given name.
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependees of nodeName. </returns>
    public IEnumerable<string> GetDependees(string nodeName)
    {
        // Checks that dependents contains nodeName and if so, stores values in set values
        if (this.dependents.TryGetValue(nodeName, out HashSet<string>? values))
        {
            return values;
        }

        // Returns empty if nodeName is not found
        return [];
    }

    /// <summary>
    /// <para>
    ///   Adds the ordered pair (dependee, dependent), if it doesn't already exist (otherwise nothing happens).
    /// </para>
    /// <para>
    ///   This can be thought of as: dependee must be evaluated before dependent.
    /// </para>
    /// </summary>
    /// <param name="dependee"> The name of the node that must be evaluated first. </param>
    /// <param name="dependent"> The name of the node that cannot be evaluated until after the other node has been. </param>
    public void AddDependency(string dependee, string dependent)
    {
        bool sizeIncreased = false;

        // Creates a new key slot for any new dependee value
        if (!this.dependees.ContainsKey(dependee))
        {
            this.dependees[dependee] = [];
        }

        if (this.dependees[dependee].Add(dependent))
        {
            sizeIncreased = true;
        }

        // Creates a new key slot for any new dependent value
        if (!this.dependents.ContainsKey(dependent))
        {
            this.dependents[dependent] = [];
        }

        if (this.dependents[dependent].Add(dependee))
        {
            sizeIncreased = true;
        }

        if (sizeIncreased)
        {
            this.size++;
        }
    }

    /// <summary>
    ///   <para>
    ///     Removes the ordered pair (dependee, dependent), if it exists (otherwise nothing happens).
    ///   </para>
    /// </summary>
    /// <param name="dependee"> The name of the node that must be evaluated first. </param>
    /// <param name="dependent"> The name of the node that cannot be evaluated until the other node has been. </param>
    public void RemoveDependency(string dependee, string dependent)
    {
        // Check if values exist, otherwise, errors will be thrown
        if (this.dependees.ContainsKey(dependee) && this.dependents.ContainsKey(dependent))
        {
            // True if element is found and removed in both dictionaries, otherwise false (key isn't found).
            if (this.dependees[dependee].Remove(dependent) && this.dependents[dependent].Remove(dependee))
            {
                this.size--;
            }

            // If there are no dependent nodes for a given dependee, then it should no longer be considered a dependee and should be removed.
            if (this.dependees[dependee].Count == 0)
            {
                this.dependees.Remove(dependee);
            }

            // If there are no dependee nodes for a given dependent node, then it should no longer be considered dependent and should be removed.
            if (this.dependents[dependent].Count == 0)
            {
                this.dependents.Remove(dependent);
            }
        }
    }

    /// <summary>
    ///   Removes all existing ordered pairs of the form (nodeName, *).  Then, for each
    ///   t in newDependents, adds the ordered pair (nodeName, t).
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependents are being replaced. </param>
    /// <param name="newDependents"> The new dependents for nodeName. </param>
    public void ReplaceDependents(string nodeName, IEnumerable<string> newDependents)
    {
        // If the nodeName is a valid dependee, remove each of its dependent values.
        if (this.dependees.ContainsKey(nodeName))
        {
            foreach(string dependent in this.dependees[nodeName])
            {
                RemoveDependency(nodeName, dependent);
            }
        }

        // Add all new dependents to dependee.
        foreach(string newDependent in newDependents)
        {
            AddDependency(nodeName, newDependent);
        }
    }

    /// <summary>
    ///   <para>
    ///     Removes all existing ordered pairs of the form (*, nodeName).  Then, for each
    ///     t in newDependees, adds the ordered pair (t, nodeName).
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependees are being replaced. </param>
    /// <param name="newDependees"> The new dependees for nodeName. Could be empty.</param>
    public void ReplaceDependees(string nodeName, IEnumerable<string> newDependees)
    {
        // If the nodeName is a valid dependee, remove each of its dependent values.
        if (this.dependents.ContainsKey(nodeName))
        {
            foreach (string dependee in this.dependents[nodeName])
            {
                RemoveDependency(dependee, nodeName);
            }
        }

        // Add all new dependents to dependee.
        foreach (string newDependee in newDependees)
        {
            AddDependency(newDependee, nodeName);
        }
    }
}
