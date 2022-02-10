namespace OpilioCraft.PortableDevices
{
    public partial class PortableDevice
    {
        public class File : ContentItem
        {
            public File(PortableDeviceHelper _, string fileId, string fileName) : base(fileId, fileName)
            {}
        }
    }
}
