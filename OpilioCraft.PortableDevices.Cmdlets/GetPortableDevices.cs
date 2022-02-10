using System.Management.Automation;

namespace OpilioCraft.PortableDevices.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "PortableDevices")]
    [OutputType(typeof(PortableDevice))]
    public class GetPortableDevices : PSCmdlet {
        // params
        [Parameter()]
        public SwitchParameter Refresh { get; set; } = new (false);

        // cmdlet functionality
        protected override void ProcessRecord()
        {
            try
            {
                var devices = PortableDeviceManager.GetPortableDevices(refresh: Refresh.ToBool());

                foreach (var dev in devices)
                {
                    WriteObject(sendToPipeline: dev);
                }
            }
            catch (System.Exception exn)
            {
                WriteError(errorRecord: new ErrorRecord(exn, null, ErrorCategory.DeviceError, this));
            }
        }
    }
}
