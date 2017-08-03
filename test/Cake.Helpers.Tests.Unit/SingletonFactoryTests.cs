using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cake.Helpers.Tests.Unit
{
  [TestClass]
  public class SingletonFactoryTests
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
    public void SingletonFactoryClear_Success()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;

      var settings = SingletonFactory.GetHelperSettings();
      Assert.IsNotNull(settings);

      SingletonFactory.ClearFactory();
      Assert.IsNull(SingletonFactory.Context);

      SingletonFactory.Context = context;
      var newSettings = SingletonFactory.GetHelperSettings();

      Assert.IsNotNull(newSettings);
      Assert.AreNotEqual(settings, newSettings);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void GetHelperSettings_Success()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;

      var settings = SingletonFactory.GetHelperSettings();

      Assert.IsNotNull(settings);
      Assert.IsNotNull(settings.Context);
      Assert.IsFalse(settings.RunAllDependencies);

      Assert.IsNotNull(settings.DotNetCoreSettings);

      Assert.IsNotNull(settings.DotNetCoreSettings.NugetSettings);
      Assert.IsNotNull(settings.DotNetCoreSettings.NugetSettings.Context);
      Assert.AreEqual(1, settings.DotNetCoreSettings.NugetSettings.NugetSources.Count());

      Assert.IsNotNull(settings.DotNetCoreSettings.BuildSettings);
      Assert.AreEqual("./BuildTemp", settings.DotNetCoreSettings.BuildSettings.BuildTempFolder);

      Assert.IsNotNull(settings.DotNetCoreSettings.TestSettings);
      Assert.AreEqual("./TestTemp", settings.DotNetCoreSettings.TestSettings.TestTempFolder);

      var newSettings = SingletonFactory.GetHelperSettings();
      Assert.AreEqual(settings, newSettings);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void GetCommandHelper_NoContext()
    {
      var helper = SingletonFactory.GetCommandHelper();
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void GetCommandHelper_Success()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;

      var helper = SingletonFactory.GetCommandHelper();

      Assert.IsNotNull(helper);
      Assert.IsNotNull(helper.Context);
      Assert.AreEqual(context, helper.Context);

      var newHelper = SingletonFactory.GetCommandHelper();
      Assert.AreEqual(helper, newHelper);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void GetDotNetCoreHelper_Success()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;

      var helper = SingletonFactory.GetDotNetCoreHelper();

      Assert.IsNotNull(helper);
      Assert.IsNotNull(helper.Context);
      Assert.AreEqual(context, helper.Context);

      var newHelper = SingletonFactory.GetDotNetCoreHelper();
      Assert.AreEqual(helper, newHelper);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void GetTaskHelper_NoContext()
    {
      var taskHelper = SingletonFactory.GetTaskHelper();
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void GetTaskHelper_Success()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;

      var taskHelper = SingletonFactory.GetTaskHelper();

      Assert.IsNotNull(taskHelper);
      Assert.IsNotNull(taskHelper.Context);
      Assert.AreEqual(context, taskHelper.Context);

      var newHelper = SingletonFactory.GetTaskHelper();
      Assert.AreEqual(taskHelper, newHelper);
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

    #endregion
  }
}