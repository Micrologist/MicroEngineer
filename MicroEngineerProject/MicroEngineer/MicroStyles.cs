using SpaceWarp.API.UI;
using UnityEngine;

namespace MicroMod
{
    public static class MicroStyles
    {
        public static int WindowWidth = 290;
        public static int WindowHeight = 700;

        public static GUISkin SpaceWarpUISkin;
        public static GUIStyle MainWindowStyle;
        public static GUIStyle PopoutWindowStyle;
        public static GUIStyle PopoutBtnStyle;
        public static GUIStyle SectionToggleStyle;
        public static GUIStyle NameLabelStyle;
        public static GUIStyle ValueLabelStyle;
        public static GUIStyle UnitLabelStyle;
        public static GUIStyle CloseBtnStyle;
        public static GUIStyle SaveLoadBtnStyle;
        public static GUIStyle TableHeaderLabelStyle;

        public static string UnitColorHex { get => ColorUtility.ToHtmlStringRGBA(UnitLabelStyle.normal.textColor); }

        public static int SpacingAfterHeader = -12;
        public static int SpacingAfterEntry = -12;
        public static int SpacingAfterSection = 5;
        public static float SpacingBelowPopout = 10;

        public static void InitializeStyles()
        {
            SpaceWarpUISkin = Skins.ConsoleSkin;

            MainWindowStyle = new GUIStyle(SpaceWarpUISkin.window)
            {
                padding = new RectOffset(8, 8, 20, 8),
                contentOffset = new Vector2(0, -22),
                fixedWidth = WindowWidth
            };

            PopoutWindowStyle = new GUIStyle(MainWindowStyle)
            {
                padding = new RectOffset(MainWindowStyle.padding.left, MainWindowStyle.padding.right, 0, MainWindowStyle.padding.bottom - 5),
                fixedWidth = WindowWidth
            };

            PopoutBtnStyle = new GUIStyle(SpaceWarpUISkin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                contentOffset = new Vector2(0, 2),
                fixedHeight = 15,
                fixedWidth = 15,
                fontSize = 28,
                clipping = TextClipping.Overflow,
                margin = new RectOffset(0, 0, 10, 0)
            };

            SectionToggleStyle = new GUIStyle(SpaceWarpUISkin.toggle)
            {
                padding = new RectOffset(14, 0, 3, 3)
            };

            NameLabelStyle = new GUIStyle(SpaceWarpUISkin.label);
            NameLabelStyle.normal.textColor = new Color(.7f, .75f, .75f, 1);

            ValueLabelStyle = new GUIStyle(SpaceWarpUISkin.label)
            {
                alignment = TextAnchor.MiddleRight
            };
            ValueLabelStyle.normal.textColor = new Color(.6f, .7f, 1, 1);

            UnitLabelStyle = new GUIStyle(SpaceWarpUISkin.label)
            {
                fixedWidth = 24,
                alignment = TextAnchor.MiddleLeft
            };
            UnitLabelStyle.normal.textColor = new Color(.7f, .75f, .75f, 1);

            CloseBtnStyle = new GUIStyle(SpaceWarpUISkin.button)
            {
                fontSize = 8
            };

            SaveLoadBtnStyle = new GUIStyle(SpaceWarpUISkin.button)
            {
                alignment = TextAnchor.MiddleCenter
            };

            TableHeaderLabelStyle = new GUIStyle(NameLabelStyle)
            {
                alignment = TextAnchor.MiddleRight
            };
        }
    }
}
