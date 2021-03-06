﻿// SampSharp
// Copyright 2017 Tim Potze
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SampSharp.Core.CodePages;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;

namespace TestMode
{
    internal class GameMode : BaseMode
    {
        private int _ticks;

        [Command("kickme")]
        public static async void KickMeCommand(BasePlayer player)
        {
            player.SendClientMessage("Bye!");
            await Task.Delay(10);
            player.Kick();
        }

        [Command("spawn")]
        public static void SpawnCommand(BasePlayer player, VehicleModelType type)
        {
            var vehicle = BaseVehicle.Create(type, player.Position + Vector3.Up, player.Angle, -1, -1);
            player.PutInVehicle(vehicle);
            vehicle.GetDamageStatus(out var panels, out var doors, out var lights, out var tires);
            Console.WriteLine(panels.ToString());
            Console.WriteLine(doors.ToString());
            Console.WriteLine(lights.ToString());
            Console.WriteLine(tires.ToString());
        }

        [Command("status")]
        public static void StatusCommand(BasePlayer player, int vehicleid)
        {
            var vehicle = BaseVehicle.Find(vehicleid);
            int panels = 99;
            int doors = 100;
            int lights = 101;
            int tires = 102;
            vehicle.GetDamageStatus(out panels, out doors, out lights, out tires);
            Console.WriteLine(panels.ToString());
            Console.WriteLine(doors.ToString());
            Console.WriteLine(lights.ToString());
            Console.WriteLine(tires.ToString());
        }

        [Command("setstatus")]
        public static void SetStatusCommand(BasePlayer player, int vehicleid)
        {
            var vehicle = BaseVehicle.Find(vehicleid);
            vehicle.SetDoorsParameters(true, true, true, true);

            vehicle.GetDamageStatus(out var panels, out var doors, out var lights, out var tires);
            Console.WriteLine(panels.ToString());
            Console.WriteLine(doors.ToString());
            Console.WriteLine(lights.ToString());
            Console.WriteLine(tires.ToString());
        }

        [Command("give")]
        public static void GiveCommand(BasePlayer player, Weapon weapon, int ammo)
        {
            player.GiveWeapon(weapon, ammo);
        }

        [Command("enter")]
        public static void EnterCommand(BasePlayer player, int vehicle)
        {
            player.PutInVehicle(BaseVehicle.Find(vehicle));
        }

        [Command("myfirstcommand")]
        public static void MyFirstCommand(BasePlayer player, string message)
        {
            player.SendClientMessage($"Hello, world! You said {message}");
        }

        [Command("pos")]
        public static async void PositionCommand(BasePlayer player)
        {
            player.SendClientMessage(Color.Yellow, $"Position: {player.Position}");

            await Task.Delay(1000);

            player.SendClientMessage("Still here!");
        }

        [Command("dialogtest")]
        public static async void DialogTest(BasePlayer player)
        {
            var dialog = new MessageDialog("Test dialog", "This message should hide in 2 seconds.", "Don't click me!");
            dialog.Response += (sender, args) =>
            {
                player.SendClientMessage("You responed to the dialog with button" + args.DialogButton);
            };

            player.SendClientMessage("Showing dialog");
            dialog.Show(player);

            await Task.Delay(2000);

            player.SendClientMessage("Hiding dialog");
            Dialog.Hide(player);
        }

        [Command("asyncdialog")]
        public static async void DialogAsyncTest(BasePlayer player)
        {
            var dialog = new MessageDialog("Async dialog test", "Quit with this dialog still open.", "Don't click me!");

            Console.WriteLine("Showing dialog");
            try
            {

                await dialog.ShowAsync(player);
                Console.WriteLine("Dialog ended");
            }
            catch (PlayerDisconnectedException e)
            {
                Console.WriteLine($"{player} left.");
                Console.WriteLine(e);
            }
    }

        #region Overrides of BaseMode
        
        protected override void OnPlayerDied(BasePlayer player, DeathEventArgs e)
        {
            Console.WriteLine("Death");
            base.OnPlayerDied(player, e);
        }

        #endregion

        [Command("weapon")]
        public static void WeaponCommand(BasePlayer player, Weapon weapon, int ammo = 30)
        {
            player.GiveWeapon(weapon, ammo);
        }

        [Command("kick")]
        public static void Kick(BasePlayer player, BasePlayer target)
        {
            target.Kick();
        }
        
        #region Overrides of BaseMode
        
        /// <summary>
        ///     Raises the <see cref="BaseMode.Tick" /> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data. </param>
        protected override void OnTick(EventArgs e)
        {
            base.OnTick(e);

            if (_ticks++ % 1000 == 0)
            {
                Console.WriteLine("Server is still ticking...");
            }
        }
        
        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            
            Console.WriteLine("The game mode has loaded.");
            AddPlayerClass(0, Vector3.Zero, 0);

            SetGameModeText("Before delay");
            await Task.Delay(10);

            Console.WriteLine("waited 2");
            SetGameModeText("After delay");

//            for (var i = 0; i < 1000; i++)
//            {
//                await Task.Delay(10);
//                SetGameModeText("Loop " + i);
//                Console.WriteLine("Loop " + i);
//            }
            Console.WriteLine("RCON commands: sd (shutdown) msg (repeat message)");
        }

        protected override void OnRconCommand(RconEventArgs e)
        {
            Console.WriteLine($"Received RCON Command: {e.Command}");

            if (e.Command == "sd")
            {
                Client.ShutDown();
                e.Success = true;
                Console.WriteLine("Shutting down client...");
            }

            if (e.Command.StartsWith("msg"))
            {
                var msg = e.Command.Substring(4);

                Console.WriteLine("Received: " + msg);
                msg = new string(msg.Reverse().ToArray());

                Console.WriteLine("Sending: " + msg);

                e.Success = true;
                Server.Print(msg);
            }
            
            base.OnRconCommand(e);
        }

        [Command("help")]
        private static void Help(BasePlayer player)
        {
            player.SendClientMessage("/reverse, /help");
        }

        [Command("reverse")]
        private static void Reverse(BasePlayer player, string message)
        {
            player.SendClientMessage($"{message} reversed: ");
            message = new string(message.Reverse().ToArray());
            player.SendClientMessage(message);
        }
        
        protected override void OnPlayerConnected(BasePlayer player, EventArgs e)
        {
            Console.WriteLine($"Player {player.Name} connected.");
            base.OnPlayerConnected(player, e);
        }

        protected override void OnPlayerDisconnected(BasePlayer player, DisconnectEventArgs e)
        {
            Console.WriteLine($"Player {player.Name} disconnected. Reason: {e.Reason}.");
            base.OnPlayerDisconnected(player, e);
        }
        
        #endregion
    }
}