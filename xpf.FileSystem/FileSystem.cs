using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xpf.FileSystem
{
    public class FileSystem : IFileSystem
    {
        public IPath Path { get { return new Path(); } }
    }
}
