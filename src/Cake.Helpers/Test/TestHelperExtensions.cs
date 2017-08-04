using System;
using System.Diagnostics.CodeAnalysis;
using Cake.Core;
using Cake.Helpers.Clean;
using Cake.Helpers.Tasks;

namespace Cake.Helpers.Test
{
  public static class TestHelperExtensions
  {
    #region Constants

    private const string TargetCategory = "Test";

    #endregion

    #region Static Members

    public static IHelperTask AddToTestCleanTask(
      this ITaskHelper helper,
      string targetName,
      string testCategory,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (string.IsNullOrWhiteSpace(targetName))
        throw new ArgumentNullException(nameof(targetName));

      if (string.IsNullOrWhiteSpace(testCategory))
        throw new ArgumentNullException(nameof(testCategory));

      if (!isTarget && string.IsNullOrWhiteSpace(parentTaskName))
        throw new ArgumentNullException(nameof(parentTaskName));

      var newTaskName = isTarget ? targetName : $"{parentTaskName}-{targetName}";
      var parentTask = isTarget
        ? helper.GetTestCleanTask(testCategory)
        : helper.AddToTestCleanTask(parentTaskName, testCategory);
      var newTask = helper.GetTestCleanTask(testCategory, newTaskName, isTarget);

      parentTask.GetTaskBuilder()
        .IsDependentOn(newTask.TaskName);

      return newTask;
    }

    public static IHelperTask AddToTestTask(
      this ITaskHelper helper,
      string targetName,
      string testCategory,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (string.IsNullOrWhiteSpace(targetName))
        throw new ArgumentNullException(nameof(targetName));

      if (string.IsNullOrWhiteSpace(testCategory))
        throw new ArgumentNullException(nameof(testCategory));

      if (!isTarget && string.IsNullOrWhiteSpace(parentTaskName))
        throw new ArgumentNullException(nameof(parentTaskName));

      var newTaskName = isTarget ? targetName : $"{parentTaskName}-{targetName}";
      var parentTask = isTarget
        ? helper.GetTestTask(testCategory)
        : helper.AddToTestTask(parentTaskName, testCategory);
      var newTask = helper.GetTestTask(testCategory, newTaskName, isTarget);

      parentTask.GetTaskBuilder()
        .IsDependentOn(newTask.TaskName);

      return newTask;
    }

    [ExcludeFromCodeCoverage]
    internal static IHelperTask GetDefaultTestTask(
      this ITaskHelper helper)
    {
      if (helper == null)
        return null;

      var targetName = "All";
      var taskName = $"{TargetCategory}-{targetName}";

      return helper.GetTask(taskName, true, TargetCategory);
    }

    [ExcludeFromCodeCoverage]
    public static IHelperTask GetTestCleanTask(
      this ITaskHelper helper,
      string testCategory = "Generic",
      string targetName = "All",
      bool isTarget = true)
    {
      if (helper == null)
        return null;

      var taskName = $"{testCategory}-{targetName}";

      var task = helper.GetCleanTask(TargetCategory, taskName, isTarget);
      if (targetName == "All")
      {
        var defaultTask = helper.GetDefaultCleanTask();
        helper.AddTaskDependency(defaultTask, task);
      }

      return task;
    }

    [ExcludeFromCodeCoverage]
    public static IHelperTask GetTestTask(
      this ITaskHelper helper,
      string testCategory = "Generic",
      string targetName = "All",
      bool isTarget = true)
    {
      if (helper == null)
        return null;

      testCategory = $"{TargetCategory}-{testCategory}";
      var taskName = $"{testCategory}-{targetName}";

      var task = helper.GetTask(taskName, isTarget, TargetCategory, testCategory);
      if (targetName == "All")
      {
        var defaultTask = helper.GetDefaultTestTask();
        helper.AddTaskDependency(defaultTask, task);
      }

      return task;
    }

    #endregion
  }
}