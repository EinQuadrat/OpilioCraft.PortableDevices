using System.Management.Automation;
using Windows.Storage;

namespace OpilioCraft.PortableDevices.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "PortableDeviceItem")]
    //[OutputType(typeof(StorageDevice))]
    public class GetPortableDeviceItem : PSCmdlet
    {
        // params
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
        public required StorageFolder DeviceRoot { get; set; }

        [Parameter()]
        public string Path { get; set; } = string.Empty;

        // cmdlet functionality
        protected override void ProcessRecord()
        {
            try
            {
                var items = DeviceRoot.GetItemsAsync().AsTask().Result;

                // enumerate content
                foreach (IStorageItem item in items)
                {
                    WriteObject(sendToPipeline: item);
                }
            }
            catch (Exception exn)
            {
                WriteError(errorRecord: new ErrorRecord(exn, null, ErrorCategory.DeviceError, this));
            }
        }
    }
}
