using SpaceWarp.API.Assets;
using SpaceWarp.API.UI;
using UnityEngine;

namespace MicroMod
{
    // MARGIN: how much the element is offset from other elements
    // PADDING: how much the text inside the element is offset from the default alignment

    public static class Styles
    {
        private static MicroEngineerMod _plugin;

        public static int WindowWidth = 290;
        public static int WindowHeight = 1440;
        public static int WindowWidthStageOAB = 645;
        public static int WindowWidthSettingsOAB = 300;
        public static int WindowWidthSettingsFlight = WindowWidth;

        public static GUISkin SpaceWarpUISkin;
        public static GUIStyle MainWindowStyle;
        public static GUIStyle PopoutWindowStyle;
        public static GUIStyle EditWindowStyle;
        public static GUIStyle StageOABWindowStyle;

        public static GUIStyle CelestialSelectionStyle;
        public static GUIStyle SettingsOabStyle;
        public static GUIStyle SettingsFlightStyle;
        public static GUIStyle PopoutBtnStyle;
        public static GUIStyle SectionToggleStyle;
        
        public static GUIStyle NameLabelStyle;
        public static GUIStyle ValueLabelStyle;
        public static GUIStyle BlueLabelStyle;
        public static GUIStyle UnitLabelStyle;
        public static GUIStyle UnitLabelStyleStageOAB;
        public static GUIStyle NormalLabelStyle;
        public static GUIStyle TitleLabelStyle;
        public static GUIStyle NormalCenteredLabelStyle;

        public static GUIStyle WindowSelectionTextFieldStyle;
        public static GUIStyle WindowSelectionAbbrevitionTextFieldStyle;

        public static GUIStyle CloseBtnStyle;
        public static GUIStyle SettingsBtnStyle;
        public static GUIStyle SettingsMainGuiBtnStyle;
        public static GUIStyle CloseBtnStageOABStyle;
        public static GUIStyle NormalBtnStyle;
        public static GUIStyle CelestialBodyBtnStyle;
        public static GUIStyle CelestialSelectionBtnStyle;
        public static GUIStyle OneCharacterBtnStyle;
        public static GUIStyle OneCharacterHighBtnStyle;

        public static GUIStyle TableHeaderLabelStyle;
        public static GUIStyle TableHeaderCenteredLabelStyle;

        public static GUIStyle EntryBackground_WhiteTheme_First;
        public static GUIStyle EntryBackground_WhiteTheme_Middle;
        public static GUIStyle EntryBackground_WhiteTheme_Last;
        public static GUIStyle EntryBackground_GrayTheme_First;
        public static GUIStyle EntryBackground_GrayTheme_Middle;
        public static GUIStyle EntryBackground_GrayTheme_Last;
        public static GUIStyle EntryBackground_BlackTheme_First;
        public static GUIStyle EntryBackground_BlackTheme_Middle;
        public static GUIStyle EntryBackground_BlackTheme_Last;
        public static GUIStyle EntryBackground_First;
        public static GUIStyle EntryBackground_Middle;
        public static GUIStyle EntryBackground_Last;

        public static string UnitColorHex { get => ColorUtility.ToHtmlStringRGBA(_unitColor); }

        public static int SpacingAfterHeader = -12;
        public static int SpacingAfterEntry = -12;
        public static int SpacingAfterSection = 10;
        public static float SpacingBelowPopout = 15;

        public static float PoppedOutX = Screen.width * 0.6f;
        public static float PoppedOutY = Screen.height * 0.2f;
        public static float MainGuiX = Screen.width * 0.8f;
        public static float MainGuiY = Screen.height * 0.2f;

        public static Rect CloseBtnRect = new Rect(Styles.WindowWidth - 23, 6, 16, 16);
        public static Rect SettingsMainBtnBtnRect = new Rect(6, 6, 30, 20);
        public static Rect SettingsWindowBtnBtnRect = new Rect(Styles.WindowWidth - 23, 6, 20, 20);
        public static Rect CloseBtnStagesOABRect = new Rect(Styles.WindowWidthStageOAB - 23, 6, 16, 16);
        public static Rect CloseBtnSettingsOABRect = new Rect(Styles.WindowWidthSettingsOAB - 23, 6, 16, 16);
        public static Rect SettingsOABRect = new Rect(Styles.WindowWidthStageOAB - 50, 6, 16, 16);
        public static Rect EditWindowRect = new Rect(Screen.width * 0.5f - Styles.WindowWidth / 2, Screen.height * 0.2f, Styles.WindowWidth, 0);

