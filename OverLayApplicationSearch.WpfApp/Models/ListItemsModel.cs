using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OverLayApplicationSearch.Contract.Persistence.Entity;
using OverLayApplicationSearch.Logic;

namespace OverLayApplicationSearch.WpfApp.Models
{
    internal class ListItemsModel
    {
        /// <summary>
        /// Returns a new <see cref="ObservableCollection{T}"/> of tasks from Task Repository.
        /// </summary>
        /// <returns>ObservableCollection{T}</returns>
        public ObservableCollection<IConfiguredTask> GetAllTasks()
        {
            using (var controller = Factory.CreateConfiguredTaskController())
            {
                return new ObservableCollection<IConfiguredTask>(controller.GetAll());
            }
        }

        /// <summary>
        /// Deletes the given <see cref="tasks"/> from Task Repository.
        /// </summary>
        /// <param name="tasks"><see cref="IEnumerator{T}"/></param>
        public void DeleteTasks(IEnumerable<IConfiguredTask> tasks)
        {
            using (var controller = Factory.CreateConfiguredTaskController())
            {
                foreach (var task in tasks)
                {
                    controller.Remove(task);
                }
            }
        }
    }
}