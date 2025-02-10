namespace OpilioCraft.PortableDevices
{
    public partial class PortableDevice
    {
        public abstract class ContentItem(string id, string name)
        {
            public string Id { get; } = id;
            public string Name { get; } = name;
        }
    }
}
