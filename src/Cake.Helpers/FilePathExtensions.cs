using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.Helpers
{
  public static class FilePathExtensions
  {
    public static FilePath MakeRelative(this ICakeContext context, FilePath file)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      if (file == null)
        throw new ArgumentNullException(nameof(file));

      var cwd = context.Directory("./");

      return context.MakeAbsolute(cwd).GetRelativePath(context.MakeAbsolute(file));
    }
  }
}
