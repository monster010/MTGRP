﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;
using MongoDB.Driver;
using RoleplayServer.resources.core;
using RoleplayServer.resources.database_manager;
using RoleplayServer.resources.door_manager;
using RoleplayServer.resources.inventory;
using RoleplayServer.resources.player_manager;

namespace RoleplayServer.resources.property_system
{
    public class PropertyManager : Script
    {
        public static List<Property> Properties;

        public PropertyManager()
        {
            API.onResourceStart += API_onResourceStart;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onResourceStart()
        {
            Properties = DatabaseManager.PropertyTable.Find(FilterDefinition<Property>.Empty).ToList();
            foreach (var prop in Properties)
            {
                prop.CreateProperty();
            }
            API.consoleOutput("Created Properties.");
        }

        public enum PropertyTypes
        {
            Clothing,
            TwentyFourSeven,
            Hardware,
            Restaurent,
            Bank
        }

        #region ColShapeKnowing

        private void API_onEntityExitColShape(ColShape colshape, GTANetworkShared.NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player && colshape.hasData("property_enterance"))
            {
                if (API.getEntityData(entity, "at_interance_property_id") == colshape.getData("property_enterance"))
                {
                    API.resetEntityData(entity, "at_interance_property_id");
                }
            }

            if (API.getEntityType(entity) == EntityType.Player && colshape.hasData("property_interaction"))
            {
                if (API.getEntityData(entity, "at_interaction_property_id") == colshape.getData("property_interaction"))
                {
                    API.resetEntityData(entity, "at_interaction_property_id");
                }
            }

            if (API.getEntityType(entity) == EntityType.Player && colshape.hasData("property_exit"))
            {
                if (API.getEntityData(entity, "at_exit_property_id") == colshape.getData("property_exit"))
                {
                    API.resetEntityData(entity, "at_exit_property_id");
                }
            }
        }

        private void API_onEntityEnterColShape(ColShape colshape, GTANetworkShared.NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player && colshape.hasData("property_enterance"))
            {
                API.setEntityData(entity, "at_interance_property_id", colshape.getData("property_enterance"));
            }

            if (API.getEntityType(entity) == EntityType.Player && colshape.hasData("property_interaction"))
            {
                API.setEntityData(entity, "at_interaction_property_id", colshape.getData("property_interaction"));
            }

            if (API.getEntityType(entity) == EntityType.Player && colshape.hasData("property_exit"))
            {
                API.setEntityData(entity, "at_exit_property_id", colshape.getData("property_exit"));
            }
        }

        public static Property IsAtPropertyEnterance(Client player)
        {
            if (API.shared.hasEntityData(player, "at_interance_property_id"))
            {
                int id = API.shared.getEntityData(player, "at_interance_property_id");
                var property = Properties.SingleOrDefault(x => x.Id == id);
                return property;
            }
            return null;
        }

        public static Property IsAtPropertyExit(Client player)
        {
            if (API.shared.hasEntityData(player, "at_exit_property_id"))
            {
                int id = API.shared.getEntityData(player, "at_exit_property_id");
                var property = Properties.SingleOrDefault(x => x.Id == id);
                return property;
            }
            return null;
        }

        public static Property IsAtPropertyInteraction(Client player)
        {
            if (API.shared.hasEntityData(player, "at_interaction_property_id"))
            {
                int id = API.shared.getEntityData(player, "at_interaction_property_id");
                var property = Properties.SingleOrDefault(x => x.Id == id);
                return property;
            }
            return null;
        }

