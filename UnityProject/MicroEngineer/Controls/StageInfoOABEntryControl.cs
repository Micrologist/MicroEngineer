using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class StageInfoOABEntryControl : VisualElement
    {
        public static string UssBaseClassName = "stage-info-oab";
        public static string UssClassName = "stage-oab";

        public static string UssStageNumberClassName = UssClassName + "__number";
        public static string UssTwrClassName = UssClassName + "__twr";
        public static string UssSltClassName = UssClassName + "__slt";
        public static string UssAslDeltaVValueClassName = UssClassName + "__asl-deltav-value";
        public static string UssAslDeltaVUnitClassName = UssClassName + "__asl-deltav-unit";
        public static string UssVacDeltaVValueClassName = UssClassName + "__vac-deltav-value";
        public static string UssVacDeltaVUnitClassName = UssClassName + "__vac-deltav-unit";

        public static string UssBurnContainerClassName = UssClassName + "__burn-container";
        public static string UssBurnValueClassName = UssClassName + "__burn-value";
        public static string UssBurnUnitClassName = UssClassName + "__burn-unit";

        public static string UssBodyClassName = UssClassName + "__body";

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

        public Label TwrLabel;
        public string Twr
        {
            get => TwrLabel.text;
            set => TwrLabel.text = value;
        }

        public Label SltLabel;
        public string Slt
        {
            get => SltLabel.text;
            set => SltLabel.text = value;
        }

        public Label AslDeltaVValueLabel;
        public Label AslDeltaVUnitLabel;
        public string AslDeltaV
        {
            get => AslDeltaVValueLabel.text;
            set => AslDeltaVValueLabel.text = value;
        }

        public Label VacDeltaVValueLabel;
        public Label VacDeltaVUnitLabel;
        public string VacDeltaV
        {
            get => VacDeltaVValueLabel.text;
            set => VacDeltaVValueLabel.text = value;
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

        public DropdownField BodyDropdown;
        public List<string> Body
        {
            get => new List<string>() { BodyDropdown.value };
            set => BodyDropdown.choices = value;
        }

        public void SetValue(int stageNumber, float twr, float slt, double aslDeltaV, double vacDeltaV, int burnDays, int burnHours, int burnMinutes, int burnSeconds, List<string> bodies, string selectedBody)
        {
            StageNumber = stageNumber.ToString("00");
            Twr = twr.ToString("0.00");
            Slt = slt.ToString("0.00");
            AslDeltaV = string.Format("{0:N0}", aslDeltaV);
            VacDeltaV = string.Format("{0:N0}", vacDeltaV);

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

            Body = bodies;
            BodyDropdown.SetValueWithoutNotify(selectedBody);
            BodyDropdown.SetEnabled(true);
        }

        public StageInfoOABEntryControl(int stageNumber, float twr, float slt, double aslDeltaV, double vacDeltaV, int burnDays, int burnHours, int burnMinutes, int burnSeconds, List<string> bodies, string selectedBody) : this()
        {
            SetValue(stageNumber, twr, slt, aslDeltaV, vacDeltaV, burnDays, burnHours, burnMinutes, burnSeconds, bodies, selectedBody);
        }

        public StageInfoOABEntryControl()
        {
            AddToClassList(UssBaseClassName);
            AddToClassList(UssClassName);
            style.flexDirection = FlexDirection.Row;

            StageNumberLabel = new Label()
            {
                name = "stage-number",
                text = "-"
            };
            StageNumberLabel.AddToClassList(UssStageNumberClassName);
            hierarchy.Add(StageNumberLabel);

            // TWR
            TwrLabel = new Label()
            {
                name = "twr-value",
                text = "-"
            };
            TwrLabel.AddToClassList(UssTwrClassName);
            hierarchy.Add(TwrLabel);

            // SLT
            SltLabel = new Label()
            {
                name = "slt-value",
                text = "-"
            };
            SltLabel.AddToClassList(UssSltClassName);
            hierarchy.Add(SltLabel);

            // ASL DeltaV
            AslDeltaVValueLabel = new Label()
            {
                name = "asl-deltav-value",
                text = "-"
            };
            AslDeltaVValueLabel.AddToClassList(UssAslDeltaVValueClassName);
            hierarchy.Add(AslDeltaVValueLabel);
            AslDeltaVUnitLabel = new Label()
            {
                name = "asl-deltav-unit",
                text = DELTA_V_UNIT
            };
            AslDeltaVUnitLabel.AddToClassList(UssAslDeltaVUnitClassName);
            hierarchy.Add(AslDeltaVUnitLabel);

            // Vac DeltaV
            VacDeltaVValueLabel = new Label()
            {
                name = "vac-deltav-value",
                text = "-"
            };
            VacDeltaVValueLabel.AddToClassList(UssVacDeltaVValueClassName);
            hierarchy.Add(VacDeltaVValueLabel);
            VacDeltaVUnitLabel = new Label()
            {
                name = "vac-deltav-unit",
                text = DELTA_V_UNIT
            };
            VacDeltaVUnitLabel.AddToClassList(UssVacDeltaVUnitClassName);
            hierarchy.Add(VacDeltaVUnitLabel);

            BurnValueContainer = new VisualElement();
            BurnValueContainer.AddToClassList(UssBurnContainerClassName);
            hierarchy.Add(BurnValueContainer);

            // Burn DAYS
            {
                BurnDaysValueLabel = new Label()
                {
                    name = "burndays-value",
                    text = "-"
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
                    name = "burnhours-value",
                    text = "-"
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
                    name = "burnminutes-value",
                    text = "-"
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
                    name = "burnseconds-value",
                    text = "-"
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

            BodyDropdown = new DropdownField()
            {
                name = "body-dropdown",
            };
            BodyDropdown.SetValueWithoutNotify("-");
            BodyDropdown.SetEnabled(false);
            BodyDropdown.AddToClassList(UssBodyClassName);
            hierarchy.Add(BodyDropdown);
        }

        public new class UxmlFactory : UxmlFactory<StageInfoOABEntryControl, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlIntAttributeDescription _stageNumber = new UxmlIntAttributeDescription() { name = "stageNumber", defaultValue = 0 };
            UxmlIntAttributeDescription _twr = new UxmlIntAttributeDescription() { name = "twr", defaultValue = 123 };
            UxmlIntAttributeDescription _slt = new UxmlIntAttributeDescription() { name = "slt", defaultValue = 456 };
            UxmlIntAttributeDescription _aslDeltaV = new UxmlIntAttributeDescription() { name = "asl_deltav", defaultValue = 99999 };
            UxmlIntAttributeDescription _vacDeltaV = new UxmlIntAttributeDescription() { name = "vac_deltav", defaultValue = 99999 };
            UxmlIntAttributeDescription _burnDays = new UxmlIntAttributeDescription() { name = "burndays", defaultValue = 123 };
            UxmlIntAttributeDescription _burnHours = new UxmlIntAttributeDescription() { name = "burnhours", defaultValue = 23 };
            UxmlIntAttributeDescription _burnMinutes = new UxmlIntAttributeDescription() { name = "burnminutes", defaultValue = 59 };
            UxmlIntAttributeDescription _burnSeconds = new UxmlIntAttributeDescription() { name = "burnseconds", defaultValue = 59 };
            UxmlStringAttributeDescription _body = new UxmlStringAttributeDescription() { name = "bodies", defaultValue = "Kerbol,Moho,Eve,Gilly,Kerbin,Mun" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (ve is StageInfoOABEntryControl entry)
                {
                    entry.SetValue(
                        _stageNumber.GetValueFromBag(bag, cc),
                        _twr.GetValueFromBag(bag, cc),
                        _slt.GetValueFromBag(bag, cc),
                        _aslDeltaV.GetValueFromBag(bag, cc),
                        _vacDeltaV.GetValueFromBag(bag, cc),
                        _burnDays.GetValueFromBag(bag, cc),
                        _burnHours.GetValueFromBag(bag, cc),
                        _burnMinutes.GetValueFromBag(bag, cc),
                        _burnSeconds.GetValueFromBag(bag, cc),
                        _body.GetValueFromBag(bag, cc).Split(',').ToList(),
                        "Kerbin"
                        );
                }
            }
        }
    }
}