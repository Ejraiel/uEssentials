﻿/*
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

using Essentials.Api.Command;
using Essentials.Api.Command.Source;
using Essentials.I18n;

namespace Essentials.Commands
{
    [CommandInfo(
        Name = "ping",
        Description = "View your/another player ping",
        Usage = "<player>"
    )]
    public class CommandPing : EssCommand
    {
        public override void OnExecute( ICommandSource source, ICommandArgs parameters )
        {

            if ( parameters.IsEmpty || parameters.Length > 1 )
            {
                if ( source.IsConsole )
                    source.SendMessage( "Use /ping [player]" );
                else
                    EssLang.PING.SendTo( source, source.ToPlayer().Ping );
            }
            else
            {
                var target = parameters[0].ToPlayer;

                if ( target == null )
                {
                    EssLang.PLAYER_NOT_FOUND.SendTo( source, parameters[0] );
                }
                else
                {
                    EssLang.PING_OTHER.SendTo( source, target.DisplayName, target.Ping );
                }
            }
        }
    }
}
