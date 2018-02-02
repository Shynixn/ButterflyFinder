using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OverLayApplicationSearch.Contract.Business.Entity;
using OverLayApplicationSearch.Contract.Business.Service;
using System.IO;
using OverLayApplicationSearch.Logic.Lib;

namespace OverLayApplicationSearch.Logic.Business.Service
{
    public class SearchService : ISearchService
    {
        private List<string> result;
        private readonly Icon fallBackIcon;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public SearchService(Icon fallBackIcon)
        {
            this.fallBackIcon = fallBackIcon;
        }

        /// <inheritdoc />
        /// <summary>
        ///  Initializes a single viewModel by the item in the position.
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="viewModel">Model</param>
        public bool InitModel(int position, IViewModel viewModel)
        {
            var line = this.result[position];
            try
            {
                SetViewHolder(viewModel, line,
                    line.EndsWith(".exe") ? line : WindowsFileExtension.GetDefaultExecutablePathFromFile(line));
            }
            catch (Exception)
            {
                viewModel.Text = Path.GetFileName(line);
                viewModel.Header = line;
                viewModel.Icon = fallBackIcon.GenerateImageSource();
            }
            return true;
        }

        /// <inheritdoc />
        /// <summary>
        /// Handles an action when an viewModel item is pressed or clicked.
        /// </summary>
        /// <param name="position">position</param>
        /// <param name="viewModel">viewModel</param>
        public void OnAction(int position, IViewModel viewModel)
        {
            try
            {
                ProcessExtension.LaunchProcess(viewModel.Header, viewModel.Cache as string);
            }
            catch (Exception)
            {
                try
                {
                    var path = new FileInfo(viewModel.Header).Directory?.FullName;
                    ProcessExtension.LaunchProcess(path, null);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns the amount of items.
        /// </summary>
        public int Count => this.result.Count;

        /// <inheritdoc />
        /// <summary>
        /// Searches for the given text in the database.
        /// </summary>
        /// <param name="text">text.</param>
        public async Task Search(string text)
        {
            this.result = await SearchTask(text);
        }

        private static void SetViewHolder(IViewModel model, string path, string exePath)
        {
            model.Text = Path.GetFileName(path);
            model.Header = path;
            model.Icon = exePath.GenerateImageSource();
            model.Cache = exePath;
        }

        private static Task<List<string>> SearchTask(string searchText)
        {
            return Task<List<string>>.Factory.StartNew(() =>
            {
                using (var controller = Factory.CreateFilecacheController())
                {
                    return controller.Search(searchText, 100);
                }
            });
        }
    }
}