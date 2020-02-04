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
    public partial class Main : Form
    {
        /// <summary>
        /// This project is used to check text files on github for updates, and if there is an update, it will download 
        /// and install it. 
        /// 
        /// You will need to configure the "For easy reuse" variables
        /// 
        /// For the program to work correctly, you need to set the "url" variable to the url of the RAW text file
        /// on github, which you can view by going to your text file on github, then pressing the "RAW" button.
        /// The program will then attempt to read that text file, and store its result in the "downloadedString" variable.
        /// Some configuration will likely be necessary, as it needs to remove everything in "downloadedString" until the
        /// only thing left is the URL of the most updated version of your project. This link needs to be the url of the
        /// actual file itself, not a main webpage or release page, but the actual download link for it. Some configuration
        /// may also be necessary with the console messages, as some of them are specifically for this project.
        /// 
        /// 
        /// In short, this project will read a text file off of github and process it down to a url, then download the file
        /// located at that url, extract it, delete all files (except for the exe) related to this updater, then launch the
        /// updated program.
        /// 
        /// </summary>



        //================================================================================
        //For easy reuse of project, change these variables
        string filename;// = "BTD Toolbox";                                             //This will be used in all of the console messages
        string program_ProcessName;// = "BTDToolbox";                                   //Used to check if the program is already running. Must set this
        string exeName;// = "BTDToolbox.exe";                                           //Used to restart program after update. Do not put slashes in front of it, just name
        string updateZip_Name = "";// = "BTDToolbox_Updater.zip";                       //The name of the zip file that is created from the downloaded update
        string url = "";// https://raw.githubusercontent.com/TDToolbox/BTDToolbox-2019_LiveFIles/master/Version";    //URL of github config file
        string[] ignoreFiles = new string[] { };//{ "BTDToolbox_Updater", "Backups", "DotNetZip", ".json" };  //list of files to ignore during deletion, based on the full path name
        string[] deleteFiles = new string[] { };// { "BTDToolbox_Updater.zip", "Update" };  //list of files to delete AFTER the update has finished. This is case specific
        string[] replaceText = new string[] { };                                       //A list of all of the words and characters that you want to delete from the downloaded url. EX: BTDToolbox: www.toolbox.com.  In this example we want to delete BTDToolbox:   from the link. Btw that website isnt ours so go at your own risk
        string lineNumber; //= " ";                                                             // The line number that the url is on, starting from 0. Look at the commented "url" above. It is refering to index 0
        //================================================================================


        //project varibales
        Thread bgThread;
        WebClient client;

        //string variables
        string releaseVersion;
        string downloadedString = "";

        //extraction variables
        int totalFiles;
        int filesTransfered;

        //other variables
        string lastMessage;
        bool exit = false;
        bool deleteProjects = false;


        public Main()
        {
            InitializeComponent();            
            client = new WebClient();
        }

        //Events
        private void Form1_Shown(object sender, EventArgs e)
        {
            bool error = false;
            printToConsole("Program Initialized...");
            printToConsole("Reading launch parameters...");
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg.Contains("-fileName:"))
                {
                    filename = arg.Replace("-fileName:", "");
                }
                else if (arg.Contains("-processName:"))
                {
                    program_ProcessName = arg.Replace("-processName:", "");
                }
                else if (arg.Contains("-exeName:"))
                {
                    exeName = arg.Replace("-exeName:", "");
                }
                else if (arg.Contains("-updateZip_Name:"))
                {
                    updateZip_Name = arg.Replace("-updateZip_Name:", "");
                }
                else if (arg.Contains("-ignoreFiles:"))
                {
                    ignoreFiles = CreateStringArray(arg, "-ignoreFiles:", ignoreFiles);
                }
                else if (arg.Contains("-deleteFiles:"))
                {
                    deleteFiles = CreateStringArray(arg, "-deleteFiles:", deleteFiles);
                }
                else if (arg.Contains("-replaceText:"))
                {
                    replaceText = CreateStringArray(arg, "-replaceText:", replaceText);
                }
                else if (arg.Contains("-url:"))
                {
                    url = arg.Replace("-url:", "");
                }
                else if (arg.Contains("-lineNumber:"))
                {
                    lineNumber = arg.Replace("-lineNumber:", "");
                }
            }
            if (error != true)
            {
                printToConsole("Welcome to " + filename + " auto-updater");
                printToConsole("Getting download link for latest version...");
                Thread bg = new Thread(load);
                bg.Start();
            }
                
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CheckForExit();
            exit = true;
        }
        
        
        //Core functions
        private void load() //this pauses the updater for 1.5 seconds so user can see console
        {
            Thread.Sleep(1500);
            CheckURL();  
        }
        private string[] CreateStringArray(string inputArg, string argName, string[] stringArray)
        {
            Array.Resize(ref stringArray, 0);

            string[] cleanArg = (inputArg.Replace(argName, "").Replace("{", "").Replace("}", "")).Split(',');
            foreach (string s in cleanArg)
            {
                Array.Resize(ref stringArray, stringArray.Length + 1);
                stringArray[stringArray.Length - 1] = s;
            }
            return stringArray;
        }
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
            if (Process.GetProcessesByName(program_ProcessName).Length > 0)
                return true;
            else
                return false;
        }
        private void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                printToConsole("Deleting directory..");
                var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

                foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
                {
                    info.Attributes = FileAttributes.Normal;
                }

                foreach (FileInfo file in directory.GetFiles())
                {
                    bool skip = false;
                    foreach (string ignore in ignoreFiles)
                    {
                        if (file.ToString().Contains(ignore))
                        {
                            skip = true;
                        }
                    }
                    if (skip == false)
                    {
                        if (file.Exists)
                            file.Delete();
                    }
                }
                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    bool skip = false;
                    foreach (string ignore in ignoreFiles)
                    {
                        if (dir.FullName.ToString().Contains(ignore))
                        {
                            skip = true;
                        }
                    }
                    if (skip == false)
                    {
                        if (dir.Exists)
                            dir.Delete(true);
                    }
                }
                printToConsole("Directory deleted.");
            }
            else
            {
                printToConsole("Directory not found. Unable to delete directory at: " + path);
            }
        }
        private void CheckForExit()
        {
            if(exit)
            {
                foreach (string delete in deleteFiles)
                {
                    if (File.Exists(Environment.CurrentDirectory + "\\" + delete))
                        File.Delete(Environment.CurrentDirectory + "\\" + delete);
                }
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
                printToConsole("ERROR! The program was unable to determine the latest version of " + filename + ". You can continue to use it like normal, or try reopening the program to check again...");
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
        private void CleanURL()
        {
            string[] split = downloadedString.Split('\n');
            int lineNum = Int32.Parse(lineNumber);

            foreach (string arg in replaceText)
            {
                releaseVersion = split[lineNum].Replace(arg, "");
            }
        }

        //Start update AFTER confirming the URL
        private void start()
        {
            CleanURL();

            printToConsole("Download link aquired!:" +  releaseVersion);

            CheckForExit();
            printToConsole("Checking if " + filename + " is running..");
            if (isToolboxRunning() == false)
            {
                printToConsole(filename + " is not running...");
                BeginUpdate();
            }
            else
            {
                try
                {
                    foreach (Process proc in Process.GetProcessesByName(program_ProcessName))
                    {
                        printToConsole(filename + "  is running, terminating " + filename + "...");
                        proc.Kill();
                        printToConsole(filename + "  terminated...");
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
            printToConsole("This tool will replace all of the old " + filename + " files...\n\n>> Do you want it to delete all of the projects as well ?");
            DialogResult result = MessageBox.Show("This tool will replace all of the old " + filename + " files... Do you want it to delete all of the projects as well?", "Delete project files?", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                Array.Resize(ref ignoreFiles, ignoreFiles.Length + 1);
                ignoreFiles[ignoreFiles.Length-1] = "proj";
            }
            DeleteDirectory(Environment.CurrentDirectory);
            DownloadUpdate();
        }
        private void DownloadUpdate()
        {
            printToConsole("Downloading update...");

            client.DownloadFile(releaseVersion, "Update"); //, Environment.CurrentDirectory);//
            printToConsole("Update successfully downloaded!");
            if (File.Exists(updateZip_Name))
            {
                File.Delete(updateZip_Name);
            }
            File.Move("Update", updateZip_Name);
            CheckForExit();
            extract();
        }


        //Extract
        private void extract()
        {
            printToConsole("Extracting update files....");
            string zipPath = Environment.CurrentDirectory + "\\"+ updateZip_Name;
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
            printToConsole("Installation files deleted. Restarting" + filename + "...");
            
            Process.Start(Environment.CurrentDirectory + "\\"+ exeName);
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
