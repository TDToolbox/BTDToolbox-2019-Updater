using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Windows.Forms;

namespace BTDToolbox_Updater.Classes
{
    class GeneralMethods
    {
        public static Main main = Main.getInstance();
        public static string[] CreateStringArray(string inputArg, string paramName, string[] stringArray)
        {
            Array.Resize(ref stringArray, 0);

            string[] cleanArg = (inputArg.Replace(paramName, "").Replace("{", "").Replace("}", "")).Split(',');
            foreach (string s in cleanArg)
            {
                Array.Resize(ref stringArray, stringArray.Length + 1);
                stringArray[stringArray.Length - 1] = s;
            }
            return stringArray;
        }
        public static bool isProcessRunning(string program_ProcessName)
        {
            if (Process.GetProcessesByName(program_ProcessName).Length > 0)
                return true;
            else
                return false;
        }
        public static void TerminateProcess(string filename, string program_ProcessName)
        {
            try
            {
                foreach (Process proc in Process.GetProcessesByName(program_ProcessName))
                {
                    printToConsole(filename + "  is running, terminating " + filename + "...", main);
                    proc.Kill();
                    printToConsole(filename + "  terminated...", main);
                }
            }
            catch (Exception ex)
            {
                printToConsole(ex.Message, main);
            }
        }

        public static void DeleteDirectory(string path, string[] files_to_ignore, string[] folders_to_ignore)
        {
            string livePath = Environment.CurrentDirectory;


            //Get the names of the folders that we shouldn't delete, because they have files we shouldnt delete!
            foreach (string ig in files_to_ignore)
            {
                var allFiles = Directory.GetFiles(Environment.CurrentDirectory, ig + "*", SearchOption.AllDirectories);
                foreach (var sd in allFiles)
                {
                    string cleanPath = sd.ToString().Replace(livePath, "");
                    string[] dirs = cleanPath.Split('\\');
                    Array.Resize(ref dirs, dirs.Length - 1);    //remove the filename from the path

                    foreach (string d in dirs)
                    {
                        if (d != "")    //We've now got the folder name
                        {
                            if (!folders_to_ignore.Contains(d))
                            {
                                Array.Resize(ref folders_to_ignore, folders_to_ignore.Length + 1);
                                folders_to_ignore[folders_to_ignore.Length - 1] = d;
                            }
                        }
                    }
                }
            }

            //Get the names of the folders that we shouldn't delete, because they have folders we shouldnt delete!
            string[] folders = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            foreach (string dir in folders)
            {
                foreach (string f in folders_to_ignore) //check if dir name is one of the ones we cant delete
                {
                    if (dir.Contains(f))
                    {
                        string cleanPath = dir.ToString().Replace(livePath, "");
                        string[] dirs = cleanPath.Split('\\');
                        foreach (string d in dirs)
                        {
                            if (d != "")    //We've now got the folder name
                            {
                                if (!folders_to_ignore.Contains(d))
                                {
                                    Array.Resize(ref folders_to_ignore, folders_to_ignore.Length + 1);
                                    folders_to_ignore[folders_to_ignore.Length - 1] = d;
                                }
                            }
                        }
                    }
                }                
            }

            //Delete shit
            DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo file in di.GetFiles())
            {
                bool skip = false;
                foreach (string fi in files_to_ignore)
                {
                    if (file.ToString().Contains(fi))
                    {
                        skip = true;
                    }
                }
                if (skip == false)
                {
                    //file.Delete();
                }
            }
            foreach (string fi in folders_to_ignore)
            {
                printToConsole(fi, main);
            }

            string[] folders2 = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            foreach (string dir in folders2)
            {
                //printToConsole(dir, main);
                bool skip = false;
                foreach (string fi in folders_to_ignore)
                {
                    //printToConsole(fi, main);
                    if (dir.Contains(fi))
                    {
                        skip = true;
                    }
                }
                if (skip == false)
                {
                    //printToConsole(dir, main);
                    //dir.Delete(true);
                }

                if (!folders_to_ignore.Contains(dir.ToString()))
                {
                    //printToConsole(dir, main);
                    //dir.Delete(true);
                }
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                
                //printToConsole(dir.ToString(), main);
                /*if (!folders_to_ignore.Contains(dir.ToString()))
                {
                    //dir.Delete(true);
                }*/
            }
            printToConsole("Done deleting", main);

        }
        public static void CheckForExit(string[] files_to_delete_on_exit)
        {
            if (Main.exit)
            {
                foreach (string delete in files_to_delete_on_exit)
                {
                    if (File.Exists(Environment.CurrentDirectory + "\\" + delete))
                        File.Delete(Environment.CurrentDirectory + "\\" + delete);
                }
                Environment.Exit(0);
            }
        }
        public static void printToConsole(string message, Main m)
        {
            if (message != Main.lastMessage)
            {
                try
                {
                    m.Invoke((MethodInvoker)delegate {
                        m.Console.AppendText(">> " + message + "\r\n");
                        m.Console.ScrollToCaret();
                        Main.lastMessage = message;
                    });
                }
                catch
                { }
            }
        }
    }
}
