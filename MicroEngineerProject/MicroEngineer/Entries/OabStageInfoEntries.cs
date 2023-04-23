﻿using KSP.Game;
using KSP.Sim.DeltaV;
using KSP.Sim.impl;
using Newtonsoft.Json;

namespace MicroMod
{
    public class OabStageInfoEntry : BaseEntry
    { }

    public class TotalBurnTime_OAB : OabStageInfoEntry
    {
        public bool UseDHMSFormatting; // TODO: implement

        public TotalBurnTime_OAB()
        {
            Name = "Total Burn Time (OAB)";
            Description = "Shows the total length of burn the vessel can mantain.";
            Category = MicroEntryCategory.OAB;
            IsDefault = true;
            Unit = "s";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
            UseDHMSFormatting = true;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.VesselDeltaVComponentOAB?.TotalBurnTime;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                if (UseDHMSFormatting)
                    return Utility.SecondsToTimeString((double)EntryValue);
                else
                    return String.IsNullOrEmpty(this.Formatting) ? EntryValue.ToString() : String.Format(Formatting, EntryValue);
            }
        }
    }

    public class TotalDeltaVASL_OAB : OabStageInfoEntry
    {
        public TotalDeltaVASL_OAB()
        {
            Name = "Total ∆v ASL (OAB)";
            Description = "Shows the vessel's total delta velocity At Sea Level.";
            Category = MicroEntryCategory.OAB;
            IsDefault = true;
            Unit = "m/s";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }
        public override void RefreshData()
        {
            EntryValue = Utility.VesselDeltaVComponentOAB?.TotalDeltaVASL;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TotalDeltaVActual_OAB : OabStageInfoEntry
    {
        public TotalDeltaVActual_OAB()
        {
            Name = "Total ∆v Actual (OAB)";
            Description = "Shows the vessel's actual total delta velocity (not used in OAB).";
            Category = MicroEntryCategory.OAB;
            IsDefault = true;
            Unit = "m/s";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }
        public override void RefreshData()
        {
            EntryValue = Utility.VesselDeltaVComponentOAB?.TotalDeltaVActual;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TotalDeltaVVac_OAB : OabStageInfoEntry
    {
        public TotalDeltaVVac_OAB()
        {
            Name = "Total ∆v Vac (OAB)";
            Description = "Shows the vessel's total delta velocity in Vacuum.";
            Category = MicroEntryCategory.OAB;
            IsDefault = true;
            Unit = "m/s";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }
        public override void RefreshData()
        {
            EntryValue = Utility.VesselDeltaVComponentOAB?.TotalDeltaVVac;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    /// <summary>
    /// Calculates torque from the Center of Thrust and Center of Mass
    /// </summary>    
    public class Torque : OabStageInfoEntry
    {
        [JsonProperty]
        internal bool IsActive = false;

        public Torque()
        {
            Name = "Torque";
            Description = "Thrust torque that is generated by not having Thrust Vector and Center of Mass aligned. Turn on the Center of Thrust and Center of Mass VAB indicators to get an accurate value.";
            Category = MicroEntryCategory.OAB;
            IsDefault = true;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            if (!this.IsActive)
                return;

            Vector3d com = GameManager.Instance?.Game?.OAB?.Current?.Stats?.MainAssembly?.CenterOfProperties?.CoM ?? Vector3d.zero;
            Vector3d cot = GameManager.Instance?.Game?.OAB?.Current?.Stats?.MainAssembly?.CenterOfProperties?.CoT ?? Vector3d.zero;

            if (com == Vector3d.zero || cot == Vector3d.zero)
                return;

            List<DeltaVEngineInfo> engines = GameManager.Instance?.Game?.OAB?.Current?.Stats?.MainAssembly?.VesselDeltaV?.EngineInfo;
            if (engines == null || engines.Count == 0)
                return;

            Vector3d force = new Vector3d();

            foreach (var engine in engines)
            {
                force += engine.ThrustVectorVac;
            }

            var leverArm = cot - com;

            Vector3d torque = Vector3d.Cross(force, (Vector3d)leverArm);

            this.EntryValue = torque.magnitude;
            this.Unit = (double)EntryValue >= 1.0 ? "kNm" : "Nm";
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null || !this.IsActive)
                    return "-";

                if ((double)EntryValue >= 1.0)
                    return $"{String.Format("{0:F2}", EntryValue)}";

                return Math.Abs((double)EntryValue) > double.Epsilon ? $"{String.Format("{0:F2}", (double)EntryValue * 1000.0)}" : $"{String.Format("{0:F0}", (double)EntryValue)}";
            }
        }
    }

    /// <summary>
    /// Holds stage info parameters for each stage. Also keeps information about the celestial body user selected in the window.
    /// </summary>
    public class StageInfo_OAB : OabStageInfoEntry
    {
        public List<string> CelestialBodyForStage = new();

        public StageInfo_OAB()
        {
            Name = "Stage Info (OAB)";
            Description = "Holds a list of stage info parameters.";
            Category = MicroEntryCategory.OAB;
            IsDefault = true;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue ??= new List<DeltaVStageInfo_OAB>();

            ((List<DeltaVStageInfo_OAB>)EntryValue).Clear();

            if (Utility.VesselDeltaVComponentOAB?.StageInfo == null) return;

            foreach (var stage in Utility.VesselDeltaVComponentOAB.StageInfo)
            {
                ((List<DeltaVStageInfo_OAB>)EntryValue).Add(new DeltaVStageInfo_OAB
                {
                    DeltaVActual = stage.DeltaVActual,
                    DeltaVASL = stage.DeltaVatASL,
                    DeltaVVac = stage.DeltaVinVac,
                    DryMass = stage.DryMass,
                    EndMass = stage.EndMass,
                    FuelMass = stage.FuelMass,
                    IspASL = stage.IspASL,
                    IspActual = stage.IspActual,
                    IspVac = stage.IspVac,
                    SeparationIndex = stage.SeparationIndex,
                    Stage = stage.Stage,
                    StageBurnTime = stage.StageBurnTime,
                    StageMass = stage.StageMass,
                    StartMass = stage.StartMass,
                    TWRASL = stage.TWRASL,
                    TWRActual = stage.TWRActual,
                    TWRVac = stage.TWRVac,
                    ThrustASL = stage.ThrustASL,
                    ThrustActual = stage.ThrustActual,
                    ThrustVac = stage.ThrustVac,
                    TotalExhaustVelocityASL = stage.TotalExhaustVelocityASL,
                    TotalExhaustVelocityActual = stage.TotalExhaustVelocityActual,
                    TotalExhaustVelocityVAC = stage.TotalExhaustVelocityVAC,
                    DeltaVStageInfo = stage
                });
            }
        }

        public override string ValueDisplay
        {
            get
            {
                return "-";
            }
        }

        /// <summary>
        /// Adds a new string to the CelestialBodyForStage list that corresponds to the HomeWorld, i.e. Kerbin
        /// </summary>
        /// <param name="celestialBodies"></param>
        internal void AddNewCelestialBody(MicroCelestialBodies celestialBodies)
        {
            this.CelestialBodyForStage.Add(celestialBodies.Bodies.Find(b => b.IsHomeWorld).Name);
        }
    }

    /// <summary>
    /// Parameters for one stage
    /// </summary>
    internal class DeltaVStageInfo_OAB
    {
        internal double DeltaVActual;
        internal double DeltaVASL;
        internal double DeltaVVac;
        internal double DryMass;
        internal double EndMass;
        internal double FuelMass;
        internal double IspASL;
        internal double IspActual;
        internal double IspVac;
        internal int SeparationIndex;
        internal int Stage;
        internal double StageBurnTime;
        internal double StageMass;
        internal double StartMass;
        internal float TWRASL;
        internal float TWRActual;
        internal float TWRVac;
        internal float ThrustASL;
        internal float ThrustActual;
        internal float ThrustVac;
        internal float TotalExhaustVelocityASL;
        internal float TotalExhaustVelocityActual;
        internal float TotalExhaustVelocityVAC;
        internal DeltaVStageInfo DeltaVStageInfo;

        private float GetThrustAtAltitude(double altitude, CelestialBodyComponent cel) => this.DeltaVStageInfo.EnginesActiveInStage?.Select(e => e.Engine.MaxThrustOutputAtm(atmPressure: cel.GetPressure(altitude) / 101.325))?.Sum() ?? 0;
        private double GetISPAtAltitude(double altitude, CelestialBodyComponent cel)
        {
            float sum = 0;
            foreach (DeltaVEngineInfo engInfo in this.DeltaVStageInfo.EnginesActiveInStage)
                sum += engInfo.Engine.MaxThrustOutputAtm(atmPressure: cel.GetPressure(altitude) / 101.325) /
                 engInfo.Engine.currentEngineModeData.atmosphereCurve.Evaluate((float)cel.GetPressure(altitude) / 101.325f);
            return GetThrustAtAltitude(altitude, cel) / sum;
        }
        private double GetDeltaVelAlt(double altitude, CelestialBodyComponent cel) => GetISPAtAltitude(altitude, cel) * 9.80665 * Math.Log(this.DeltaVStageInfo.StartMass / this.DeltaVStageInfo.EndMass);
        private double GetTWRAtAltitude(double altitude, CelestialBodyComponent cel) => this.DeltaVStageInfo.TWRVac * (GetThrustAtAltitude(altitude, cel) / this.DeltaVStageInfo.ThrustVac);
        internal double GetTWRAtSeaLevel(CelestialBodyComponent cel) => this.GetTWRAtAltitude(0, cel);
        internal double GetDeltaVelAtSeaLevel(CelestialBodyComponent cel) => GetDeltaVelAlt(0, cel);
    }
}
