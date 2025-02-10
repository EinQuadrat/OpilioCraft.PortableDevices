namespace OpilioCraft.PortableDevices
{
    public class PortableDeviceException : Exception
    {
        public PortableDeviceException()
        {
        }

        public PortableDeviceException(string message)
            : base(message)
        {
        }

        public PortableDeviceException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
