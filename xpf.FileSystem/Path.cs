using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Proxies;
using System.Text;

namespace xpf.FileSystem
{
    /*
    Path.Current.File("xxxx").Delete()
    Path.File("xxxx").Delete()
    Path.Current.File("XXX").Create()
    Path.Current.Folder("XXX").Create()
    Path.Current.Folder("XXX").WithFailIfExits.Create
    Path.Current.Setting("name")
    Path.Current.Setting("name").Value("XXX")
    Path.Currnet.File("XXX").Read()
 * 

*/

    public class TestAPI
    {
        public void Test()
        {
            var fs = new FileSystem();

            fs.Path.Current.File("XXX").Create().WriteText("XXXX").Close();
            fs.Path.Current.LocalSearch("*.pdf")[0].WithFailIfExists.Create();
            fs.Path.Current.File("xxxx").Delete();
            //fs.Path.File("xxxx").Delete();
            fs.Path.Current.File("XXX").Create();
            fs.Path.Current.Folder("XXX").Create();
            fs.Path.Current.Folder("XXX").FailIfExists.Create();
            fs.Path.Current.Folder("XXX").Exists();
            //fs.Path.Current.Setting("name");
            //fs.Path.Current.Setting("name").Value("XXX");
            fs.Path.Current.File("XXX").Open();
        }
    }

    public class  Path : IPath
    {
        public IFolderPath Current { get { return new FolderPath(Environment.CurrentDirectory + "\\"); } }

        public IFolderPath Local { get { return new FolderPath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\"); } }

        public IFolderPath Roaming
        {
            get
            {
                return new FolderPath(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\");
            }
        }

        public IFolderPath Temporary
        {
            get { return new FolderPath(System.IO.Path.GetTempPath()); }
        }

        public IFolderPath Root
        {
            get { return new FolderPath(System.IO.Path.GetPathRoot(this.Local.Path)); }
        }
    }
}
