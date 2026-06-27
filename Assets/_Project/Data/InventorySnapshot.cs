namespace Project.Data
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class InventorySnapshot
    {
        public int softCurrency;
        public List<string> unlockedBuildingIds = new List<string>();
        public List<string> unlockedChunkIds = new List<string>();

        public static InventorySnapshot FromInventory(InventoryData inventory)
        {
            return new InventorySnapshot
            {
                softCurrency = inventory.SoftCurrency,
                unlockedBuildingIds = new List<string>(inventory.UnlockedBuildingIds ?? new List<string>()),
                unlockedChunkIds = new List<string>(inventory.UnlockedChunkIds ?? new List<string>())
            };
        }

        public void ApplyTo(InventoryData inventory)
        {
            inventory.ResetData();
            if (softCurrency > 0) inventory.AddCurrency(softCurrency);
            if (unlockedBuildingIds != null)
                foreach (string id in unlockedBuildingIds)
                    inventory.UnlockBuilding(id);
            if (unlockedChunkIds != null)
                foreach (string id in unlockedChunkIds)
                    inventory.UnlockChunk(id);
        }
    }
}
