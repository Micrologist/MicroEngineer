namespace MicroMod
{
    /// <summary>
    /// Each entry has a category we assign to it depending on where it fits best
    /// </summary>
    public enum MicroEntryCategory
    {
        Vessel,
        Orbital,
        Surface,
        Flight,
        Target,
        Maneuver,
        Stage,
        Body,
        Misc,
        OAB
    }

    /// <summary>
    /// Main windows cannot be deleted. Value None indicated that the window is not a main window
    /// </summary>
    public enum MainWindow
    {
        None = 0,
        MainGui,
        Vessel,
        Stage,
        Orbital,
        Surface,
        Flight,
        Target,
        Maneuver,
        Settings,
        StageInfoOAB
    }

    public enum Theme
    {
        munix,
        Gray,
        Black
    }

    public enum EntryType
    {
        BasicText,
        Time,
        LatitudeLongitude,
        StageInfo,
        StageInfoOAB,
        Separator
    }
}
