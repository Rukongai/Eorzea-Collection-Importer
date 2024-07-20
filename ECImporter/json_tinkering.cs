using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Lumina.Excel.GeneratedSheets;

namespace ECImporter
{
    public class ItemRow
    {
        public EquipSlotCategory EquipSlotCategory { get; set; }
        public int RowID { get; set; }
    }

    public class EquipSlotCategory
    {
        public int RawRow { get; set; }
    }

    public class DefaultDesign
    {
        public Equipment Equipment { get; set; }
    }

    public class Equipment
    {
        public Slot MainHand { get; set; }
        public Slot OffHand { get; set; }
        public Slot Head { get; set; }
        public Slot Body { get; set; }
        public Slot Hands { get; set; }
        public Slot Legs { get; set; }
        public Slot Feet { get; set; }
        public Slot Ears { get; set; }
        public Slot Neck { get; set; }
        public Slot Wrists { get; set; }
        public Slot RFinger { get; set; }
        public Slot LFinger { get; set; }
    }

    public class Slot
    {
        public int ItemID { get; set; }
    }

    public static class Tinker
    {
        private static readonly Dictionary<int, string> EquipSlotMap = new Dictionary<int, string>
        {
            { 1, "MainHand" },
            { 2, "OffHand" },
            { 3, "Head" },
            { 4, "Body" },
            { 5, "Hands" },
            { 7, "Legs" },
            { 8, "Feet" },
            { 9, "Ears" },
            { 10, "Neck" },
            { 11, "Wrists" },
            { 12, "LFinger" },
            { 13, "MainHand" }
        };

        public static void UpdateEquipmentSlot(JObject defaultDesign, Item itemRow)
        {
            if (!EquipSlotMap.TryGetValue((int)itemRow.EquipSlotCategory.RawRow.RowId, out string slotName))
            {
                Service.PluginLog.Info($"EquipSlotCategory {itemRow.EquipSlotCategory.RawRow} not recognized.");
                return;
            }

            var equipment = defaultDesign["Equipment"];
            if (equipment == null)
            {
                Service.PluginLog.Info("Equipment section not found in the JSON.");
                return;
            }

            var slot = equipment[slotName];
            if (slot == null)
            {
                Service.PluginLog.Info($"Slot {slotName} not found in Equipment.");
                return;
            }

            slot["ItemId"] = itemRow.RowId;
            Service.PluginLog.Info($"Updated {itemRow.EquipSlotCategory.RawRow.RowId}{slotName} ItemID to {itemRow.RowId}");
        }

        public static void UpdateDyeSlot(JObject defaultDesign, Item itemRow, Stain stainRow, int index)
        {
            if (!EquipSlotMap.TryGetValue((int)itemRow.EquipSlotCategory.RawRow.RowId, out string slotName))
            {
                Service.PluginLog.Info($"{stainRow} not recognized.");
                return;
            }

            var equipment = defaultDesign["Equipment"];
            if (equipment == null)
            {
                Service.PluginLog.Info("Equipment section not found in the JSON.");
                return;
            }

            var slot = equipment[slotName];
            if (slot == null)
            {
                Service.PluginLog.Info($"Slot {slotName} not found in Equipment.");
                return;
            }

            string stainKey = index == 0 ? "Stain" : "Stain2";
            slot[stainKey] = stainRow.RowId;
            Service.PluginLog.Info($"Updated {slotName} dye {index + 1} to {stainRow.RowId}");
        }
    }
}
