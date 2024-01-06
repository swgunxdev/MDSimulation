using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaDataModeling.Providers
{
    public enum MetadataEnum : ushort
    {
        None,
        GenericProperty,
        BaseContainer,
        RangedProperty,
        ArrayOfProperty,
        StringProperty,
    }

    public static class MetadataTypeProvider
    {
        static Dictionary<Type, MetadataEnum> _typeToEnum;
        static Dictionary<MetadataEnum, Type> _enumToType;

        static MetadataTypeProvider()
        {
            // Here are a couple for illustration. Yes, I am suggesting that
            // in reality you would add a BytesProvider<T> for each T
            // supported by the BitConverter class.
            _typeToEnum = new Dictionary<Type, MetadataEnum>
        {
            { typeof(GenericProperty<>), MetadataEnum.GenericProperty },
            { typeof(RangedProperty<>), MetadataEnum.RangedProperty},
            { typeof(BaseContainer), MetadataEnum.BaseContainer },
            { typeof(StringProperty), MetadataEnum.StringProperty },
            { typeof(ArrayOfProperty<>), MetadataEnum.ArrayOfProperty },
        };
            _enumToType = new Dictionary<MetadataEnum, Type>
        {
            { MetadataEnum.GenericProperty, typeof(GenericProperty<>)},
            { MetadataEnum.RangedProperty, typeof(RangedProperty<>)},
            { MetadataEnum.BaseContainer, typeof(BaseContainer) },
            { MetadataEnum.StringProperty, typeof(StringProperty) },
            { MetadataEnum.ArrayOfProperty, typeof(ArrayOfProperty<>) },
        };

        }

        public static MetadataEnum TypeToTypeEnum<T>()
        {
            return _typeToEnum[typeof(T)];
        }

        public static Type TypeEnumToType(MetadataEnum type)
        {
            return _enumToType[type];
        }
    }

}
