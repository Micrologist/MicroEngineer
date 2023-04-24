using KSP.Sim.DeltaV;
using UnityEngine;

namespace MicroMod
{
    internal class StageWindow : EntryWindow
    {
        private UI _ui;

        internal void DrawWindow(UI ui)
        {
            _ui = ui;

            if (this.IsFlightPoppedOut)
            {
                this.FlightRect = GUILayout.Window(
                    GUIUtility.GetControlID(FocusType.Passive),
                    this.FlightRect,
                    DrawStages,
                    "",
                    Styles.PopoutWindowStyle,
                    GUILayout.Width(Styles.WindowWidth),
                    GUILayout.Height(0)
                    );

                this.FlightRect.position = Utility.ClampToScreen(this.FlightRect.position, this.FlightRect.size);
            }
            else
            {
                DrawStages(int.MinValue);
            }
        }

        private void DrawStages(int id)
        {
            DrawStagesHeader();

            List<DeltaVStageInfo> stages = (List<DeltaVStageInfo>)this.Entries.Find(entry => entry.Name == "Stage Info").EntryValue;

            int stageCount = stages?.Count ?? 0;
            if (stages != null && stageCount > 0)
            {
                float highestTwr = Mathf.Floor(stages.Max(stage => stage.TWRActual));
                int preDecimalDigits = Mathf.FloorToInt(Mathf.Log10(highestTwr)) + 1;
                string twrFormatString = "N2";

                if (preDecimalDigits == 3)
                {
                    twrFormatString = "N1";
                }
                else if (preDecimalDigits == 4)
                {
                    twrFormatString = "N0";
                }

                int numberOfNonEmptyStagesToDraw = stages.Where(s => s.DeltaVinVac > 0.0001 || s.DeltaVatASL > 0.0001).Count();
                int stageBeingDrawn = 0;
                for (int i = stages.Count - 1; i >= 0; i--)
                {
                    DeltaVStageInfo stageInfo = stages[i];
                    if (stageInfo.DeltaVinVac > 0.0001 || stageInfo.DeltaVatASL > 0.0001)
                    {
                        GUIStyle style = stageBeingDrawn < numberOfNonEmptyStagesToDraw - 1 ? Styles.EntryBackground_Middle : Styles.EntryBackground_Last;
                        int stageNum = stageCount - stageInfo.Stage;
                        DrawStageEntry(style, stageNum, stageInfo, twrFormatString);
                    }
                }
            }

            DrawSectionEnd();
        }

        private void DrawStagesHeader()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{this.Name}", Styles.WindowTitleLabelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Button(Styles.Settings15Texture, Styles.SettingsBtnStyle);
            // If window is popped out and it's not locked => show the close button. If it's not popped out => show to popup arrow
            this.IsFlightPoppedOut = this.IsFlightPoppedOut && !this.IsLocked ? !_ui.CloseButton(Styles.CloseBtnStyle) : !this.IsFlightPoppedOut ? GUILayout.Button(Styles.PopoutTexture, Styles.PopoutBtnStyle) : this.IsFlightPoppedOut;
            GUILayout.EndHorizontal();
            GUILayout.Space(Styles.NegativeSpacingAfterHeader);

            GUILayout.BeginHorizontal(Styles.EntryBackground_First);
            GUILayout.FlexibleSpace();
            GUILayout.Label("∆v", Styles.TableHeaderLabelStyle);
            GUILayout.Space(16);
            GUILayout.Label($"TWR", Styles.TableHeaderLabelStyle, GUILayout.Width(40));
            GUILayout.Space(16);
            GUILayout.Label($"Burn", Styles.TableHeaderLabelStyle, GUILayout.Width(56));
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.Space(Styles.NegativeSpacingAfterEntry);
        }

        private void DrawStageEntry(GUIStyle horizontalStyle, int stageID, DeltaVStageInfo stageInfo, string twrFormatString)
        {
            GUILayout.BeginHorizontal(horizontalStyle);
            GUILayout.Label($"{stageID:00.}", Styles.NameLabelStyle, GUILayout.Width(24));
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{stageInfo.DeltaVActual:N0} <color=#{Styles.UnitColorHex}>m/s</color>", Styles.ValueLabelStyle);
            GUILayout.Space(16);
            GUILayout.Label($"{stageInfo.TWRActual.ToString(twrFormatString)}", Styles.ValueLabelStyle, GUILayout.Width(40));
            GUILayout.Space(16);
            string burnTime = Utility.SecondsToTimeString(stageInfo.StageBurnTime, false);
            string lastUnit = "s";
            if (burnTime.Contains('h'))
            {
                burnTime = burnTime.Remove(burnTime.LastIndexOf("<color"));
                lastUnit = "m";
            }
            if (burnTime.Contains('d'))
            {
                burnTime = burnTime.Remove(burnTime.LastIndexOf("<color"));
                lastUnit = "h";
            }

            GUILayout.Label($"{burnTime}<color=#{Styles.UnitColorHex}>{lastUnit}</color>", Styles.ValueLabelStyle, GUILayout.Width(56));
            GUILayout.EndHorizontal();
            GUILayout.Space(Styles.NegativeSpacingAfterEntry);
        }

        private void DrawSectionEnd()
        {
            if (this.IsFlightPoppedOut)
            {
                if (!this.IsLocked)
                    GUI.DragWindow(new Rect(0, 0, Styles.WindowWidth, Styles.WindowHeight));

                GUILayout.Space(Styles.SpacingBelowPopout);
            }
            else
            {
                GUILayout.Space(Styles.SpacingAfterSection);
            }
        }
    }
}