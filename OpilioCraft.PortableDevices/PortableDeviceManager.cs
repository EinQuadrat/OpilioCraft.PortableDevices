using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.Storage;

namespace OpilioCraft.PortableDevices
{
    public class PortableDeviceManager
    {
        private static readonly string _deviceSelector = StorageDevice.GetDeviceSelector(); // AQS-String für tragbare Speichergeräte (WPD/MTP, USB-Massenspeicher)

        // access devices
        public static async Task<DeviceInformationCollection> GetPortableDevices(bool refresh = true)
        {
            return await DeviceInformation.FindAllAsync(_deviceSelector); // Alle passenden Geräte holen
        }

        public static bool Exists(string friendlyName) =>
            GetPortableDevices().Result.Any(dev => dev.Name == friendlyName);

        public static DeviceInformation GetDeviceByName(string friendlyName, bool refresh = false) =>
            GetPortableDevices().Result.First(dev => dev.Name == friendlyName);

        public static DeviceInformation GetDeviceById(string deviceId, bool refresh = false) =>
            GetPortableDevices().Result.First(dev => dev.Id == deviceId);
    }
}
