using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Helpers.Nuget
{
  /// <summary>
  /// Nuget Source Contract
  /// </summary>
  public interface INugetSource
  {
    /// <summary>
    /// Name of Feed
    /// </summary>
    string FeedName { get; set; }
    /// <summary>
    /// Feed URI
    /// </summary>
    string FeedSource { get; set; }
    /// <summary>
    /// Set true if Nuget Source is secured with user/pass
    /// </summary>
    bool IsSecure { get; set; }
    /// <summary>
    /// Secured username
    /// </summary>
    string Username { get; set; }
    /// <summary>
    /// Secured password
    /// </summary>
    string Password { get; set; }
  }
}
