using System.Collections.Generic;

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
                RawDevice = new();
            }

            public PortableDeviceClass RawDevice { get; }

            public IPortableDeviceContent DeviceContent { get; private set; }
            public IPortableDeviceProperties DeviceProperties { get; private set; }

            public void Refresh()
            {
                RawDevice.Content(out IPortableDeviceContent content);
                DeviceContent = content;

                DeviceContent.Properties(out IPortableDeviceProperties properties);
                DeviceProperties = properties;
            }

            // item specific methods
            public IPortableDeviceValues GetItemValues(string itemId)
            {
                DeviceProperties.GetSupportedProperties(itemId, out IPortableDeviceKeyCollection propKeys);
                DeviceProperties.GetValues(itemId, propKeys, out IPortableDeviceValues propValues);

                return propValues;
            }

            // item factory
            public TItem CreateItemFromId<TItem>(string itemId) where TItem : ContentItem
            {
                ContentItem item;
                var itemValues = GetItemValues(itemId);

                // retrieve name for itemId
                var nameProperty = PropertyKeys.WPD_OBJECT_NAME;
                itemValues.GetStringValue(nameProperty, out string name);

                // determine item type and create wrapper object
                var typeProperty = PropertyKeys.WPD_OBJECT_CONTENT_TYPE;
                itemValues.GetGuidValue(typeProperty, out System.Guid itemType);

                if (itemType == LowLevelAPI.FunctionalType || itemType == LowLevelAPI.FolderType)
                {
                    item = new Folder(this, itemId, name);
                }
                else
                {
                    item = new File(this, itemId, name);
                }

                return item as TItem;
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
