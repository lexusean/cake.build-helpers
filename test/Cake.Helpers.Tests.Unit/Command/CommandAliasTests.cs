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
  public class CommandAliasTests
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
    public void CommandAlias_CommandHelper_Success()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var defaultSln = SolutionFixture.GetDefaultSolution(context.FileSystem);
      context.AddDotNetCoreProject(defaultSln.SlnFile.Path.FullPath);

      var helperSetting = SingletonFactory.GetHelperSettings();
      Assert.IsNotNull(helperSetting);

      var commandHelper = context.CommandHelper();
      Assert.IsNotNull(commandHelper);

      Assert.ThrowsException<ArgumentNullException>(() => ((ICakeContext) null).CommandHelper());
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void CommandAlias_RunCommand_Success()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var defaultSln = SolutionFixture.GetDefaultSolution(context.FileSystem);
      context.AddDotNetCoreProject(defaultSln.SlnFile.Path.FullPath);
      var settings = this.GetSettings(context);
      
      Assert.ThrowsException<ArgumentNullException>(() => ((ICakeContext)null).RunCommand());

      context.RunCommand();
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