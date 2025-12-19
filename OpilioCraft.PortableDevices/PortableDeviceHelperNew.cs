using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.Storage;

namespace OpilioCraft.PortableDevices
{
    partial class PortableDeviceNew
    {
        // Neue ContentItem / Folder / File kannst du anpassen;
        // hier wird davon ausgegangen, dass sie Name/Id/Typ speichern.
        public class PortableDeviceHelperNew
        {
            public PortableDeviceHelperNew(string deviceId)
            {
                DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));

                // StorageDevice.FromId darf nicht im Ctor blockierend aufgerufen werden,
                // daher Async-Init-Methode benutzen.
            }

            public string DeviceId { get; }
            public StorageFolder RootFolder { get; private set; } = default!;

            /// <summary>
            /// Asynchrone Initialisierung: StorageDevice / RootFolder holen.
            /// Diese Methode MUSS vor den anderen Methoden einmal aufgerufen werden.
            /// </summary>
            public async Task InitializeAsync()
            {
                // Gerät anhand der Id auflösen
                RootFolder = StorageDevice.FromId(DeviceId); // WPD-Storage des Geräts[web:73]
                // Optional: einmal lesen, um Fehler früh zu sehen
                await RootFolder.GetItemsAsync();
            }

            // item factory – statt WPD-Objekt-ID einfach mit StorageItem arbeiten
            public async Task<ContentItem> CreateContentItemFromPathAsync(string relativePath)
            {
                if (RootFolder == null)
                    throw new InvalidOperationException("Helper not initialized. Call InitializeAsync() first.");

                // relativePath kann z. B. "DCIM/100MEDIA" oder "DCIM/100MEDIA/IMG_0001.JPG" sein
                StorageItem item =
                    await RootFolder.TryGetItemAsync(relativePath)
                    ?? throw new ArgumentException($"Item '{relativePath}' not found on device.");

                // Name & Typ direkt vom StorageItem
                string name = item.Name;
                bool isFolder = item.IsOfType(StorageItemTypes.Folder);

                if (isFolder)
                {
                    var folder = (StorageFolder)item;
                    return new Folder(this, folder.Path, name); // oder eigene Id/Path-Logik
                }
                else
                {
                    var file = (StorageFile)item;
                    return new File(file.Path, name);
                }
            }

            public async Task<Folder> CreateFolderItemFromPathAsync(string relativePath)
            {
                var item = await CreateContentItemFromPathAsync(relativePath);

                return item is Folder folderItem
                    ? folderItem
                    : throw new ArgumentException($"{relativePath} is not an existing folder");
            }

            // Container handling – Kinder eines Ordners auflisten
            public async Task<IList<string>> EnumerateContainerAsync(string? relativeFolderPath)
            {
                if (RootFolder == null)
                    throw new InvalidOperationException("Helper not initialized. Call InitializeAsync() first.");

                StorageFolder folder;

                if (string.IsNullOrEmpty(relativeFolderPath))
                {
                    folder = RootFolder;
                }
                else
                {
                    folder =
                        await RootFolder.TryGetItemAsync(relativeFolderPath) as StorageFolder
                        ?? throw new ArgumentException($"{relativeFolderPath} is not an existing folder");
                }

                var result = new List<string>();

                IReadOnlyList<IStorageItem> items = await folder.GetItemsAsync(); // listet Dateien & Unterordner[web:25]

                // Du kannst hier frei entscheiden, welche "Id" du verwendest:
                // - Name
                // - Vollständiger Pfad (item.Path)
                // - Eigene synthetische ID
                foreach (var item in items)
                {
                    result.Add(item.Path);
                }

                return result;
            }
        }
    }
}
