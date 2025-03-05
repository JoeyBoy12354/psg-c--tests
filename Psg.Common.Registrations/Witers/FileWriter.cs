using System.Collections.Concurrent;

namespace Psg.Common.Registrations.Witers
{
    public static class FileWriter
    {
        public static string Folder => _folder;

        private static readonly string _folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        private static readonly ConcurrentQueue<Func<Task>> WriteQueue = new ConcurrentQueue<Func<Task>>();
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(1);
        private const int MaxRetryAttempts = 3;

        public static string GetFullFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "";

            return Path.Combine(Folder, fileName);
        }


        public static async Task WriteBytesAsync(string fileName, byte[] bytes)
        {
            await EnqueueWriteAsync(async () =>
            {
                var filePath = GetFilePath(fileName);
                await RetryOnFailure(async () => await File.WriteAllBytesAsync(filePath, bytes));
            });
        }

        public static async Task WriteTextAsync(string fileNameWithoutExtension, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            fileNameWithoutExtension = fileNameWithoutExtension + ".txt";

            await EnqueueWriteAsync(async () =>
            {
                var filePath = GetFilePath(fileNameWithoutExtension);
                await RetryOnFailure(async () => await File.WriteAllTextAsync(filePath, text));
            });
        }

        public static async Task WriteHtmlAsync(string fileNameWithoutExtension, string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            fileNameWithoutExtension = fileNameWithoutExtension + ".html";

            await EnqueueWriteAsync(async () =>
            {
                var filePath = GetFilePath(fileNameWithoutExtension);
                await RetryOnFailure(async () => await File.WriteAllTextAsync(filePath, text));
            });
        }



        private static async Task EnqueueWriteAsync(Func<Task> writeTask)
        {
            WriteQueue.Enqueue(writeTask);
            await ProcessQueue();
        }

        private static async Task ProcessQueue()
        {
            await Semaphore.WaitAsync();
            try
            {
                while (WriteQueue.TryDequeue(out var writeTask))
                {
                    await writeTask();
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }

        private static async Task RetryOnFailure(Func<Task> action)
        {
            int attempts = 0;
            while (true)
            {
                try
                {
                    await action();
                    break; // Success, exit the retry loop
                }
                catch (IOException) when (attempts < MaxRetryAttempts)
                {
                    attempts++;
                    await Task.Delay(RetryDelay);
                }
            }
        }

        private static string GetFilePath(string fileName)
        {
            var filePath = Path.Combine(_folder, fileName);
            return filePath;
        }
    }
}