        #endregion

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                case "editproperty_setname":
                    if (sender.GetAccount().AdminLevel >= 5)
                    {
                        var id = Convert.ToInt32(arguments[0]);
                        var prop = Properties.SingleOrDefault(x => x.Id == id);
                        if (prop == null)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Property Id.");
                            return;
                        }
                        prop.PropertyName = (string) arguments[1];
                        prop.Save();
                        prop.UpdateMarkers();
                        API.sendChatMessageToPlayer(sender,
                            $"[Property Manager] Name of Property #{id} was changed to: '{arguments[1]}'");
                    }
                    break;

                case "editproperty_settype":
                    if (sender.GetAccount().AdminLevel >= 5)
                    {
                        var id = Convert.ToInt32(arguments[0]);
                        var prop = Properties.SingleOrDefault(x => x.Id == id);
                        if (prop == null)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Property Id.");
                            return;
                        }
                        PropertyTypes type;
                        if (Enum.TryParse((string) arguments[1], out type))
                        {
                            prop.Type = type;
                            ItemManager.SetDefaultPrices(prop);
                            prop.Save();
                            prop.UpdateMarkers();
                            API.sendChatMessageToPlayer(sender,
                                $"[Property Manager] Type of Property #{id} was changed to: '{prop.Type}'");
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Type Entered.");
                        }
                    }
                    break;

                case "editproperty_setsupplies":
                    if (sender.GetAccount().AdminLevel >= 5)
                    {
                        var id = Convert.ToInt32(arguments[0]);
                        var prop = Properties.SingleOrDefault(x => x.Id == id);
                        if (prop == null)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Property Id.");
                            return;
                        }
                        int sup;
                        if (int.TryParse((string) arguments[1], out sup))
                        {
                            prop.Supplies = sup;
                            prop.Save();
                            API.sendChatMessageToPlayer(sender,
                                $"[Property Manager] Supplies of Property #{id} was changed to: '{sup}'");
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Supplies Entered.");
                        }
                    }
                    break;

                case "editproperty_setenterancepos":
                    if (sender.GetAccount().AdminLevel >= 5)
                    {
                        var id = Convert.ToInt32(arguments[0]);
                        var prop = Properties.SingleOrDefault(x => x.Id == id);
                        if (prop == null)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Property Id.");
                            return;
                        }
                        prop.EnterancePos = sender.position;
                        prop.EnteranceRot = sender.rotation;
                        prop.Save();
                        prop.UpdateMarkers();
                        API.sendChatMessageToPlayer(sender,
                            $"[Property Manager] Enterance position of property #{id} was changed.");
                    }
                    break;

                case "editproperty_gotoenterance":
                    if (sender.GetAccount().AdminLevel >= 5)
                    {
                        var id = Convert.ToInt32(arguments[0]);
                        var prop = Properties.SingleOrDefault(x => x.Id == id);
                        if (prop == null)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Property Id.");
                            return;
                        }
                        sender.position = prop.EnterancePos;
                        sender.rotation = prop.EnteranceRot;
                        sender.dimension = 0;
                    }
                    break;

                case "editproperty_setmaindoor":
                    if (sender.GetAccount().AdminLevel >= 5)
                    {
                        var id = Convert.ToInt32(arguments[0]);
                        var prop = Properties.SingleOrDefault(x => x.Id == id);
                        if (prop == null)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Property Id.");
                            return;
                        }
                        int doorid;
                        if (int.TryParse((string) arguments[1], out doorid))
                        {
                            if (Door.Doors.Exists(x => x.Id == doorid))
                            {
                                prop.MainDoorId = doorid;
                                prop.Save();
                                API.sendChatMessageToPlayer(sender,
                                    $"[Property Manager] Main Door of Property #{id} was changed to: '{doorid}'");
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid DoorId Entered.");
                            }
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid DoorId Entered.");
                        }
                    }
                    break;

                case "editproperty_toggleteleportable":
                    if (sender.GetAccount().AdminLevel >= 5)
                    {
                        var id = Convert.ToInt32(arguments[0]);
                        var prop = Properties.SingleOrDefault(x => x.Id == id);
                        if (prop == null)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Property Id.");
                            return;
                        }
                        prop.IsTeleportable = !prop.IsTeleportable;
                        prop.Save();
                        API.sendChatMessageToPlayer(sender,
                            $"[Property Manager] Property #{id} was made to be '" +
                            (prop.IsTeleportable ? "Teleportable" : "UnTeleportable") + "'");
                    }
                    break;

                case "editproperty_setteleportpos":
                    if (sender.GetAccount().AdminLevel >= 5)
                    {
                        var id = Convert.ToInt32(arguments[0]);
                        var prop = Properties.SingleOrDefault(x => x.Id == id);
                        if (prop == null)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Property Id.");
                            return;
                        }
                        if (!prop.IsTeleportable)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Property isn't teleportable.");
                            return;
                        }
                        prop.TargetPos = sender.position;
                        prop.TargetRot = sender.rotation;
                        prop.TargetDimension = sender.dimension;
                        prop.Save();
                        API.sendChatMessageToPlayer(sender,
                            $"[Property Manager] Interior TP position of property #{id} was changed.");
                    }
                    break;

                case "editproperty_toggleinteractable":
                    if (sender.GetAccount().AdminLevel >= 5)
                    {
                        var id = Convert.ToInt32(arguments[0]);
                        var prop = Properties.SingleOrDefault(x => x.Id == id);
                        if (prop == null)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Property Id.");
                            return;
                        }
                        prop.IsInteractable = !prop.IsInteractable;
                        prop.Save();
                        API.sendChatMessageToPlayer(sender,
                            $"[Property Manager] Property #{id} was made to be '" +
                            (prop.IsInteractable ? "Interactable" : "UnInteractable") + "'");
                    }
                    break;

                case "editproperty_setinteractpos":
                    if (sender.GetAccount().AdminLevel >= 5)
                    {
                        var id = Convert.ToInt32(arguments[0]);
                        var prop = Properties.SingleOrDefault(x => x.Id == id);
                        if (prop == null)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Property Id.");
                            return;
                        }
                        if (!prop.IsInteractable)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Property isn't interactable.");
                            return;
                        }
                        prop.InteractionPos = sender.position;
                        prop.InteractionRot = sender.rotation;
                        prop.InteractionDimension = sender.dimension;
                        prop.UpdateMarkers();
                        prop.Save();
                        API.sendChatMessageToPlayer(sender,
                            $"[Property Manager] Interaction position of property #{id} was changed.");
                    }
                    break;

                case "editproperty_togglelock":
                    if (sender.GetAccount().AdminLevel >= 5)
                    {
                        var id = Convert.ToInt32(arguments[0]);
                        var prop = Properties.SingleOrDefault(x => x.Id == id);
                        if (prop == null)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Property Id.");
                            return;
                        }
                        prop.IsLocked = !prop.IsLocked;
                        prop.UpdateLockStatus();
                        prop.Save();
                        API.sendChatMessageToPlayer(sender,
                            $"[Property Manager] Property #{id} was made to be '" +
                            (prop.IsLocked ? "Locked" : "UnLocked") + "'");
                    }
                    break;

                case "editproperty_deleteproperty":
                    if (sender.GetAccount().AdminLevel >= 5)
                    {
                        var id = Convert.ToInt32(arguments[0]);
                        var prop = Properties.SingleOrDefault(x => x.Id == id);
                        if (prop == null)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Property Id.");
                            return;
                        }
                        prop.Delete();
                        API.sendChatMessageToPlayer(sender, $"[Property Manager] Property #{id} was deleted.");
                    }
                    break;

                case "editproperty_setprice":
                    if (sender.GetAccount().AdminLevel >= 5)
                    {
                        var id = Convert.ToInt32(arguments[0]);
                        var prop = Properties.SingleOrDefault(x => x.Id == id);
                        if (prop == null)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Property Id.");
                            return;
                        }
                        int price;
                        if (int.TryParse((string) arguments[1], out price))
                        {
                            prop.PropertyPrice = price;
                            prop.Save();
                            prop.UpdateMarkers();
                            API.sendChatMessageToPlayer(sender,
                                $"[Property Manager] Price of Property #{id} was changed to: '{price}'");
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Price Entered.");
                        }
                    }
                    break;

                case "editproperty_setowner":
                    if (sender.GetAccount().AdminLevel >= 5)
                    {
                        var id = Convert.ToInt32(arguments[0]);
                        var prop = Properties.SingleOrDefault(x => x.Id == id);
                        if (prop == null)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Property Id.");
                            return;
                        }
                        var player = PlayerManager.ParseClient((string) arguments[1]);
                        if (player == null)
                        {
                            API.sendChatMessageToPlayer(sender, "[Property Manager] Invalid Player Entered.");
                            return;
                        }
                        prop.OwnerId = player.GetCharacter().Id;
                        prop.Save();
                        prop.UpdateMarkers();
                        API.sendChatMessageToPlayer(sender,
                            $"[Property Manager] Owner of Property #{id} was changed to: '{player.GetCharacter().CharacterName}'");
                    }
                    break;
            }
        }

        public static string GetInteractText(PropertyTypes type)
        {
            switch (type)
            {
                case PropertyTypes.Clothing:
                    return "/buyclothes";
                case PropertyTypes.TwentyFourSeven:
                    return "/buy";
                case PropertyTypes.Hardware:
                    return "/buy";
            }
            return "";
        }

        [Command("enter")]
        public void Enterproperty(Client player)
        {
            var prop = IsAtPropertyEnterance(player);
            if (prop != null)
            {
                if (prop.IsTeleportable && (!prop.IsLocked || prop.OwnerId == player.GetCharacter().Id))
                {
                    player.position = prop.TargetPos;
                    player.rotation = prop.TargetRot;
                    player.dimension = prop.TargetDimension;
                }
                else
                {
                    API.sendNotificationToPlayer(player,
                        prop.IsLocked ? "Property is locked." : "Property is not teleportable.");
                }
            }
        }

        [Command("exit")]
        public void Exitproperty(Client player)
        {
            var prop = IsAtPropertyExit(player);
            if (prop != null)
            {
                if (prop.IsTeleportable && (!prop.IsLocked || prop.OwnerId == player.GetCharacter().Id))
                {
                    player.position = prop.EnterancePos;
                    player.rotation = prop.EnteranceRot;
                    player.dimension = 0;
                }
                else
                {
                    API.sendNotificationToPlayer(player,
                        prop.IsLocked ? "Property is locked." : "Property is not teleportable.");
                }
            }
        }

        [Command("manageprices")]
        public void Manageprices(Client player, string item = "", int price = 0)
        {
            var prop = IsAtPropertyEnterance(player) ?? IsAtPropertyInteraction(player);
            if (prop == null)
            {
                API.sendChatMessageToPlayer(player, "You aren't at an enteraction point or enterance.");
                return;
            }

            if (prop.OwnerId == player.GetCharacter().Id)
            {
                switch (prop.Type)
                {
                    case PropertyTypes.Clothing:
                        if (item == "")
                        {                                                              //0    ,1    ,2          ,3          ,4   ,5   ,6      ,7
                            API.sendChatMessageToPlayer(player, "[ERROR] Choose a type: [Pants,Shoes,Accessories,Undershirts,Tops,Hats,Glasses,Earrings]");
                            return;
                        }
                        if (price == 0)
                        {
                            API.sendChatMessageToPlayer(player, "[ERROR] Price can't be zero.");
                            return;
                        }

                        switch (item.ToLower())
                        {
                            case "pants":
                                prop.ItemPrices["0"] = price;
                                API.sendChatMessageToPlayer(player, $"Changed ~g~Pants~w~ price to {price}");
                                break;
                            case "shoes":
                                prop.ItemPrices["1"] = price;
                                API.sendChatMessageToPlayer(player, $"Changed ~g~Shoes~w~ price to {price}");
                                break;
                            case "accessories":
                                prop.ItemPrices["2"] = price;
                                API.sendChatMessageToPlayer(player, $"Changed ~g~Accessories~w~ price to {price}");
                                break;
                            case "undershirts":
                                prop.ItemPrices["3"] = price;
                                API.sendChatMessageToPlayer(player, $"Changed ~g~Undershirts~w~ price to {price}");
                                break;
                            case "tops":
                                prop.ItemPrices["4"] = price;
                                API.sendChatMessageToPlayer(player, $"Changed ~g~Tops~w~ price to {price}");
                                break;
                            case "hats":
                                prop.ItemPrices["5"] = price;
                                API.sendChatMessageToPlayer(player, $"Changed ~g~Hats~w~ price to {price}");
                                break;
                            case "glasses":
                                prop.ItemPrices["6"] = price;
                                API.sendChatMessageToPlayer(player, $"Changed ~g~Glasses~w~ price to {price}");
                                break;
                            case "earrings":
                                prop.ItemPrices["7"] = price;
                                API.sendChatMessageToPlayer(player, $"Changed ~g~Earrings~w~ price to {price}");
                                break;
                        }
                        break;
                }
            }
            else
            {
                API.sendChatMessageToPlayer(player, "You don't own that property.");
            }
        }

        [Command("lockproperty")]
        public void LockProperty(Client player)
        {
            var prop = IsAtPropertyEnterance(player) ?? IsAtPropertyInteraction(player);
            if (prop == null)
            {
                API.sendChatMessageToPlayer(player, "You aren't at an enteraction point or enterance.");
                return;
            }

            if (prop.OwnerId == player.GetCharacter().Id)
            {
                prop.IsLocked = !prop.IsLocked;
                prop.UpdateLockStatus();
                API.sendNotificationToPlayer(player,
                    prop.IsLocked ? "Property has been ~g~locked." : "Property has been ~r~unlocked.");
            }
            else
            {
                API.sendChatMessageToPlayer(player, "You don't own that property.");
            }
        }

        [Command("propertyname", GreedyArg = true)]
        public void PropertyName(Client player, string name)
        {
            var prop = IsAtPropertyEnterance(player) ?? IsAtPropertyInteraction(player);
            if (prop == null)
            {
                API.sendChatMessageToPlayer(player, "You aren't at an enteraction point or enterance.");
                return;
            }

            if (prop.OwnerId == player.GetCharacter().Id)
            {
                prop.PropertyName = name;
                prop.UpdateMarkers();
                API.sendNotificationToPlayer(player, "Property name has been changed.");
            }
            else
            {
                API.sendChatMessageToPlayer(player, "You don't own that property.");
            }
        }

        [Command("propertystorage")]
        public void PropertyStorage(Client player)
        {
            var prop = IsAtPropertyInteraction(player);
            if (prop == null)
            {
                API.sendChatMessageToPlayer(player, "You aren't at an enteraction point.");
                return;
            }

            InventoryManager.ShowInventoryManager(player, player.GetCharacter(), prop, "Inventory: ", "Property: ");
        }

        [Command("createproperty")]
        public void create_property(Client player, PropertyTypes type)
        {
            var account = player.GetAccount();
            if (account.AdminLevel >= 5)
            {
                var property = new Property(type, player.position, player.rotation, type.ToString());
                ItemManager.SetDefaultPrices(property);
                property.Insert();
                property.CreateProperty();
                Properties.Add(property);
                API.sendChatMessageToPlayer(player, "You have sucessfully create a property of type " + type.ToString());
            } 
        }

        [Command("editproperty")]
        public void edit_property(Client player, int id)
        {
            var account = player.GetAccount();
            if (account.AdminLevel >= 5)
            {
                var prop = Properties.SingleOrDefault(x => x.Id == id);
                if (prop == null)
                {
                    API.sendChatMessageToPlayer(player, "Invalid Property Id.");
                    return;
                }
                API.triggerClientEvent(player, "editproperty_showmenu", prop.Id);
            }
        }
    }
}
