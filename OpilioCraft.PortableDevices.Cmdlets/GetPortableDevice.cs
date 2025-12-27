using System.Management.Automation;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.Storage;

namespace OpilioCraft.PortableDevices.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "PortableDevice")]
    [OutputType(typeof(StorageFolder))]
    public class GetPortableDevice : PSCmdlet {
        // params
        [Parameter(Mandatory = false, HelpMessage = "If specified, retrieves only devices that match the given device id.")]
        public string ById { get; set; } = string.Empty;

        [Parameter(Mandatory = false, HelpMessage = "If specified, retrieves only devices that match the given name.")]
        public string ByName { get; set; } = string.Empty;

        // behaviour
        protected override void EndProcessing()
        {
            base.EndProcessing();

            try
            {
                string selector = StorageDevice.GetDeviceSelector(); // AQS-String für tragbare Speichergeräte (WPD/MTP, USB-Massenspeicher)
                DeviceInformationCollection devCollection = DeviceInformation.FindAllAsync(selector).AsTask<DeviceInformationCollection>().Result; // Alle passenden Geräte holen
                IEnumerable<DeviceInformation> devices = devCollection.ToList();

                if (!string.IsNullOrEmpty(ById))
                {
                    devices = devices.Where(dev => dev.Id.Equals(ById));
                }

                if (!string.IsNullOrEmpty(ByName))
                {
                    devices = devices.Where(dev => dev.Name.Equals(ByName));
                }

                foreach (DeviceInformation dev in devices)
                {
                    WriteObject(sendToPipeline: StorageDevice.FromId(dev.Id));
                }
            }
            catch (Exception exn)
            {
                WriteError(errorRecord: new ErrorRecord(exn, null, ErrorCategory.DeviceError, this));
            }
        }
    }
}
