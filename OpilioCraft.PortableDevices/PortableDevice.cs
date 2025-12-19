using System.Runtime.InteropServices;

using IStream_ComType = System.Runtime.InteropServices.ComTypes.IStream;

using PortableDeviceApiLib;
using PortableDeviceTypesLib;
using System.Diagnostics.CodeAnalysis;

namespace OpilioCraft.PortableDevices
{
    public partial class PortableDevice
    {
        // used for thread safety
        private readonly Lock syncLock = new();

        // device access handler
        private readonly PortableDeviceHelper _deviceHelper;

        // track connection state
        private bool _isConnected = false;

        public PortableDevice(string deviceId)
        {
            DeviceId = deviceId;
            FriendlyName = PortableDeviceManager.GetFriendlyDeviceName(DeviceId);

            // to access anything of the device
            _deviceHelper = new PortableDeviceHelper(deviceId);
        }

        // simple properties
        public string DeviceId { get; }
        public string FriendlyName { get; }

        // access connected device
        public PortableDeviceHelper DeviceHelper => EnsureConnection()._deviceHelper;

        // connection handling
        public void Connect(bool readOnly = false)
        {
            // prevent multiple concurrent connect request
            syncLock.Enter();
            try
            {
                if (!_isConnected)
                {
                    _deviceHelper.RawDevice.Open(DeviceId, readOnly ? LowLevelAPI.RequestReadOnlyAccess : LowLevelAPI.RequestReadWriteAccess);
                    _isConnected = true;
                }
            }
            finally
            {
                syncLock.Exit();
            }
        }

        public void Disconnect()
        {
            syncLock.Enter();
            try
            {
                if (_isConnected)
                {
                    _deviceHelper.RawDevice.Close();
                    _isConnected = false;
                }
            }
            finally
            {
                syncLock.Exit();
            }
        }

        public PortableDevice EnsureConnection()
        {
            if (!_isConnected)
            {
                Connect();
            }

            return this;
        }

        // NOTE: the following method expects the device to be connected! it is ensured via DeviceInfo

        // device root
        public Folder GetRootFolder() => DeviceHelper.CreateFolderItemFromId("DEVICE");

        public bool TryGetRootFolder([MaybeNullWhen(returnValue: false)] out Folder rootFolder)
        {
            try
            {
                rootFolder = GetRootFolder();
            }
            catch (Exception)
            {
                rootFolder = null;
            }

            return rootFolder is not null;
        }

        // device folder
        public Folder GetFolder(string pathToFolder)
        {
            static Folder treeWalker(Folder folder, string[] path)
            {
                if (path.Length == 0)
                {
                    return folder;
                }
                else
                {
                    Folder subfolder = folder.GetItemByName(path[0]) as Folder
                    ?? throw new InvalidOperationException($"[{nameof(PortableDevice)}] content item is not a folder: {path[0]}");

                    return path.Length == 1 ? subfolder : treeWalker(subfolder, path[1..]);
                }
            }

            return treeWalker(GetRootFolder(), pathToFolder.Split('/'));
        }

        public bool TryGetFolder(string pathToFolder, [MaybeNullWhen(returnValue: false)] out Folder folder)
        {
            try
            {
                folder = GetFolder(pathToFolder);
            }
            catch (Exception)
            {
                folder = null;
            }

            return folder is not null;
        }

        // high-level device API
        public void DownloadFile(File file, string saveToPath)
        {
            IStream_ComType? sourceStream = null;

            try
            {
                DeviceHelper.DeviceContent.Transfer(out IPortableDeviceResources resources);

                uint optimalTransferSize = 0;
                var property = PropertyKeys.WPD_RESOURCE_DEFAULT;

                resources.GetStream(file.Id, ref property, 0, ref optimalTransferSize, out PortableDeviceApiLib.IStream wpdStream);
                sourceStream = wpdStream as IStream_ComType;

                using FileStream targetStream = new(Path.Combine(saveToPath, file.Name), FileMode.Create, FileAccess.Write);

                unsafe
                {
                    var buffer = new byte[optimalTransferSize];
                    int bytesRead;

                    if (sourceStream != null)
                    {
                        do
                        {
                            sourceStream.Read(buffer, (int)optimalTransferSize, new IntPtr(&bytesRead));
                            if (bytesRead > 0) { targetStream.Write(buffer, 0, bytesRead); }
                        } while (bytesRead > 0);
                    }
                }

                targetStream.Close();
            }
            catch (Exception exn)
            {
                throw new PortableDeviceException($"[unexpected failure while transferring item to PC", exn);
            }
            finally
            {
                if (null != sourceStream) {
                    Marshal.FinalReleaseComObject(sourceStream);
                    sourceStream = null;

                    //GC.Collect();
                    //GC.WaitForPendingFinalizers();
                }
            }
        }

        public void DeleteFile(File file)
        {
            var objectIds = (PortableDeviceApiLib.IPortableDevicePropVariantCollection) new PortableDevicePropVariantCollection();

            PortableDeviceApiLib.tag_inner_PROPVARIANT propVariant = LowLevelAPI.StringToPropVariant(file.Id);
            objectIds.Add(propVariant);

            DeviceHelper.DeviceContent.Delete(0, objectIds, null);
        }

        public void MoveFile(File file, string saveToPath)
        {
            DownloadFile(file, saveToPath);
            DeleteFile(file);
        }
    }
}
