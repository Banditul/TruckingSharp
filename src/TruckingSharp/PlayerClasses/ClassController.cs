﻿using SampSharp.GameMode;
using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using System;
using TruckingSharp.Constants;
using TruckingSharp.PlayerClasses.ClassesSpawn;
using TruckingSharp.PlayerClasses.Data;

namespace TruckingSharp.PlayerClasses
{
    [Controller]
    public class ClassController : IController, IEventListener
    {
        public void RegisterEvents(BaseMode gameMode)
        {
            gameMode.PlayerRequestClass += Class_PlayerRequestClass;
            gameMode.PlayerRequestSpawn += Class_PlayerRequestSpawn;
            gameMode.PlayerSpawned += Class_PlayerSpawned;
        }

        private void Class_PlayerSpawned(object sender, SampSharp.GameMode.Events.SpawnEventArgs e)
        {
            var player = sender as Player;

            switch (player.PlayerClass)
            {
                case PlayerClassType.TruckDriver:
                    player.Color = PlayerClassColor.TruckerColor;
                    break;

                case PlayerClassType.BusDriver:
                    player.Color = PlayerClassColor.BusDriverColor;
                    break;

                case PlayerClassType.Pilot:
                    player.Color = PlayerClassColor.PilotColor;
                    break;

                case PlayerClassType.Police:
                    player.Color = PlayerClassColor.PoliceColor;
                    break;

                case PlayerClassType.Mafia:
                    player.Color = PlayerClassColor.MafiaColor;
                    break;

                case PlayerClassType.Courier:
                    player.Color = PlayerClassColor.CourierColor;
                    break;

                case PlayerClassType.Assistance:
                    player.Color = PlayerClassColor.AssistanceColor;
                    break;

                case PlayerClassType.RoadWorker:
                    player.Color = PlayerClassColor.RoadWorkerColor;
                    break;
            }
        }

        private void Class_PlayerRequestSpawn(object sender, SampSharp.GameMode.Events.RequestSpawnEventArgs e)
        {
            var player = sender as Player;

            var randomIndex = new Random();
            int index;
            float angle = 0.0f;
            Vector3 position = Vector3.Zero;

            switch (player.PlayerClass)
            {
                case PlayerClassType.TruckDriver:
                    index = randomIndex.Next(0, TruckerSpawn.TruckerSpawns.Length);

                    position = TruckerSpawn.TruckerSpawns[index].Position;
                    angle = TruckerSpawn.TruckerSpawns[index].Angle;

                    BasePlayer.SendClientMessageToAll(Messages.PlayerJoinedTruckerClass, player.Name);
                    break;

                case PlayerClassType.BusDriver:
                    index = randomIndex.Next(0, BusDriverSpawn.BusDriverSpawns.Length);

                    position = BusDriverSpawn.BusDriverSpawns[index].Position;
                    angle = BusDriverSpawn.BusDriverSpawns[index].Angle;

                    BasePlayer.SendClientMessageToAll(Messages.PlayerJoinedBusDriverClass, player.Name);
                    break;

                case PlayerClassType.Pilot:
                    index = randomIndex.Next(0, PilotSpawn.PilotSpawns.Length);

                    position = PilotSpawn.PilotSpawns[index].Position;
                    angle = PilotSpawn.PilotSpawns[index].Angle;

                    BasePlayer.SendClientMessageToAll(Messages.PlayerJoinedPilotClass, player.Name);
                    break;

                case PlayerClassType.Police:

                    if (!player.CheckIfPlayerCanJoinPolice())
                    {
                        return;
                    }

                    if (player.Account.Score < 100)
                    {
                        player.GameText("You need 100 scorepoints for police class", 5000, 4);
                        player.SendClientMessage(Color.Red, "You need 100 scorepoints for police class");
                        e.PreventSpawning = true;
                        return;
                    }

                    if (player.Account.Wanted > 0)
                    {
                        player.GameText("You are not allowed to choose police class when you're wanted", 5000, 4);
                        player.SendClientMessage(Color.Red, "You are not allowed to choose police class when you're wanted");
                        e.PreventSpawning = true;
                        return;
                    }

                    index = randomIndex.Next(0, PoliceSpawn.PoliceSpawns.Length);

                    position = PoliceSpawn.PoliceSpawns[index].Position;
                    angle = PoliceSpawn.PoliceSpawns[index].Angle;

                    BasePlayer.SendClientMessageToAll(Messages.PlayerJoinedPoliceClass, player.Name);
                    break;

                case PlayerClassType.Mafia:
                    index = randomIndex.Next(0, MafiaSpawn.MafiaSpawns.Length);

                    position = MafiaSpawn.MafiaSpawns[index].Position;
                    angle = MafiaSpawn.MafiaSpawns[index].Angle;

                    BasePlayer.SendClientMessageToAll(Messages.PlayerJoinedMafiaClass, player.Name);
                    break;

                case PlayerClassType.Courier:
                    index = randomIndex.Next(0, CourierSpawn.CourierSpawns.Length);

                    position = CourierSpawn.CourierSpawns[index].Position;
                    angle = CourierSpawn.CourierSpawns[index].Angle;

                    BasePlayer.SendClientMessageToAll(Messages.PlayerJoinedCourierClass, player.Name);
                    break;

                case PlayerClassType.Assistance:
                    index = randomIndex.Next(0, AssistanceSpawn.AssistanceSpawns.Length);

                    position = AssistanceSpawn.AssistanceSpawns[index].Position;
                    angle = AssistanceSpawn.AssistanceSpawns[index].Angle;

                    BasePlayer.SendClientMessageToAll(Messages.PlayerJoinedAssistanceClass, player.Name);
                    break;

                case PlayerClassType.RoadWorker:
                    index = randomIndex.Next(0, RoadWorkerSpawn.RoadworkerSpawns.Length);

                    position = RoadWorkerSpawn.RoadworkerSpawns[index].Position;
                    angle = RoadWorkerSpawn.RoadworkerSpawns[index].Angle;

                    BasePlayer.SendClientMessageToAll(Messages.PlayerJoinedRoadWorkerClass, player.Name);
                    break;
            }

            player.SetSpawnInfo(0, player.Skin, position, angle);
        }

