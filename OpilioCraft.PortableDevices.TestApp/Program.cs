using OpilioCraft.PortableDevices;

namespace TestApp
{
    class Program
    {
        static void Main(string[] _)
        {
            Console.WriteLine("Retrieve devices...");
            var devices = PortableDeviceManager.GetPortableDevices(refresh: true);
            Console.WriteLine($"{devices.Count} devices found");

            foreach (var dev in devices)
            {
                Console.WriteLine($"Device ID = {dev.DeviceId}");
                Console.WriteLine($"Friendly Name = {dev.FriendlyName}");
                Console.WriteLine();

                Console.Write("Getting device root... ");
                var rootFolder = dev.GetRootFolder();
                Console.WriteLine("OK");

                Console.WriteLine();
                foreach (var item in rootFolder.GetSubFolders())
                {
                    Console.WriteLine($"{item.Name}");
                }
                Console.WriteLine();

                Console.Write("Getting phone folder... ");
                var phoneFolder = dev.GetFolder("Interner Speicher");
                Console.WriteLine("OK");

                Console.WriteLine();
                foreach (var item in phoneFolder.ChildItems)
                {
                    Console.WriteLine($"{item.Name}");
                }
                Console.WriteLine();

                Console.Write("Getting camera folder... ");
                var cameraFolder = dev.GetFolder("Interner Speicher/DCIM/Camera");
                Console.WriteLine("OK");

                Console.WriteLine();
                foreach (var file in cameraFolder.GetFiles())
                {
                    Console.Write($"Transferring {file.Name} ");
                    dev.DownloadFile(file, "C:/opt/Testing");
                    Console.WriteLine("OK");
                }
                Console.WriteLine();

                Console.Write("Disconnecting from device... ");
                dev.Disconnect();
                Console.WriteLine("OK");
            }
        }
    }
}
