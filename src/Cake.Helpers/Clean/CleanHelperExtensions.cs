using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Helpers.Tasks;

namespace Cake.Helpers.Clean
{
  [ExcludeFromCodeCoverage]
  internal static class CleanHelperExtensions
  {
    [ExcludeFromCodeCoverage]
    internal static IHelperTask GetDefaultCleanTask(
      this ITaskHelper helper)
    {
      if (helper == null)
        return null;

      var targetName = "All";
      var taskName = $"Clean-{targetName}";

      return helper.GetTask(taskName, true, "Clean");
    }

    public static IHelperTask GetCleanTask(
      this ITaskHelper helper,
      string cleanCategory = "",
      string targetName = "All",
      bool isTarget = true)
    {
      if (helper == null)
        return null;

      if (string.IsNullOrWhiteSpace(targetName))
        targetName = "All";

      if (string.IsNullOrWhiteSpace(cleanCategory))
        cleanCategory = "Generic";

      cleanCategory = $"Clean-{cleanCategory}";
      var taskName = $"{cleanCategory}-{targetName}";

      var task = helper.GetTask(taskName, isTarget, "Clean", cleanCategory);
      if (targetName == "All")
      {
        var defaultTask = helper.GetDefaultCleanTask();
        helper.AddTaskDependency(defaultTask, task);
      }

      return task;
    }

    public static IHelperTask AddToCleanTask(
      this ITaskHelper helper,
      string taskName,
      string cleanCategory = "",
      bool isTarget = true,
      string parentTaskName = "")
    {
      if(string.IsNullOrWhiteSpace(taskName))
        throw new ArgumentNullException(nameof(taskName));

      if(!isTarget && string.IsNullOrWhiteSpace(parentTaskName))
        throw new ArgumentNullException(nameof(parentTaskName));

      var newTaskName = isTarget ? taskName : $"{parentTaskName}-{taskName}";
      var parentTask = isTarget
        ? helper.GetCleanTask(cleanCategory)
        : helper.AddToCleanTask(parentTaskName, cleanCategory);
      var newTask = helper.GetCleanTask(cleanCategory, newTaskName, isTarget);

      parentTask
        .GetTaskBuilder()
        .IsDependentOn(newTask.TaskName);

      return newTask;
    }
  }
}
