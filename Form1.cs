using Ionic.Zip;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
//using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTDToolbox_Updater
{
    public partial class Form1 : Form
    {

        Thread bgThread;
        WebClient client;

        string toolboxRelease;
        public string lastMessage;
        string downloadedString = "";
        string url = "https://raw.githubusercontent.com/TDToolbox/BTDToolbox-2019_LiveFIles/master/Version";

        int totalFiles;
        int filesTransfered;
        bool exit = false;

        public Form1()
        {
            InitializeComponent();            
            client = new WebClient();
        }

        //Events
        private void Form1_Shown(object sender, EventArgs e)
        {
            printToConsole("Program Initialized...");
            printToConsole("Welcome to BTD Toolbox 2019 auto-updater");
            printToConsole("Getting download link for latest version...");

            CheckURL();           
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CheckForExit();
            exit = true;
        }
        
        
        //Core functions
        private void printToConsole(string message)
        {
            if (message != lastMessage)
            {
                try
                {
                    Invoke((MethodInvoker)delegate {
                        Console.AppendText(">> " + message + "\r\n");
                        lastMessage = message;
                    });
                }
                catch
                { }
            }
        }
        private bool isToolboxRunning()
        {
            if (Process.GetProcessesByName("BTDToolbox").Length > 0)
                return true;
            else
                return false;
        }
        private void DeleteDirectory(string path, bool deleteProjs)
        {
            if (Directory.Exists(path))
            {
                printToConsole("Deleting directory..");
                var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

                foreach (FileInfo file in directory.GetFiles())
                {
                    if ((deleteProjs == true))
                    {
                        if (!file.ToString().Contains("BTDToolbox_Updater") && (!file.ToString().Contains("Backups")) && (!file.ToString().Contains(".dll")) && (!file.ToString().Contains(".json")))
                            file.Delete();
                    }
                    else
                    {
                        if (!file.ToString().Contains("BTDToolbox_Updater") && (!file.ToString().Contains("Backups")) && (!file.ToString().Contains("proj")) && (!file.ToString().Contains(".dll")) && (!file.ToString().Contains(".json")))
                            file.Delete();
                    }
                    

                }
                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    
                    if ((deleteProjs == true))
                    {
                        if ((dir.ToString() != Environment.CurrentDirectory) && (!dir.ToString().Contains("Backups")))
                            dir.Delete(true);
                    }
                    else
                    {
                        if ((dir.ToString() != Environment.CurrentDirectory) && (!dir.ToString().Contains("proj") && (!dir.ToString().Contains("Backups"))))
                            dir.Delete(true);
                    }
                }
                printToConsole("Directory deleted.");
            }
            else
            {
                printToConsole("Directory not found. Unable to delete directory at:\r\n" + path);
            }
        }
        private void CheckForExit()
        {
            if(exit)
            {
                if(File.Exists(Environment.CurrentDirectory + "\\BTDToolbox_Updater.zip"))
                    File.Delete(Environment.CurrentDirectory + "\\BTDToolbox_Updater.zip");
                if (File.Exists(Environment.CurrentDirectory + "\\Update"))
                    File.Delete(Environment.CurrentDirectory + "\\Update");
                Environment.Exit(0);
            }
        }


        //Get the URL with some error handling
        public void CheckURL()
        {
            Thread thread = new Thread(() => GetURL(url));
            thread.Start();
        }
        private void GetURL(string url)
        {
            bool success = false;
            
            try
            {
                downloadedString = client.DownloadString(url);
                //MessageBox.Show(downloadedString);
                success = true;
            }
            catch(Exception)
            {
                printToConsole("There was an issue reading the version file. Trying again... This will take up to 10 seconds...");
                if (success == false)
                {
                    for (int i = 0; i <= 100; i++)
                    {
                        if (!exit)
                        {
                            Thread.Sleep(100);
                            try
                            {
                                downloadedString = client.DownloadString(url);
                                if (downloadedString != null && downloadedString != "")
                                {
                                    success = true;
                                    break;
                                }
                                
                            }
                            catch { }
                        }
                        else
                        {
                            Environment.Exit(0);
                        }
                    }
                }
            }

            if (success == true)
            {
                start();
            }
            else
            {
                printToConsole("ERROR! The program was unable to determine the latest version of BTD Toolbox. You can continue to use toolbox like normal, or try reopening the program to check again...");
                for (int i = 0; i <= 120; i++)
                {
                    if (!exit)
                    {
                        Thread.Sleep(100);
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                }
            }
        }


        //Start update AFTER confirming the URL
        private void start()
        {
            string[] split = downloadedString.Split('\n');
            toolboxRelease = split[0].Replace("toolbox2019: ", "");
            printToConsole("Download link aquired!");

            CheckForExit();
            printToConsole("Checking if Toolbox is running..");
            if (isToolboxRunning() == false)
            {
                printToConsole("Toolbox is not running...");
                BeginUpdate();
            }
            else
            {
                try
                {
                    foreach (Process proc in Process.GetProcessesByName("BTDToolbox"))
                    {
                        printToConsole("Toolbox is running, terminating toolbox...");
                        proc.Kill();
                        printToConsole("Toolbox terminated...");
                    }
                    BeginUpdate();
                }
                catch (Exception ex)
                {
                    printToConsole(ex.Message);
                }
            }
        }
        private void BeginUpdate()
        {
            printToConsole("Beginning update...");
            printToConsole("The update will delete the old toolbox files...\n>> Do you want to delete all of the projects as well?");
            DialogResult result = MessageBox.Show("The update will delete the old toolbox files...\n\nDo you want to delete all of the projects as well?", "Delete project files?", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (!exit)
                    DeleteDirectory(Environment.CurrentDirectory, true);
            }
            else
            {
                if (!exit)
                    DeleteDirectory(Environment.CurrentDirectory, false);
            }
            CheckForExit();
            DownloadUpdate();
        }
        private void DownloadUpdate()
        {
            printToConsole("Downloading update...");

            client.DownloadFile(toolboxRelease, "Update"); //, Environment.CurrentDirectory);//
            printToConsole("Update successfully downloaded!");
            File.Move("Update", "BTDToolbox.zip");
            CheckForExit();
            extract();
        }


        //Extract
        private void extract()
        {
            printToConsole("Extracting update files....");
            string zipPath = Environment.CurrentDirectory + "\\BTDToolbox.zip";
            string extractedFilePath = Environment.CurrentDirectory;
            ZipFile archive = new ZipFile(zipPath);

            totalFiles = archive.Count();
            filesTransfered = 0;
            archive.ExtractProgress += ZipExtractProgress;

            foreach (ZipEntry e in archive)
            {
                e.Extract(extractedFilePath, ExtractExistingFileAction.DoNotOverwrite);
            }
            archive.Dispose();
            printToConsole("Update files successfully extracted!!!\n>> Deleting installation files...");
            File.Delete(zipPath);
            printToConsole("Installation files deleted. Restarting Toolbox...");
            
            Process.Start(Environment.CurrentDirectory + "\\BTDToolbox.exe");
            exit = true;
            CheckForExit();
        }
        private void ZipExtractProgress(object sender, ExtractProgressEventArgs e)
        {

            if (e.EventType != ZipProgressEventType.Extracting_BeforeExtractEntry)
                return;
            try
            {
                progressBar1.Invoke(new Action(() => progressBar1.Value = 100 * filesTransfered / totalFiles));
            }
            catch (Exception ex)
            {
                printToConsole(ex.Message);
            }
            filesTransfered++;
        }
    }
}
