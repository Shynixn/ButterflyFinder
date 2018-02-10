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

namespace OverLayApplicationSearch.Updater
{
    public partial class UpdateControlWindow : Form
    {
        const string Program_Name = "ButterflyFinder";
        private bool updated = false;
        private string onlineVersion;
        private string[] protected_Files = new string[] { "storage.sqlite", "Update.zip" };

        public UpdateControlWindow()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await CheckLatestVersionAndUpdate();
        }

        private Task CheckLatestVersionAndUpdate()
        {  
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string currentVersion = "0";
                    if (File.Exists("cache.dat"))
                    {
                        currentVersion = File.ReadAllText("cache.dat");
                    }
                    onlineVersion = RetrieveVersion();
                    if (currentVersion.Equals(onlineVersion))
                    {
                        this.setLabelMessage("You already have the latest version.");
                        CloseForm();
                    }
                    else
                    {
                        this.setLabelMessage("Downloading latest release...");                      
                        DownloadReleaseZip(onlineVersion);
                    }
                }
                catch(Exception ex)
                {
                    this.setLabelMessage("Failed to check for updates. Are you connected to the internet?");
                    CloseForm();
                }             
            });   
        }

        private void CloseForm()
        {
            Thread.Sleep(1500);
            this.labelMessage.Invoke((MethodInvoker)delegate {
                this.Close();
            });
        }


        private void setLabelMessage(string message)
        {
            this.labelMessage.Invoke((MethodInvoker)delegate {
                labelMessage.Text = message;
            });
        }


        private void DownloadReleaseZip(string releaseVersion)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri("https://github.com/Shynixn/" + Program_Name + "/releases/download/" + releaseVersion + "/" + Program_Name + ".zip"),
                "Update.zip");
            }     
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBar1.Invoke((MethodInvoker)delegate {
                this.progressBar1.Value = e.ProgressPercentage;
            });
            if(e.ProgressPercentage >= 100 && updated == false) {
                updated = true;
                this.setLabelMessage("Unzipping and replacing existing files...");
                foreach (string file in Directory.GetDirectories(Directory.GetCurrentDirectory()))
                {
                    try
                    {
                        Directory.Delete(file, true);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory()))
                {
                    try
                    {

                        if (!this.protected_Files.Contains(Path.GetFileName(file)))
                        {
                            File.Delete(file);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                ZipFile.ExtractToDirectory("Update.zip", Directory.GetCurrentDirectory());
                File.WriteAllText("cache.dat", onlineVersion);
                this.setLabelMessage("Finished.");
                try
                {

                    Process.Start("butterflyfinder.exe");
                }
                catch (Exception ex)
                {

                }
                CloseForm();
            }
        }

        private string RetrieveVersion()
        {
            HtmlWeb web = new HtmlWeb();
            var doc = web.Load("https://github.com/Shynixn/" + Program_Name + "/releases/latest");
            foreach (var item in doc.DocumentNode.SelectNodes("//a"))
            {
                if (item.Attributes["href"] != null && item.Attributes["href"].Value.StartsWith("/Shynixn/ButterflyFinder/tree"))
                {
                    return item.Attributes["href"].Value.Replace("/Shynixn/ButterflyFinder/tree/", "");
                }
            }
            return null;
        }
    }
}
