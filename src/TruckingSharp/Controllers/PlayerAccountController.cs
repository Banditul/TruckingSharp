﻿using SampSharp.GameMode;
using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP;
using System;
using System.Threading.Tasks;
using TruckingSharp.Database.Entities;
using TruckingSharp.Database.Repositories;
using TruckingSharp.Events;
using TruckingSharp.Services;

namespace TruckingSharp.Controllers
{
    [Controller]
    public class PlayerAccountController : IEventListener
    {
        private static PlayerAccountRepository AccountRepository => new PlayerAccountRepository(ConnectionFactory.GetConnection);

        private PlayerBanRepository _banRepository => new PlayerBanRepository(ConnectionFactory.GetConnection);

        public void RegisterEvents(BaseMode gameMode)
        {
            PlayerLogin += PlayerAccountController_PlayerLogin;
            gameMode.PlayerConnected += GameMode_PlayerConnected;
            gameMode.PlayerDisconnected += GameMode_PlayerDisconnected;
        }

        private async void GameMode_PlayerDisconnected(object sender, SampSharp.GameMode.Events.DisconnectEventArgs e)
        {
            if (!(sender is Player player))
                return;

            var playerAccount = player.Account;
            playerAccount.MetersDriven += player.MetersDriven;
            await AccountRepository.UpdateAsync(playerAccount);
        }

        public event EventHandler<PlayerLoginEventArgs> PlayerLogin;

        private async void GameMode_PlayerConnected(object sender, EventArgs e)
        {
            if (!(sender is Player player))
                return;

            var playerBan = await _banRepository.FindAsync(player.Name);

            if (playerBan != null)
            {
                if (playerBan.Duration < DateTime.Now)
                {
                    await _banRepository.DeleteAsync(playerBan);
                    CheckAccount(player);
                    return;
                }

                player.SendClientMessage(Color.Red, "You are still banned.");
                player.SendClientMessage(Color.Red, "Until: {0:dd/MM/yyyy H:mm:ss zzz}", playerBan.Duration);

                await Task.Delay(Configuration.Instance.KickDelay);
                player.Kick();

                return;
            }

            CheckAccount(player);
        }

        private void CheckAccount(Player player)
        {
            if (player.Account == null)
                RegisterPlayer(player);
            else
                LoginPlayer(player);
        }

        private void LoginPlayer(Player player)
        {
            var message =
                $"Insert your password. Tries left: {player.LoginTries}/{Configuration.Instance.MaximumLogins}";
            var dialog = new InputDialog("Login", message, true, "Login", "Cancel");
            dialog.Show(player);
            dialog.Response += async (sender, ev) =>
            {
                if (ev.DialogButton == DialogButton.Left)
                {
                    if (player.LoginTries >= Configuration.Instance.MaximumLogins)
                    {
                        player.SendClientMessage(Color.OrangeRed,
                            "You exceed maximum login tries. You have been kicked!");
                        await Task.Delay(Configuration.Instance.KickDelay);
                        player.Kick();
                    }
                    else if (PasswordHashingService.VerifyPasswordHash(ev.InputText, player.Account.Password))
                    {
                        player.IsLoggedIn = true;

                        PlayerLogin?.Invoke(player, new PlayerLoginEventArgs { Success = true });
                    }
                    else
                    {
                        player.LoginTries++;
                        player.SendClientMessage(Color.Red, "Wrong password");

                        dialog.Message =
                            $"Wrong password! Retype your password! Tries left: {player.LoginTries}/{Configuration.Instance.MaximumLogins}";

                        LoginPlayer(player);
                    }
                }
                else
                {
                    player.Kick();
                }
            };
        }

        private void PlayerAccountController_PlayerLogin(object sender, PlayerLoginEventArgs e)
        {
            if (!(sender is Player player))
                return;

            if (!e.Success)
                return;

            player.ToggleSpectating(false);
            player.Money = player.Account.Money;
            player.Score = player.Account.Score;
        }

        private void RegisterPlayer(Player player)
        {
            var registerDialog = new InputDialog("Register", "Insert your password", true, "Submit", "Cancel");
            registerDialog.Show(player);
            registerDialog.Response += async (senderPlayer, ev) =>
            {
                if (ev.DialogButton == DialogButton.Left)
                {
                    var hash = PasswordHashingService.GetPasswordHash(ev.InputText);

                    var newAccount = new PlayerAccount { Name = player.Name, Password = hash };

                    await AccountRepository.AddAsync(newAccount);

                    LoginPlayer(player);
                }
                else
                {
                    player.Kick();
                }
            };
        }
    }
}