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
using Essentials.Api.Command;
using Essentials.Api.Command.Source;
using Essentials.I18n;
using Essentials.InternalModules.Kit.Item;
using static Essentials.Common.Util.ItemUtil;
using SDG.Unturned;

namespace Essentials.InternalModules.Kit.Commands
{
    [CommandInfo(
        Name = "createkit",
        Aliases = new [] {"ckit"},
        Description = "",
        Usage = "[name] [cooldown] [resetCooldownWhenDie[true or false]]",
        AllowedSource = AllowedSource.PLAYER
    )]
    public class CommandCreateKit : EssCommand
    {
        public override void OnExecute( ICommandSource src, ICommandArgs args )
        {
            var player = src.ToPlayer();

            if ( args.Length != 3 )
            {
                ShowUsage( src );
                return;
            }

            var name = args[0].ToString();
            uint cooldown;
            bool resetCooldownWhenDie;

            if ( !args[1].IsInt )
            {
                EssLang.INVALID_BOOLEAN.SendTo( src, args[1] );
                return;
            }

            if ( args[1].ToInt < 0 )
            {
                EssLang.MUST_POSITIVE.SendTo( src );
                return;
            }

            cooldown = args[1].ToUint;

            if ( !args[2].IsBool )
            {
                EssLang.INVALID_BOOLEAN.SendTo( src, args[2] );
                return;
            }

            resetCooldownWhenDie = args[2].ToBool;

            var inventory = player.Inventory;
            var clothing = player.Clothing;
            var items = new List<AbstractKitItem>();

            Action<byte> addItemsFromPage = page =>
            {
                var count = inventory.getItemCount( page );

                for ( byte index = 0; index < count; index++ )
                {
                    var item = inventory.getItem( page, index ).item;

                    if ( item == null )
                    {
                        continue;
                    }

                    var asset = GetItem( item.id ).Value;
                    KitItem kitItem;

                    if ( asset is ItemWeaponAsset )
                    {
                        var ammo = GetWeaponAmmo( item );
                        var firemode = GetWeaponFiremode( item ).OrElse( EFiremode.SAFETY );

                        var kItem = new KitItemWeapon( item.id, item.Durability, 1, ammo, firemode )
                        {
                            Magazine = GetWeaponAttachment( item, AttachmentType.MAGAZINE ).OrElse( null ),
                            Barrel = GetWeaponAttachment( item, AttachmentType.BARREL ).OrElse( null ),
                            Sight = GetWeaponAttachment( item, AttachmentType.SIGHT ).OrElse( null ),
                            Grip = GetWeaponAttachment( item, AttachmentType.GRIP ).OrElse( null ),
                            Tactical = GetWeaponAttachment( item, AttachmentType.TACTICAL ).OrElse( null )
                        };

                        kitItem = kItem;
                    }
                    else if ( asset is ItemMagazineAsset || asset is ItemSupplyAsset )
                    {
                        kitItem = new KitItemMagazine( item.id, item.Durability, 1, item.Amount );
                    }
                    else
                    {
                        kitItem = new KitItem( item.id, item.Durability, 1 );
                    }

                    kitItem.Metadata = item.Metadata;

                    items.Add( kitItem );
                }
            };

            addItemsFromPage( 0 ); // Primary slot
            addItemsFromPage( 1 ); // Secondary slot
            addItemsFromPage( 2 ); // Hands

            // Backpack

            if ( clothing.backpack != 0 )
            {
                items.Add( new KitItem( clothing.backpack, clothing.backpackQuality, 1 ) {
                    Metadata = clothing.backpackState
                } );
            }

            addItemsFromPage( 3 );

            // End Backpack

            // Shirt

            if ( clothing.shirt != 0 )
            {
                items.Add( new KitItem( clothing.shirt, clothing.shirtQuality, 1 ) {
                    Metadata = clothing.shirtState
                } );
            }

            addItemsFromPage( 5 );

            // End Shirt

            // Vest

            if ( clothing.vest != 0 )
            {
                items.Add( new KitItem( clothing.vest, clothing.vestQuality, 1 ) {
                    Metadata = clothing.vestState
                } );
            }

            addItemsFromPage( 4 );

            // End Vest

            // Pants

            if ( clothing.pants != 0 )
            {
                items.Add( new KitItem( clothing.pants, clothing.pantsQuality, 1 ) {
                    Metadata = clothing.pantsState
                } );
            }

            addItemsFromPage( 6 );

            // End Pants

            // Mask & Hat

            if ( clothing.mask != 0 )
            {
                items.Add( new KitItem( clothing.mask, clothing.maskQuality, 1 ) {
                    Metadata = clothing.maskState
                } );
            }

            if ( clothing.hat != 0 )
            {
                items.Add( new KitItem( clothing.hat, clothing.hatQuality, 1 ) {
                    Metadata = clothing.hatState
                } );
            }

            // End Mask & Hat

            var kit = new Kit( name, cooldown, resetCooldownWhenDie ) {
                Items = items
            };

            KitModule.Instance.KitManager.Add( kit );
            EssLang.CREATED_KIT.SendTo( src, name );
        } 
    }
}