//using System;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using Windows.Devices.Enumeration;
//using Windows.Devices.Portable;
//using Windows.Storage;
//using Windows.UI.Xaml;

//public partial class MainWindow : Window
//{
//    public MainWindow()
//    {
//        InitializeComponent();
//    }

//    private async void BtnListDevices_Click(object sender, RoutedEventArgs e)
//    {
//        try
//        {
//            string result = await ListPortableStorageDevicesAsync();
//            MessageBox.Show(result, "WPD / MTP Geräte");
//        }
//        catch (Exception ex)
//        {
//            MessageBox.Show(ex.ToString(), "Fehler");
//        }
//    }

//    private async Task<string> ListPortableStorageDevicesAsync()
//    {
//        var sb = new StringBuilder();

//        // AQS-String für tragbare Speichergeräte (WPD/MTP)
//        string selector = StorageDevice.GetDeviceSelector(); // Windows.Devices.Portable[web:25]

//        // Geräte suchen
//        DeviceInformationCollection devices =
//            await DeviceInformation.FindAllAsync(selector); // Windows.Devices.Enumeration[web:25]

//        if (devices.Count == 0)
//        {
//            sb.AppendLine("Keine tragbaren Speichergeräte gefunden.");
//            return sb.ToString();
//        }

//        foreach (var dev in devices)
//        {
//            sb.AppendLine($"Gerät: {dev.Name}");
//            sb.AppendLine($"Id: {dev.Id}");

//            // StorageDevice -> StorageFolder (Root des Geräts)
//            StorageFolder root = StorageDevice.FromId(dev.Id); // liefert Root-Ordner des Geräts[web:25]

//            var items = await root.GetItemsAsync(); // Dateien/Ordner im Root[web:25]
//            sb.AppendLine($"  Einträge im Root ({items.Count}):");

//            foreach (var item in items)
//            {
//                sb.AppendLine($"    {item.Name} ({item.GetType().Name})");
//            }

//            sb.AppendLine();
//        }

//        return sb.ToString();
//    }
//}