        public static Texture2D SettingsIcon;
        public static Texture2D EntryBackgroundTexture_WhiteTheme_First;
        public static Texture2D EntryBackgroundTexture_WhiteTheme_Middle;
        public static Texture2D EntryBackgroundTexture_WhiteTheme_Last;
        public static Texture2D EntryBackgroundTexture_GrayTheme_First;
        public static Texture2D EntryBackgroundTexture_GrayTheme_Middle;
        public static Texture2D EntryBackgroundTexture_GrayTheme_Last;
        public static Texture2D EntryBackgroundTexture_BlackTheme_First;
        public static Texture2D EntryBackgroundTexture_BlackTheme_Middle;
        public static Texture2D EntryBackgroundTexture_BlackTheme_Last;
        public static Texture2D IncreaseDecimalDigitsTexture;
        public static Texture2D DecreaseDecimalDigitsTexture;

        private static Color _nameColor_DarkThemes = new Color(.7f, .75f, .75f, 1);
        private static Color _valueColor_DarkThemes = new Color(.6f, .7f, 1, 1);
        private static Color _unitColor_DarkThemes = new Color(.7f, .75f, .75f, 1);

        private static Color _nameColor_LightThemes = new Color(.0f, .0f, .0f, 1);
        private static Color _valueColor_LightThemes = new Color(.2f, .3f, 1, 1);
        private static Color _unitColor_LightThemes = new Color(.0f, .0f, .0f, 1);

        private static Color _nameColor = _nameColor_DarkThemes;
        private static Color _valueColor = _valueColor_DarkThemes;
        private static Color _unitColor = _unitColor_DarkThemes;

        public static Theme ActiveTheme;

        public static void Initialize(MicroEngineerMod plugin)
        {
            _plugin = plugin;

            InitializeStyles();
            InitializeTextures();
            SetActiveTheme(Theme.Gray);
        }

        private static void InitializeStyles()
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

            EditWindowStyle = new GUIStyle(PopoutWindowStyle)
            {
                padding = new RectOffset(8, 8, 30, 8)
            };

            StageOABWindowStyle = new GUIStyle(SpaceWarpUISkin.window)
            {
                padding = new RectOffset(8, 8, 0, 8),
                contentOffset = new Vector2(0, -22),
                fixedWidth = WindowWidthStageOAB
            };

            CelestialSelectionStyle = new GUIStyle(SpaceWarpUISkin.window)
            {
                padding = new RectOffset(8, 8, 0, 8),
                contentOffset = new Vector2(0, -22)
            };

            SettingsOabStyle = new GUIStyle(SpaceWarpUISkin.window)
            {
                padding = new RectOffset(8, 8, 0, 16),
                contentOffset = new Vector2(0, -22),
                fixedWidth = WindowWidthSettingsOAB
            };

            SettingsFlightStyle = new GUIStyle(SpaceWarpUISkin.window)
            {
                padding = new RectOffset(8, 8, 0, 16),
                contentOffset = new Vector2(0, -22),
                fixedWidth = WindowWidthSettingsFlight
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
                fixedHeight = 22,
                margin = new RectOffset(0, 4, 0, 0)
            };

            NameLabelStyle = new GUIStyle(SpaceWarpUISkin.label);
            NameLabelStyle.normal.textColor = _nameColor;
            NameLabelStyle.contentOffset = new Vector2(0, -1); // Background texture is hiding entry's bottom pixel, so this is needed

            ValueLabelStyle = new GUIStyle(SpaceWarpUISkin.label)
            {
                alignment = TextAnchor.MiddleRight
            };
            ValueLabelStyle.normal.textColor = _valueColor;
            ValueLabelStyle.contentOffset = new Vector2(0, -1); // Background texture is hiding entry's bottom pixel, so this is needed

            UnitLabelStyle = new GUIStyle(SpaceWarpUISkin.label)
            {
                fixedWidth = 24,
                alignment = TextAnchor.MiddleLeft
            };
            UnitLabelStyle.normal.textColor = _unitColor;
            UnitLabelStyle.contentOffset = new Vector2(0, -1); // Background texture is hiding entry's bottom pixel, so this is needed

            UnitLabelStyleStageOAB = new GUIStyle(SpaceWarpUISkin.label)
            {
                alignment = TextAnchor.MiddleRight
            };
            UnitLabelStyleStageOAB.normal.textColor = _unitColor;

            NormalLabelStyle = new GUIStyle(SpaceWarpUISkin.label)
            {
                fixedWidth = 120
            };

            TitleLabelStyle = new GUIStyle(SpaceWarpUISkin.label)
            {
                fontSize = 18,
                fixedWidth = 100,
                fixedHeight = 50,
                contentOffset = new Vector2(0, -20),
            };

