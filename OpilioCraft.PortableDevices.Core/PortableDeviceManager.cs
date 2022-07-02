using PortableDeviceApiLib;

namespace OpilioCraft.PortableDevices
{
    public class PortableDeviceManager
    {
        private static readonly IPortableDeviceManager _deviceManager = new PortableDeviceApiLib.PortableDeviceManager();
        private static readonly IDictionary<string, PortableDevice> _deviceCache = new Dictionary<string, PortableDevice>();

        public static void RefreshDeviceList()
        {
            _deviceCache.Clear();
            _deviceManager.RefreshDeviceList();

            unsafe
            {
                // Determine how many WPD devices are connected
                uint count = 1U; // according to API docs, it should work with 0U, but that did not show any results

                // Retrieve found devices
                if (count > 0)
                {
                    var deviceIds = new string[count];
                    _deviceManager.GetDevices(ref deviceIds[0], ref count);

                    foreach (var deviceId in deviceIds)
                    {
                        var device = new PortableDevice(deviceId);
                        _deviceCache.Add(device.FriendlyName, device);
                    }
                }
            }
        }

        // access devices
        public static ICollection<PortableDevice> GetPortableDevices(bool refresh = true)
        {
            if (refresh)
            {
                RefreshDeviceList();
            }

            return _deviceCache.Values;
        }

        public static bool Exists(string friendlyName) => _deviceCache.ContainsKey(friendlyName);

        public static PortableDevice GetDeviceByName(string friendlyName, bool refresh = false) {
            if (refresh) { RefreshDeviceList(); }
            return _deviceCache[friendlyName];
        }

        public static PortableDevice GetDeviceById(string deviceId, bool refresh = false)
        {
            if (refresh) { RefreshDeviceList(); }
            return _deviceCache.First(item => item.Value.DeviceId == deviceId).Value;
        }

        // helper methods
        public static string GetFriendlyDeviceName(string deviceId)
        {
            // Refresh first
            _deviceManager.RefreshDeviceList();

            // Request friendly device name
            var friendlyNameLength = 0U;

            unsafe // need to pass in null as pointer to a w_char array to get needed length of the w_char array
            {
                ushort* emptyName = null;
                _deviceManager.GetDeviceFriendlyName(deviceId, ref *emptyName, ref friendlyNameLength);
            }

            var rawFriendlyName = new ushort[friendlyNameLength];
            _deviceManager.GetDeviceFriendlyName(deviceId, ref rawFriendlyName[0], ref friendlyNameLength);
            friendlyNameLength -= 1; // skip trailing 0 byte

            // transform result to string
            var friendlyNameAsCharArray = new char[friendlyNameLength];

            for (var idx = 0; idx < friendlyNameLength; ++idx)
            {
                friendlyNameAsCharArray[idx] = (char)rawFriendlyName[idx];
            }

            return new string(friendlyNameAsCharArray);
        }
    }
}
