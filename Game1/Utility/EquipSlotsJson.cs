﻿using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components;
using Omniplatformer.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Utility
{
    class EquipSlotsJson
    {
        public static object ToJson(EquipSlotCollection obj)
        {
            return new
            {
                LeftHand = obj.LeftHandSlot.Item?.AsJson(),
                RightHand = obj.RightHandSlot.Item?.AsJson(),
                ChannelSlot = obj.ChannelSlot.Item?.AsJson(),
                MiscSlots = obj.MiscSlots.Select(x => x.Item)
                            .Where(x => x != null)
                            .Select(x => x.AsJson()).ToArray()
            };
        }

        public static EquipSlotCollection FromJson(JToken input, Deserializer deserializer)
        {
            var es = new EquipSlotCollection();
            es.LeftHandSlot.Item = (Item)deserializer.decodeToken(input["LeftHand"]);
            es.RightHandSlot.Item = (Item)deserializer.decodeToken(input["RightHand"]);
            es.ChannelSlot.Item = (Item)deserializer.decodeToken(input["ChannelSlot"]);
            foreach(var misc in (JArray)input["MiscSlots"])
            {
                es.MiscSlots.Add(new MiscSlot() { Item = (Item)deserializer.decodeObject((JObject)misc) });
            }
            return es;
        }
    }
}
