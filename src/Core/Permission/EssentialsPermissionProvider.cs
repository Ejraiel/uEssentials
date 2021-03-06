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

using System;
using System.Collections.Generic;
using Essentials.Api;
using Essentials.Commands;
using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Permissions;

namespace Essentials.Core.Permission
{
    internal class EssentialsPermissionsProvider : IRocketPermissionsProvider
    {
        private readonly IRocketPermissionsProvider _defaultProvider;

        internal EssentialsPermissionsProvider()
        {
            _defaultProvider = R.Instance.GetComponent<RocketPermissionsManager>();
        }

        internal EssentialsPermissionsProvider( IRocketPermissionsProvider _defaultProvider )
        {
            this._defaultProvider = _defaultProvider;
        }

        public bool HasPermission( IRocketPlayer player, string perm, bool defaultReturnValue = false )
        {
            if ( perm.Contains( "." ) && _defaultProvider.HasPermission( player,
                        perm.Substring( 0, perm.LastIndexOf( ".", StringComparison.Ordinal ) ) + ".*" ) )
            {
                return true;
            }

            return _defaultProvider.HasPermission( player, perm, defaultReturnValue );
        }

        public bool HasPermission( IRocketPlayer player, string perm, out uint? cooldownLeft, bool defaultReturnValue = false )
        {
            var essCommand = EssProvider.CommandManager.GetByName( perm );

            if ( essCommand is CommandEssentials )
            {
                cooldownLeft = 0;
                return true;
            }

            if ( essCommand != null )
            {
                perm = essCommand.Permission;
            }

            return _defaultProvider.HasPermission( player, perm, out cooldownLeft, defaultReturnValue );
        }

        public List<RocketPermissionsGroup> GetGroups( IRocketPlayer player, bool includeParentGroups )
        {
            return _defaultProvider.GetGroups( player, includeParentGroups );
        }

        public List<Rocket.API.Serialisation.Permission> GetPermissions( IRocketPlayer player )
        {
            return _defaultProvider.GetPermissions( player );
        }

        public bool SetGroup( IRocketPlayer player, string groupID )
        {
            return _defaultProvider.SetGroup( player, groupID );
        }

        public void Reload()
        {
            _defaultProvider.Reload();
        }
    }
}