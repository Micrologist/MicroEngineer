using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class TimeControl : VisualElement
    {
        public static string UssClassName = "time-control";
        public static string UssValueClassName = UssClassName + "__value";
        public static string UssUnitClassName = UssClassName + "__unit";

        public const string UNIT_YEAR = "y";
        public const string UNIT_MONTH = "m";
        public const string UNIT_DAY = "d";
        public const string UNIT_HOUR = "h";
        public const string UNIT_MINUTE = "m";
        public const string UNIT_SECOND = "s";

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

        public TimeControl(int days, int hours, int minutes, int seconds) : this()
        {
            SetValue(days, hours, minutes, seconds);
        }

        public TimeControl()
        {
            AddToClassList(UssClassName);
            style.flexDirection = FlexDirection.Row;

            // DAYS
            {
                DaysValueLabel = new Label()
                {
                    name = "days-value"
                };
                DaysValueLabel.AddToClassList(UssValueClassName);
                hierarchy.Add(DaysValueLabel);
                DaysUnitLabel = new Label()
                {
                    name = "days-unit",
                    text = UNIT_DAY
                };
                DaysUnitLabel.AddToClassList(UssUnitClassName);
                hierarchy.Add(DaysUnitLabel);
            }

            // HOURS
            {
                HoursValueLabel = new Label()
                {
                    name = "hours-value"
                };
                HoursValueLabel.AddToClassList(UssValueClassName);
                hierarchy.Add(HoursValueLabel);
                HoursUnitLabel = new Label()
                {
                    name = "hours-unit",
                    text = UNIT_HOUR
                };
                HoursUnitLabel.AddToClassList(UssUnitClassName);
                hierarchy.Add(HoursUnitLabel);
            }

            // MINUTES
            {
                MinutesValueLabel = new Label()
                {
                    name = "minutes-value"
                };
                MinutesValueLabel.AddToClassList(UssValueClassName);
                hierarchy.Add(MinutesValueLabel);
                MinutesUnitLabel = new Label()
                {
                    name = "minutes-unit",
                    text = UNIT_MINUTE
                };
                MinutesUnitLabel.AddToClassList(UssUnitClassName);
                hierarchy.Add(MinutesUnitLabel);
            }

            // SECONDS
            {
                SecondsValueLabel = new Label()
                {
                    name = "seconds-value"
                };
                SecondsValueLabel.AddToClassList(UssValueClassName);
                hierarchy.Add(SecondsValueLabel);
                SecondsUnitLabel = new Label()
                {
                    name = "seconds-unit",
                    text = UNIT_SECOND
                };
                SecondsUnitLabel.AddToClassList(UssUnitClassName);
                hierarchy.Add(SecondsUnitLabel);
            }
        }

        public new class UxmlFactory : UxmlFactory<TimeControl, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlIntAttributeDescription _days = new UxmlIntAttributeDescription() { name = "days", defaultValue = 123 };
            UxmlIntAttributeDescription _hours = new UxmlIntAttributeDescription() { name = "hours", defaultValue = 23 };
            UxmlIntAttributeDescription _minutes = new UxmlIntAttributeDescription() { name = "minutes", defaultValue = 59 };
            UxmlIntAttributeDescription _seconds = new UxmlIntAttributeDescription() { name = "seconds", defaultValue = 59 };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (ve is TimeControl entry)
                {
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