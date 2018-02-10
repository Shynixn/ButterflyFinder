using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OverLayApplicationSearch.Contract.Persistence.Entity;
using OverLayApplicationSearch.Logic;
using static OverLayApplicationSearch.Contract.Persistence.Enumeration.TimeSchedule;

namespace OverLayApplicationSearch.Test
{
    [TestClass]
    public class SearchTest
    {
        private static DirectoryInfo CreateTestDirectory(string testDirectory)
        {
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
            DirectoryInfo info = Directory.CreateDirectory(testDirectory);
            Directory.CreateDirectory(testDirectory + "/users");
            Directory.CreateDirectory(testDirectory + "/temp");
            Directory.CreateDirectory(testDirectory + "/program files");
            File.Create(testDirectory + "/log.txt");
            Directory.CreateDirectory(testDirectory + "/users/" + "Christoph");
            File.Create(testDirectory + "/temp/dictionary.java");
            File.Create(testDirectory + "/temp/sample.java");
            File.Create(testDirectory + "/temp/eternal.cs");
            Directory.CreateDirectory(testDirectory + "/temp/" + "MyProject");
            File.Create(testDirectory + "/program files/chrome.exe");
            File.Create(testDirectory + "/program files/chrome.log");
            return info;
        }

        [TestMethod]
        public void SimpleScan()
        {
            if (File.Exists("storage.sqlite"))
            {
                File.Delete("storage.sqlite");
            }
            DirectoryInfo info = CreateTestDirectory("testtemp");
            IConfiguredTask task;
            Factory.ReInitializeContext();
            using (var taskController = Factory.CreateConfiguredTaskController())
            {
                task = taskController.Create();
                task.Path = info.FullName;
                task.TimeScheduled = HOURS_12;
                taskController.Store(task);
            }

            using (var scanner = Factory.CreateScanner(task))
            {
               Assert.AreEqual(12, scanner.GetAmountOfFiles(task.Path));
               scanner.Scan(task.Path);
               Assert.AreEqual(21, scanner.Count);              
            }

            using (var controller = Factory.CreateFilecacheController())
            {
                Assert.AreEqual(21, controller.Count);
            }
        }



        [TestMethod]
        public void ScanAndSearch()
        {
            if (File.Exists("storage.sqlite"))
            {
                File.Delete("storage.sqlite");
            }
            DirectoryInfo info = CreateTestDirectory("tester");
            IConfiguredTask task;
            Factory.ReInitializeContext();
            using (var taskController = Factory.CreateConfiguredTaskController())
            {
                task = taskController.Create();
                task.Path = info.FullName;
                task.TimeScheduled = HOURS_12;
                taskController.Store(task);
            }

            using (var scanner = Factory.CreateScanner(task))
            {
                Assert.AreEqual(12, scanner.GetAmountOfFiles(task.Path));
                scanner.Scan(task.Path);
                Assert.AreEqual(21, scanner.Count);
            }

            using (var controller = Factory.CreateFilecacheController())
            {
                Assert.AreEqual(21, controller.Count);
                List<string> result1 = controller.Search("a", 100);
                Assert.AreEqual(7, result1.Count);
            }
        }
    }
}
