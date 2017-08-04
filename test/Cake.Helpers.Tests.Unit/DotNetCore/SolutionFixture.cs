using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.IO;
using Cake.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cake.Helpers.Tests.Unit.DotNetCore
{
  public class ProjectFixture
  {
    internal string ProjectFilePath { get; set; }
    private FakeFileSystem FakeFs { get; set; }
    public FakeFile ProjFile { get; private set; }

    internal ProjectFixture(
      string projFile,
      IFileSystem fs)
    {
      if (string.IsNullOrWhiteSpace(projFile))
        throw new ArgumentNullException(nameof(projFile));

      var fakeFs = fs as FakeFileSystem;
      if (fakeFs == null)
        throw new ArgumentNullException(nameof(fs));

      this.ProjectFilePath = projFile;
      this.FakeFs = fakeFs;
    }

    public FakeFile SetContents(string contents)
    {
      this.ProjFile = this.FakeFs.CreateFile(this.ProjectFilePath);
      this.ProjFile.SetContent(contents);

      return this.ProjFile;
    }
  }

  internal class SolutionFixture
  {
    public static SolutionFixture GetDefaultSolution(IFileSystem fs)
    {
      var slnFilePath = Properties.Resources.DefaultSlnFileLocation;
      var sln = new SolutionFixture(slnFilePath, fs);
      sln.SetContents(Properties.Resources.DefaultSlnContent);
      sln.AddProject(Properties.Resources.DefaultProject1Location, Properties.Resources.DefaultProject1Content);
      sln.AddProject(Properties.Resources.DefaultProject2Location, Properties.Resources.DefaultProject2Content);

      return sln;
    }

    internal string SlnFilePath { get; set; }
    private FakeFileSystem FakeFs { get; set; }
    public List<ProjectFixture> Projects { get; private set; } = new List<ProjectFixture>();
    public FakeFile SlnFile { get; private set; }

    internal SolutionFixture(
      string slnFile,
      IFileSystem fs)
    {
      if(string.IsNullOrWhiteSpace(slnFile))
        throw new ArgumentNullException(nameof(slnFile));

      var fakeFs = fs as FakeFileSystem;
      if (fakeFs == null)
        throw new ArgumentNullException(nameof(fs));

      this.SlnFilePath = slnFile;
      this.FakeFs = fakeFs;
    }

    public FakeFile SetContents(string contents)
    {
      this.SlnFile = this.FakeFs.CreateFile(this.SlnFilePath);
      this.SlnFile.SetContent(contents);

      return this.SlnFile;
    }

    private void AddProject(string projectFileLocation, string contents)
    {
      var newProj = new ProjectFixture(projectFileLocation, this.FakeFs);
      newProj.SetContents(contents);
      
      this.Projects.Add(newProj);
    }
  }
}
