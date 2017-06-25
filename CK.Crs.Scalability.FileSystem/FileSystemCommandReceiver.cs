using CK.Core;
using CK.Crs.Runtime;
using System;
using System.IO;
using System.Threading;

namespace CK.Crs.Scalability.FileSystem
{
    class FileSystemCommandReceiver : ICommandListener
    {
        readonly FileSystemConfiguration _configuration;
        readonly CommandConverter _commandConverter;
        public FileSystemCommandReceiver(FileSystemConfiguration configuration, IJsonConverter jsonConverter)
        {
            _configuration = configuration;
            _commandConverter = new CommandConverter(jsonConverter);
        }

        public void Receive(ICommandWorkerQueue queue, CancellationToken token)
        {
            IActivityMonitor monitor = new ActivityMonitor();
            using (monitor.OpenInfo().Send("FileSystem Command Receiver Started."))
            {
                FileSystemWatcher watcher = new FileSystemWatcher(_configuration.Path, "*.command.xml");
                try
                {
                    ActivityMonitor.DependentToken monitorToken = monitor.DependentActivity().CreateToken();
                    watcher.Created += (object sender, FileSystemEventArgs e) =>
                    {
                        if (e.ChangeType == WatcherChangeTypes.Created)
                        {
                            IActivityMonitor dependentMonitor = new ActivityMonitor();
                            using (dependentMonitor.StartDependentActivity(monitorToken))
                            {
                                using (dependentMonitor.OpenInfo().Send("New file detected"))
                                {
                                    var enveloppe = _commandConverter.ReadCommand( dependentMonitor, e.FullPath);
                                    if (enveloppe != null)
                                    {
                                        dependentMonitor.Trace().Send("Queue job");
                                        queue.Add(enveloppe);
                                    }
                                }
                            }
                        }
                    };
                    token.WaitHandle.WaitOne();
                }
                finally
                {
                    watcher.Dispose();
                }
            }
        }
    }
}
