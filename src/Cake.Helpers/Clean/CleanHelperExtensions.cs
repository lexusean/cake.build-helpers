using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Helpers.Tasks;

namespace Cake.Helpers.Clean
{
  public static class CleanHelperExtensions
  {
    public static IHelperTask GetCleanTask(
      this TaskHelper helper,
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

      return helper.GetTask(taskName, isTarget, "Clean", cleanCategory);
    }

    public static IHelperTask AddToCleanTask(
      this TaskHelper helper,
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
        : helper.GetCleanTask(cleanCategory, newTaskName, false);
      var newTask = helper.GetCleanTask(cleanCategory, newTaskName, isTarget);

      parentTask
        .GetBuildTask()
        .IsDependentOn(newTask.GetBuildTask());

      return newTask;
    }
  }
}
