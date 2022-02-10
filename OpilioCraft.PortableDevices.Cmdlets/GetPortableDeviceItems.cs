using System.Management.Automation;

namespace OpilioCraft.PortableDevices.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "PortableDeviceItems")]
    [OutputType(typeof(PortableDevice.ContentItem))]
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

                // navigate to folder
                var folder = device.GetFolder(Path);

                // enumerate content
                foreach (var item in folder.ChildItems)
                {
                    WriteObject(sendToPipeline: item);
                }

                // free resources
                device.Disconnect();
            }
            catch (System.Exception exn)
            {
                WriteError(errorRecord: new ErrorRecord(exn, null, ErrorCategory.DeviceError, this));
            }
        }
    }
}
