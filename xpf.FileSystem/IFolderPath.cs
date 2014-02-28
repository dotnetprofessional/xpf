using System.Collections.Generic;

namespace xpf.FileSystem
{
    public interface IFolderPath
    {
        bool Exists();
        string Path { get; }

        IFolderPath FailIfExists { get; }
        IFile File(string filename);
        List<IFile> LocalSearch(string criteria);
        List<IFile> Search(string criteria);
        IFolderPath Folder(string name);
        IFolderPath Create();

        IFolderPath Delete();

        IFolderPath Previous();
    }
}