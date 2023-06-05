using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace MicroMod
{
    public class LatLonEntryControl : VisualElement
    {
        //These are the classes that you reference on your .uss file.
        public static string UssClassName = "entry";
        public static string UssEntryClassName = UssClassName + "__name";
        
        public static string UssOuterValueClassName = UssClassName + "__outer-value";
        public static string UssInnerValueClassName = UssClassName + "__inner-value";
        public static string UssValueClassName = UssClassName + "__value";
        
        public static string UssInnerUnitClassName = UssClassName + "__inner-unit";
        public static string UssUnitClassName = UssClassName + "__unit";
        //public static string UssValueContainerClassName = UssClassName + "__value-container";

        public const string UNIT_DEGREE = "°";
        public const string UNIT_MINUTE = "'";
        public const string UNIT_SECOND = "\"";

        public VisualElement ValueContainer;

        public Label NameLabel;        
        public string EntryName
        {
            get => NameLabel.text;
            set => NameLabel.text = value;
        }        

        public Label DegreesValueLabel;
        public Label DegreesUnitLabel;
        public string Degrees
        {
            get => DegreesValueLabel.text;
            set => DegreesValueLabel.text = value;
        }

        public Label MinutesValueLabel;
        public Label MinutesUnitLabel;
        public string Minutes
        {
            get => MinutesValueLabel.text;
            set => MinutesValueLabel.text = value;
        }

        public Label SecondsValueLabel;
        public Label SecondsUnitLabel;
        public string Seconds
        {
            get => SecondsValueLabel.text;
            set => SecondsValueLabel.text = value;
        }

        public Label UnitLabel;
        public string Unit
        {
            get => UnitLabel.text;
            set => UnitLabel.text = value;
        }

        public void SetValue(int degrees, int minutes, int seconds)
        {

            degrees = Mathf.Clamp(degrees, 0, 359);
            minutes = Mathf.Clamp(minutes, 0, 59);
            seconds = Mathf.Clamp(seconds, 0, 59);

            Degrees = degrees.ToString("0");
            //DisplayStyle showDegrees = degrees != 0 ? DisplayStyle.Flex : DisplayStyle.None;
            //DaysValueLabel.style.display = showDays;
            //DaysUnitLabel.style.display = showDays;

            Minutes = minutes.ToString("00");
            //Hours = days == 0 ? hours.ToString("0") : hours.ToString("00");
            //DisplayStyle showHours = (days != 0 || hours != 0) ? DisplayStyle.Flex : DisplayStyle.None;
            //HoursValueLabel.style.display = showHours;
            //HoursUnitLabel.style.display = showHours;

            Seconds = seconds.ToString("00");
            //Minutes = (hours == 0 && days == 0) ? minutes.ToString("0") : minutes.ToString("00");
            //DisplayStyle showMinutes = (days != 0 || hours != 0 || minutes != 0) ? DisplayStyle.Flex : DisplayStyle.None;
            //MinutesValueLabel.style.display = showMinutes;
            //MinutesUnitLabel.style.display = showMinutes;

            //Seconds = (minutes == 0 && hours == 0 && days == 0) ? seconds.ToString("0") : seconds.ToString("00");
        }

        public LatLonEntryControl(string entry, int degrees, int minutes, int seconds, string unit) : this()
        {
            this.EntryName = entry;
            SetValue(degrees, minutes, seconds);
            this.Unit = unit;            

            /*
            this.EntryName = entry;
            this.Days = days.ToString();
            this.Hours = hours.ToString();
            this.Minutes = minutes.ToString();
            this.Seconds = seconds.ToString();
            */
        }

        public LatLonEntryControl()
        {
            AddToClassList(UssClassName);
            style.flexDirection = FlexDirection.Row;

            NameLabel = new Label()
            {
                //Name that you access with Q<Name>(NameHere)
                name = "entry-name",
                text = "placeholder"
            };
            NameLabel.AddToClassList(UssEntryClassName);
            hierarchy.Add(NameLabel);

            ValueContainer = new VisualElement();
            ValueContainer.style.flexGrow = 1;
            ValueContainer.style.flexDirection = FlexDirection.Row;
            ValueContainer.style.justifyContent = Justify.FlexEnd;
            hierarchy.Add(ValueContainer);

            {
                DegreesValueLabel = new Label()
                {
                    name = "degrees-value"
                };
                DegreesValueLabel.AddToClassList(UssValueClassName);
                DegreesValueLabel.AddToClassList(UssOuterValueClassName);
                ValueContainer.Add(DegreesValueLabel);
                DegreesUnitLabel = new Label()
                {
                    name = "degrees-unit",
                    text = UNIT_DEGREE
                };
                DegreesUnitLabel.AddToClassList(UssUnitClassName);
                DegreesUnitLabel.AddToClassList(UssInnerUnitClassName);
                ValueContainer.Add(DegreesUnitLabel);
            }//days

            {
                MinutesValueLabel = new Label()
                {
                    name = "minutes-value"
                };
                MinutesValueLabel.AddToClassList(UssValueClassName);
                MinutesValueLabel.AddToClassList(UssInnerValueClassName);
                ValueContainer.Add(MinutesValueLabel);
                MinutesUnitLabel = new Label()
                {
                    name = "minutes-unit",
                    text = UNIT_MINUTE
                };
                MinutesUnitLabel.AddToClassList(UssUnitClassName);
                MinutesUnitLabel.AddToClassList(UssInnerUnitClassName);
                ValueContainer.Add(MinutesUnitLabel);

            }//minutes

            {
                SecondsValueLabel = new Label()
                {
                    name = "seconds-value"
                };
                SecondsValueLabel.AddToClassList(UssValueClassName);
                SecondsValueLabel.AddToClassList(UssInnerValueClassName);
                ValueContainer.Add(SecondsValueLabel);
                SecondsUnitLabel = new Label()
                {
                    name = "seconds-unit",
                    text = UNIT_SECOND
                };
                SecondsUnitLabel.AddToClassList(UssUnitClassName);
                SecondsUnitLabel.AddToClassList(UssInnerUnitClassName);
                ValueContainer.Add(SecondsUnitLabel);
            }//seconds

            UnitLabel = new Label()
            {
                name = "entry-unit",
                text = String.Empty
            };
            UnitLabel.AddToClassList(UssUnitClassName);
            hierarchy.Add(UnitLabel);
        }

        public new class UxmlFactory : UxmlFactory<LatLonEntryControl, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription _entry = new UxmlStringAttributeDescription() { name = "Entry", defaultValue = "Name" };
            UxmlIntAttributeDescription _degrees = new UxmlIntAttributeDescription() { name = "degrees", defaultValue = 350 };
            UxmlIntAttributeDescription _minutes = new UxmlIntAttributeDescription() { name = "minutes", defaultValue = 42 };
            UxmlIntAttributeDescription _seconds = new UxmlIntAttributeDescription() { name = "seconds", defaultValue = 24 };
            UxmlStringAttributeDescription _unit = new UxmlStringAttributeDescription() { name = "unit", defaultValue = "unit" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (ve is LatLonEntryControl entry)
                {
                    entry.EntryName = _entry.GetValueFromBag(bag, cc);
                    entry.SetValue(
                        _degrees.GetValueFromBag(bag, cc),
                        _minutes.GetValueFromBag(bag, cc),
                        _seconds.GetValueFromBag(bag, cc)
                        );
                    entry.Unit = _unit.GetValueFromBag(bag, cc);
                }
            }
        }
    }
}