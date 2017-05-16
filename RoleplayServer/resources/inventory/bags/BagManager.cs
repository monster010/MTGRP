﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using RoleplayServer.resources.core;
using RoleplayServer.resources.player_manager;

namespace RoleplayServer.resources.inventory.bags
{
    class BagManager : Script
    {
        public BagManager()
        {
            InventoryManager.OnStorageGetItem += InventoryManager_OnStorageGetItem;
            InventoryManager.OnStorageLoseItem += InventoryManager_OnStorageLoseItem;
        }

        private void InventoryManager_OnStorageLoseItem(IStorage sender, InventoryManager.OnLoseItemEventArgs args)
        {
            if (sender.GetType() == typeof(Character))
            {
                if (args.Item == typeof(BagItem))
                {
                    Character chr = (Character)sender;
                    API.setPlayerClothes(chr.Client, 5, 0, 0);
                }
            }
        }

        private void InventoryManager_OnStorageGetItem(IStorage sender, InventoryManager.OnGetItemEventArgs args)
        {
            if (sender.GetType() == typeof(Character))
            {
                if (args.Item.GetType() == typeof(BagItem))
                {
                    Character chr = (Character) sender;
                    BagItem item = (BagItem) args.Item;
                    API.setPlayerClothes(chr.Client, 5, item.BagType, item.BagDesign);
                }
            }
        }

        [Command("managebag")]
        public void Managebag(Client player)
        {
            Character character = player.GetCharacter();
            IInventoryItem[] bag = InventoryManager.DoesInventoryHaveItem(character, typeof(BagItem));
            if (bag.Length != 1)
            {
                API.sendNotificationToPlayer(player, "You don't have a bag.");
                return;
            }

            //Show the window.
            InventoryManager.ShowInventoryManager(player, character, (BagItem)bag[0], "Inventory: ", "Bag: ");
        }

        //TODO: test cmd.
        [Command("givemebag")]
        public void GiveMeBag(Client player, int type, int design)
        {
            API.sendChatMessageToPlayer(player, InventoryManager.GiveInventoryItem(player.GetCharacter(), new BagItem() { BagDesign = design, BagType = type }, 1, true).ToString());
        }
        [Command("setmyclothes")]
        public void setmyclothes(Client player, int slot, int drawable, int texture)
        {
            API.setPlayerClothes(player, slot, drawable, texture);
        }
    }
}