            NormalCenteredLabelStyle = new GUIStyle(SpaceWarpUISkin.label)
            {
                fixedWidth = 80,
                alignment = TextAnchor.MiddleCenter
            };

            BlueLabelStyle = new GUIStyle(SpaceWarpUISkin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true
            };
            BlueLabelStyle.normal.textColor = _valueColor;

            WindowSelectionTextFieldStyle = new GUIStyle(SpaceWarpUISkin.textField)
            {
                alignment = TextAnchor.MiddleCenter,
                fixedWidth = 80
            };

            WindowSelectionAbbrevitionTextFieldStyle = new GUIStyle(SpaceWarpUISkin.textField)
            {
                alignment = TextAnchor.MiddleCenter,
                fixedWidth = 40
            };

            CloseBtnStyle = new GUIStyle(SpaceWarpUISkin.button)
            {
                fontSize = 8
            };

            SettingsBtnStyle = new GUIStyle(SpaceWarpUISkin.button)
            {
                fontSize = 24
            };

            SettingsMainGuiBtnStyle = new GUIStyle(SpaceWarpUISkin.button)
            {
                fixedWidth = 30,
                fixedHeight = 20,
                padding = new RectOffset(-2, -2, -2, -2)
            };


            NormalBtnStyle = new GUIStyle(SpaceWarpUISkin.button)
            {
                alignment = TextAnchor.MiddleCenter
            };

            CelestialBodyBtnStyle = new GUIStyle(SpaceWarpUISkin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fixedWidth = 80,
                fixedHeight = 20
            };

            CelestialSelectionBtnStyle = new GUIStyle(SpaceWarpUISkin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                fixedWidth = 120
            };

            OneCharacterBtnStyle = new GUIStyle(SpaceWarpUISkin.button)
            {
                fixedWidth = 20,
                fixedHeight = 19,
                alignment = TextAnchor.MiddleCenter,
                margin = new RectOffset(0, 5, 9, 0),
                padding = new RectOffset(0, 0, 0, 0)
            };

            OneCharacterHighBtnStyle = new GUIStyle(SpaceWarpUISkin.button)
            {
                fixedWidth = 20,
                alignment = TextAnchor.MiddleCenter,
            };

            TableHeaderLabelStyle = new GUIStyle(NameLabelStyle)
            {
                alignment = TextAnchor.MiddleRight
            };
            TableHeaderCenteredLabelStyle = new GUIStyle(NameLabelStyle)
            {
                alignment = TextAnchor.MiddleCenter
            };
        }

        private static void InitializeTextures()
        {
            SettingsIcon = AssetManager.GetAsset<Texture2D>($"{_plugin.SpaceWarpMetadata.ModID}/images/settings-30.png");

            EntryBackgroundTexture_WhiteTheme_First = AssetManager.GetAsset<Texture2D>($"{_plugin.SpaceWarpMetadata.ModID}/images/background_white_first.png");
            EntryBackgroundTexture_WhiteTheme_Middle = AssetManager.GetAsset<Texture2D>($"{_plugin.SpaceWarpMetadata.ModID}/images/background_white_middle.png");
            EntryBackgroundTexture_WhiteTheme_Last = AssetManager.GetAsset<Texture2D>($"{_plugin.SpaceWarpMetadata.ModID}/images/background_white_last.png");

            EntryBackgroundTexture_GrayTheme_First = AssetManager.GetAsset<Texture2D>($"{_plugin.SpaceWarpMetadata.ModID}/images/background_darkgray_first.png");
            EntryBackgroundTexture_GrayTheme_Middle = AssetManager.GetAsset<Texture2D>($"{_plugin.SpaceWarpMetadata.ModID}/images/background_darkgray_middle.png");
            EntryBackgroundTexture_GrayTheme_Last = AssetManager.GetAsset<Texture2D>($"{_plugin.SpaceWarpMetadata.ModID}/images/background_darkgray_last.png");

            EntryBackgroundTexture_BlackTheme_First = AssetManager.GetAsset<Texture2D>($"{_plugin.SpaceWarpMetadata.ModID}/images/background_black_first.png");
            EntryBackgroundTexture_BlackTheme_Middle = AssetManager.GetAsset<Texture2D>($"{_plugin.SpaceWarpMetadata.ModID}/images/background_black_middle.png");
            EntryBackgroundTexture_BlackTheme_Last = AssetManager.GetAsset<Texture2D>($"{_plugin.SpaceWarpMetadata.ModID}/images/background_black_last.png");

            EntryBackground_WhiteTheme_First = new GUIStyle { name = "WhiteFirst" };
            EntryBackground_WhiteTheme_First.normal.background = EntryBackgroundTexture_WhiteTheme_First;
            EntryBackground_WhiteTheme_Middle = new GUIStyle { name = "WhiteMiddle" };
            EntryBackground_WhiteTheme_Middle.normal.background = EntryBackgroundTexture_WhiteTheme_Middle;
            EntryBackground_WhiteTheme_Last = new GUIStyle { name = "WhiteLast" };
            EntryBackground_WhiteTheme_Last.normal.background = EntryBackgroundTexture_WhiteTheme_Last;

            EntryBackground_GrayTheme_First = new GUIStyle { name = "GrayFirst" };
            EntryBackground_GrayTheme_First.normal.background = EntryBackgroundTexture_GrayTheme_First;
            EntryBackground_GrayTheme_Middle = new GUIStyle { name = "GrayMiddle" };
            EntryBackground_GrayTheme_Middle.normal.background = EntryBackgroundTexture_GrayTheme_Middle;
            EntryBackground_GrayTheme_Last = new GUIStyle { name = "GrayLast" };
            EntryBackground_GrayTheme_Last.normal.background = EntryBackgroundTexture_GrayTheme_Last;

            EntryBackground_BlackTheme_First = new GUIStyle { name = "BlackFirst" };
            EntryBackground_BlackTheme_First.normal.background = EntryBackgroundTexture_BlackTheme_First;
            EntryBackground_BlackTheme_Middle = new GUIStyle { name = "BlackMiddle" };
            EntryBackground_BlackTheme_Middle.normal.background = EntryBackgroundTexture_BlackTheme_Middle;
            EntryBackground_BlackTheme_Last = new GUIStyle { name = "BlackLast" };
            EntryBackground_BlackTheme_Last.normal.background = EntryBackgroundTexture_BlackTheme_Last;

            IncreaseDecimalDigitsTexture = AssetManager.GetAsset<Texture2D>($"{_plugin.SpaceWarpMetadata.ModID}/images/increase-decimal-19.png");
            DecreaseDecimalDigitsTexture = AssetManager.GetAsset<Texture2D>($"{_plugin.SpaceWarpMetadata.ModID}/images/decrease-decimal-19.png");
        }

