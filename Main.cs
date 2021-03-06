﻿using BTDToolbox_Updater.Classes;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using static UpdaterInfo.LaunchParameters;
using static BTDToolbox_Updater.Classes.GeneralMethods;

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
        string toolName = "";// = "BTD Toolbox";                                             //This will be used in all of the console messages
        string filename = "";// = "BTD Toolbox";                                             //This will be used in all of the console messages
        string program_ProcessName;// = "BTDToolbox";                                   //Used to check if the program is already running. Must set this
        string exeName;// = "BTDToolbox.exe";                                           //Used to restart program after update. Do not put slashes in front of it, just name
        string updateZip_Name = "";// = "BTDToolbox_Updater.zip";                       //The name of the zip file that is created from the downloaded update
        //string url = "";// "https://raw.githubusercontent.com/TDToolbox/BTDToolbox-2019_LiveFIles/master/Version";    //URL of github config file
        string param_url = "";//"https://raw.githubusercontent.com/TDToolbox/BTDToolbox-2019_LiveFIles/master/Updater_launch%20parameters";    //URL of github config file
        string download_url = "";
        string[] files_to_ignore = new string[] { };//{ "BTDToolbox_Updater", "Backups", "DotNetZip", ".json" };  //list of files to ignore during deletion, based on the full path name
        string[] folders_to_ignore = new string[] { };//{ "BTDToolbox_Updater", "Backups", "DotNetZip", ".json" };  //list of files to ignore during deletion, based on the full path name
        string[] files_to_delete_on_exit = new string[] { };    // { "BTDToolbox_Updater.zip", "Update" };  //list of files to delete AFTER the update has finished. This is case specific
        //string[] replaceText = new string[] { };                                       //A list of all of the words and characters that you want to delete from the downloaded url. EX: BTDToolbox: www.toolbox.com.  In this example we want to delete BTDToolbox:   from the link. Btw that website isnt ours so go at your own risk
        string replaceText = "";                                                         //A list of all of the words and characters that you want to delete from the downloaded url. EX: BTDToolbox: www.toolbox.com.  In this example we want to delete BTDToolbox:   from the link. Btw that website isnt ours so go at your own risk
        int paramNumber;                                                                 // The line number that the url is on, starting from 0. Look at the commented "url" above. It is refering to index 0
        int lineNumber;                                                                  // The line number that the url is on, starting from 0. Look at the commented "url" above. It is refering to index 0
        //================================================================================


        //project varibales

        WebClient client;
        WebHandler web;
        private static Main main;

        //refactoring variables
        public static bool exit = false;
        string unprocessedURL;
        public static string lastMessage;
        public Main()
        {
            InitializeComponent();            
            client = new WebClient();
            web = new WebHandler();
            main = this;
            

            pictureBox2.Controls.Add(pictureBox1);
            //pictureBox2.Controls.Add(label2);
        }

        //Events
        private void Form1_Shown(object sender, EventArgs e)
        {
            printToConsole("Program Initialized...", this);
            new Thread(() =>
            {
                Get_Launch_Parameters();
                if (isProcessRunning(program_ProcessName))
                    TerminateProcess(filename, program_ProcessName);
            }).Start();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            exit = true;
            CheckForExit(files_to_delete_on_exit);           
        }

        //core functions
        public static Main getInstance()
        {
            return main;
        }

        //operations
        private void Start()
        {

            if (!Guard.IsStringValid(param_url))
            {
                printToConsole("Vital launch parameters are missing! Missing initial git url.  Unable to continue", this);
                return;
            }

            printToConsole("Welcome to " + filename + " auto-updater", this);
            DeleteFiles(exeName);
            DownloadFiles();
        }
        
        
        private void Get_Launch_Parameters()    //Get url and git_param line number from Launch Parameters
        {
            printToConsole("Reading launch parameters...", this);
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                arg.Replace("$$", " ");

                if (arg.Contains("-url:"))
                {
                    param_url = arg.Replace("-url:", "");
                }
                else if (arg.Contains("-lineNumber:"))
                {
                    paramNumber = Int32.Parse(arg.Replace("-lineNumber:", "")); //param number is the same as line number
                }
            }
            if (param_url == "" || param_url == null)
            {
                printToConsole("Launch parameters are missing! Unable to continue", this);
            }
            else      //get git parameters
            {
                printToConsole("Launch parameters aquired", this);
                string gitURL = web.WaitOn_URL(param_url, files_to_delete_on_exit);
                string gitText = web.processGit_Text(gitURL, "btdmodloader: ", paramNumber);
                Read_Git_Parameters(gitText);
            }            
        }
        private void Read_Git_Parameters(string gitText)
        {
            string[] args = gitText.Split(' ');
            foreach (string argx in args)
            {
                string arg;
                arg = argx.Replace("$$", " ");

                if (argx.EndsWith(","))
                {
                    arg = arg.Remove(arg.Length - 1);
                }
                if (arg.Contains("-toolName:"))
                {
                    toolName = arg.Replace("-toolName:", "");
                }
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
                    files_to_ignore = CreateStringArray(arg, "-ignoreFiles:", files_to_ignore);
                }
                else if (arg.Contains("-deleteFiles:"))
                {
                    files_to_delete_on_exit = CreateStringArray(arg, "-deleteFiles:", files_to_delete_on_exit);
                }
                else if (arg.Contains("-replaceText:"))
                {
                    replaceText = arg.Replace("-replaceText:", "");
                }
                else if (arg.Contains("-lineNumber:"))
                {
                    lineNumber = Int32.Parse(arg.Replace("-lineNumber:", ""));
                }
                else if (arg.Contains("-url:"))
                {
                    unprocessedURL = arg.Replace("-url:", "");
                }
            }
        }
        private void DownloadFiles()
        {
            printToConsole("Getting download link for latest version...", this);
            download_url = web.processGit_Text(web.WaitOn_URL(unprocessedURL, files_to_delete_on_exit), replaceText, lineNumber);
            printToConsole("Downloading update...", this);
            client.DownloadFile(download_url, "Update");
            printToConsole("Update successfully downloaded!", this);
            if (File.Exists(updateZip_Name))
            {
                File.Delete(updateZip_Name);
            }
            File.Move("Update", updateZip_Name);

            var extract = new Extract();
            extract.extract(updateZip_Name, filename, exeName, files_to_delete_on_exit);
        }
        private void Update_Button_Click(object sender, EventArgs e)
        {
            Start();
        }
    }
}
