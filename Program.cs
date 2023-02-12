using Worker_Sevice;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<FileWatcherService>();
    })
    .Build();

await host.RunAsync();

public class FileWatcherService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var filePath = "folderEvents.txt";
        using var watcher = new FileSystemWatcher("C:\\Users\\skela\\Desktop\\Applications\\Worker_Sevice");
 
        // записываем изменения
        watcher.Changed += async (o, e) => await File.
                     AppendAllTextAsync(filePath, $"{DateTime.Now} Изменен: {e.FullPath}\n");

        // записываем данные о создании файлов и папок
        watcher.Created += async (o, e) => await File.
                      AppendAllTextAsync(filePath, $"{DateTime.Now} Создан: {e.FullPath}\n");

        // записываем данные об удалении файлов и папок
        watcher.Deleted += async (o, e) => await File.
                      AppendAllTextAsync(filePath, $"{DateTime.Now} Удален: {e.FullPath}\n");

        // записываем данные о переименовании
        watcher.Renamed += async (o, e) => await File.
                       AppendAllTextAsync(filePath, $"{DateTime.Now} Переименован: {e.OldFullPath} to {e.FullPath}\n");
                       
        // записываем данные об ошибках
        watcher.Error += async (o, e) => await File.
        AppendAllTextAsync(filePath, $"{DateTime.Now} Error: {e.GetException().Message}\n");

        watcher.IncludeSubdirectories = true; // отслеживаем изменения в подкаталогах
        watcher.EnableRaisingEvents = true;    // включаем события
 
        // если операция не отменена, то выполняем задержку в 200 миллисекунд
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Выполняется FileWatcher: {0}", DateTimeOffset.Now);
            await Task.Delay(100, stoppingToken);
        }

        await Task.CompletedTask;
    }
}