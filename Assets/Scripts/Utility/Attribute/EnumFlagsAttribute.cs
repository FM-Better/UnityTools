using UnityEngine;

namespace Utility.Attribute
{
    public class EnumFlagsAttribute : PropertyAttribute
    {
        public System.Type EnumType { get; private set; }

        public EnumFlagsAttribute(System.Type enumType) => this.EnumType = enumType;
    }
}