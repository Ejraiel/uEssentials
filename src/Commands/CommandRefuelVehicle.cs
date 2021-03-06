/*
 *  This file is part of uEssentials project.
 *      https://uessentials.github.io/
 *
 *  Copyright (C) 2015-2016  Leonardosc
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License along
 *  with this program; if not, write to the Free Software Foundation, Inc.,
 *  51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using System.Linq;
using Essentials.Api.Command;
using Essentials.Api.Command.Source;
using Essentials.Api.Unturned;
using Essentials.I18n;
using SDG.Unturned;

// ReSharper disable InconsistentNaming

namespace Essentials.Commands
{
    [CommandInfo(
        Name = "refuelvehicle",
        Aliases = new [] { "refuel" },
        Description = "Refuel current/all vehicles",
        Usage = "<all>"
     )]
    public class CommandRefuelVehicle : EssCommand
    {
        public override void OnExecute( ICommandSource src, ICommandArgs args )
        {
            if ( args.IsEmpty )
            {
                if ( src.IsConsole )
                {
                    ShowUsage( src );
                    return;
                }

                var currentVeh = src.ToPlayer().CurrentVehicle;

                if ( currentVeh != null )
                {
                    VehicleManager.sendVehicleFuel( currentVeh, currentVeh.asset.fuel );

                    EssLang.VEHICLE_REFUELED.SendTo( src );
                }
                else
                {
                    EssLang.NOT_IN_VEHICLE.SendTo( src );
                }
            }
            else if ( args[0].Is( "all" ) )
            {
                if ( !src.HasPermission( Permission + ".all" ) )
                {
                    EssLang.COMMAND_NO_PERMISSION.SendTo( src );
                    return;
                }

                lock ( UWorld.Vehicles )
                {
                    UWorld.Vehicles
                        .Where( veh => !veh.isExploded && !veh.isUnderwater)
                        .ToList()
                        .ForEach( vehicle => {
                            VehicleManager.sendVehicleFuel( vehicle, vehicle.asset.fuel );
                        });

                    EssLang.VEHICLE_REFUELED_ALL.SendTo( src );
                }
            }
        }
    }
}
