using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;

namespace BASpark
{
    internal static class WebView2EnvironmentHolder
    {
        private static CoreWebView2Environment? _environment;
        private static readonly SemaphoreSlim InitLock = new(1, 1);

        public static async Task<CoreWebView2Environment> GetOrCreateAsync()
        {
            if (_environment != null)
            {
                return _environment;
            }

            await InitLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_environment != null)
                {
                    return _environment;
                }

                var options = new CoreWebView2EnvironmentOptions(
                    "--disable-background-timer-throttling --disable-features=CalculateNativeWinOcclusion --enable-begin-frame-scheduling"
                );

                string userDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "BASpark_WebView2");

                _environment = await CoreWebView2Environment
                    .CreateAsync(null, userDataFolder, options)
                    .ConfigureAwait(false);

                return _environment;
            }
            finally
            {
                InitLock.Release();
            }
        }
    }
}
