namespace xpf.FileSystem
{
    public class File : IFile
    {
        public File(string name)
        {
            this.Name = name;
        }

        public string Name { get; internal set; }

        public IFile WithFailIfExists { get { return this; } }

        public IFile WithReplaceIfExits { get { return this; } }

        public IWriteFileStream Create()
        {
            return new WriteFileStream();
        }

        public IReadFileStream Open()
        {
            return new ReadFileStream();
        }

        public void Delete() { }
    }
}