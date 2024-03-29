﻿namespace OpilioCraft.PortableDevices
{
    public partial class PortableDevice
    {
        public abstract class ContentItem
        {
            protected ContentItem(string id, string name)
            {
                Id = id;
                Name = name;
            }

            public string Id { get; }
            public string Name { get; }
        }
    }
}
