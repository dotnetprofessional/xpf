namespace xpf.FileSystem
{
    public interface IWriteFileStream
    {
        IWriteFileStream WriteText(string text);
        IWriteFileStream WriteBytes(byte[] bytes);
        void Close();
    }
}