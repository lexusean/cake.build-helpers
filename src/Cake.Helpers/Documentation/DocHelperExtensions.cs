using System;
using System.Diagnostics.CodeAnalysis;
using Cake.Core;
using Cake.Helpers.Clean;
using Cake.Helpers.Tasks;

namespace Cake.Helpers.Documentation
{
  internal static class DocHelperExtensions
  {
    #region Constants

    private const string DocExtractTaskName = "DocExtract";
    private const string DocBuildTaskName = "DocBuild";
    private const string DocPostBuildTaskName = "DocPostBuild";
    private const string TargetCategory = "Doc";

    #endregion

    #region Static Members

    internal static IHelperTask AddToDocCleanTask(
      this ITaskHelper helper,
      string targetName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      return helper.AddToCleanTask(targetName, TargetCategory, isTarget, parentTaskName);
    }

    internal static IHelperTask AddToDocExtractTask(
      this ITaskHelper helper,
      string targetName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (string.IsNullOrWhiteSpace(targetName))
        throw new ArgumentNullException(nameof(targetName));

      if (!isTarget && string.IsNullOrWhiteSpace(parentTaskName))
        throw new ArgumentNullException(nameof(parentTaskName));

      var newTaskName = isTarget ? targetName : $"{parentTaskName}-{targetName}";
      var parentTask = isTarget ? helper.GetDocExtractTask() : helper.AddToDocExtractTask(parentTaskName);
      var newTask = helper.GetDocExtractTask(newTaskName, isTarget);

      helper.AddTaskDependency(parentTask, newTask);

      return newTask;
    }

    internal static IHelperTask AddToDocBuildTask(
      this ITaskHelper helper,
      string targetName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (string.IsNullOrWhiteSpace(targetName))
        throw new ArgumentNullException(nameof(targetName));

      if (!isTarget && string.IsNullOrWhiteSpace(parentTaskName))
        throw new ArgumentNullException(nameof(parentTaskName));

      var newTaskName = isTarget ? targetName : $"{parentTaskName}-{targetName}";
      var parentTask = isTarget ? helper.GetDocBuildTask() : helper.AddToDocBuildTask(parentTaskName);
      var newTask = helper.GetDocBuildTask(newTaskName, isTarget);

      helper.AddTaskDependency(parentTask, newTask);

      return newTask;
    }

    internal static IHelperTask AddToDocPostBuildTask(
      this ITaskHelper helper,
      string targetName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (string.IsNullOrWhiteSpace(targetName))
        throw new ArgumentNullException(nameof(targetName));

      if (!isTarget && string.IsNullOrWhiteSpace(parentTaskName))
        throw new ArgumentNullException(nameof(parentTaskName));

      var newTaskName = isTarget ? targetName : $"{parentTaskName}-{targetName}";
      var parentTask = isTarget ? helper.GetDocPostBuildTask() : helper.GetDocPostBuildTask(parentTaskName);
      var newTask = helper.GetDocPostBuildTask(newTaskName, isTarget);

      helper.AddTaskDependency(parentTask, newTask);

      return newTask;
    }

    [ExcludeFromCodeCoverage]
    internal static IHelperTask GetDocCleanTask(
      this ITaskHelper helper,
      string targetName = "All",
      bool isTarget = true)
    {
      if (helper == null)
        return null;

      if (string.IsNullOrWhiteSpace(targetName))
        targetName = "All";


      return helper.GetCleanTask(TargetCategory, targetName, isTarget);
    }

    [ExcludeFromCodeCoverage]
    internal static IHelperTask GetDocExtractTask(
      this ITaskHelper helper,
      string targetName = "All",
      bool isTarget = true)
    {
      if (helper == null)
        return null;

      var buildTask = helper.GetTask($"{DocExtractTaskName}-{targetName}", isTarget, TargetCategory, DocExtractTaskName);

      if (isTarget)
      {
        var clnTask = helper.GetDocCleanTask(targetName);
        helper.AddTaskDependency(buildTask, clnTask);
      }

      return buildTask;
    }

    [ExcludeFromCodeCoverage]
    internal static IHelperTask GetDocBuildTask(
      this ITaskHelper helper,
      string targetName = "All",
      bool isTarget = true)
    {
      if (helper == null)
        return null;

      var buildTask = helper.GetTask($"{DocBuildTaskName}-{targetName}", isTarget, TargetCategory, DocBuildTaskName);

      if (isTarget)
      {
        var extractTask = helper.GetDocExtractTask(targetName);
        helper.AddTaskDependency(buildTask, extractTask);
      }

      return buildTask;
    }

    [ExcludeFromCodeCoverage]
    internal static IHelperTask GetDocPostBuildTask(
      this ITaskHelper helper,
      string targetName = "All",
      bool isTarget = true)
    {
      if (helper == null)
        return null;

      var postBuildTask =
        helper.GetTask($"{DocPostBuildTaskName}-{targetName}", isTarget, TargetCategory, DocPostBuildTaskName);

      if (isTarget && helper.BuildAllDependencies)
      {
        var buildTask = helper.GetDocBuildTask(targetName);
        helper.AddTaskDependency(postBuildTask, buildTask);
      }

      return postBuildTask;
    }

    #endregion
  }
}