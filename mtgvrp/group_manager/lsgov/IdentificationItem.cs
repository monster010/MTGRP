﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mtgvrp.inventory;
using mtgvrp.player_manager;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mtgvrp.group_manager.lsgov
{
    public class IdentificationItem : IInventoryItem
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int Amount { get; set; }

        public int AmountOfSlots => 1;

        public bool CanBeDropped => true;
        public bool CanBeGiven => false;
        public bool CanBeStacked => false;
        public bool CanBeStashed => false;
        public bool IsBlocking => false;
        public bool CanBeStored => false;

        public Dictionary<Type, int> MaxAmount { get; set; } = new Dictionary<Type, int>
        {
            {typeof(Character), 1}
        };

        public string CommandFriendlyName => $"id";

        public string LongName => $"ID";

        public int Object => 0;
    }
}
