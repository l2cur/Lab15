using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
/*
  3 вида паттерна прог. 
1- Наблюдатель (Observer)
2- Репозиторий 
3- Одиночка (Singleton) 
*/
class FileObserver
{
    private string directoryPath;
    private Dictionary<string, DateTime> files;
    private Timer timer;

    public event EventHandler<FileChangedEventArgs> FileChanged; // - Ивент

    public FileObserver(string path)
    {
        directoryPath = path;
        files = new Dictionary<string, DateTime>();
        InitializeFiles();
        timer = new Timer(CheckForChanges, null, 0, 1000);
    }

    private void InitializeFiles()
    {
        var existingFiles = Directory.GetFiles(directoryPath);
        foreach (var file in existingFiles)
        {
            files[file] = File.GetLastWriteTime(file);
        }
    }

    private void CheckForChanges(object state)
    {
        var currentFiles = Directory.GetFiles(directoryPath);

        foreach (var file in currentFiles)
        {
            if (!files.ContainsKey(file))
            {
                OnFileChanged(file, FileChangeType.Created);
                files[file] = File.GetLastWriteTime(file);
            }
            else
            {
                if (files[file] < File.GetLastWriteTime(file))
                {
                    OnFileChanged(file, FileChangeType.Modified);
                    files[file] = File.GetLastWriteTime(file);
                }
            }
        }

        var deletedFiles = new List<string>();
        foreach (var file in files.Keys)
        {
            if (!File.Exists(file))
            {
                OnFileChanged(file, FileChangeType.Deleted);
                deletedFiles.Add(file);
            }
        }

        foreach (var deletedFile in deletedFiles)
        {
            files.Remove(deletedFile);
        }
    }

    protected virtual void OnFileChanged(string filePath, FileChangeType changeType)
    {
        FileChanged?.Invoke(this, new FileChangedEventArgs(filePath, changeType));
    }
}

class FileChangedEventArgs : EventArgs
{
    public string FilePath { get; }
    public FileChangeType ChangeType { get; }

    public FileChangedEventArgs(string filePath, FileChangeType changeType)
    {
        FilePath = filePath;
        ChangeType = changeType;
    }
}

enum FileChangeType
{
    Created,
    Modified,
    Deleted
}

class Program
{
    static void Main()
    {
        string path = "C:\\Users\\l2cur\\Documents";
        var fileObserver = new FileObserver(path);

        fileObserver.FileChanged += (sender, e) => //е - объект FileChangedEventArgs, принимает в себя ПУТЬ ДО ФАЙЛА + ИЗМЕНЯЕМЫЙ ТИП, подпись тут
        {
            Console.WriteLine($"File '{e.FilePath}' has been {e.ChangeType.ToString().ToLower()}");
        };

        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();
    }
}
