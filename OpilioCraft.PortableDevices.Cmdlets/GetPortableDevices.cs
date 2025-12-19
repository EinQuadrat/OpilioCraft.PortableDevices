using System.Management.Automation;
using Windows.Devices.Portable;

namespace OpilioCraft.PortableDevices.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "PortableDevices")]
    [OutputType(typeof(StorageDevice))]
    public class GetPortableDevices : PSCmdlet {
        // params
        [Parameter()]
        public SwitchParameter Refresh { get; set; } = new (false);

        // cmdlet functionality
        protected override async void ProcessRecord()
        {
            try
            {
                var devices = await PortableDeviceManager.GetPortableDevices(refresh: Refresh.ToBool());

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
