﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public static void DeleteFiles(string exeName)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);

            foreach (FileInfo file in di.GetFiles())
            {
                if(file.Name.Contains(exeName))
                {
                    file.Delete();
                    printToConsole("Deleted old version...", main);
                }
            }
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
