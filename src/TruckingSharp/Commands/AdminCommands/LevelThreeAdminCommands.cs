﻿using SampSharp.GameMode;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using TruckingSharp.Commands.Permissions;
using TruckingSharp.Constants;
using TruckingSharp.Database.Repositories;
using TruckingSharp.World;

namespace TruckingSharp.Commands.AdminCommands
{
    [CommandGroup("admin", PermissionChecker = typeof(LevelThreeAdminPermission))]
    public class LevelThreeAdminCommands
    {
        [Command("setadmin", Shortcut = "setadmin")]
        public static async void OnSetAdminCommandAsync(BasePlayer sender, Player target, int level)
        {
            if (!target.IsLoggedIn)
            {
                sender.SendClientMessage(Color.Red, Messages.PlayerNotLoggedIn);
                return;
            }

            if (target == sender)
            {
                sender.SendClientMessage(Color.Red, Messages.CommandNotAllowedOnSelf);
                return;
            }

            if (level > 3)
            {
                sender.SendClientMessage(Color.Red, "The level must be between 0 and 3.");
                return;
            }

            var targetAccount = target.Account;
            targetAccount.AdminLevel = (byte)level;
            await new PlayerAccountRepository(ConnectionFactory.GetConnection).UpdateAsync(targetAccount);

            target.SendClientMessage(Color.GreenYellow,
                $"Your admin level have been set to {level} by {sender.Name}.");
        }

        [Command("resetplayer", Shortcut = "resetplayer")]
        public static async void OnResetPlayerCommandAsync(BasePlayer sender, Player target, int money, int score, int stats, string reason)
        {
            if (!target.IsLoggedIn)
            {
                sender.SendClientMessage(Color.Red, Messages.PlayerNotLoggedIn);
                return;
            }

            if (target.Account.AdminLevel > 0)
            {
                sender.SendClientMessage(Color.Red, "You can't use this command on admins.");
                return;
            }

            if (target == sender)
            {
                sender.SendClientMessage(Color.Red, Messages.CommandNotAllowedOnSelf);
                return;
            }

            if (money + score + stats == 0)
                return;

            var targetAccount = target.Account;

            if (money == 1)
            {
                targetAccount.Money = 0;

                target.SendClientMessage(Color.Red, $"Your money has been reset by {sender.Name}.");
                sender.SendClientMessage(Color.GreenYellow, $"You have reset the money of {target.Name}.");
            }

            if (score == 1)
            {
                targetAccount.Score = 0;

                target.SendClientMessage(Color.Red, $"Your score has been reset by {sender.Name}.");
                sender.SendClientMessage(Color.GreenYellow, $"You have reset the score of {target.Name}.");
            }

            if (stats == 1)
            {
                targetAccount.TruckerJobs = 0;
                targetAccount.ConvoyJobs = 0;
                targetAccount.BusDriverJobs = 0;
                targetAccount.PilotJobs = 0;
                targetAccount.MafiaJobs = 0;
                targetAccount.PoliceFined = 0;
                targetAccount.PoliceJailed = 0;
                targetAccount.AssistanceJobs = 0;
                targetAccount.MetersDriven = 0;

                target.SendClientMessage(Color.Red, $"Your stats have been reset by {sender.Name}.");
                sender.SendClientMessage(Color.GreenYellow, $"You have reset the stats of {target.Name}.");
            }

            target.SendClientMessage(Color.Red, $"Reason: {reason}.");

            await new PlayerAccountRepository(ConnectionFactory.GetConnection).UpdateAsync(targetAccount);
        }

        [Command("createcamera", Shortcut = "createcamera")]
        public static async void OnCreateCameraCommandAsync(Player sender, int maxSpeed)
        {
            await SpeedCameraController.CreateSpeedCameraAsync(sender.Position + Vector3.Down, sender.Angle, maxSpeed);
            sender.Position += Vector3.One;
        }

        [Command("deletecamera", Shortcut = "deletecamera")]
        public static async void OnDeleteCameraCommandAsync(Player sender, int camId)
        {
            await SpeedCameraController.RemoveSpeedCameraAsync(camId);
        }
    }
}