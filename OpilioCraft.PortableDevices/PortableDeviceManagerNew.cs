using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.Storage;

namespace OpilioCraft.PortableDevices
{
    //internal class PortableDeviceManagerNew
    //{
    //}

    public class WinRtPortableDevice
    {
        public string DeviceId { get; }
        public string FriendlyName { get; }
        public StorageFolder RootFolder { get; }

        internal WinRtPortableDevice(DeviceInformation info, StorageFolder rootFolder)
        {
            DeviceId = info.Id;
            FriendlyName = info.Name;
            RootFolder = rootFolder;
        }
    }

    public static class WinRtPortableDeviceManager
    {
        private static readonly Dictionary<string, WinRtPortableDevice> _deviceCache =
            new Dictionary<string, WinRtPortableDevice>();

        public static async Task RefreshDeviceListAsync()
        {
            _deviceCache.Clear();

            // AQS-String für tragbare Speichergeräte (WPD/MTP, USB-Massenspeicher)
            string selector = StorageDevice.GetDeviceSelector(); // WinRT‑API[web:25]

            // Alle passenden Geräte holen
            DeviceInformationCollection devices =
                await DeviceInformation.FindAllAsync(selector);   // Enumeration‑API[web:61]

            foreach (var dev in devices)
            {
                // Root-StorageFolder des Geräts
                StorageFolder root = StorageDevice.FromId(dev.Id); // WinRT‑Storage‑Gerät[web:73]

                var portable = new WinRtPortableDevice(dev, root);

                // Name als Key, wie in deinem ursprünglichen Code
                if (!_deviceCache.ContainsKey(portable.FriendlyName))
                {
                    _deviceCache.Add(portable.FriendlyName, portable);
                }
            }
        }

        public static async Task<ICollection<WinRtPortableDevice>> GetPortableDevicesAsync(bool refresh = true)
        {
            if (refresh)
            {
                await RefreshDeviceListAsync();
            }

            return _deviceCache.Values.ToArray();
        }

        public static bool Exists(string friendlyName) => _deviceCache.ContainsKey(friendlyName);

        public static async Task<WinRtPortableDevice?> GetDeviceByNameAsync(string friendlyName, bool refresh = false)
        {
            if (refresh)
            {
                await RefreshDeviceListAsync();
            }

            return _deviceCache.TryGetValue(friendlyName, out var dev) ? dev : null;
        }

        public static async Task<WinRtPortableDevice?> GetDeviceByIdAsync(string deviceId, bool refresh = false)
        {
            if (refresh)
            {
                await RefreshDeviceListAsync();
            }

            return _deviceCache.Values.FirstOrDefault(d => d.DeviceId == deviceId);
        }
    }
}
