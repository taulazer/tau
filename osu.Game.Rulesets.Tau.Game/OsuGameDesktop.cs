using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Threading;

namespace osu.Game.Rulesets.Tau.Game
{
    public class OsuGameDesktop : OsuGame
    {
        public OsuGameDesktop(string[] args = null)
            : base(args)
        {
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);

            var desktopWindow = (SDL2DesktopWindow)host.Window;

            desktopWindow.CursorState |= CursorState.Hidden;
            desktopWindow.Title = Name;
            desktopWindow.DragDrop += f => fileDrop(new[] { f });
        }

        private readonly List<string> importableFiles = new List<string>();
        private ScheduledDelegate importSchedule;

        private void fileDrop(string[] filePaths)
        {
            lock (importableFiles)
            {
                string firstExtension = Path.GetExtension(filePaths.First());

                if (filePaths.Any(f => Path.GetExtension(f) != firstExtension)) return;

                importableFiles.AddRange(filePaths);

                Logger.Log($"Adding {filePaths.Length} files for import");

                // File drag drop operations can potentially trigger hundreds or thousands of these calls on some platforms.
                // In order to avoid spawning multiple import tasks for a single drop operation, debounce a touch.
                importSchedule?.Cancel();
                importSchedule = Scheduler.AddDelayed(handlePendingImports, 100);
            }
        }

        private void handlePendingImports()
        {
            lock (importableFiles)
            {
                Logger.Log($"Handling batch import of {importableFiles.Count} files");

                string[] paths = importableFiles.ToArray();
                importableFiles.Clear();

                Task.Factory.StartNew(() => Import(paths), TaskCreationOptions.LongRunning);
            }
        }
    }
}
