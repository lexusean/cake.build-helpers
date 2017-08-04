using System;
using System.Diagnostics.CodeAnalysis;
using Cake.Core;
using Cake.Helpers.Clean;
using Cake.Helpers.Tasks;

namespace Cake.Helpers.Build
{
  public static class BuildHelperExtensions
  {
    #region Constants

    private const string BuildTaskName = "Build";
    private const string PostBuildTaskName = "PostBuild";
    private const string PreBuildTaskName = "PreBuild";
    private const string TargetCategory = "Build";

    #endregion

    #region Static Members

    public static IHelperTask AddToBuildCleanTask(
      this ITaskHelper helper,
      string targetName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      return helper.AddToCleanTask(targetName, TargetCategory, isTarget, parentTaskName);
    }

    public static IHelperTask AddToBuildTask(
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
      var parentTask = isTarget ? helper.GetBuildTask() : helper.AddToBuildTask(parentTaskName);
      var newTask = helper.GetBuildTask(newTaskName, isTarget);

      parentTask.GetTaskBuilder()
        .IsDependentOn(newTask.TaskName);

      return newTask;
    }

    public static IHelperTask AddToPostBuildTask(
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
      var parentTask = isTarget ? helper.GetPostBuildTask() : helper.AddToPostBuildTask(parentTaskName);
      var newTask = helper.GetPostBuildTask(newTaskName, isTarget);

      parentTask.GetTaskBuilder()
        .IsDependentOn(newTask.TaskName);

      return newTask;
    }

    public static IHelperTask AddToPreBuildTask(
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
      var parentTask = isTarget ? helper.GetPreBuildTask() : helper.AddToPreBuildTask(parentTaskName);
      var newTask = helper.GetPreBuildTask(newTaskName, isTarget);

      parentTask.GetTaskBuilder()
        .IsDependentOn(newTask.TaskName);

      return newTask;
    }

    [ExcludeFromCodeCoverage]
    public static IHelperTask GetBuildCleanTask(
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
    public static IHelperTask GetBuildTask(
      this ITaskHelper helper,
      string targetName = "All",
      bool isTarget = true)
    {
      if (helper == null)
        return null;

      var buildTask = helper.GetTask($"{BuildTaskName}-{targetName}", isTarget, TargetCategory, BuildTaskName);

      if (isTarget)
      {
        var preBuildTask = helper.GetPreBuildTask(targetName);
        helper.AddTaskDependency(buildTask, preBuildTask);
      }

      return buildTask;
    }

    [ExcludeFromCodeCoverage]
    public static IHelperTask GetPostBuildTask(
      this ITaskHelper helper,
      string targetName = "All",
      bool isTarget = true)
    {
      if (helper == null)
        return null;

      var postBuildTask =
        helper.GetTask($"{PostBuildTaskName}-{targetName}", isTarget, TargetCategory, PostBuildTaskName);

      if (isTarget && helper.BuildAllDependencies)
      {
        var buildTask = helper.GetBuildTask(targetName);
        helper.AddTaskDependency(postBuildTask, buildTask);
      }

      return postBuildTask;
    }

    [ExcludeFromCodeCoverage]
    public static IHelperTask GetPreBuildTask(
      this ITaskHelper helper,
      string targetName = "All",
      bool isTarget = true)
    {
      if (helper == null)
        return null;

      var preBuildTask = helper.GetTask($"{PreBuildTaskName}-{targetName}", isTarget, TargetCategory, PreBuildTaskName);

      if (isTarget)
      {
        var clnTask = helper.GetBuildCleanTask(targetName);
        helper.AddTaskDependency(preBuildTask, clnTask);
      }

      return preBuildTask;
    }

    #endregion
  }
}