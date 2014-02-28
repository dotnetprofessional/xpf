using System.Collections.Generic;

namespace xpf.FileSystem
{
    public interface IPath
    {
        IFolderPath Current { get; }
        IFolderPath Local { get; }
        IFolderPath Roaming { get; }
        IFolderPath Temporary { get; }
        IFolderPath Root { get; }
    }
}