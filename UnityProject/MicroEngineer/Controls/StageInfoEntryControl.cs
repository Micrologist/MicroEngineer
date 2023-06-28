using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class StageInfoEntryControl : VisualElement
    {
        public static string UssBaseClassName = "entry";
        public static string UssClassName = "stage";

        public static string UssStageNumberClassName = UssClassName + "__number";
        public static string UssDeltaVValueClassName = UssClassName + "__deltav-value";
        public static string UssDeltaVUnitClassName = UssClassName + "__deltav-unit";
        public static string UssTwrClassName = UssClassName + "__twr";
        public static string UssBurnContainerClassName = UssClassName + "__burn-container";
        public static string UssBurnValueClassName = UssClassName + "__burn-value";
        public static string UssBurnUnitClassName = UssClassName + "__burn-unit";

        public const string DELTA_V_UNIT = "m/s";
        public const string BURN_UNIT_YEAR = "y";
        public const string BURN_UNIT_MONTH = "m";
        public const string BURN_UNIT_DAY = "d";
        public const string BURN_UNIT_HOUR = "h";
        public const string BURN_UNIT_MINUTE = "m";
        public const string BURN_UNIT_SECOND = "s";

        public VisualElement BurnValueContainer;

        public Label StageNumberLabel;
        public string StageNumber
        {
            get => StageNumberLabel.text;
            set => StageNumberLabel.text = value;
        }

        public Label DeltaVValueLabel;
        public Label DeltaVUnitLabel;
        public string DeltaV
        {
            get => DeltaVValueLabel.text;
            set => DeltaVValueLabel.text = value;
        }

        public Label TwrLabel;
        public string Twr
        {
            get => TwrLabel.text;
            set => TwrLabel.text = value;
        }

        public Label BurnDaysValueLabel;
        public Label BurnDaysUnitLabel;
        public string BurnDays
        {
            get => BurnDaysValueLabel.text;
            set => BurnDaysValueLabel.text = value;
        }

        public Label BurnHoursValueLabel;
        public Label BurnHoursUnitLabel;
        public string BurnHours
        {
            get => BurnHoursValueLabel.text;
            set => BurnHoursValueLabel.text = value;
        }

        public Label BurnMinutesValueLabel;
        public Label BurnMinutesUnitLabel;
        public string BurnMinutes
        {
            get => BurnMinutesValueLabel.text;
            set => BurnMinutesValueLabel.text = value;
        }

        public Label BurnSecondsValueLabel;
        public Label BurnSecondsUnitLabel;
        public string BurnSeconds
        {
            get => BurnSecondsValueLabel.text;
            set => BurnSecondsValueLabel.text = value;
        }

        public void SetValue(int stageNumber, double deltaV, float twr, int burnDays, int burnHours, int burnMinutes, int burnSeconds)
        {
            StageNumber = stageNumber.ToString("00");
            DeltaV = string.Format("{0:N0}", deltaV);
            Twr = twr.ToString("0.00");

            BurnDays = burnDays.ToString("0");
            DisplayStyle showBurnDays = burnDays != 0 ? DisplayStyle.Flex : DisplayStyle.None;
            BurnDaysValueLabel.style.display = showBurnDays;
            BurnDaysUnitLabel.style.display = showBurnDays;

            BurnHours = burnDays == 0 ? burnHours.ToString("0") : burnHours.ToString("00");
            DisplayStyle showBurnHours = burnDays != 0 || burnHours != 0 ? DisplayStyle.Flex : DisplayStyle.None;
            BurnHoursValueLabel.style.display = showBurnHours;
            BurnHoursUnitLabel.style.display = showBurnHours;

            BurnMinutes = burnHours == 0 && burnDays == 0 ? burnMinutes.ToString("0") : burnMinutes.ToString("00");
            DisplayStyle showBurnMinutes = burnDays != 0 || burnHours != 0 || burnMinutes != 0 ? DisplayStyle.Flex : DisplayStyle.None;
            BurnMinutesValueLabel.style.display = showBurnMinutes;
            BurnMinutesUnitLabel.style.display = showBurnMinutes;

            BurnSeconds = burnMinutes == 0 && burnHours == 0 && burnDays == 0 ? burnSeconds.ToString("0") : burnSeconds.ToString("00");
        }

        public StageInfoEntryControl(int stageNumber, double deltaV, float twr, int burnDays, int burnHours, int burnMinutes, int burnSeconds) : this()
        {
            SetValue(stageNumber, deltaV, twr, burnDays, burnHours, burnMinutes, burnSeconds);
        }

        public StageInfoEntryControl()
        {
            AddToClassList(UssBaseClassName);
            AddToClassList(UssClassName);
            style.flexDirection = FlexDirection.Row;

            StageNumberLabel = new Label()
            {
                name = "stage-number",
                text = "00"
            };
            StageNumberLabel.AddToClassList(UssStageNumberClassName);
            hierarchy.Add(StageNumberLabel);

            // DeltaV
            DeltaVValueLabel = new Label()
            {
                name = "deltav-value"
            };
            DeltaVValueLabel.AddToClassList(UssDeltaVValueClassName);
            hierarchy.Add(DeltaVValueLabel);
            DeltaVUnitLabel = new Label()
            {
                name = "deltav-unit",
                text = DELTA_V_UNIT
            };
            DeltaVUnitLabel.AddToClassList(UssDeltaVUnitClassName);
            hierarchy.Add(DeltaVUnitLabel);

            // TWR
            TwrLabel = new Label()
            {
                name = "twr-value"
            };
            TwrLabel.AddToClassList(UssTwrClassName);
            hierarchy.Add(TwrLabel);

            BurnValueContainer = new VisualElement();
            BurnValueContainer.AddToClassList(UssBurnContainerClassName);
            hierarchy.Add(BurnValueContainer);

            // Burn DAYS
            {
                BurnDaysValueLabel = new Label()
                {
                    name = "burndays-value"
                };
                BurnDaysValueLabel.AddToClassList(UssBurnValueClassName);
                BurnValueContainer.Add(BurnDaysValueLabel);
                BurnDaysUnitLabel = new Label()
                {
                    name = "burndays-unit",
                    text = BURN_UNIT_DAY
                };
                BurnDaysUnitLabel.AddToClassList(UssBurnUnitClassName);
                BurnValueContainer.Add(BurnDaysUnitLabel);
            }

            // Burn HOURS
            {
                BurnHoursValueLabel = new Label()
                {
                    name = "burnhours-value"
                };
                BurnHoursValueLabel.AddToClassList(UssBurnValueClassName);
                BurnValueContainer.Add(BurnHoursValueLabel);
                BurnHoursUnitLabel = new Label()
                {
                    name = "burnhours-unit",
                    text = BURN_UNIT_HOUR
                };
                BurnHoursUnitLabel.AddToClassList(UssBurnUnitClassName);
                BurnValueContainer.Add(BurnHoursUnitLabel);
            }

            // Burn MINUTES
            {
                BurnMinutesValueLabel = new Label()
                {
                    name = "burnminutes-value"
                };
                BurnMinutesValueLabel.AddToClassList(UssBurnValueClassName);
                BurnValueContainer.Add(BurnMinutesValueLabel);
                BurnMinutesUnitLabel = new Label()
                {
                    name = "burnminutes-unit",
                    text = BURN_UNIT_MINUTE
                };
                BurnMinutesUnitLabel.AddToClassList(UssBurnUnitClassName);
                BurnValueContainer.Add(BurnMinutesUnitLabel);
            }

            // Burn SECONDS
            {
                BurnSecondsValueLabel = new Label()
                {
                    name = "burnseconds-value"
                };
                BurnSecondsValueLabel.AddToClassList(UssBurnValueClassName);
                BurnValueContainer.Add(BurnSecondsValueLabel);
                BurnSecondsUnitLabel = new Label()
                {
                    name = "burnseconds-unit",
                    text = BURN_UNIT_SECOND
                };
                BurnSecondsUnitLabel.AddToClassList(UssBurnUnitClassName);
                BurnValueContainer.Add(BurnSecondsUnitLabel);
            }
        }

        public new class UxmlFactory : UxmlFactory<StageInfoEntryControl, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlIntAttributeDescription _stageNumber = new UxmlIntAttributeDescription() { name = "stageNumber", defaultValue = 0 };
            UxmlIntAttributeDescription _deltaV = new UxmlIntAttributeDescription() { name = "deltav", defaultValue = 99999 };
            UxmlIntAttributeDescription _twr = new UxmlIntAttributeDescription() { name = "twr", defaultValue = 123 };
            UxmlIntAttributeDescription _burnDays = new UxmlIntAttributeDescription() { name = "burndays", defaultValue = 123 };
            UxmlIntAttributeDescription _burnHours = new UxmlIntAttributeDescription() { name = "burnhours", defaultValue = 23 };
            UxmlIntAttributeDescription _burnMinutes = new UxmlIntAttributeDescription() { name = "burnminutes", defaultValue = 59 };
            UxmlIntAttributeDescription _burnSeconds = new UxmlIntAttributeDescription() { name = "burnseconds", defaultValue = 59 };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (ve is StageInfoEntryControl entry)
                {
                    entry.SetValue(
                        _stageNumber.GetValueFromBag(bag, cc),
                        _deltaV.GetValueFromBag(bag, cc),
                        _twr.GetValueFromBag(bag, cc),
                        _burnDays.GetValueFromBag(bag, cc),
                        _burnHours.GetValueFromBag(bag, cc),
                        _burnMinutes.GetValueFromBag(bag, cc),
                        _burnSeconds.GetValueFromBag(bag, cc)
                        );
                }
            }
        }
    }
}