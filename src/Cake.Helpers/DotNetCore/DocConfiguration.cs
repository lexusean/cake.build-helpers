using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;

namespace Cake.Helpers.DotNetCore
{
  /// <summary>
  /// Documentation framework types for DotNetCore
  /// </summary>
  public enum DocTypeEnum
  {
    /// <summary>
    /// Uses Docfx
    /// </summary>
    Default,
    /// <summary>
    /// Uses Docfx
    /// </summary>
    Docfx
  }

  /// <summary>
  /// Documentation Configuration Contract
  /// </summary>
  /// <inheritdoc />
  public interface IDocConfiguration : IHelperContext
  {
    /// <summary>
    /// Documentation Framework
    /// </summary>
    DocTypeEnum DocType { get; set; }
    /// <summary>
    /// Name of documentation configuration
    /// </summary>
    string Name { get; set; }
  }

  internal abstract class DocConfiguration : IDocConfiguration
  {
    public ICakeContext Context { get; set; }
    public DocTypeEnum DocType { get; set; }
    public string Name { get; set; } = "Unknown";
  }
}
