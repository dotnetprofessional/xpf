namespace xpf.FileSystem
{
    public interface IFile
    {
        string Name { get; }
        IFile WithFailIfExists { get; }
        IFile WithReplaceIfExits { get; }
        IWriteFileStream Create();
        IReadFileStream Open();
        void Delete();
    }
}