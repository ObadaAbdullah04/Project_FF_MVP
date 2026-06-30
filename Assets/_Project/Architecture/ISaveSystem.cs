namespace Project.Architecture
{
    using System;
    using Project.Data;

    public interface ISaveSystem
    {
        void Initialize(Action onComplete);
        void SaveInventory(InventoryData inventory, Action onComplete);
        void LoadInventory(InventoryData inventory, Action onComplete);
        void SaveProgress(ChildProgressData progress, Action onComplete);
        void LoadProgress(ChildProgressData progress, Action onComplete);
    }
}
