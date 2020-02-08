using BTDToolbox_Updater.Classes;
using Ionic.Zip;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        string[] optional_DeleteFiles = new string[] {  };                                   //A list of all of the words and characters that you want the updater to ask about deleting
        string[] optional_DeleteFiles_Text = new string[] {  };                                   //A list of all of the words and characters that you want the updater to ask about deleting
        string[] optional_DeleteFolders = new string[] { "Mods", "Plugins", "Game Backup", "Config" };// { "Mods","Plugins", "Game Backup" };                                   //A list of all of the words and characters that you want the updater to ask about deleting
        string[] optional_DeleteFolders_Text = new string[] { "Mods", "Plugins", "Backups", "Settings" };//{ "Mods","Plugins","Backups" };                           //The message that will be asked if you add files to "askToDeleteFiles"
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
            Get_Launch_Parameters();
            if (GeneralMethods.isProcessRunning(program_ProcessName))
                TerminateProcess(filename, program_ProcessName);
            PopulateOptionalFiles();
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
            if (param_url == "" || param_url == null)
            {
                printToConsole("Vital launch parameters are missing! Missing initial git url.  Unable to continue", this);
            }
            else
            {
                printToConsole("Welcome to " + filename + " auto-updater", this);
                DeleteDirectory(Environment.CurrentDirectory, files_to_ignore, folders_to_ignore);
                //DownloadFiles();
            }
        }
        private void PopulateOptionalFiles()
        {
            foreach (string folder in optional_DeleteFolders_Text)
            {
                deleteFolders_list.Items.Add(folder, CheckState.Unchecked);
            }


            //deleteFiles_list.Items.Add(filename, CheckState.Checked);
            if (deleteFiles_list.Items.Count== 0)
            {
                None_Label.Visible = true;
            }
            else
            {
                foreach (string file in optional_DeleteFiles_Text)
                {
                    deleteFiles_list.Items.Add(file, CheckState.Unchecked);
                }
            }
        }
        /*private void AskToDelete()  //if there are important files, ask if you want them deleted...
        {
            int askMessage_index = 0;
            MessageBox.Show("This updater is build to delete all of the old files and replace them with new ones.");
            foreach(string askFile in askToDeleteFiles)
            {
                DialogResult result = DialogResult = MessageBox.Show(askToDeleteFiles_ListBox_Text[askMessage_index], "Delete?", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    Array.Resize(ref files_to_ignore, files_to_ignore.Length + 1);
                    files_to_ignore[files_to_ignore.Length - 1] = askFile;                    
                }
                else
                {
                    Array.Resize(ref files_to_delete_on_exit, files_to_delete_on_exit.Length + 1);
                    files_to_delete_on_exit[files_to_delete_on_exit.Length - 1] = askFile;
                }
                askMessage_index++;
            }
        }*/
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
                else if (arg.Contains("-optional_DeleteFiles_Name:"))
                {
                    optional_DeleteFiles = CreateStringArray(arg, "-optional_DeleteFiles_Name:", optional_DeleteFiles);
                }
                else if (arg.Contains("-optional_DeleteFiles_Text:"))
                {
                    optional_DeleteFiles_Text = CreateStringArray(arg, "-optional_DeleteFiles_Text:", optional_DeleteFiles_Text);
                }
                else if (arg.Contains("-optional_DeleteFolders_Name"))
                {
                    optional_DeleteFolders = CreateStringArray(arg, "-optional_DeleteFolders_Name:", optional_DeleteFolders);
                }
                else if (arg.Contains("-optional_DeleteFolders_Text"))
                {
                    optional_DeleteFolders_Text = CreateStringArray(arg, "-optional_DeleteFolders_Text:", optional_DeleteFolders_Text);
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
            if (optional_DeleteFolders.Length > 0)
                folders_to_ignore = GetCheckedBoxes(deleteFolders_list, optional_DeleteFolders, optional_DeleteFolders_Text, folders_to_ignore);


            if (optional_DeleteFiles.Length > 0)
                GetCheckedBoxes(deleteFiles_list, optional_DeleteFiles, optional_DeleteFiles_Text, files_to_ignore);
            Start();
        }
        private string[] GetCheckedBoxes(CheckedListBox checked_listbox_name, string[] optional_Delete_Array, string[] optional_Delete_Array_Text, string[] output_DontDelete_Array)
        {
            string[] items = new string[] { };
            int count = optional_Delete_Array_Text.Length;// + 1;   //had to get rid of the +1, which was only good for delete files

            if (checked_listbox_name.Items.Count > 0)
            {


                for (int x = 0; x < count; x++)
                {
                    if (checked_listbox_name.GetItemCheckState(x) == CheckState.Unchecked)
                    {
                        Array.Resize(ref items, items.Length + 1);
                        items[items.Length - 1] = optional_Delete_Array_Text[x];// - 1];
                    }
                }

                foreach (object item in items)
                {

                    string checkedItem = item.ToString();
                    int i = 0;


                    foreach (string askDelete in optional_Delete_Array_Text)
                    {
                        if (askDelete == checkedItem)
                        {
                            object dontDelete = optional_Delete_Array.GetValue(i);
                            bool duplicate = false;

                            foreach (string ignore in output_DontDelete_Array)
                            {
                                if (ignore == dontDelete.ToString())
                                    duplicate = true;
                            }
                            if (!duplicate)
                            {
                                Array.Resize(ref output_DontDelete_Array, output_DontDelete_Array.Length + 1);
                                output_DontDelete_Array[output_DontDelete_Array.Length - 1] = dontDelete.ToString();
                            }
                            duplicate = false;
                        }
                        i++;
                    }
                }
            }
            return output_DontDelete_Array;
        }
    }
}
