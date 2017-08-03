using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Helpers.Build;
using Cake.Helpers.Settings;
using Cake.Helpers.Test;
using Cake.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cake.Helpers.Tests.Unit.Build
{
  [TestClass]
  public class BuildHelperAliasTests
  {
    #region Test Setup and Teardown

    [TestInitialize]
    public void TestInit()
    {
      SingletonFactory.ClearFactory();
    }

    #endregion

    #region Test Methods

    #region BuildClean

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void BuildCleanTask_Default_Success()
    {
      var cleanCategory = "Build";
      var category = "Clean";
      var taskType = $"{category}-{cleanCategory}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "BuildClean";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.BuildCleanTask(taskName);

      var taskHelper = SingletonFactory.GetTaskHelper();

      Assert.IsNotNull(cleanTask);
      Assert.AreEqual($"{taskType}-{taskName}", cleanTask.Task.Name);

      var defaultCleanAllTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == $"{category}-All");

      Assert.IsNotNull(defaultCleanAllTask);
      Assert.IsTrue(defaultCleanAllTask.IsTarget);
      Assert.AreEqual($"{category}-All", defaultCleanAllTask.TaskName);
      Assert.AreEqual(category, defaultCleanAllTask.Category);
      Assert.IsNotNull(defaultCleanAllTask.Task);
      Assert.IsTrue(defaultCleanAllTask.Task.Dependencies.Contains(parentTaskName));

      var cleanAllTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == parentTaskName);

      Assert.IsNotNull(cleanAllTask);
      Assert.IsTrue(cleanAllTask.IsTarget);
      Assert.AreEqual(parentTaskName, cleanAllTask.TaskName);
      Assert.AreEqual(category, cleanAllTask.Category);
      Assert.AreEqual($"{category}-{cleanCategory}", cleanAllTask.TaskType);
      Assert.IsNotNull(cleanAllTask.Task);
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains(cleanTask.Task.Name));

      var cleanHelperTask = taskHelper.Tasks.FirstOrDefault(t => t.Task == cleanTask.Task);

      Assert.IsNotNull(cleanHelperTask);
      Assert.IsTrue(cleanHelperTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}", cleanHelperTask.TaskName);
      Assert.AreEqual(category, cleanHelperTask.Category);
      Assert.AreEqual($"{category}-{cleanCategory}", cleanHelperTask.TaskType);
      Assert.IsNotNull(cleanHelperTask.Task);
      Assert.IsFalse(cleanHelperTask.Task.Dependencies.Any());
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void BuildCleanTask_EmptyParentTaskName()
    {
      var cleanCategory = "Build";
      var category = "Clean";
      var taskType = $"{category}-{cleanCategory}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "BuildClean";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.BuildCleanTask(taskName, false, string.Empty);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void BuildCleanTask_EmptyTargetName()
    {
      var cleanCategory = "Build";
      var category = "Clean";
      var taskType = $"{category}-{cleanCategory}";
      var parentTaskName = $"{taskType}-All";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.BuildCleanTask(string.Empty);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void BuildCleanTask_NoContext()
    {
      var cleanCategory = "Build";
      var category = "Clean";
      var taskType = $"{category}-{cleanCategory}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "BuildClean";

      ICakeContext context = null;
      var cleanTask = context.BuildCleanTask(taskName);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void BuildCleanTask_SubTask_Success()
    {
      var cleanCategory = "Build";
      var category = "Clean";
      var taskType = $"{category}-{cleanCategory}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "BuildClean";
      var subtaskName = "Cleaning";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.BuildCleanTask(subtaskName, false, taskName);

      var taskHelper = SingletonFactory.GetTaskHelper();

      Assert.IsNotNull(cleanTask);
      Assert.AreEqual($"{taskType}-{taskName}-{subtaskName}", cleanTask.Task.Name);

      var parentTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == $"{taskType}-{taskName}");

      Assert.IsNotNull(parentTask);
      Assert.IsTrue(parentTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}", parentTask.TaskName);
      Assert.AreEqual(category, parentTask.Category);
      Assert.AreEqual($"{category}-{cleanCategory}", parentTask.TaskType);
      Assert.IsNotNull(parentTask.Task);
      Assert.IsTrue(parentTask.Task.Dependencies.Contains(cleanTask.Task.Name));

      var cleanAllTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == parentTaskName);

      Assert.IsNotNull(cleanAllTask);
      Assert.IsTrue(cleanAllTask.IsTarget);
      Assert.AreEqual(parentTaskName, cleanAllTask.TaskName);
      Assert.AreEqual(category, cleanAllTask.Category);
      Assert.AreEqual($"{category}-{cleanCategory}", cleanAllTask.TaskType);
      Assert.IsNotNull(cleanAllTask.Task);
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains(parentTask.TaskName));

      var cleanHelperTask = taskHelper.Tasks.FirstOrDefault(t => t.Task == cleanTask.Task);

      Assert.IsNotNull(cleanHelperTask);
      Assert.IsFalse(cleanHelperTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}-{subtaskName}", cleanHelperTask.TaskName);
      Assert.AreEqual(category, cleanHelperTask.Category);
      Assert.AreEqual($"{category}-{cleanCategory}", cleanHelperTask.TaskType);
      Assert.IsNotNull(cleanHelperTask.Task);
      Assert.IsFalse(cleanHelperTask.Task.Dependencies.Any());
    }

    #endregion

    #region PreBuild

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void PreBuildTask_Default_Success()
    {
      var category = "Build";
      var buildCategory = "PreBuild";
      var taskType = $"{buildCategory}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "Building";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.PreBuildTask(taskName);

      var taskHelper = SingletonFactory.GetTaskHelper();

      Assert.IsNotNull(cleanTask);
      Assert.AreEqual($"{taskType}-{taskName}", cleanTask.Task.Name);

      var cleanAllTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == parentTaskName);

      Assert.IsNotNull(cleanAllTask);
      Assert.IsTrue(cleanAllTask.IsTarget);
      Assert.AreEqual(parentTaskName, cleanAllTask.TaskName);
      Assert.AreEqual(category, cleanAllTask.Category);
      Assert.AreEqual($"{buildCategory}", cleanAllTask.TaskType);
      Assert.IsNotNull(cleanAllTask.Task);
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains(cleanTask.Task.Name));
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains($"Clean-{category}-All"));

      var cleanHelperTask = taskHelper.Tasks.FirstOrDefault(t => t.Task == cleanTask.Task);

      Assert.IsNotNull(cleanHelperTask);
      Assert.IsTrue(cleanHelperTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}", cleanHelperTask.TaskName);
      Assert.AreEqual(category, cleanHelperTask.Category);
      Assert.AreEqual($"{buildCategory}", cleanHelperTask.TaskType);
      Assert.IsNotNull(cleanHelperTask.Task);
      Assert.IsTrue(cleanHelperTask.Task.Dependencies.Contains($"Clean-{category}-{taskName}"));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void PreBuildTask_EmptyParentTaskName()
    {
      var category = "PreBuild";
      var taskType = $"{category}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "PreBuild";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.PreBuildTask(taskName, false, string.Empty);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void PreBuildTask_EmptyTargetName()
    {
      var category = "PreBuild";
      var taskType = $"{category}";
      var parentTaskName = $"{taskType}-All";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.PreBuildTask(string.Empty);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void PreBuildTask_NoContext()
    {
      var category = "PreBuild";
      var taskType = $"{category}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "PreBuild";

      ICakeContext context = null;
      var cleanTask = context.PreBuildTask(taskName);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void PreBuildTask_SubTask_Success()
    {
      var category = "Build";
      var buildCategory = "PreBuild";
      var taskType = $"{buildCategory}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "Building";
      var subtaskName = "Restoring";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.PreBuildTask(subtaskName, false, taskName);

      var taskHelper = SingletonFactory.GetTaskHelper();

      Assert.IsNotNull(cleanTask);
      Assert.AreEqual($"{taskType}-{taskName}-{subtaskName}", cleanTask.Task.Name);

      var parentTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == $"{taskType}-{taskName}");

      Assert.IsNotNull(parentTask);
      Assert.IsTrue(parentTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}", parentTask.TaskName);
      Assert.AreEqual(category, parentTask.Category);
      Assert.AreEqual($"{buildCategory}", parentTask.TaskType);
      Assert.IsNotNull(parentTask.Task);
      Assert.IsTrue(parentTask.Task.Dependencies.Contains($"Clean-{category}-{taskName}"));
      Assert.IsTrue(parentTask.Task.Dependencies.Contains(cleanTask.Task.Name));

      var cleanAllTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == parentTaskName);

      Assert.IsNotNull(cleanAllTask);
      Assert.IsTrue(cleanAllTask.IsTarget);
      Assert.AreEqual(parentTaskName, cleanAllTask.TaskName);
      Assert.AreEqual(category, cleanAllTask.Category);
      Assert.AreEqual($"{buildCategory}", cleanAllTask.TaskType);
      Assert.IsNotNull(cleanAllTask.Task);
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains(parentTask.TaskName));
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains($"Clean-{category}-All"));

      var cleanHelperTask = taskHelper.Tasks.FirstOrDefault(t => t.Task == cleanTask.Task);

      Assert.IsNotNull(cleanHelperTask);
      Assert.IsFalse(cleanHelperTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}-{subtaskName}", cleanHelperTask.TaskName);
      Assert.AreEqual(category, cleanHelperTask.Category);
      Assert.AreEqual($"{buildCategory}", cleanHelperTask.TaskType);
      Assert.IsNotNull(cleanHelperTask.Task);
      Assert.IsFalse(cleanHelperTask.Task.Dependencies.Any());
    }

    #endregion

    #region Build

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void BuildTask_Default_Success()
    {
      var category = "Build";
      var buildCategory = "Build";
      var taskType = $"{buildCategory}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "Building";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.BuildTask(taskName);

      var taskHelper = SingletonFactory.GetTaskHelper();

      Assert.IsNotNull(cleanTask);
      Assert.AreEqual($"{taskType}-{taskName}", cleanTask.Task.Name);

      var cleanAllTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == parentTaskName);

      Assert.IsNotNull(cleanAllTask);
      Assert.IsTrue(cleanAllTask.IsTarget);
      Assert.AreEqual(parentTaskName, cleanAllTask.TaskName);
      Assert.AreEqual(category, cleanAllTask.Category);
      Assert.AreEqual($"{buildCategory}", cleanAllTask.TaskType);
      Assert.IsNotNull(cleanAllTask.Task);
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains(cleanTask.Task.Name));
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains($"Pre{category}-All"));

      var cleanHelperTask = taskHelper.Tasks.FirstOrDefault(t => t.Task == cleanTask.Task);

      Assert.IsNotNull(cleanHelperTask);
      Assert.IsTrue(cleanHelperTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}", cleanHelperTask.TaskName);
      Assert.AreEqual(category, cleanHelperTask.Category);
      Assert.AreEqual($"{buildCategory}", cleanHelperTask.TaskType);
      Assert.IsNotNull(cleanHelperTask.Task);
      Assert.IsTrue(cleanHelperTask.Task.Dependencies.Contains($"Pre{category}-{taskName}"));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void BuildTask_EmptyParentTaskName()
    {
      var category = "Build";
      var taskType = $"{category}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "Building";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.BuildTask(taskName, false, string.Empty);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void BuildTask_EmptyTargetName()
    {
      var category = "Build";
      var taskType = $"{category}";
      var parentTaskName = $"{taskType}-All";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.BuildTask(string.Empty);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void BuildTask_NoContext()
    {
      var category = "Build";
      var taskType = $"{category}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "PreBuild";

      ICakeContext context = null;
      var cleanTask = context.BuildTask(taskName);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void BuildTask_SubTask_Success()
    {
      var category = "Build";
      var buildCategory = "Build";
      var taskType = $"{buildCategory}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "Building";
      var subtaskName = "BuildProject";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.BuildTask(subtaskName, false, taskName);

      var taskHelper = SingletonFactory.GetTaskHelper();

      Assert.IsNotNull(cleanTask);
      Assert.AreEqual($"{taskType}-{taskName}-{subtaskName}", cleanTask.Task.Name);

      var parentTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == $"{taskType}-{taskName}");

      Assert.IsNotNull(parentTask);
      Assert.IsTrue(parentTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}", parentTask.TaskName);
      Assert.AreEqual(category, parentTask.Category);
      Assert.AreEqual($"{buildCategory}", parentTask.TaskType);
      Assert.IsNotNull(parentTask.Task);
      Assert.IsTrue(parentTask.Task.Dependencies.Contains($"Pre{category}-{taskName}"));
      Assert.IsTrue(parentTask.Task.Dependencies.Contains(cleanTask.Task.Name));

      var cleanAllTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == parentTaskName);

      Assert.IsNotNull(cleanAllTask);
      Assert.IsTrue(cleanAllTask.IsTarget);
      Assert.AreEqual(parentTaskName, cleanAllTask.TaskName);
      Assert.AreEqual(category, cleanAllTask.Category);
      Assert.AreEqual($"{buildCategory}", cleanAllTask.TaskType);
      Assert.IsNotNull(cleanAllTask.Task);
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains(parentTask.TaskName));
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains($"Pre{category}-All"));

      var cleanHelperTask = taskHelper.Tasks.FirstOrDefault(t => t.Task == cleanTask.Task);

      Assert.IsNotNull(cleanHelperTask);
      Assert.IsFalse(cleanHelperTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}-{subtaskName}", cleanHelperTask.TaskName);
      Assert.AreEqual(category, cleanHelperTask.Category);
      Assert.AreEqual($"{buildCategory}", cleanHelperTask.TaskType);
      Assert.IsNotNull(cleanHelperTask.Task);
      Assert.IsFalse(cleanHelperTask.Task.Dependencies.Any());
    }

    #endregion

    #region PostBuild

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void PostBuildTask_Default_BuildAll_Success()
    {
      var category = "Build";
      var buildCategory = "PostBuild";
      var taskType = $"{buildCategory}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "Building";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      settings.RunAllDependencies = true;
      var cleanTask = context.PostBuildTask(taskName);

      var taskHelper = SingletonFactory.GetTaskHelper();

      Assert.IsNotNull(cleanTask);
      Assert.AreEqual($"{taskType}-{taskName}", cleanTask.Task.Name);

      var cleanAllTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == parentTaskName);

      Assert.IsNotNull(cleanAllTask);
      Assert.IsTrue(cleanAllTask.IsTarget);
      Assert.AreEqual(parentTaskName, cleanAllTask.TaskName);
      Assert.AreEqual(category, cleanAllTask.Category);
      Assert.AreEqual($"{buildCategory}", cleanAllTask.TaskType);
      Assert.IsNotNull(cleanAllTask.Task);
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains(cleanTask.Task.Name));
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains($"{category}-All"));

      var cleanHelperTask = taskHelper.Tasks.FirstOrDefault(t => t.Task == cleanTask.Task);

      Assert.IsNotNull(cleanHelperTask);
      Assert.IsTrue(cleanHelperTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}", cleanHelperTask.TaskName);
      Assert.AreEqual(category, cleanHelperTask.Category);
      Assert.AreEqual($"{buildCategory}", cleanHelperTask.TaskType);
      Assert.IsNotNull(cleanHelperTask.Task);
      Assert.IsTrue(cleanHelperTask.Task.Dependencies.Contains($"{category}-{taskName}"));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void PostBuildTask_EmptyParentTaskName()
    {
      var category = "Build";
      var taskType = $"{category}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "Building";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.PostBuildTask(taskName, false, string.Empty);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void PostBuildTask_EmptyTargetName()
    {
      var category = "Build";
      var taskType = $"{category}";
      var parentTaskName = $"{taskType}-All";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.PostBuildTask(string.Empty);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void PostBuildTask_NoContext()
    {
      var category = "Build";
      var taskType = $"{category}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "PreBuild";

      ICakeContext context = null;
      var cleanTask = context.PostBuildTask(taskName);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void PostBuildTask_SubTask_BuildAll_Success()
    {
      var category = "Build";
      var buildCategory = "PostBuild";
      var taskType = $"{buildCategory}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "Building";
      var subtaskName = "BuildProject";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      settings.RunAllDependencies = true;
      var cleanTask = context.PostBuildTask(subtaskName, false, taskName);

      var taskHelper = SingletonFactory.GetTaskHelper();

      Assert.IsNotNull(cleanTask);
      Assert.AreEqual($"{taskType}-{taskName}-{subtaskName}", cleanTask.Task.Name);

      var parentTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == $"{taskType}-{taskName}");

      Assert.IsNotNull(parentTask);
      Assert.IsTrue(parentTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}", parentTask.TaskName);
      Assert.AreEqual(category, parentTask.Category);
      Assert.AreEqual($"{buildCategory}", parentTask.TaskType);
      Assert.IsNotNull(parentTask.Task);
      Assert.IsTrue(parentTask.Task.Dependencies.Contains($"{category}-{taskName}"));
      Assert.IsTrue(parentTask.Task.Dependencies.Contains(cleanTask.Task.Name));

      var cleanAllTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == parentTaskName);

      Assert.IsNotNull(cleanAllTask);
      Assert.IsTrue(cleanAllTask.IsTarget);
      Assert.AreEqual(parentTaskName, cleanAllTask.TaskName);
      Assert.AreEqual(category, cleanAllTask.Category);
      Assert.AreEqual($"{buildCategory}", cleanAllTask.TaskType);
      Assert.IsNotNull(cleanAllTask.Task);
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains(parentTask.TaskName));
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains($"{category}-All"));

      var cleanHelperTask = taskHelper.Tasks.FirstOrDefault(t => t.Task == cleanTask.Task);

      Assert.IsNotNull(cleanHelperTask);
      Assert.IsFalse(cleanHelperTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}-{subtaskName}", cleanHelperTask.TaskName);
      Assert.AreEqual(category, cleanHelperTask.Category);
      Assert.AreEqual($"{buildCategory}", cleanHelperTask.TaskType);
      Assert.IsNotNull(cleanHelperTask.Task);
      Assert.IsFalse(cleanHelperTask.Task.Dependencies.Any());
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void PostBuildTask_Default_NoBuild_Success()
    {
      var category = "Build";
      var buildCategory = "PostBuild";
      var taskType = $"{buildCategory}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "Building";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      settings.RunAllDependencies = false;
      var cleanTask = context.PostBuildTask(taskName);

      var taskHelper = SingletonFactory.GetTaskHelper();

      Assert.IsNotNull(cleanTask);
      Assert.AreEqual($"{taskType}-{taskName}", cleanTask.Task.Name);

      var cleanAllTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == parentTaskName);

      Assert.IsNotNull(cleanAllTask);
      Assert.IsTrue(cleanAllTask.IsTarget);
      Assert.AreEqual(parentTaskName, cleanAllTask.TaskName);
      Assert.AreEqual(category, cleanAllTask.Category);
      Assert.AreEqual($"{buildCategory}", cleanAllTask.TaskType);
      Assert.IsNotNull(cleanAllTask.Task);
      Assert.AreEqual(1, cleanAllTask.Task.Dependencies.Count());
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains(cleanTask.Task.Name));

      var cleanHelperTask = taskHelper.Tasks.FirstOrDefault(t => t.Task == cleanTask.Task);

      Assert.IsNotNull(cleanHelperTask);
      Assert.IsTrue(cleanHelperTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}", cleanHelperTask.TaskName);
      Assert.AreEqual(category, cleanHelperTask.Category);
      Assert.AreEqual($"{buildCategory}", cleanHelperTask.TaskType);
      Assert.IsNotNull(cleanHelperTask.Task);
      Assert.IsFalse(cleanHelperTask.Task.Dependencies.Any());
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void PostBuildTask_SubTask_NoBuild_Success()
    {
      var category = "Build";
      var buildCategory = "PostBuild";
      var taskType = $"{buildCategory}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "Building";
      var subtaskName = "BuildProject";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      settings.RunAllDependencies = false;
      var cleanTask = context.PostBuildTask(subtaskName, false, taskName);

      var taskHelper = SingletonFactory.GetTaskHelper();

      Assert.IsNotNull(cleanTask);
      Assert.AreEqual($"{taskType}-{taskName}-{subtaskName}", cleanTask.Task.Name);

      var parentTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == $"{taskType}-{taskName}");

      Assert.IsNotNull(parentTask);
      Assert.IsTrue(parentTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}", parentTask.TaskName);
      Assert.AreEqual(category, parentTask.Category);
      Assert.AreEqual($"{buildCategory}", parentTask.TaskType);
      Assert.IsNotNull(parentTask.Task);
      Assert.AreEqual(1, parentTask.Task.Dependencies.Count());
      Assert.IsTrue(parentTask.Task.Dependencies.Contains(cleanTask.Task.Name));

      var cleanAllTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == parentTaskName);

      Assert.IsNotNull(cleanAllTask);
      Assert.IsTrue(cleanAllTask.IsTarget);
      Assert.AreEqual(parentTaskName, cleanAllTask.TaskName);
      Assert.AreEqual(category, cleanAllTask.Category);
      Assert.AreEqual($"{buildCategory}", cleanAllTask.TaskType);
      Assert.IsNotNull(cleanAllTask.Task);
      Assert.AreEqual(1, cleanAllTask.Task.Dependencies.Count());
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains(parentTask.TaskName));

      var cleanHelperTask = taskHelper.Tasks.FirstOrDefault(t => t.Task == cleanTask.Task);

      Assert.IsNotNull(cleanHelperTask);
      Assert.IsFalse(cleanHelperTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}-{subtaskName}", cleanHelperTask.TaskName);
      Assert.AreEqual(category, cleanHelperTask.Category);
      Assert.AreEqual($"{buildCategory}", cleanHelperTask.TaskType);
      Assert.IsNotNull(cleanHelperTask.Task);
      Assert.IsFalse(cleanHelperTask.Task.Dependencies.Any());
    }

    #endregion

    #endregion

    #region Test Helpers

    private ICakeArguments GetMoqArguments(
      IDictionary<string, bool> hasArgs,
      IDictionary<string, string> argValues)
    {
      var argsMock = new Mock<ICakeArguments>();
      argsMock.Setup(t => t.HasArgument(It.IsAny<string>()))
        .Returns((string arg) =>
        {
          if (!hasArgs.ContainsKey(arg))
            return false;

          return hasArgs[arg];
        });

      argsMock.Setup(t => t.GetArgument(It.IsAny<string>()))
        .Returns((string arg) =>
        {
          if (!argValues.ContainsKey(arg))
            return string.Empty;

          return argValues[arg];
        });

      return argsMock.Object;
    }

    private ICakeContext GetMoqContext(
      IDictionary<string, bool> hasArgs,
      IDictionary<string, string> argValues)
    {
      var fixture = HelperFixture.CreateFixture();
      var args = this.GetMoqArguments(hasArgs, argValues);
      var globber = this.GetMoqGlobber(fixture.FileSystem, fixture.Environment);
      var reg = this.GetMoqRegistry();

      return this.GetMoqContext(fixture, globber, reg, args);
    }

    private ICakeContext GetMoqContext(
      HelperFixture fixture,
      IGlobber globber,
      IRegistry registry,
      ICakeArguments args)
    {
      var log = new FakeLog();

      var contextMock = new Mock<ICakeContext>();
      contextMock.SetupGet(t => t.FileSystem).Returns(fixture.FileSystem);
      contextMock.SetupGet(t => t.Environment).Returns(fixture.Environment);
      contextMock.SetupGet(t => t.Globber).Returns(globber);
      contextMock.SetupGet(t => t.Log).Returns(log);
      contextMock.SetupGet(t => t.Arguments).Returns(args);
      contextMock.SetupGet(t => t.ProcessRunner).Returns(fixture.ProcessRunner);
      contextMock.SetupGet(t => t.Registry).Returns(registry);
      contextMock.SetupGet(t => t.Tools).Returns(fixture.Tools);

      return contextMock.Object;
    }

    private IGlobber GetMoqGlobber(
      IFileSystem fs,
      ICakeEnvironment env)
    {
      return new Globber(fs, env);
    }

    private IRegistry GetMoqRegistry()
    {
      var regMock = new Mock<IRegistry>();
      regMock.SetupGet(t => t.LocalMachine).Returns((IRegistryKey) null);

      return regMock.Object;
    }

    private IHelperSettings GetSettings(ICakeContext context)
    {
      SingletonFactory.Context = context;
      var settings = SingletonFactory.GetHelperSettings();
      settings.TaskTargetFunc = taskName =>
      {
        var task = new ActionTask(taskName);
        return new CakeTaskBuilder<ActionTask>(task);
      };

      settings.RunTargetFunc = targetName =>
      {
        var report = new CakeReport {{targetName, TimeSpan.Zero, CakeTaskExecutionStatus.Executed}};

        return report;
      };

      return settings;
    }

    #endregion
  }
}