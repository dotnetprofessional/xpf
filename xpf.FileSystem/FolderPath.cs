using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace xpf.FileSystem
{
    public class FolderPath : IFolderPath
    {
        internal bool ShouldFailIfExits { get; set; }

        public FolderPath(string path)
        {
            this.Path = path;
        }

        public bool Exists()
        {
            return System.IO.Directory.Exists(this.Path);
        }

        public IFolderPath FailIfExists
        {
            get
            {
                var clone = this.MemberwiseClone() as FolderPath;
                clone.ShouldFailIfExits = true;
                return clone;
            }
        }

        public IFile File(string filename)
        {
            return new File(filename);
        }

        public List<IFile> LocalSearch(string criteria)
        {
            if (string.IsNullOrEmpty(criteria))
                criteria = "*.*";

            var files = Directory.GetFiles(this.Path, criteria, SearchOption.TopDirectoryOnly);

            return files.Select(f => new File(f)).Cast<IFile>().ToList();
        }

        public List<IFile> Search(string criteria)
        {
            if (string.IsNullOrEmpty(criteria))
                criteria = "*.*";

            var files = Directory.GetFiles(this.Path, criteria, SearchOption.AllDirectories);

            return files.Select(f => new File(f)).Cast<IFile>().ToList();
        }

        public IFolderPath Folder(string name)
        {
            var newPath = this.Path + name;
            if (!newPath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
                newPath = newPath + System.IO.Path.DirectorySeparatorChar;

            this.Path = newPath;

            return this;
        }

        public IFolderPath Create()
        {
            if (ShouldFailIfExits && this.Exists())
                throw new FolderExitsException(this.Path);

            Directory.CreateDirectory(this.Path);
            return this;
        }

        public IFolderPath Delete()
        {
            Directory.Delete(this.Path);
            return Previous();
        }

        public IFolderPath Previous()
        {
            // As every folder has a trailing \ start to look before that to find the end of the folder
            // Again as a folder includes the trailing space ensure to include it in the result
            var lastIndex = this.Path.LastIndexOf('\\', this.Path.Length - 2);
            this.Path = this.Path.Substring(0, lastIndex + 1);
            return this;
        }

        public string Path { get; private set; }
    }
}