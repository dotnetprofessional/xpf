using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xpf.FileSystem
{
    public class FolderExitsException:Exception
    {
        public FolderExitsException()
        {
        }

        public FolderExitsException(string message) : base(message)
        {
        }
    }
}
