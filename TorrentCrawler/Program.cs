using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace TorrentCrawler
{
    internal static class Program
    {
        static public DebugForm debugForm { set; get; }
        static public MainForm mainForm;
        static public Thread threadsManager; // both not needed?
        static private Thread formsManager;

        static void Main()
        {
            threadsManager = Thread.CurrentThread;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // create Thread to  run MainForm
            Thread newThread = new Thread(() => Application.Run(new MainForm()));
            newThread.Start();

            formsManager = newThread;

            Thread.Sleep(1000); //fishy
            debug("This is the main thread with id: " + Thread.CurrentThread.ManagedThreadId + ". He spwaned the Thread responsible for the formWindows with id: "
                + formsManager.ManagedThreadId);
                
        }

        private delegate void logPrint (string str);

        static public void debug(String s)
        {
            logPrint lP = new logPrint(debugForm.addDebugLog);
            debugForm.Invoke(lP, s);

            //debugForm.addDebugLog(s);
        }

        private delegate void mainArchiveProgressBarInit (object obj);

        static public void archiveProgressBarInitialization(int maximum, int current)
        {
            MainForm.ArchiveProgressBarInitializationObject initObj = new MainForm.ArchiveProgressBarInitializationObject(maximum, current);
            mainArchiveProgressBarInit init = new mainArchiveProgressBarInit(mainForm.initializeArchiveProgressBar);
            mainForm.Invoke(init, initObj);
        }

        private delegate void mainArchiveProgressBarStep();

        static public void archiveProgressBarStep()
        {
            mainArchiveProgressBarStep step = new mainArchiveProgressBarStep(mainForm.stepArchiveProgressBar);

            mainForm.Invoke(step);
        }

        private delegate void mainBlocksCompleted(int i);

        static public void blocksCompleted(int count)
        {
            mainBlocksCompleted completed = new mainBlocksCompleted(mainForm.setBlocksCompleted);
            mainForm.Invoke(completed, count);
        }

        private delegate void mainTorrentsInserted(int i);
        static public void torrentsInserted(int count)
        {
            mainTorrentsInserted inserted = new mainTorrentsInserted(mainForm.setInsertedFirstTime);
            mainForm.Invoke(inserted, count);
        }

        private delegate void mainTorrentsUpdated(int i);
        static public void torrentsUpdated(int count)
        {
            mainTorrentsUpdated updated = new mainTorrentsUpdated(mainForm.setUpdated);
            mainForm.Invoke(updated, count);
        }

        private delegate void mainDidntRespond(int i);
        static public void torrentsNotResponding(int count)
        {
            mainDidntRespond respond = new mainDidntRespond(mainForm.setNotResponding);
            mainForm.Invoke(respond, count);
        }

        private delegate void mainTotalInspected(int i);
        static public void totalInspected(int count)
        {
            mainTotalInspected inspected = new mainTotalInspected(mainForm.setTotalTorrents);
            mainForm.Invoke(inspected, count);
        }

        private delegate void mainTimeUpdate(TimeSpan i);
        static public void updateTimeRunning(TimeSpan time)
        {
            mainTimeUpdate timeUpdate = new mainTimeUpdate(mainForm.setTimeRunning);
            mainForm.Invoke(timeUpdate, time);
        }

        private delegate int mainGetStartingBlockInput();
        static public int getStartingBlockInput()
        {
            mainGetStartingBlockInput getStart = new mainGetStartingBlockInput(mainForm.startingBlockGetInput);
            return (int) mainForm.Invoke(getStart);
        }
    }
}
