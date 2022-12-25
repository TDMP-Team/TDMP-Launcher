using Gameloop.Vdf.Linq;
using Gameloop.Vdf.JsonConverter;
using Gameloop.Vdf;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace TeardownMultiplayerLauncher.Core.Utilities
{
    public class DetectTeardownPathUtility
    {
        public static async Task<string?> TryGetTeardownPathAsync(string regKey)
        {
            RegistryKey? key = Registry.LocalMachine.OpenSubKey(regKey);
            if (key != null)
            {
                object? o = key.GetValue("InstallPath");
                if (o != null)
                {
                    VProperty volvo = VdfConvert.Deserialize(File.ReadAllText(o.ToString() + "/config/libraryfolders.vdf"));
                    foreach (var location in volvo.Value.ToList())
                    {
                        foreach (var item in location.ToJson().Children())
                        {
                            string installPath = item.SelectToken("path").ToString();
                            if (Directory.Exists(installPath + "\\steamapps\\common\\Teardown"))
                            {
                                return installPath + "\\steamapps\\common\\Teardown\\teardown.exe";
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
