using System.Management.Automation;
using Windows.Devices.Portable;

namespace OpilioCraft.PortableDevices.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "PortableDeviceItems")]
    [OutputType(typeof(StorageDevice))]
    public class GetPortableDeviceItems : PSCmdlet {
        // params
        [Parameter(Position = 0, Mandatory = true)]
        public string Device { get; set; } = string.Empty;

        [Parameter()]
        public string Path { get; set; } = string.Empty;

        [Parameter()]
        public SwitchParameter Refresh { get; set; } = new(false);

        // cmdlet functionality
        protected override void ProcessRecord()
        {
            try
            {
                // lookup device
                var device = PortableDeviceManager.GetDeviceByName(Device, refresh: Refresh.ToBool());

                if (device is not null)
                {
                    var rootFolder = StorageDevice.FromId(device.Id);

                    // navigate to folder
                    var folder = rootFolder.GetFolder(Path);

                    // enumerate content
                    foreach (var item in folder.ChildItems)
                    {
                        WriteObject(sendToPipeline: item);
                    }
                }
                else
                {
                    throw new Exception($"Device '{Device}' not found.");
                }
            }
            catch (Exception exn)
            {
                WriteError(errorRecord: new ErrorRecord(exn, null, ErrorCategory.DeviceError, this));
            }
        }
    }
}
