using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class ConfiguredTaskSqliteTest
    {
        [TestMethod]
        public void SimpleInsertAndSelect()
        {
            if (File.Exists("storage.sqlite"))
            {
                File.Delete("storage.sqlite");
            }
            Factory.ReInitializeContext();
            using (var controller = Factory.CreateConfiguredTaskController())
            {
                IConfiguredTask task1 = controller.Create();
                task1.Path = "C:/Temp";
                task1.TimeScheduled = HOURS_12;              
                controller.Store(task1);
                Assert.AreEqual(1, controller.Count);
                Assert.AreNotEqual(0, task1.Id);

                IConfiguredTask task2 = controller.GetAll()[0];
                Assert.AreEqual(task1.Id, task2.Id);
                Assert.AreEqual(task1.Path, task2.Path);
                Assert.AreEqual(task1.TimeScheduled, task2.TimeScheduled);
                Assert.AreEqual(task1.LastTimeIndexed, task2.LastTimeIndexed);

                IConfiguredTask task3 = controller.GetById(task1.Id);
                Assert.AreEqual(task1.Path, task3.Path);
                Assert.AreEqual(task1.TimeScheduled, task3.TimeScheduled);

                controller.Remove(task3);
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
            using (var controller = Factory.CreateConfiguredTaskController())
            {
                IConfiguredTask task1 = controller.Create();
                task1.Path = "C:/Temp";
                task1.TimeScheduled = HOURS_12;
                controller.Store(task1);
                Assert.AreEqual(1, controller.Count);
                Assert.AreNotEqual(0, task1.Id);

                IConfiguredTask task2 = controller.Create();
                task2.Path = "D:/Temp";
                task2.TimeScheduled = HOURS_12;
                task2.LastTimeIndexed = DateTime.Now;
                controller.Store(task2);
                Assert.AreEqual(2, controller.Count);

                IConfiguredTask downloadedTask2 = controller.GetById(task2.Id);
                Assert.AreEqual(task2.LastTimeIndexed.Second, downloadedTask2.LastTimeIndexed.Second);
                Assert.AreEqual(task2.Path, downloadedTask2.Path);

                downloadedTask2.LastTimeIndexed = downloadedTask2.LastTimeIndexed.AddDays(5);
                controller.Store(downloadedTask2);

                IConfiguredTask againDownloadedTask2 = controller.GetById(task2.Id);
                Assert.AreEqual(downloadedTask2.LastTimeIndexed, againDownloadedTask2.LastTimeIndexed);
                Assert.AreEqual(task1.Path, controller.GetById(task1.Id).Path);
            }
        }
    }
}
