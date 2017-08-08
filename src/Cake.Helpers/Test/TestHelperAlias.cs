using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Helpers.Tasks;

namespace Cake.Helpers.Test
{
  /// <summary>
  /// Cake Generic Test Task Aliases
  /// </summary>
  [CakeAliasCategory("Helper")]
  [CakeAliasCategory("Test")]
  public static class TestHelperAlias
  {
    #region Static Members

    /// <summary>
    /// Gets Test Clean Task for defined parameters
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <param name="taskName">Target Name</param>
    /// <param name="testCategory">Test Category (Unit, System, etc)</param>
    /// <param name="isTarget">True=Public Target(Listed),False=Private Target(Not Listed)</param>
    /// <param name="parentTaskName">Parent Target name. Required if isTarget is False</param>
    /// <returns>Task</returns>
    /// <example>
    /// <code>
    /// // Creates task "Clean-Test-All"
    /// // Creates task "Clean-Unit-All"
    /// // Creates task "Clean-Unit-Sln"
    /// TestCleanTask("Sln", "Unit", true)
    ///   .Does(() => { "Clean Solution" });
    /// </code>
    /// </example>
    [CakeMethodAlias]
    public static CakeTaskBuilder<ActionTask> TestCleanTask(
      this ICakeContext context,
      string taskName,
      string testCategory,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return context.TaskHelper()
        .AddToTestCleanTask(taskName, testCategory, isTarget, parentTaskName)
        .GetTaskBuilder();
    }

    /// <summary>
    /// Gets Test Task for defined parameters
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <param name="taskName">Target Name</param>
    /// <param name="testCategory">Test Category (Unit, System, etc)</param>
    /// <param name="isTarget">True=Public Target(Listed),False=Private Target(Not Listed)</param>
    /// <param name="parentTaskName">Parent Target name. Required if isTarget is False</param>
    /// <returns>Task</returns>
    /// <example>
    /// <code>
    /// // Creates task "Test-All"
    /// // Creates task "Test-Unit-All"
    /// // Creates task "Test-Unit-Sln"
    /// TestTask("Sln", "Unit", true)
    ///   .Does(() => { "Test Solution" });
    /// </code>
    /// </example>
    [CakeMethodAlias]
    public static CakeTaskBuilder<ActionTask> TestTask(
      this ICakeContext context,
      string taskName,
      string testCategory,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return context.TaskHelper()
        .AddToTestTask(taskName, testCategory, isTarget, parentTaskName)
        .GetTaskBuilder();
    }

    #endregion
  }
}