        public static void SetActiveTheme(Theme theme)
        {
            if (ActiveTheme == theme) return;

            switch (theme)
            {
                case Theme.munix:
                    EntryBackground_First = EntryBackground_WhiteTheme_First;
                    EntryBackground_Middle = EntryBackground_WhiteTheme_Middle;
                    EntryBackground_Last = EntryBackground_WhiteTheme_Last;
                    _nameColor = _nameColor_LightThemes;
                    _valueColor = _valueColor_LightThemes;
                    _unitColor = _unitColor_LightThemes;
                    break;                    
                case Theme.Gray:
                    EntryBackground_First = EntryBackground_GrayTheme_First;
                    EntryBackground_Middle = EntryBackground_GrayTheme_Middle;
                    EntryBackground_Last = EntryBackground_GrayTheme_Last;
                    _nameColor = _nameColor_DarkThemes;
                    _valueColor = _valueColor_DarkThemes;
                    _unitColor = _unitColor_DarkThemes;
                    break;
                case Theme.Black:
                    EntryBackground_First = EntryBackground_BlackTheme_First;
                    EntryBackground_Middle = EntryBackground_BlackTheme_Middle;
                    EntryBackground_Last = EntryBackground_BlackTheme_Last;
                    _nameColor = _nameColor_DarkThemes;
                    _valueColor = _valueColor_DarkThemes;
                    _unitColor = _unitColor_DarkThemes;
                    break;
                default:
                    SetActiveTheme(Theme.Gray);
                    break;
            }

            ActiveTheme = theme;
            InitializeStyles();
        }

        /// <summary>
        /// Draws a white horizontal line accross the container it's put in
        /// </summary>
        /// <param name="height">Height/thickness of the line</param>
        public static void DrawHorizontalLine(float height)
        {
            Texture2D horizontalLineTexture = new Texture2D(1, 1);
            horizontalLineTexture.SetPixel(0, 0, Color.white);
            horizontalLineTexture.Apply();
            GUI.DrawTexture(GUILayoutUtility.GetRect(Screen.width, height), horizontalLineTexture);
        }

        /// <summary>
        /// Draws a white horizontal line accross the container it's put in with height of 1 px
        /// </summary>
        public static void DrawHorizontalLine() { Styles.DrawHorizontalLine(1); }

        internal static void SetStylesForOldSpaceWarpSkin()
        {
            SectionToggleStyle = new GUIStyle(SpaceWarpUISkin.toggle)
            {
                margin = new RectOffset(0, 30, 0, 5)
            };
        }
    }

    public enum Theme
    {
        munix,
        Gray,
        Black
    }
}
