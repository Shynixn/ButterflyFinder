using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace OverLayApplicationSearch.Updater.Models
{
    internal class DownloadInstallUpdateModel
    {
        #region Private Fields

        private const string RELEASE_PASE = "https://github.com/Shynixn/ButterFlyFinder/releases/latest";

        private const string DOWNLOAD_PATH =
            "https://github.com/Shynixn/ButterFlyFinder/releases/download/RELEASE_VERSION/ButterFlyFinder.zip";

        private const string TEMP_DOWNLOAD_FILE = "Update.zip";
        private const string TREE_CONTENT = "/Shynixn/ButterflyFinder/tree";
        private const string CACHE_FILE_NAME = "cache.dat";

        private readonly string[] PROTECTED_FILES = new string[] {"storage.sqlite", "Update.zip"};

        #endregion

        #region Public Methods

        /// <summary>
        /// Calls the Web Site of the <see cref="RELEASE_PASE"/> and compares the version with the version 
        /// placed in the local <see cref="CACHE_FILE_NAME"/> file.
        /// </summary>
        /// <returns>isUpdate available</returns>
        public bool IsNewUpdateAvailable()
        {
            var currentVersion = GetCurrentVersion();
            var onlineVersion = GetOnlineVersion();

            return currentVersion != onlineVersion;
        }

        /// <summary>
        /// Downloads the latest release from the <see cref="RELEASE_PASE"/> into the current exe folder 
        /// and replaces all non storage files.
        /// </summary>
        public void DownloadLatestUpdate(DownloadProgressChangedEventHandler onProgressChange,
            Action<bool> onDownloadFinished)
        {
            var onlineVersion = GetOnlineVersion();

            using (var wc = new WebClient())
            {
                wc.DownloadProgressChanged += onProgressChange;
                wc.DownloadProgressChanged += (sender, args) =>
                {
                    if (args.ProgressPercentage < 100 || !File.Exists(TEMP_DOWNLOAD_FILE)) return;

                    DeleteNonProtectedFiles();

                    try
                    {
                        ZipFile.ExtractToDirectory(TEMP_DOWNLOAD_FILE, Directory.GetCurrentDirectory());
                        File.WriteAllText("cache.dat", onlineVersion);
                        File.Delete(TEMP_DOWNLOAD_FILE);
                        onDownloadFinished(true);
                    }
                    catch (IOException e)
                    {
                        Debug.WriteLine(e);
                        onDownloadFinished(false);
                    }
                };

                wc.DownloadFileAsync(new Uri(DOWNLOAD_PATH.Replace("RELEASE_VERSION", onlineVersion)),
                    TEMP_DOWNLOAD_FILE);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Deletes all non <see cref="PROTECTED_FILES"/>.
        /// </summary>
        private void DeleteNonProtectedFiles()
        {
            foreach (var file in Directory.GetDirectories(Directory.GetCurrentDirectory()))
            {
                try
                {
                    Directory.Delete(file, true);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory()))
            {
                try
                {
                    if (!PROTECTED_FILES.Contains(Path.GetFileName(file)))
                    {
                        File.Delete(file);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// Returns the current version from the local <see cref="CACHE_FILE_NAME"/> file.
        /// </summary>
        /// <returns></returns>
        private string GetCurrentVersion()
        {
            var currentVersion = "0";
            if (File.Exists(CACHE_FILE_NAME))
            {
                currentVersion = File.ReadAllText(CACHE_FILE_NAME);
            }

            return currentVersion;
        }


        /// <summary>
        /// Returns the latest version from the github repository <see cref="RELEASE_PASE"/>.
        /// </summary>
        /// <returns>version</returns>
        private string GetOnlineVersion()
        {
            var web = new HtmlWeb();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var doc = web.Load(RELEASE_PASE);

            return (from item in doc.DocumentNode.SelectNodes("//a")
                where item.Attributes["href"] != null
                      && item.Attributes["href"].Value.StartsWith("/Shynixn/ButterflyFinder/tree")
                select item.Attributes["href"].Value.Replace(TREE_CONTENT + "/", "")).FirstOrDefault();
        }

        #endregion
    }
}