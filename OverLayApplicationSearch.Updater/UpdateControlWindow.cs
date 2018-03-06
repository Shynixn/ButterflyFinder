using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OverLayApplicationSearch.Updater.Models;

namespace OverLayApplicationSearch.Updater
{
    public partial class UpdateControlWindow : Form
    {
        #region Private Fields

        private readonly DownloadInstallUpdateModel model = new DownloadInstallUpdateModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="UpdateControlWindow"/>.
        /// </summary>
        public UpdateControlWindow()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.logo;
        }

        #endregion

        #region Private Events

        /// <summary>
        /// Gets called on load and checks for updates.
        /// </summary>
        /// <param name="sender"><see cref="sender"/></param>
        /// <param name="e"><see cref="downloadProgressChangedEventArgs"/></param>
        private void onFormLoadEvent(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        /// <summary>
        /// Gets called from an event to set the task completed in <see cref="OnProgressChange"/>.
        /// </summary>
        /// <param name="success">was the task a success=</param>
        private void OnDownloadFinished(bool success)
        {
            this.progressBar1.Invoke((MethodInvoker) delegate
            {
                this.progressBar1.Value = 100;

                if (success)
                {
                    this.labelMessage.Text = "Installing Update was successful.";
                    this.buttonRestartButterflyFinder.Visible = true;
                }
                else
                {
                    this.labelMessage.Text = "Failed to install Update. Please try again later.";
                }
            });
        }

        /// <summary>
        /// Gets called from an event to display the progress of a background task.
        /// </summary>
        /// <param name="sender"><see cref="sender"/></param>
        /// <param name="downloadProgressChangedEventArgs"><see cref="downloadProgressChangedEventArgs"/></param>
        private void OnProgressChange(object sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            this.progressBar1.Invoke((MethodInvoker) delegate
            {
                this.progressBar1.Value = downloadProgressChangedEventArgs.ProgressPercentage;
            });
        }

        /// <summary>
        /// Closes the current form when being clicked on the <see cref="buttonRestartButterflyFinder"/>.
        /// </summary>
        /// <param name="sender"><see cref="sender"/></param>
        /// <param name="e"><see cref="e"/></param>
        private void buttonRestartButterflyFinder_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("butterflyfinder.exe");
            }
            catch (Exception)
            {
                // ignored
            }
            this.Close();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks asynchronly if a new version can be installed and starts the download.
        /// </summary>
        private async void CheckForUpdates()
        {
            this.labelMessage.Text = "Checking for version...";

            var task = Task.Factory.StartNew(() => this.model.IsNewUpdateAvailable());
            await Task.WhenAll(task);

            if (task.Result)
            {
                this.labelMessage.Text = "Downloading Update...";

                await Task.Run(() => this.model.DownloadLatestUpdate(OnProgressChange, OnDownloadFinished));
            }
            else
            {
                this.labelMessage.Text = "You have already got the latest version.";
            }
        }

        #endregion
    }
}