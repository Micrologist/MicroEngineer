using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace MicroMod
{
    internal class BaseEntryControl : VisualElement
    {
        //These are the classes that you reference on your .uss file.
        public static string UssClassName = "entry";
        public static string UssLabelClassName = UssClassName + "__name";
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
            NameLabel.AddToClassList(UssLabelClassName);
            hierarchy.Add(NameLabel);//Adding this to the BaseEntry, if you dont do this the element will be lost

            UnitLabel = new Label()
            {
                name = "entry-unit",
                text = String.Empty
            };
            //UnitLabel.style.width = new StyleLength(new Length(20, LengthUnit.Pixel));
            //Setting it so it will ALWAYS occupy 20px of width
            UnitLabel.AddToClassList(UssUnitClassName);

            ValueLabel = new Label()
            {
                name = "entry-value",
                text = String.Empty
            };
            //ValueLabel.style.flexGrow = 1;
            //Name occupies 50%, _unit Occupies 20px, this will tell the _value to occupy whats remaining!
            ValueLabel.AddToClassList(UssValueClassName);

            //Be sure to add the elements in the correct order
            hierarchy.Add(ValueLabel);
            hierarchy.Add(UnitLabel);
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

        public void HandleValueDisplayChanged(string value)
        {
            this.Value = value;
        }
    }
    internal class TimeEntryControl : VisualElement
    {
        //These are the classes that you reference on your .uss file.
        public static string UssClassName = "entry";
        public static string UssLabelClassName = UssClassName + "__name";
        public static string UssInnerValueClassName = UssClassName + "__inner-value";
        public static string UssValueClassName = UssClassName + "__value";
        public static string UssInnerUnitClassName = UssClassName + "__inner-unit";
        public static string UssUnitClassName = UssClassName + "__unit";

        public const string UNIT_YEAR = "y";
        public const string UNIT_MONTH = "m";
        public const string UNIT_DAY = "d";
        public const string UNIT_HOUR = "h";
        public const string UNIT_MINUTE = "m";
        public const string UNIT_SECOND = "s";
        public const string UNIT_MILLISECOND = "ms";

        public Label NameLabel;
        public VisualElement ValueContainer;
        public string EntryName
        {
            get => NameLabel.text;
            set => NameLabel.text = value;
        }

        public Label DaysValueLabel;
        public Label DaysUnitLabel;
        public string Days
        {
            get => DaysValueLabel.text;
            set => DaysValueLabel.text = value;
        }

        public Label HoursValueLabel;
        public Label HoursUnitLabel;
        public string Hours
        {
            get => HoursValueLabel.text;
            set => HoursValueLabel.text = value;
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

        public void SetValue(int days, int hours, int minutes, int seconds)
        {

            days = Mathf.Clamp(days, -9999, 9999);
            hours = Mathf.Clamp(hours, 0, 23);
            minutes = Mathf.Clamp(minutes, 0, 59);
            seconds = Mathf.Clamp(seconds, 0, 59);

            Days = days.ToString("0");
            DisplayStyle showDays = days != 0 ? DisplayStyle.Flex : DisplayStyle.None;
            DaysValueLabel.style.display = showDays;
            DaysUnitLabel.style.display = showDays;

            Hours = days == 0 ? hours.ToString("0") : hours.ToString("00");
            DisplayStyle showHours = (days != 0 || hours != 0) ? DisplayStyle.Flex : DisplayStyle.None;
            HoursValueLabel.style.display = showHours;
            HoursUnitLabel.style.display = showHours;

            Minutes = (hours == 0 && days == 0) ? minutes.ToString("0") : minutes.ToString("00");
            DisplayStyle showMinutes = (days != 0 || hours != 0 || minutes != 0) ? DisplayStyle.Flex : DisplayStyle.None;
            MinutesValueLabel.style.display = showMinutes;
            MinutesUnitLabel.style.display = showMinutes;

            Seconds = (minutes == 0 && hours == 0 && days == 0) ? seconds.ToString("0") : seconds.ToString("00");
        }

        public TimeEntryControl(string entry, int days, int hours, int minutes, int seconds) : this()
        {
            SetValue(days, hours, minutes, seconds);

            /*
            this.EntryName = entry;
            this.Days = days.ToString();
            this.Hours = hours.ToString();
            this.Minutes = minutes.ToString();
            this.Seconds = seconds.ToString();
            */
        }

        public TimeEntryControl()
        {
            //You need to do this to every VisualElement that you want to have said class
            AddToClassList(UssClassName);
            style.flexDirection = FlexDirection.Row;

            NameLabel = new Label()
            {
                //Name that you access with Q<Name>(NameHere)
                name = "entry-name",
                text = "placeholder"
            };
            //NameLabel.style.width = new StyleLength(new Length(50, LengthUnit.Percent));
            //Setting it so it will ALWAYS occupy 50% of its parent's width
            NameLabel.AddToClassList(UssLabelClassName);
            hierarchy.Add(NameLabel);//Adding this to the BaseEntry, if you dont do this the element will be lost

            ValueContainer = new VisualElement();
            ValueContainer.style.flexGrow = 1;
            ValueContainer.style.flexDirection = FlexDirection.Row;
            ValueContainer.style.justifyContent = Justify.FlexEnd;
            hierarchy.Add(ValueContainer);

            {
                DaysValueLabel = new Label()
                {
                    name = "days-value"
                };
                DaysValueLabel.AddToClassList(UssValueClassName);
                DaysValueLabel.AddToClassList(UssInnerValueClassName);
                ValueContainer.Add(DaysValueLabel);
                DaysUnitLabel = new Label()
                {
                    name = "days-unit",
                    text = UNIT_DAY
                };
                DaysUnitLabel.AddToClassList(UssUnitClassName);
                DaysUnitLabel.AddToClassList(UssInnerUnitClassName);
                ValueContainer.Add(DaysUnitLabel);
            }//days

            {
                HoursValueLabel = new Label()
                {
                    name = "hours-value"
                };
                HoursValueLabel.AddToClassList(UssValueClassName);
                HoursValueLabel.AddToClassList(UssInnerValueClassName);
                ValueContainer.Add(HoursValueLabel);
                HoursUnitLabel = new Label()
                {
                    name = "hours-unit",
                    text = UNIT_HOUR
                };
                HoursUnitLabel.AddToClassList(UssUnitClassName);
                HoursUnitLabel.AddToClassList(UssInnerUnitClassName);
                ValueContainer.Add(HoursUnitLabel);
            }//hours

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
                ValueContainer.Add(SecondsValueLabel);
                SecondsUnitLabel = new Label()
                {
                    name = "seconds-unit",
                    text = UNIT_SECOND
                };
                SecondsUnitLabel.AddToClassList(UssUnitClassName);
                ValueContainer.Add(SecondsUnitLabel);
            }//seconds
        }

        public new class UxmlFactory : UxmlFactory<TimeEntryControl, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription _entry = new UxmlStringAttributeDescription() { name = "Entry", defaultValue = "Name" };
            UxmlIntAttributeDescription _days = new UxmlIntAttributeDescription() { name = "days", defaultValue = 12 };
            UxmlIntAttributeDescription _hours = new UxmlIntAttributeDescription() { name = "hours", defaultValue = 34 };
            UxmlIntAttributeDescription _minutes = new UxmlIntAttributeDescription() { name = "minutes", defaultValue = 56 };
            UxmlIntAttributeDescription _seconds = new UxmlIntAttributeDescription() { name = "seconds", defaultValue = 60 };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (ve is TimeEntryControl entry)
                {
                    entry.EntryName = _entry.GetValueFromBag(bag, cc);
                    entry.SetValue(
                        _days.GetValueFromBag(bag, cc),
                        _hours.GetValueFromBag(bag, cc),
                        _minutes.GetValueFromBag(bag, cc),
                        _seconds.GetValueFromBag(bag, cc)
                        );
                }
            }
        }
    }
}