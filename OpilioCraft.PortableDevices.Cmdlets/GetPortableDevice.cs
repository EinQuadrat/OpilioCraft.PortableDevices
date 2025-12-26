using System.Management.Automation;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;

namespace OpilioCraft.PortableDevices.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "PortableDevice")]
    [OutputType(typeof(DeviceInformation))]
    public class GetPortableDevice : PSCmdlet {
        // params
        [Parameter(ParameterSetName = "ById")]
        [Parameter(Mandatory = false, HelpMessage = "If specified, retrieves only devices that match the given device id.")]
        public string ById { get; set; } = string.Empty;

        [Parameter(ParameterSetName = "ByName")]
        [Parameter(Mandatory = false, HelpMessage = "If specified, retrieves only devices that match the given name.")]
        public string ByName { get; set; } = string.Empty;

        // behaviour
        protected override async void EndProcessing()
        {
            base.EndProcessing();

            try
            {
                string selector = StorageDevice.GetDeviceSelector(); // AQS-String für tragbare Speichergeräte (WPD/MTP, USB-Massenspeicher)
                IEnumerable<DeviceInformation> devices = await DeviceInformation.FindAllAsync(selector); // Alle passenden Geräte holen

                if (!string.IsNullOrEmpty(ById))
                {
                    devices = devices.Where(dev => dev.Id.Equals(ById));
                }

                if (!string.IsNullOrEmpty(ByName))
                {
                    devices = devices.Where(dev => dev.Name.Equals(ByName));
                }

                foreach (var dev in devices)
                {
                    WriteObject(sendToPipeline: dev);
                }
            }
            catch (Exception exn)
            {
                WriteError(errorRecord: new ErrorRecord(exn, null, ErrorCategory.DeviceError, this));
            }
        }
    }
}
