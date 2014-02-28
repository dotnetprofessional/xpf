using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Machine.Specifications;

namespace xpf.FileSystem.Spec.FolderPathSpec
{
    [Subject("")]
    public class FolderPathSpec
    {
        protected static IFolderPath Subject;
        protected static string Filename = "test.txt";
        private static string TempPath;
        protected static List<string> TestFiles = new List<string>();
        static IFolderPath FolderWithFiles;

        Establish context = () =>
        {
            TestFiles = new List<string>();
            TempPath = new Path().Temporary.Path + "unittest\\";

            for (int i = 0; i < 10; i++)
                TestFiles.Add(TempPath + String.Format("test file{0}.unittest", i));
            for (int i = 0; i < 5; i++)
                TestFiles.Add(TempPath + String.Format("test file{0}.txt", i));
            for (int i = 0; i < 5; i++)
            {
                TestFiles.Add(String.Format("{0}subdirectory\\test file{1}.unittest", TempPath, i));
                TestFiles.Add(String.Format("{0}subdirectory\\test file{1}.txt", TempPath, i));
            }

            if (!Directory.Exists(TempPath))
            {
                Directory.CreateDirectory(TempPath);
                Directory.CreateDirectory(TempPath + "subdirectory");
                // Create some test files to test against
                foreach (var file in TestFiles)
                {
                    var f = System.IO.File.CreateText(file);
                    f.WriteAsync("test").Wait();
                    f.Close();
                }
            }
            // ... any mocking, stubbing, or other setup ...
            Subject = new FolderPath(new Path().Temporary.Path);
            FolderWithFiles = Subject.Folder("unittest");
        };


        Cleanup after = () =>
        {
            // Only run if the test setup needs to change
            //System.IO.Directory.Delete(TempPath,true);
        };

        [Subject("Search")]
        public class When_searching_current_folder_using_search
        {

            It should_return_a_collection_of_files_matching_the_pattern = () => FolderWithFiles.Search("*.unittest").Count.Should().Be(TestFiles.Count(f=>f.EndsWith(".unittest")));

            It shouild_return_an_empty_collection_if_no_files_match_pattern = () => FolderWithFiles.Search("*.nomatch").Count.Should().Be(0);

            It should_return_all_files_when_pattern_is_null = () => FolderWithFiles.Search(null).Count.Should().Be(TestFiles.Count);

            It should_return_all_files_when_pattern_is_empty_string = () => FolderWithFiles.Search("").Count.Should().Be(TestFiles.Count);
        }

        [Subject("LocalSearch")]
        public class When_searching_current_folder_using_local_search
        {
            It should_return_a_collection_of_files_matching_the_pattern_in_current_folder_only = () => FolderWithFiles.LocalSearch("*.unittest").Count.Should().Be(10);

            It shouild_return_an_empty_collection_if_no_files_match_pattern = () => FolderWithFiles.LocalSearch("*.nomatch").Count.Should().Be(0);

            It should_return_all_files_when_pattern_is_null_in_the_current_folder = () => FolderWithFiles.LocalSearch(null).Count.Should().Be(15);

            It should_return_all_files_when_pattern_is_empty_string_in_the_current_folder = () => FolderWithFiles.LocalSearch("").Count.Should().Be(15);
        }

        [Subject("Folder")]
        public class When_navigating_to_a_valid_child_folder
        {
            Because of = () => Subject = new FolderPath(new Path().Temporary.Path).Folder("unittest");

            It should_navigate_to_child_folder_if_it_exists = () => Subject.Exists().Should().BeTrue();
        }

        [Subject("Folder")]
        public class When_navigating_to_a_invalid_child_folder
        {
            Because of = () => Subject = new FolderPath(new Path().Temporary.Path).Folder("unknownfolder");

            It should_throw_Exception_when_navigating_to_a_child_folder_that_does_not_exist = () => Subject.Exists().Should().BeFalse();

        }

        [Subject("Create")]
        public class When_creating_a_folder_from_a_valid_path
        {
            Because of = () => Subject.Folder("new folder").Create();

            It should_create_the_new_folder = () => Subject.Exists().Should().BeTrue();

            It should_return_a_new_folder_path_instance_set_to_new_folder = () => Subject.Path.Should().EndWith("new folder\\");

            Cleanup clean = () => Subject.Delete();
        }

        [Subject("Create")]
        public class When_creating_a_folder_from_an_invalid_path
        {
            Establish context = () => Subject = new FolderPath(new Path().Temporary.Path).Folder("badfolder").Folder("new folder");

            Because of = () => { Subject.Create(); };

            It should_create_the_new_folder_with_full_path = () => Subject.Exists().Should().BeTrue();

            Cleanup clean = () => Subject.Delete();
        }

        [Subject("Create")]
        public class When_creating_a_folder_that_already_exists
        {
            static Exception Exception;

            Establish context = () => Exception = Catch.Exception(() =>
            {
                Subject = new FolderPath(new Path().Temporary.Path).Folder("new folder");
                Subject.Create(); // Create folder first (shouldn't fail)
            });

            Because of = () => Subject.Create();

            It should_not_throw_an_exception = () => Exception.Should().BeNull();

            Cleanup clean = () => Subject.Delete();
        }

        [Subject("Create")]
        public class When_creating_a_folder_that_already_exists_and_specifying_fail_if_exists
        {
            static Exception Exception;

            Establish context = () => Exception = Catch.Exception(() =>
            {
                Subject = new FolderPath(new Path().Temporary.Path).Folder("new folder");
                Subject.Create(); // Create folder first (shouldn't fail)
            });

            Because of = () => Exception = Catch.Exception(() => { Subject.FailIfExists.Create(); }); // Should fail

            It should_throw_an_exception = () => Exception.Should().BeOfType<FolderExitsException>();

            It should_have_a_message = () => Exception.Message.Should().Be(Subject.Path);

            Cleanup clean = () => Subject.Delete();
        }

        [Subject("Delete")]
        public class When_deleting_a_folder_that_exists
        {
            static Exception Exception;
            static FolderPath FolderToDelete;

            Establish context = () =>
            {
                Subject = new FolderPath(new Path().Temporary.Path).Folder("folder to delete");
                FolderToDelete = new FolderPath(Subject.Path);

                if(!Subject.Exists())
                    Subject.Create(); // Create folder first (shouldn't fail)
            };

            Because of = () => Exception = Catch.Exception(() =>
            {
                Subject.Delete();
            });

            It should_remove_folder = () => FolderToDelete.Exists().Should().BeFalse();

            Cleanup clean = () => Subject.Delete();
        }

        [Subject("Delete")]
        public class When_deleting_a_folder_that_doesnt_exist
        {
            static Exception Exception;

            Establish context = () =>
            {
                Subject = new FolderPath(new Path().Temporary.Path).Folder("folder to delete");
                if (!Subject.Exists())
                    Subject.Create(); // Create folder first (shouldn't fail)
            };

            Because of = () => Exception = Catch.Exception(() =>
            {
                Subject.Delete();
            });

            It should_not_throw_an_exception = () => Exception.Should().BeNull();

            Cleanup clean = () => Subject.Delete();
        }

        [Subject("Previous")]
        public class When_calling_previous
        {
            static FolderPath PreviousFolder { get; set; }

            Establish context = () =>
            {
                PreviousFolder = new FolderPath(new Path().Temporary.Path);
                Subject = new FolderPath(PreviousFolder.Path).Folder("new folder");
            };

            Because of = () => Subject.Previous();

            It should_return_the_folder_one_level_up = () => Subject.Path.Should().BeEquivalentTo(PreviousFolder.Path);
        }
    }
}