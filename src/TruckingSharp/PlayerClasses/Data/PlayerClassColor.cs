﻿using SampSharp.GameMode.SAMP;

namespace TruckingSharp.PlayerClasses.Data
{
    public struct PlayerClassColor
    {
        public static Color TruckerColor { get; } = Color.FromInteger(16744448, ColorFormat.RGB);
        public static Color BusDriverColor { get; } = Color.FromInteger(8454143, ColorFormat.RGB);
        public static Color PilotColor { get; } = Color.FromInteger(32896, ColorFormat.RGB);
        public static Color PoliceColor { get; } = Color.FromInteger(255, ColorFormat.RGB);
        public static Color MafiaColor { get; } = Color.FromInteger(8388863, ColorFormat.RGB);
        public static Color AssistanceColor { get; } = Color.FromInteger(8453888, ColorFormat.RGB);
    }
}