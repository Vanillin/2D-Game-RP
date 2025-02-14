using System.Collections.Generic;
using System.Configuration;
using System.Windows.Controls;

namespace TwoD_Game_RP
{
    public abstract class StorageSkelet : SystemSkelet
    {
        private IPictureList[,] _memoryPicture;
        private IDisplayCanvas _inventoryDisplay;
        private IBoxElement _inventory;

        public bool AddInBackpack(Item item)
        {
            bool ok = _inventory.AddInBackpack(item);
            if (ok)
            {
                List<(double sizeh, double sizew)>[,] depth = new List<(double sizeh, double sizew)>[_inventory.MaxSizeH, _inventory.MaxSizeW];
                var items = _inventory.ReferenceItem;
                for (int i = 0; i < _inventory.MaxSizeH; i++)
                {
                    for (int j = 0;  j < _inventory.MaxSizeW; j++)
                    {
                        depth[i, j] = new List<(double sizeh, double sizew)>();
                        _memoryPicture[i, j].Clear();
                        _memoryPicture[i, j].AddCell(new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], "emptyitem.png")), 0);
                        depth[i, j].Add((1,1));
                    }
                }
                foreach (var pair in items)
                {
                    var points = pair.Value;
                    foreach (var point in points)
                    {
                        _memoryPicture[point.X, point.Y].AddCell(pair.Key.Picture, 1);
                        depth[point.X, point.Y].Add((pair.Key.SizeH, pair.Key.SizeW));
                    }
                }
                for (int i = 0; i < _inventory.MaxSizeH; i++)
                {
                    for (int j = 0; j < _inventory.MaxSizeW; j++)
                    {
                        UpdateCell(_memoryPicture[i, j], i, j, depth[i,j].ToArray());
                    }
                }
            }
            return ok;
        }
        public void RemoveInBackpack(Item item) => _inventory.RemoveInBackpack(item);
        public bool ContainsInBackpack(Item item) => _inventory.ContainsInBackpack(item);
        public Item SearchInBackpack(int h, int w) => _inventory.SearchInBackpack(h, w);
        public void DisplayInventory(Canvas canvas, double size) => _inventoryDisplay.Display(canvas, size);
        private void UpdateCell(IPictureList pictures, int indexh, int indexw, (double sizeh, double sizew)[] sizes) => _inventoryDisplay.UpdateCell(pictures, indexh, indexw, sizes);

        internal StorageSkelet(string systemName, IPicture picture, GamePoint point, bool isClarity, IMemoryAction memoryAction, IBoxElement boxElement)
            : base(systemName, picture, point, isClarity, memoryAction)
        {
            _memoryPicture = new MemoryPictureInventory[boxElement.MaxSizeH, boxElement.MaxSizeW];
            _inventoryDisplay = new MemoryImage(1, 1);
            _inventory = boxElement;
        }
        internal StorageSkelet(string systemNamePicture, GamePoint point, bool isClarity, IMemoryAction memoryAction, IBoxElement boxElement)
            : base(systemNamePicture, point, isClarity, memoryAction)
        {
            _memoryPicture = new MemoryPictureInventory[boxElement.MaxSizeH, boxElement.MaxSizeW];
            _inventoryDisplay = new MemoryImage(1, 1);
            _inventory = boxElement;
        }
    }
    public class BoxSkelet : StorageSkelet
    {
        public BoxSkelet(string systemNamePicture, GamePoint point, bool isClarity, int inventorySizeH, int inventorySizeW)
            : base(systemNamePicture, point, isClarity, new MemoryAction(), new Inventory(inventorySizeH, inventorySizeW, new List<Item>(0)))
        { }
        public BoxSkelet(string systemNamePicture, GamePoint point, bool isClarity, int inventorySizeH, int inventorySizeW, List<Item> items)
            : base(systemNamePicture, point, isClarity, new MemoryAction(), new Inventory(inventorySizeH, inventorySizeW, items))
        { }
    }
}
