using System;
using System.IO;
using System.Runtime.InteropServices;

using IStream_ComType = System.Runtime.InteropServices.ComTypes.IStream;

using PortableDeviceApiLib;
using PortableDeviceTypesLib;

namespace OpilioCraft.PortableDevices
{
    public partial class PortableDevice
    {
        // used for thread safety
        private readonly object syncLock = new();

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
            lock (syncLock)
            {
                if (!_isConnected)
                {
                    _deviceHelper.RawDevice.Open(DeviceId, readOnly ? LowLevelAPI.RequestReadOnlyAccess : LowLevelAPI.RequestReadWriteAccess);
                    _deviceHelper.Refresh();
                    _isConnected = true;
                }
            }
        }

        public void Disconnect()
        {
            lock (syncLock)
            {
                if (_isConnected)
                {
                    _deviceHelper.RawDevice.Close();
                    _isConnected = false;
                }
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
        public Folder GetRootFolder() => DeviceHelper.CreateItemFromId<Folder>("DEVICE");

        // device folder
        public Folder GetFolder(string pathToFolder)
        {
            var folder = GetRootFolder();

            // subfolder requested?
            if (pathToFolder.Length > 0)
            {
                foreach (var part in pathToFolder.Split('/'))
                {
                    var item = folder.GetItemByName(part);

                    if (item is Folder)
                    {
                        folder = folder.GetItemByName(part) as Folder;
                    }
                    else
                    {
                        throw new InvalidOperationException($"[{nameof(PortableDevice)}] content item is not a folder: {part}");
                    }
                }
            }

            return folder;
        }

        // high-level device API
        public void DownloadFile(File file, string saveToPath)
        {
            IStream_ComType sourceStream = null;

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

                    do
                    {
                        sourceStream.Read(buffer, (int)optimalTransferSize, new IntPtr(&bytesRead));
                        if (bytesRead > 0) { targetStream.Write(buffer, 0, bytesRead); }
                    } while (bytesRead > 0);
                }

                targetStream.Close();
            }
            catch (Exception exn)
            {
                Console.Error.WriteLine($"[{nameof(PortableDevice)}] unexpected failure while transferring item to PC: {exn.Message}");
                throw;
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
            var objectIds = new PortableDevicePropVariantCollection() as PortableDeviceApiLib.IPortableDevicePropVariantCollection;

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
