namespace xpf.FileSystem
{
    public class ReadFileStream : IReadFileStream
    {
        public IReadFileStream ReadLine()
        {
            return new ReadFileStream();
        }
    }
}