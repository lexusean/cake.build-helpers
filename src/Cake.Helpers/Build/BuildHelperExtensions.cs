using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Helpers.Clean;
using Cake.Helpers.Tasks;

namespace Cake.Helpers.Build
{
  public static class BuildHelperExtensions
  {
    private const string TargetCategory = "Build";
    private const string PreBuildTaskName = "PreBuild";
    private const string BuildTaskName = "Build";
    private const string PostBuildTaskName = "PostBuild";

    public static IHelperTask GetBuildCleanTask(
      this TaskHelper helper,
      string targetName = "All",
      bool isTarget = true)
    {
      if (helper == null)
        return null;

      if (string.IsNullOrWhiteSpace(targetName))
        targetName = "All";

      return helper.GetCleanTask(TargetCategory, targetName, isTarget);
    }

    public static IHelperTask AddToBuildCleanTask(
      this TaskHelper helper,
      string targetName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      return helper.AddToCleanTask(targetName, TargetCategory, isTarget, parentTaskName);
    }

    public static IHelperTask GetPreBuildTask(
      this TaskHelper helper,
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

    public static IHelperTask AddToPreBuildTask(
      this TaskHelper helper,
      string targetName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if(string.IsNullOrWhiteSpace(targetName))
        throw new ArgumentNullException(nameof(targetName));

      if(!isTarget && string.IsNullOrWhiteSpace(parentTaskName))
        throw new ArgumentNullException(nameof(parentTaskName));

      var newTaskName = isTarget ? targetName : $"{parentTaskName}-{targetName}";
      var parentTask = isTarget ? helper.GetPreBuildTask() : helper.GetPreBuildTask(parentTaskName);
      var newTask = helper.GetPreBuildTask(newTaskName, isTarget);

      parentTask.GetBuildTask()
        .IsDependentOn(newTask.GetBuildTask());

      return newTask;
    }

    public static IHelperTask GetBuildTask(
      this TaskHelper helper,
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

    public static IHelperTask AddToBuildTask(
      this TaskHelper helper,
      string targetName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (string.IsNullOrWhiteSpace(targetName))
        throw new ArgumentNullException(nameof(targetName));

      if (!isTarget && string.IsNullOrWhiteSpace(parentTaskName))
        throw new ArgumentNullException(nameof(parentTaskName));

      var newTaskName = isTarget ? targetName : $"{parentTaskName}-{targetName}";
      var parentTask = isTarget ? helper.GetBuildTask() : helper.GetBuildTask(parentTaskName);
      var newTask = helper.GetBuildTask(newTaskName, isTarget);

      parentTask.GetBuildTask()
        .IsDependentOn(newTask.GetBuildTask());

      return newTask;
    }

    public static IHelperTask GetPostBuildTask(
      this TaskHelper helper,
      string targetName = "All",
      bool isTarget = true)
    {
      if (helper == null)
        return null;

      var postBuildTask = helper.GetTask($"{PostBuildTaskName}-{targetName}", isTarget, TargetCategory, PostBuildTaskName);

      if (isTarget && helper.BuildAllDependencies)
      {
        var buildTask = helper.GetBuildTask(targetName);
        helper.AddTaskDependency(postBuildTask, buildTask);
      }

      return postBuildTask;
    }

    public static IHelperTask AddToPostBuildTask(
      this TaskHelper helper,
      string targetName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (string.IsNullOrWhiteSpace(targetName))
        throw new ArgumentNullException(nameof(targetName));

      if (!isTarget && string.IsNullOrWhiteSpace(parentTaskName))
        throw new ArgumentNullException(nameof(parentTaskName));

      var newTaskName = isTarget ? targetName : $"{parentTaskName}-{targetName}";
      var parentTask = isTarget ? helper.GetPostBuildTask() : helper.GetPostBuildTask(parentTaskName);
      var newTask = helper.GetPostBuildTask(newTaskName, isTarget);

      parentTask.GetBuildTask()
        .IsDependentOn(newTask.GetBuildTask());

      return newTask;
    }
  }
}
