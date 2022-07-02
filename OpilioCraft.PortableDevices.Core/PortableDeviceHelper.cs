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
                PortableDeviceClass? portableDeviceClass = new();

                if (portableDeviceClass is null)
                {
                    throw new PortableDeviceException("cannot instantiate portable device class");
                }

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
            public T CreateItemFromId<T>(string itemId) where T : ContentItem
            {
                ContentItem item;
                var itemValues = GetItemValues(itemId);

                // retrieve name for itemId
                var nameProperty = PropertyKeys.WPD_OBJECT_NAME;
                itemValues.GetStringValue(nameProperty, out string name);

                // determine item type and create wrapper object
                var typeProperty = PropertyKeys.WPD_OBJECT_CONTENT_TYPE;
                itemValues.GetGuidValue(typeProperty, out Guid itemType);

                if (itemType == LowLevelAPI.FunctionalType || itemType == LowLevelAPI.FolderType)
                {
                    item = new Folder(this, itemId, name);
                }
                else
                {
                    item = new File(this, itemId, name);
                }

                return (T) item; // will throw an exception on incompatible types
            }

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
