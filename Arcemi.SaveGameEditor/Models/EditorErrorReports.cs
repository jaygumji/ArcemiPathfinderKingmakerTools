using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Arcemi.SaveGameEditor.Models
{
    public class EditorErrorReports
    {
        private readonly ConcurrentQueue<string> _pending = new();
        private readonly ILogger<EditorErrorReports> _logger;

        public EditorErrorReports(ILogger<EditorErrorReports> logger)
        {
            _logger = logger;
        }

        public event Action Reported;

        public void Report(string operation, Exception exception)
        {
            _logger.LogError(exception, "{Operation} failed", operation);
            _pending.Enqueue(Describe(exception, operation));
            Reported?.Invoke();
        }

        public bool TryTake(out string report) => _pending.TryDequeue(out report);

        public static string Describe(Exception exception, string operation = null)
        {
            var details = exception.ToString();
            var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (profile.Length > 1) {
                details = details.Replace(profile, "~",
                    OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
            }
            details = Regex.Replace(details, @"([?&]token=|/electronAuthToken=)[^\s&]+", "$1[redacted]", RegexOptions.IgnoreCase);
            return string.IsNullOrEmpty(operation) ? details : $"Operation: {operation}\n\n{details}";
        }
    }
}
