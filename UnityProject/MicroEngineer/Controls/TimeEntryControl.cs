//using MicroMod;
using UnityEngine;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class TimeEntryControl : VisualElement
    {
        public static string UssClassName = "entry";
        public static string UssEntryClassName = UssClassName + "__name";

        public static string UssOuterValueClassName = UssClassName + "__outer-value";
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
            hours = Mathf.Clamp(hours, -23, 23);
            minutes = Mathf.Clamp(minutes, -59, 59);
            seconds = Mathf.Clamp(seconds, -59, 59);

            Days = days.ToString("0");
            DisplayStyle showDays = days != 0 ? DisplayStyle.Flex : DisplayStyle.None;
            DaysValueLabel.style.display = showDays;
            DaysUnitLabel.style.display = showDays;

            Hours = days == 0 ? hours.ToString("0") : hours.ToString("00");
            DisplayStyle showHours = days != 0 || hours != 0 ? DisplayStyle.Flex : DisplayStyle.None;
            HoursValueLabel.style.display = showHours;
            HoursUnitLabel.style.display = showHours;

            Minutes = hours == 0 && days == 0 ? minutes.ToString("0") : minutes.ToString("00");
            DisplayStyle showMinutes = days != 0 || hours != 0 || minutes != 0 ? DisplayStyle.Flex : DisplayStyle.None;
            MinutesValueLabel.style.display = showMinutes;
            MinutesUnitLabel.style.display = showMinutes;

            Seconds = minutes == 0 && hours == 0 && days == 0 ? seconds.ToString("0") : seconds.ToString("00");
        }

        /*
        public TimeEntryControl(BaseEntry entry) : this()
        {
            EntryName = entry.Name;

            var time = Utility.ParseSecondsToTimeFormat((double?)entry.EntryValue ?? 0);
            SetValue(time.Days, time.Hours, time.Minutes, time.Seconds);

            entry.OnEntryTimeValueChanged += HandleEntryTimeValueChanged;
        }
        */

        public TimeEntryControl(string entry, int days, int hours, int minutes, int seconds) : this()
        {
            EntryName = entry;
            SetValue(days, hours, minutes, seconds);
        }

        public TimeEntryControl()
        {
            AddToClassList(UssClassName);
            style.flexDirection = FlexDirection.Row;

            NameLabel = new Label()
            {
                name = "entry-name",
                text = "placeholder"
            };
            NameLabel.AddToClassList(UssEntryClassName);
            hierarchy.Add(NameLabel);

            ValueContainer = new VisualElement()
            {
                name = "value-container"
            };
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
                DaysValueLabel.AddToClassList(UssOuterValueClassName);
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

        public void HandleEntryTimeValueChanged(int days, int hours, int minutes, int seconds) => SetValue(days, hours, minutes, seconds);
    }
}