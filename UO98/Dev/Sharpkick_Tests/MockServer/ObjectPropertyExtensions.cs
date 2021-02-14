using System;
using System.Collections.Generic;
using Sharpkick;

namespace Sharpkick_Tests
{
    static class ObjectPropertyExtensions
    {
        static Dictionary<ItemObject, ExtendedPropertiesStruct> ExtendedProperties = new Dictionary<ItemObject, ExtendedPropertiesStruct>();

        struct ExtendedPropertiesStruct
        {
            public int Quantity;

            public static ExtendedPropertiesStruct CreateDefault()
            {
                ExtendedPropertiesStruct toReturn;
                toReturn.Quantity = 1;
                return toReturn;
            }

        }

        public static int GetQuantity(this ItemObject item)
        {
            return GetProps(item).Quantity;
        }

        public static void SetQuantity(this ItemObject item, int amount)
        {
            ExtendedPropertiesStruct props = GetProps(item);
            props.Quantity = amount;
        }

        private static ExtendedPropertiesStruct GetProps(ItemObject item)
        {
            if (!ExtendedProperties.ContainsKey(item))
                ExtendedProperties[item] = ExtendedPropertiesStruct.CreateDefault();
            return ExtendedProperties[item];
        }

        public static void Purge(this ItemObject item)
        {
            if (ExtendedProperties.ContainsKey(item))
                ExtendedProperties.Remove(item);
        }

        private static void SetProps(ItemObject item, ExtendedPropertiesStruct props)
        {
            ExtendedProperties[item] = props;
        }

    }
}
