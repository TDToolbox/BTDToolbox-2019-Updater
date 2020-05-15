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
        int filesTransfered = 0;
        int totalFiles = 0;
        public static Main main = Main.getInstance();

        public void extract(string update_zip_name, string filename, string exeName, string[] files_to_delete_on_exit)
        {
            //printToConsole("Extracting update files....");
            string zipPath = Environment.CurrentDirectory + "\\" + update_zip_name;
            string extractedFilePath = Environment.CurrentDirectory;
            ZipFile.ExtractToDirectory(zipPath, extractedFilePath);
            /*ZipFile archive = ZipFile.OpenRead(zipPath);
            

            totalFiles = archive.Count();
            archive.ExtractProgress += ZipExtractProgress;

            foreach (ZipEntry e in archive)
            {
                e.Extract(extractedFilePath, ExtractExistingFileAction.DoNotOverwrite);
            }
            archive.Dispose();*/
            GeneralMethods.printToConsole("Update files successfully extracted!!!\n>> Deleting installation files...", main);
            File.Delete(zipPath);
            GeneralMethods.printToConsole("Installation files deleted. Restarting" + filename + "...", main);

            Process.Start(Environment.CurrentDirectory + "\\" + exeName);
            Main.exit = true;
            GeneralMethods.CheckForExit(files_to_delete_on_exit);
        }
        /*private void ZipExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.EventType != ZipProgressEventType.Extracting_BeforeExtractEntry)
                return;
            try
            {
                main.progressBar1.Invoke(new Action(() => main.progressBar1.Value = 100 * filesTransfered / totalFiles));
            }
            catch (Exception ex)
            {
                GeneralMethods.printToConsole(ex.Message, main);
            }
            filesTransfered++;
        }*/
    }
}
