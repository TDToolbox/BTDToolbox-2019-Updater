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

        public string lastMessage;
        Thread bgThread;
        WebClient client;
        int totalFiles;
        int filesTransfered;
        string toolboxRelease;

        public Form1()
        {
            InitializeComponent();
            
            client = new WebClient();
            
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            printToConsole("Program Initialized...");
            printToConsole("Welcome to BTD Toolbox 2019 auto-updater");
            printToConsole("Getting download link for latest version...");
            string versionText = client.DownloadString("https://raw.githubusercontent.com/TDToolbox/BTDToolbox-2019_LiveFIles/master/Version");
            string[] split = versionText.Split('\n');
            toolboxRelease = split[0].Replace("toolbox2019: ", "");
            printToConsole("Download link aquired!");
            start();
        }
        private void start()
        {
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
                DeleteDirectory(Environment.CurrentDirectory, true);
            else
                DeleteDirectory(Environment.CurrentDirectory, false);

            bgThread = new Thread(DownloadUpdate);
            bgThread.Start();
        }
        private void DownloadUpdate()
        {
            printToConsole("Downloading update...");
            client.DownloadFile(toolboxRelease, "Update"); //, Environment.CurrentDirectory);//
            printToConsole("Update successfully downloaded!");
            File.Move("Update", "BTDToolbox.zip");
            extract();
        }
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
            Application.Exit();
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
        private bool isToolboxRunning()
        {
            if (Process.GetProcessesByName("BTDToolbox").Length > 0)
                return true;
            else
                return false;
        }
        private void printToConsole(string message)
        {
            if (message != lastMessage)
            {
                Invoke((MethodInvoker)delegate {
                    Console.AppendText(">> " + message + "\r\n");
                    lastMessage = message;
                });
            }
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
        private bool checkForFiles(string path)
        {
            bool filesExist = false;
            printToConsole("Confirming if directory is empty..");
            var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

            foreach (FileInfo file in directory.GetFiles())
            {
                if (!file.ToString().Contains("BTDToolbox_Updater") && (!file.ToString().Contains("proj")) && (!file.ToString().Contains(".dll")) && (!file.ToString().Contains(".json")))
                {
                    filesExist = true;
                    break;
                }
            }
            return filesExist;
        }

    }
}
