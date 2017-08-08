using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Helpers.Clean;
using Cake.Helpers.Command;
using Cake.Helpers.DotNetCore;
using Cake.Helpers.Settings;
using Cake.Helpers.Tests.Unit.DotNetCore;
using Cake.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cake.Helpers.Tests.Unit.Command
{
  [TestClass]
  public class CommandHelperTests
  {
    #region Test Setup and Teardown

    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void TestInit()
    {
      SingletonFactory.ClearFactory();
    }

    #endregion

    #region Test Methods

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void CommandHelper_Success()
    {
      var context = this.GetMoqContext(
        new Dictionary<string, bool>() 
        {
          { "run", true },
          { "t", false},
        }, 
        new Dictionary<string, string>()
        {
          { "run", "Build-All" },
        });
      var defaultSln = SolutionFixture.GetDefaultSolution(context.FileSystem);
      context.AddDotNetCoreProject(defaultSln.SlnFile.Path.FullPath);
      var helperSettings = this.GetSettings(context);

      var helper = SingletonFactory.GetCommandHelper();
      Assert.IsNotNull(helper);

      Assert.AreEqual("Build Script", helper.ScriptDescription);
      helper.ScriptDescription = "Test";
      Assert.AreEqual("Test", helper.ScriptDescription);

      Assert.IsTrue(string.IsNullOrWhiteSpace(helper.DefaultTarget));
      helper.DefaultTarget = "TestTarget";
      Assert.AreEqual("TestTarget", helper.DefaultTarget);

      Assert.AreEqual(3, helper.Arguments.Count());
      Assert.IsNotNull(helper.HelpArgument);
      Assert.AreEqual("support", helper.HelpArgument.Name);
      Assert.AreEqual("h", helper.HelpArgument.Shortname);

      Assert.IsNotNull(helper.AvailableTargetsArgument);
      Assert.AreEqual("available-targets", helper.AvailableTargetsArgument.Name);
      Assert.AreEqual("at", helper.AvailableTargetsArgument.Shortname);

      Assert.IsNotNull(helper.RunArgument);
      Assert.AreEqual("run", helper.RunArgument.Name);
      Assert.AreEqual("r", helper.RunArgument.Shortname);

      var badArg = helper.AddArgument("test", "t", string.Empty);
      Assert.IsNull(badArg);

      var goodArg = helper.AddArgument("test", "t", "testing");
      Assert.IsNotNull(goodArg);
      Assert.AreEqual(4, helper.Arguments.Count());

      Assert.ThrowsException<ArgumentNullException>(() => ((ICommandArgument)null).HasArgument(context));
      Assert.ThrowsException<ArgumentNullException>(() => goodArg.HasArgument(null));
      Assert.IsFalse(goodArg.HasArgument(context));
      Assert.ThrowsException<ArgumentNullException>(() => ((ICommandArgument)null).GetArgumentValue(context));
      Assert.ThrowsException<ArgumentNullException>(() => goodArg.GetArgumentValue(null));
      Assert.AreEqual(string.Empty, goodArg.GetArgumentValue(context));

      Assert.IsTrue(helper.RunArgument.HasArgument(context));
      Assert.AreEqual("Build-All", helper.RunArgument.GetArgumentValue(context));

      var commandHelper = helper as CommandHelper;
      Assert.IsNotNull(commandHelper);
      Assert.IsNotNull(commandHelper.TaskHelper);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void CommandHelper_NoTaskHelper()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var defaultSln = SolutionFixture.GetDefaultSolution(context.FileSystem);
      context.AddDotNetCoreProject(defaultSln.SlnFile.Path.FullPath);
      var helperSettings = this.GetSettings(context);

      Assert.ThrowsException<ArgumentNullException>(() => new CommandHelper(null));
    }

    #endregion

    #region Test Helpers

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

    #endregion
  }
}