        private void Class_PlayerRequestClass(object sender, SampSharp.GameMode.Events.RequestClassEventArgs e)
        {
            var player = sender as Player;

            player.Interior = 14;
            player.Position = new Vector3(258.4893, -41.4008, 1002.0234);
            player.Angle = 270.0f;
            player.CameraPosition = new Vector3(256.0815, -43.0475, 1004.0234);
            player.SetCameraLookAt(new Vector3(258.4893, -41.4008, 1002.0234));

            switch (e.ClassId)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    player.GameText(Messages.TruckerClass, 3000, 4);
                    player.PlayerClass = PlayerClassType.TruckDriver;
                    break;

                case 8:
                case 9:
                    player.GameText(Messages.BusDriverClass, 3000, 4);
                    player.PlayerClass = PlayerClassType.BusDriver;
                    break;

                case 10:
                    player.GameText(Messages.PilotClass, 3000, 4);
                    player.PlayerClass = PlayerClassType.Pilot;
                    break;

                case 11:
                case 12:
                case 13:
                    player.GameText(Messages.PoliceClass, 3000, 4);
                    player.PlayerClass = PlayerClassType.Police;
                    break;

                case 14:
                case 15:
                case 16:
                    player.GameText(Messages.MafiaClass, 3000, 4);
                    player.PlayerClass = PlayerClassType.Mafia;
                    break;

                case 17:
                case 18:
                    player.GameText(Messages.CourierClass, 3000, 4);
                    player.PlayerClass = PlayerClassType.Courier;
                    break;

                case 19:
                    player.GameText(Messages.AssistanceClass, 3000, 4);
                    player.PlayerClass = PlayerClassType.Assistance;
                    break;

                case 20:
                case 21:
                case 22:
                    player.GameText(Messages.RoadWorkerClass, 3000, 4);
                    player.PlayerClass = PlayerClassType.RoadWorker;
                    break;
            }
        }
    }
}