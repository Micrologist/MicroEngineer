using System;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class SeparatorEntryControl : VisualElement
    {
        public static string UssClassName = "separator";

        public SeparatorEntryControl()
        {
            AddToClassList(UssClassName);
        }

        public new class UxmlFactory : UxmlFactory<SeparatorEntryControl, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }
    }
}