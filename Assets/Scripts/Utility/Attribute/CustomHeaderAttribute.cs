using UnityEngine;

namespace Utility.Attribute
{
    public class CustomHeaderAttribute : PropertyAttribute
    {
        public readonly string headerText;
        public readonly Color textColor;
        public readonly int fontSize;
        public readonly FontStyle fontStyle;
        public readonly TextAnchor alignment;

        public CustomHeaderAttribute(string headerText, float r, float g, float b, int fontSize = 14,
            FontStyle fontStyle = FontStyle.Bold, TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            this.headerText = headerText;
            this.textColor = new Color(r, g, b, 1);
            this.fontSize = fontSize;
            this.fontStyle = fontStyle;
            this.alignment = alignment;
        }
    }
}
