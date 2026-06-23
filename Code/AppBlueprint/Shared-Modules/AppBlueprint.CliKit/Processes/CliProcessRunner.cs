using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.CliKit.Processes;

public sealed record CliProcessRequest(
    string FileName,
    string Arguments = "",
    string? WorkingDirectory = null,
    TimeSpan? Timeout = null,
    bool StreamOutput = true,
    bool UseShellExecute = false);

public sealed record CliProcessResult(
    int ExitCode,
    string StandardOutput,
    string StandardError,
    bool TimedOut)
{
    public bool Succeeded => ExitCode == 0 && !TimedOut;
}

public interface ICliProcessRunner
{
    Task<CliProcessResult> RunAsync(CliProcessRequest request, CancellationToken cancellationToken = default);
}

public sealed class CliProcessRunner : ICliProcessRunner
{
    private readonly ILogger<CliProcessRunner> _logger;

    public CliProcessRunner(ILogger<CliProcessRunner> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CliProcessResult> RunAsync(CliProcessRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.FileName);

#pragma warning disable CA2000 // Disposed by using var; analyzer does not follow nullable factory here.
        using var timeoutCts = CreateTimeoutTokenSource(request.Timeout);
#pragma warning restore CA2000
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            timeoutCts?.Token ?? CancellationToken.None);

        var startInfo = new ProcessStartInfo
        {
            FileName = request.FileName,
            Arguments = request.Arguments,
            WorkingDirectory = request.WorkingDirectory ?? Directory.GetCurrentDirectory(),
            UseShellExecute = request.UseShellExecute,
            RedirectStandardOutput = !request.UseShellExecute,
            RedirectStandardError = !request.UseShellExecute,
            CreateNoWindow = !request.UseShellExecute
        };

        using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
        var output = new System.Text.StringBuilder();
        var error = new System.Text.StringBuilder();

        if (!request.UseShellExecute)
        {
            process.OutputDataReceived += (_, args) =>
            {
                if (args.Data is null) return;
                output.AppendLine(args.Data);
                if (request.StreamOutput) _logger.LogInformation("{Line}", args.Data);
            };

            process.ErrorDataReceived += (_, args) =>
            {
                if (args.Data is null) return;
                error.AppendLine(args.Data);
                if (request.StreamOutput) _logger.LogWarning("{Line}", args.Data);
            };
        }

        _logger.LogDebug("Starting process: {FileName} {Arguments}", request.FileName, request.Arguments);

        if (!process.Start())
        {
            throw new InvalidOperationException($"Failed to start process '{request.FileName}'.");
        }

        if (!request.UseShellExecute)
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        try
        {
            await process.WaitForExitAsync(linkedCts.Token);
            return new CliProcessResult(process.ExitCode, output.ToString(), error.ToString(), TimedOut: false);
        }
        catch (OperationCanceledException) when (timeoutCts?.IsCancellationRequested == true && !cancellationToken.IsCancellationRequested)
        {
            TryKill(process);
            return new CliProcessResult(-1, output.ToString(), error.ToString(), TimedOut: true);
        }
    }

    private static CancellationTokenSource? CreateTimeoutTokenSource(TimeSpan? timeout)
    {
        return timeout.HasValue ? new CancellationTokenSource(timeout.Value) : null;
    }

    private static void TryKill(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch (InvalidOperationException)
        {
            // Best-effort cleanup for a CLI helper.
        }
        catch (NotSupportedException)
        {
            // Best-effort cleanup for a CLI helper.
        }
        catch (System.ComponentModel.Win32Exception)
        {
            // Best-effort cleanup for a CLI helper.
        }
    }
}
