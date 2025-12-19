using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.Storage;

namespace OpilioCraft.PortableDevices.TestApp
{
    class Program
    {
        public static async Task Main(string[] _)
        {
            Console.WriteLine("Retrieve devices...");
            string selector = StorageDevice.GetDeviceSelector(); // AQS-String für tragbare Speichergeräte (WPD/MTP, USB-Massenspeicher)
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector); // Alle passenden Geräte holen

            Console.WriteLine($"{devices.Count} devices found");

            foreach (var dev in devices)
            {
                Console.WriteLine($"Device ID = {dev.Id}");
                Console.WriteLine($"Friendly name = {dev.Name}");
                Console.WriteLine();

                // StorageDevice -> StorageFolder (Root des Geräts)
                StorageFolder root = StorageDevice.FromId(dev.Id); // liefert Root-Ordner des Geräts

                var items = await root.GetItemsAsync(); // Dateien/Ordner im Root
                Console.WriteLine($"Einträge im Root ({items.Count}):");

                foreach (var item in items)
                {
                    Console.WriteLine($" {item.Name} ({item.GetType().Name})");
                }
            }

                    //    foreach (var item in rootFolder.GetSubFolders())
                    //    {
                    //        Console.WriteLine($"{item.Name}");
                    //    }
                    //    Console.WriteLine();

                    //    Console.Write("Trying to get phone folder... ");
                    //    if (dev.TryGetFolder("Interner Speicher", out var phoneFolder))
                    //    {
                    //        Console.WriteLine("OK");

                    //        foreach (var item in phoneFolder.ChildItems)
                    //        {
                    //            Console.WriteLine($"{item.Name}");
                    //        }

                    //        Console.Write("\nGetting camera folder... ");
                    //        var cameraFolder = dev.GetFolder("Interner Speicher/DCIM/Camera");
                    //        Console.WriteLine("OK\n");

                    //        foreach (var file in cameraFolder.GetFiles())
                    //        {
                    //            Console.Write($"Transferring {file.Name} ");
                    //            dev.DownloadFile(file, "C:/opt/Testing");
                    //            Console.WriteLine("OK");
                    //        }
                    //        Console.WriteLine();
                    //    }
                    //    Console.WriteLine("No phone folder found.");
        }
    }
}
