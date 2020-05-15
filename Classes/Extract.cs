using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;

namespace BTDToolbox_Updater.Classes
{
    class Extract
    {
        public static Main main = Main.getInstance();

        public void extract(string update_zip_name, string filename, string exeName, string[] files_to_delete_on_exit)
        {
            //printToConsole("Extracting update files....");
            string zipPath = Environment.CurrentDirectory + "\\" + update_zip_name;
            string extractedFilePath = Environment.CurrentDirectory;

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string destinationPath = Path.GetFullPath(Path.Combine(extractedFilePath, entry.FullName));
                    if (File.Exists(destinationPath))
                        File.Delete(destinationPath);

                    entry.ExtractToFile(destinationPath);
                }
            }

            GeneralMethods.printToConsole("Update files successfully extracted!!!\n>> Deleting installation files...", main);
            File.Delete(zipPath);
            GeneralMethods.printToConsole("Installation files deleted. Restarting" + filename + "...", main);

            Process.Start(Environment.CurrentDirectory + "\\" + exeName);
            Main.exit = true;
            GeneralMethods.CheckForExit(files_to_delete_on_exit);
        }
    }
}
