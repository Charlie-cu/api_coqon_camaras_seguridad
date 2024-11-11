using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using api_coqon.Services;

public class OldFileCleanupService : BackgroundService
{
    private readonly pCloudService _pCloudService;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1); // Repite la tarea cada hora

    public OldFileCleanupService(pCloudService pCloudService)
    {
        _pCloudService = pCloudService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _pCloudService.DeleteOldFilesAsync();
            await Task.Delay(_interval, stoppingToken);
        }
    }
}
