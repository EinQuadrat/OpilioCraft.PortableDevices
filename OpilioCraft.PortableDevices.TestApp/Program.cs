namespace OpilioCraft.PortableDevices.TestApp
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

                Console.Write("Trying to get device root... ");

                if (dev.TryGetRootFolder(out var rootFolder))
                {
                    Console.WriteLine("OK");

                    foreach (var item in rootFolder.GetSubFolders())
                    {
                        Console.WriteLine($"{item.Name}");
                    }
                    Console.WriteLine();

                    Console.Write("Trying to get phone folder... ");
                    if (dev.TryGetFolder("Interner Speicher", out var phoneFolder))
                    {
                        Console.WriteLine("OK");

                        foreach (var item in phoneFolder.ChildItems)
                        {
                            Console.WriteLine($"{item.Name}");
                        }

                        Console.Write("\nGetting camera folder... ");
                        var cameraFolder = dev.GetFolder("Interner Speicher/DCIM/Camera");
                        Console.WriteLine("OK\n");

                        foreach (var file in cameraFolder.GetFiles())
                        {
                            Console.Write($"Transferring {file.Name} ");
                            dev.DownloadFile(file, "C:/opt/Testing");
                            Console.WriteLine("OK");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine("No phone folder found.");
                }
                else
                {
                    Console.WriteLine("Oops... Found device is not compatible.");
                }

                Console.Write("Disconnecting from device... ");
                dev.Disconnect();
                Console.WriteLine("OK");
            }
        }
    }
}
