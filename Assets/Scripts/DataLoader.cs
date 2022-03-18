using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nevergreen
{
    public class DataLoader : PersistantSingleton<DataLoader>
    {
        public BlockDatabase Database;

        protected override void Awake()
        {
            base.Awake();
            
            LoadOfficialData();
            
            //UnityEditor.AssetDatabase.CreateAsset(Database.BlockDataLookup[1].Tile, "Assets/DebugTile.asset");
        }

        public void LoadOfficialData()
        {
            if (Database.BlockDataLookup == null)
            {
                Database.BlockDataLookup = new Dictionary<uint, BlockData>();
            }
            
            // Load all the blocks
            foreach (BlockData blockData in LoadHelper.LoadOfficialBlockData())
            {
                Database.BlockDataLookup.Add(blockData.ID, blockData);
            }
        }

        private void OnDestroy()
        {
            //UnityEditor.AssetDatabase.DeleteAsset("Assets/DebugTile.asset");
        }
    }
}