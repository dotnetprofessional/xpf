using System;
using FluentAssertions;
using Machine.Specifications;

/*
 * 0	Desktop			        C:\Documents and Settings\Charlie\Desktop
 * 2	Programs		        C:\Documents and Settings\Charlie\Start Menu\Programs
 * 5	Personal		        D:\documents
 * 6	Favorites		        C:\Documents and Settings\Charlie\Favorites
 * 8	Recent			        C:\Documents and Settings\Charlie\Recent
 * 9	SendTo			        C:\Documents and Settings\Charlie\SendTo
 * 11	StartMenu		        C:\Documents and Settings\Charlie\Start Menu
 * 13	MyMusic			        D:\documents\My Music
 * 16	DesktopDirectory	    C:\Documents and Settings\Charlie\Desktop
 * 17	MyComputer			 
 * 26	ApplicationData		    C:\Documents and Settings\Charlie\Application Data
 * 28	LocalApplicationData	C:\Documents and Settings\Charlie\Local Settings\Application Data
 * 32	InternetCache		    C:\Documents and Settings\Charlie\Local Settings\Temporary Internet Files
 * 33	Cookies			        C:\Documents and Settings\Charlie\Cookies
 * 34	History			        C:\Documents and Settings\Charlie\Local Settings\History
 * 35	CommonApplicationData	C:\Documents and Settings\All Users\Application Data
 * 37	System			        C:\WINDOWS\System32
 * 38	ProgramFiles		    C:\Program Files
 * 39	MyPictures		        D:\documents\My Pictures
 * 43	CommonProgramFiles	    C:\Program FilesCommon Files
 */

namespace xpf.FileSystem.Test.PathSpec
{
    [Subject("Dependencies")]
    public class PathSpec
    {
        protected static string Filename;
        protected static IPath Subject;
        protected static string UserAlias;

        Establish context = () =>
        {
            // ... any mocking, stubbing, or other setup ...
            Filename = "Test.txt";

            Subject = new FileSystem().Path;
            UserAlias = Environment.UserName;
        };

        /// <summary>
        ///     As these tests are around file system locations, they differ depending on OS version. These tests were performed on
        ///     Windows 8.1
        /// </summary>
        It should_be_running_windows_8 =
            () => Environment.OSVersion.Version.Major.Should().BeGreaterOrEqualTo(6);

        [Subject("")]
        public class When_path_initialized 
        {
            It should_return_the_current_system_folder = () => Subject.Current.Path.Should().Be(Environment.CurrentDirectory + "\\");

            It should_return_the_local_folder = () => Subject.Local.Path.Should().Be(string.Format(@"C:\Users\{0}\AppData\Local\", UserAlias));

            It should_return_the_roaming_folder = () => Subject.Roaming.Path.Should().Be(string.Format(@"C:\Users\{0}\Documents\", UserAlias));

            It should_return_the_root_folder = () => Subject.Root.Path.Should().Be(@"C:\");

            It should_return_the_temporary_folder = 
                () => Subject.Temporary.Path.Should().Be(string.Format(@"C:\Users\{0}\AppData\Local\Temp\", UserAlias));
        }
    }
}