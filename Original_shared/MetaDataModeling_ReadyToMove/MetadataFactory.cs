//
// File Name: MetadataFactory.cs
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MetaDataModeling.Providers;
using MetaDataModeling.Utils;

namespace MetaDataModeling
{
    /// <summary>
    /// This is a metadata object factory that create instances
    /// of metadata objects.
    /// </summary>
    public static class MetaDataFactory
    {
        internal static IdType CreateMDInstance(UInt16 mdt, UInt16 bdt, UInt16 objId, UInt16 objType)
        {
            Type constructed = null;

            // Translate the integer value into an enumeration
            MetadataEnum metaDataType = EnumExtensions.ToEnum<UInt16, MetadataEnum>(mdt);

            // Translate the enumeration into an actual c# type object
            Type mdInstanceType = MetadataTypeProvider.TypeEnumToType(metaDataType);

            // if this is a string property or a base container
            // type then we do not need to know the type that is
            // wrapped by a generic metadata type.
            if (metaDataType == MetadataEnum.StringProperty
                || metaDataType == MetadataEnum.BaseContainer)
            {
                constructed = mdInstanceType;
            }
            else
            {
                // Translate the integer into a CLR integral 
                Type[] typeArgs = { CLRIntegralTypeProvider.TypeEnumToType(EnumExtensions.ToEnum<UInt16, TypeCode>(bdt)) };

                // Construct the generic metadata type
                constructed = mdInstanceType.MakeGenericType(typeArgs);
            }

            // create an instance of the metadata object
            object mdInstance = Activator.CreateInstance(constructed);
            return  mdInstance as IdType;
        }
    }
}
