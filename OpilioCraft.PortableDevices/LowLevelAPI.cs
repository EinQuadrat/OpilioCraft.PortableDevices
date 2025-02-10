using PortableDeviceTypesLib;

using _tagpropertykey = PortableDeviceApiLib._tagpropertykey;
using IPortableDeviceValues = PortableDeviceApiLib.IPortableDeviceValues;

namespace OpilioCraft.PortableDevices
{
    internal sealed class LowLevelAPI
    {
        // client desired access
        private static IPortableDeviceValues CreateClientDesiredAccessInfo(uint accessMode)
        {
            var clientInfo = (IPortableDeviceValues)new PortableDeviceValuesClass();
            clientInfo.SetUnsignedIntegerValue(PropertyKeys.WPD_CLIENT_DESIRED_ACCESS, accessMode);

            return clientInfo;
        }

        public static readonly IPortableDeviceValues RequestReadOnlyAccess = CreateClientDesiredAccessInfo(Constants.GENERIC_READ);
        public static readonly IPortableDeviceValues RequestReadWriteAccess = CreateClientDesiredAccessInfo(Constants.GENERIC_READ | Constants.GENERIC_WRITE);

        // item types
        public static readonly Guid FolderType = new(0x27E2E392, 0xA111, 0x48E0, 0xAB, 0x0C, 0xE1, 0x77, 0x05, 0xA0, 0x5F, 0x85);
        public static readonly Guid FunctionalType = new(0x99ED0160, 0x17FF, 0x4C44, 0x9D, 0x98, 0x1D, 0x7A, 0x6F, 0x94, 0x19, 0x21);

        // transformations
        public static PortableDeviceApiLib.tag_inner_PROPVARIANT StringToPropVariant(string value)
        {
            //PortableDeviceApiLib.tag_inner_PROPVARIANT propVariant = new();

            IPortableDeviceValues pValues = (IPortableDeviceValues)new PortableDeviceValuesClass();

            var WPD_OBJECT_ID = new _tagpropertykey
            {
                fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C),
                pid = 2
            };

            pValues.SetStringValue(ref WPD_OBJECT_ID, value);
            pValues.GetValue(ref WPD_OBJECT_ID, out PortableDeviceApiLib.tag_inner_PROPVARIANT propVariant);

            return propVariant;
        }
    }
}
