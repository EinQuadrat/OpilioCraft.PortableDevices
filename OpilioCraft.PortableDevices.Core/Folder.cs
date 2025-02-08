namespace OpilioCraft.PortableDevices
{
    public partial class PortableDevice
    {
        public class Folder : ContentItem {
            private readonly IList<string> _childItemIds;
            private readonly Lazy<IList<ContentItem>> _childItems; // delayed child creation

            public Folder(PortableDeviceHelper deviceHelper, string folderId, string folderName) : base(folderId, folderName)
            {
                _childItemIds = deviceHelper.EnumerateContainer(folderId);
                IList<ContentItem> contentFetcher() => _childItemIds.Select(deviceHelper.CreateContentItemFromId).ToList();
                _childItems = new Lazy<IList<ContentItem>>(contentFetcher);
            }

            // properties
            public IList<ContentItem> ChildItems => _childItems.Value;
            public bool HasChildren => _childItemIds.Count > 0;

            // methods
            public ContentItem GetItemByName(string name) => ChildItems.First(item => item.Name == name);

            public IList<Folder> GetSubFolders() => ChildItems.Where(item => item is Folder).Select(item => (Folder) item).ToList();
            public IList<File> GetFiles() => ChildItems.Where(item => item is File).Select(item => (File) item).ToList();
        }
    }
}
