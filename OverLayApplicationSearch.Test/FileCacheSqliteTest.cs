using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OverLayApplicationSearch.Contract.Persistence.Entity;
using OverLayApplicationSearch.Contract.Persistence.Enumeration;
using OverLayApplicationSearch.Logic;

namespace OverLayApplicationSearch.Test
{
    [TestClass]
    public class FileCacheSqliteTest
    {
        [TestMethod]
        public void SimpleInsertAndSelect()
        {
            if (File.Exists("storage.sqlite"))
            {
                File.Delete("storage.sqlite");
            }
            Factory.ReInitializeContext();
            using (var controller = Factory.CreateFilecacheController())
            {
                IFileCache cache1 = controller.Create();
                cache1.FileName = "C:/";      
                controller.Store(cache1);
                Assert.AreEqual(1, controller.Count);
                Assert.AreNotEqual(0, cache1.Id);

                IFileCache cache2 = controller.GetAll()[0];
                Assert.AreEqual(cache1.Id, cache2.Id);
                Assert.AreEqual(cache1.FileName, cache2.FileName);

                IFileCache cache3 = controller.GetById(cache1.Id);
                Assert.AreEqual(cache1.Id, cache2.Id);
                Assert.AreEqual(cache1.FileName, cache2.FileName);

                controller.Remove(cache3);
                Assert.AreEqual(0, controller.Count);
            }
        }

        [TestMethod]
        public void MultipleInsertAndSelect()
        {
            if (File.Exists("storage.sqlite"))
            {
                File.Delete("storage.sqlite");
            }
            Factory.ReInitializeContext();
            using (var controller = Factory.CreateFilecacheController())
            {
                IFileCache cache1 = controller.Create();
                cache1.FileName = "C:/";
                controller.Store(cache1);
                Assert.AreEqual(1, controller.Count);
                Assert.AreNotEqual(0, cache1.Id);

                IFileCache cache2 = controller.Create();
                cache2.FileName = "Temp";
                controller.Store(cache2);
                Assert.AreEqual(2, controller.Count);

                IFileCache downloadedCache2 = controller.GetById(cache2.Id);
                Assert.AreEqual(cache2.FileName, downloadedCache2.FileName);

                controller.Remove(downloadedCache2);
                Assert.AreEqual(1, controller.Count);

                IFileCache downloadedCache1 = controller.GetById(cache1.Id);
                Assert.AreEqual(cache1.FileName, downloadedCache1.FileName);
            }
        }



        [TestMethod]
        public void InsertAndSelectWithTask()
        {
            if (File.Exists("storage.sqlite"))
            {
                File.Delete("storage.sqlite");
            }
            Factory.ReInitializeContext();
            using (var fileCacheController = Factory.CreateFilecacheController())
            {
                using (var taskController = Factory.CreateConfiguredTaskController())
                {
                    IConfiguredTask task1 = taskController.Create();
                    task1.Path = "C:";
                    task1.TimeScheduled = TimeSchedule.HOURS_12;
                    taskController.Store(task1);
                    Assert.AreEqual(1, taskController.Count);

                    IFileCache cache1 = fileCacheController.Create();
                    cache1.FileName = "C:";
                    cache1.ParentId = 0;
                    cache1.TimeTaskId = task1.Id;
                    fileCacheController.Store(cache1);

                    IFileCache cache2 = fileCacheController.Create();
                    cache2.FileName = "Temp";
                    cache2.ParentId = cache1.Id;
                    cache2.TimeTaskId = task1.Id;
                    fileCacheController.Store(cache2);

                    IFileCache cache3 = fileCacheController.Create();
                    cache3.FileName = "Users";
                    cache3.ParentId = cache1.Id;
                    cache3.TimeTaskId = task1.Id;
                    fileCacheController.Store(cache3);

                    IFileCache downloaded2 = fileCacheController.GetById(cache2.Id);
                    Assert.AreEqual(cache2.FileName, downloaded2.FileName);
                    Assert.AreEqual(cache2.TimeTaskId, downloaded2.TimeTaskId);
                    Assert.AreEqual(cache3.ParentId, downloaded2.ParentId);
                }
            }
        }
    }
}
