﻿using System;
using UnityEngine.UIElements;

namespace MicroMod
{
    public class BaseEntryControl : VisualElement
    {
        //These are the classes that you reference on your .uss file.
        public static string UssClassName = "entry";
        public static string UssEntryClassName = UssClassName + "__name";
        public static string UssValueClassName = UssClassName + "__value";
        public static string UssUnitClassName = UssClassName + "__unit";

        /* TODO delete. Units are set by the BaseEntry model class
        public const string UNIT_FORCE = "N";
        public const string UNIT_SPEED = "m/s";
        public const string UNIT_ACCELERATION = "m/s2";
        public const string UNIT_DEGREE = "º";
        public const string UNIT_GEE = "g";
        */

        public Label NameLabel;
        public Label ValueLabel;
        public Label UnitLabel;

        public string EntryName
        {
            get => NameLabel.text;
            set
            {
                if (EntryName != value)
                    NameLabel.text = value;
            }
        }

        public string Value
        {
            get => ValueLabel.text;
            set => ValueLabel.text = value;
        }

        public string Unit
        {
            get => UnitLabel.text;
            set => UnitLabel.text = value;
        }

        public BaseEntryControl(string name, string value) : this()
        {
            this.EntryName = name;
            this.Value = value;
            this.Unit = string.Empty;
        }
        public BaseEntryControl(string name, string value, string unit) : this()
        {
            this.EntryName = name;
            this.Value = value;
            this.Unit = unit;
        }

        public BaseEntryControl()
        {
            //You need to do this to every VisualElement that you want to have said class
            AddToClassList(UssClassName);
            //style.flexDirection = FlexDirection.Row;

            NameLabel = new Label()
            {
                //Name that you access with Q<Name>(NameHere)
                name = "entry-name",
                text = String.Empty
            };
            //NameLabel.style.width = new StyleLength(new Length(50, LengthUnit.Percent));
            //Setting it so it will ALWAYS occupy 50% of its parent's width
            NameLabel.AddToClassList(UssEntryClassName);
            hierarchy.Add(NameLabel);//Adding this to the BaseEntry, if you dont do this the element will be lost            

            ValueLabel = new Label()
            {
                name = "entry-value",
                text = String.Empty
            };
            //ValueLabel.style.flexGrow = 1;
            //Name occupies 50%, _unit Occupies 20px, this will tell the _value to occupy whats remaining!
            ValueLabel.AddToClassList(UssValueClassName);
            hierarchy.Add(ValueLabel);

            UnitLabel = new Label()
            {
                name = "entry-unit",
                text = String.Empty
            };
            //UnitLabel.style.width = new StyleLength(new Length(20, LengthUnit.Pixel));
            //Setting it so it will ALWAYS occupy 20px of width
            UnitLabel.AddToClassList(UssUnitClassName);
            hierarchy.Add(UnitLabel); //Be sure to add the elements in the correct order
        }

        public new class UxmlFactory : UxmlFactory<BaseEntryControl, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription _entry = new UxmlStringAttributeDescription() { name = "entry", defaultValue = "Entry" };
            UxmlStringAttributeDescription _value = new UxmlStringAttributeDescription() { name = "value", defaultValue = "value" };
            UxmlStringAttributeDescription _unit = new UxmlStringAttributeDescription() { name = "unit", defaultValue = "unit" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (ve is BaseEntryControl entry)
                {
                    entry.EntryName = _entry.GetValueFromBag(bag, cc);
                    entry.Value = _value.GetValueFromBag(bag, cc);
                    entry.Unit = _unit.GetValueFromBag(bag, cc);
                }
            }
        }

        public void HandleEntryValueChanged(string value, string unit)
        {
            Value = value;
            if (Unit != unit)
                Unit = unit;
        }
    }    
}