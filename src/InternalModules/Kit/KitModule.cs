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

using Essentials.Api.Module;
using Essentials.InternalModules.Kit.Commands;
using static Essentials.Api.EssProvider;

namespace Essentials.InternalModules.Kit
{
    [ModuleInfo( Name = "Kits", Flags = ModuleFlags.NONE )]
    public class KitModule : InternalModule
    {
        public KitManager KitManager        { get; internal set; }
        public static KitModule Instance    { get; private set; }

        public override void OnLoad()
        {
            Instance = this;

            KitManager = new KitManager();
            KitManager.Load();
            
            Logger.LogInfo( $"Loaded {KitManager.Count} kits" );

            CommandManager.Register<CommandKit>();
            CommandManager.Register<CommandKits>();
            CommandManager.Register<CommandCreateKit>();
        }

        public override void OnUnload()
        {
            CommandManager.Unregister<CommandKit>();
            CommandManager.Unregister<CommandKits>();
            CommandManager.Unregister<CommandCreateKit>();
        }
    }
}
