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

using System.IO;
using Steamworks;
using System;
using Essentials.Api.Command;
using Essentials.Api.Command.Source;
using Essentials.I18n;

namespace Essentials.Commands
{
    [CommandInfo(
        Name = "resetplayer",
        Description = "Reset all player data.",
        Usage = "[player/playerid]"
    )]
    public class CommandResetPlayer : EssCommand
    {
        private static readonly Action<ulong> ResetPlayer = steamId =>
        {
            foreach ( var dic in Directory.GetDirectories(Directory.GetParent( 
                    Directory.GetCurrentDirectory() ).ToString() + "\\Players\\" ) )
            {
                if ( dic.Substring( dic.LastIndexOf( "\\", StringComparison.Ordinal ) + 1 ).StartsWith( steamId.ToString() ) )
                {
                    Directory.Delete( dic, true );
                }
            }
        };

        public override void OnExecute(ICommandSource source, ICommandArgs parameters)
        {
            if ( parameters.IsEmpty || parameters.Length > 1 )
            {
                source.SendMessage( "Use /resetplayer [player/playerid]" );
            }
            else
            {
                try
                {
                    var steamId = new CSteamID( ulong.Parse( parameters[0].ToString() ) );

                    if ( !steamId.IsValid() )
                    {
                        EssLang.INVALID_STEAMID.SendTo( source, steamId.m_SteamID);
                    }
                    else
                    {
                        ResetPlayer( steamId.m_SteamID );
                        EssLang.PLAYER_RESET.SendTo( source );
                    }
                }
                catch (FormatException)
                {
                    var target = parameters[0].ToPlayer;

                    if ( target == null )
                    {
                        EssLang.PLAYER_NOT_FOUND.SendTo( source, parameters[0] );
                    }
                    else
                    {
                        target.Kick( EssLang.PLAYER_RESET_KICK.GetMessage() );
                        ResetPlayer( target.CSteamId.m_SteamID );
                        EssLang.PLAYER_RESET.SendTo( source );
                    }
                }
            }
        }
    }
}
