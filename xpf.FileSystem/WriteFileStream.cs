namespace xpf.FileSystem
{
    public class  WriteFileStream : IWriteFileStream
    {
        public IWriteFileStream WriteText(string text)
        {
            return new WriteFileStream();
        }

        public IWriteFileStream WriteBytes(byte[] bytes)
        {
            return new WriteFileStream();
        }

        public void Close() { }
    }
}