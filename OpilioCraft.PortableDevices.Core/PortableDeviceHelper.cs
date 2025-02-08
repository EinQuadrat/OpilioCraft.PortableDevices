using PortableDeviceApiLib;
using IPortableDeviceValues = PortableDeviceApiLib.IPortableDeviceValues;

namespace OpilioCraft.PortableDevices
{
    partial class PortableDevice
    {
        public class PortableDeviceHelper
        {
            public PortableDeviceHelper(string _)
            {
                PortableDeviceClass portableDeviceClass = new PortableDeviceClass() ?? throw new PortableDeviceException("cannot instantiate portable device class");
                RawDevice = portableDeviceClass;
            }

            public PortableDeviceClass RawDevice { get; }

            public IPortableDeviceContent DeviceContent {
                get
                {
                    RawDevice.Content(out IPortableDeviceContent content);
                    return content;
                }
            }

            public IPortableDeviceProperties DeviceProperties
            {
                get
                {
                    DeviceContent.Properties(out IPortableDeviceProperties properties);
                    return properties;
                }
            }

            // item specific methods
            public IPortableDeviceValues GetItemValues(string itemId)
            {
                DeviceProperties.GetSupportedProperties(itemId, out IPortableDeviceKeyCollection propKeys);
                DeviceProperties.GetValues(itemId, propKeys, out IPortableDeviceValues propValues);

                return propValues;
            }

            // item factory
            public ContentItem CreateContentItemFromId(string itemId)
            {
                var itemValues = GetItemValues(itemId);

                // retrieve name for itemId
                var nameProperty = PropertyKeys.WPD_OBJECT_NAME;
                itemValues.GetStringValue(nameProperty, out string name);

                // determine item type and create wrapper object
                var typeProperty = PropertyKeys.WPD_OBJECT_CONTENT_TYPE;
                itemValues.GetGuidValue(typeProperty, out Guid itemType);

                return
                    (itemType == LowLevelAPI.FunctionalType || itemType == LowLevelAPI.FolderType)
                    ? new Folder(this, itemId, name)
                    : new File(this, itemId, name);
            }

            public Folder CreateFolderItemFromId(string itemId) =>
                    CreateContentItemFromId(itemId) is Folder folderItem
                    ? folderItem
                    : throw new ArgumentException($"{itemId} is not an existing folder");

            // container handling
            public IList<string> EnumerateContainer(string containerId)
            {
                var childItems = new List<string>();

                DeviceContent.EnumObjects(0, containerId, null, out IEnumPortableDeviceObjectIDs objectIds);

                uint fetched = 0;
                string objectId;

                bool hasNext() { objectIds.Next(1, out objectId, ref fetched); return fetched > 0; }

                while (hasNext())
                {
                    childItems.Add(objectId);
                }

                return childItems;
            }
        }
    }
}
