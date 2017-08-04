using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Helpers.Settings;
using Cake.Helpers.Tasks;
using Cake.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cake.Helpers.Tests.Unit.Tasks
{
  [TestClass]
  public class TaskAliasTests
  {
    #region Test Setup and Teardown

    [TestInitialize]
    public void TestInit()
    {
      SingletonFactory.ClearFactory();
    }

    #endregion

    #region Test Methods

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void HelperTask_Default_Success()
    {
      var taskName = "Blah";
      var category = HelperTask.DefaultTaskCategory;
      var taskType = HelperTask.DefaultTaskType;

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var task = context.HelperTask(taskName);

      Assert.IsNotNull(task);

      var taskHelper = SingletonFactory.GetTaskHelper();

      Assert.IsNotNull(task);
      Assert.AreEqual(taskName, task.Task.Name);

      var helperTask = taskHelper.Tasks.FirstOrDefault(t => t.Task == task.Task);
      Assert.IsNotNull(helperTask);
      Assert.AreEqual(taskName, helperTask.TaskName);
      Assert.AreEqual(true, helperTask.IsTarget);
      Assert.AreEqual(category, helperTask.Category);
      Assert.AreEqual(taskType, helperTask.TaskType);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void HelperTask_Category_TaskType_Success()
    {
      var taskName = "Blah";
      var category = "UnitTest";
      var taskType = "UnitTest";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var task = context.HelperTask(taskName, true, category, taskType);

      Assert.IsNotNull(task);

      var taskHelper = SingletonFactory.GetTaskHelper();

      Assert.IsNotNull(task);
      Assert.AreEqual(taskName, task.Task.Name);

      var helperTask = taskHelper.Tasks.FirstOrDefault(t => t.Task == task.Task);
      Assert.IsNotNull(helperTask);
      Assert.AreEqual(taskName, helperTask.TaskName);
      Assert.AreEqual(true, helperTask.IsTarget);
      Assert.AreEqual(category, helperTask.Category);
      Assert.AreEqual(taskType, helperTask.TaskType);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void HelperTask_NoContext()
    {
      var taskName = "Blah";

      ICakeContext context = null;
      Assert.ThrowsException<ArgumentNullException>(() => context.HelperTask(taskName));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_Default_Success()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var task = context.TaskHelper();

      Assert.IsNotNull(task);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_NoContext()
    {
      ICakeContext context = null;
      Assert.ThrowsException<ArgumentNullException>(() => context.TaskHelper());
    }

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