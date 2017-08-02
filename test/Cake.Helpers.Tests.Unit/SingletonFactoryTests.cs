using System;
using System.Collections.Generic;
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
      SingletonFactory.Context = null;
    }

    #endregion

    #region Test Methods

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