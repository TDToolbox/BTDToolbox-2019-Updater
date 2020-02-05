using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTDToolbox_Updater.Classes
{
    class WebHandler
    {
        WebHandler reader;

        bool exitLoop = false;
        public string startURL { get; set; }
        public string readURL { get; set; }
        public bool urlAquired { get; set; }

        public void TryReadURL_StartThread()
        {
            Thread thread = new Thread(() => urlAquired = TryReadURL_OnThread(startURL));
            thread.Start();
        }
        private bool TryReadURL_OnThread(string url)    //Do not call this one
        {
            bool success = false;
            WebClient client = new WebClient();

            try
            {
                readURL = client.DownloadString(url);
                success = true;
            }
            catch
            {
                if (success == false)
                {
                    for (int i = 0; i <= 100; i++)
                    {
                        Thread.Sleep(100);
                        try
                        {
                            if (!exitLoop)
                            {
                                readURL = client.DownloadString(url);
                                if (readURL != null && readURL != "")
                                {
                                    success = true;
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch { }
                    }
                }
            }
            return success;
        }
        public string WaitOn_URL(string url, string[] files_to_delete_on_exit)   //call this one to read the text from the git url
        {
            WebHandler get = new WebHandler();
            get.startURL = url;
            get.TryReadURL_StartThread();

            for (int i = 0; i <= 100; i++)
            {
                GeneralMethods.CheckForExit(files_to_delete_on_exit);
                Thread.Sleep(100);
                if (get.urlAquired)
                {
                    break;
                }
            }
            return get.readURL;
        }
        public string processGit_Text(string url, string deleteText, int lineNumber)    //call this one after getting the url to read git text and return the url we want. Delete text is the starting word, for example "toolbox2019: "
        {
            if (url != null && url != "")
            {
                string[] split = url.Split('\n');
                return split[lineNumber].Replace(deleteText, "").Replace("{", "").Replace("}", "");
            }
            else
            {
                return null;
            }
        }
    }
}
