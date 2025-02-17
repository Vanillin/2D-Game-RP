using System.Collections.Generic;

namespace TwoD_Game_RP
{
    internal interface IStorageItem
    {
        CustomDictionary<Item, List<GamePoint>> ReferenceItem { get; }
        int MaxSizeH { get; }
        int MaxSizeW { get; }
        Item SearchItem(int H, int W);
        bool Add(Item item);
        GamePoint Remove(Item item);
        bool Contains(Item item);
        //private static bool CheckSquareInInventory(int x1, int y1, int x2, int y2, bool[,] Contein);
        //private static List<Item> ToListItem(CustomDictionary<Item, List<GamePoint>> dict);
        //private static bool IsInclude(int sizeH, int sizeW, CustomDictionary<Item, List<GamePoint>> oldReference, out CustomDictionary<Item, List<GamePoint>> newReference);
        //private static bool IsInclude(int sizeH, int sizeW, List<Item> list, out CustomDictionary<Item, List<GamePoint>> newReference);
    }
    internal class Backpack : IStorageItem
    {
        int _maxSizeH;
        int _maxSizeW;
        CustomDictionary<Item, List<GamePoint>> _referenceItem;

        public CustomDictionary<Item, List<GamePoint>> ReferenceItem => _referenceItem;
        public int MaxSizeH => _maxSizeH;
        public int MaxSizeW => _maxSizeW;

        private Backpack(int maxH, int maxW, CustomDictionary<Item, List<GamePoint>> reference)
        {
            _maxSizeH = maxH;
            _maxSizeW = maxW;
            _referenceItem = reference;
        }
        private Backpack(int maxH, int maxW)
        {
            _maxSizeH = maxH;
            _maxSizeW = maxW;
            _referenceItem = new CustomDictionary<Item, List<GamePoint>>();
        }
        public static Backpack Creating(int maxH, int maxW, List<Item> list)
        {
            if (list == null || list.Count == 0)
            {
                return new Backpack(maxH, maxW);
            }
            if (IsInclude(maxH, maxW, list, out CustomDictionary<Item, List<GamePoint>> newReference))
            {
                return new Backpack(maxH, maxW, newReference);
            }

            throw new CustomException("Inventory not included all Items");
        }
        public Item SearchItem(int H, int W)
        {
            foreach (var pair in _referenceItem)
            {
                foreach (var point in pair.Value)
                {
                    if (point.X <= H && H < point.X + pair.Key.SizeH &&
                        point.Y <= W && W < point.Y + pair.Key.SizeW)
                    {
                        return pair.Key;
                    }
                }
            }
            return null;
        }
        public bool Add(Item item)
        {
            CustomDictionary<Item, List<GamePoint>> newReference = _referenceItem;
            if (newReference.ContainsKey(item))
            {
                newReference[item].Add(null);
            }
            else
            {
                newReference.Add(item, new List<GamePoint>() { null });
            }

            if (IsInclude(_maxSizeH, _maxSizeW, newReference, out newReference))
            {
                _referenceItem = newReference;
                return true;
            }
            else
            {
                return false;
            }
        }
        public GamePoint Remove(Item item)
        {
            GamePoint retur = null;
            foreach (var pair in _referenceItem)
            {
                if (pair.Key == item)
                {
                    retur = _referenceItem[pair.Key][0];
                    if (_referenceItem[pair.Key].Count == 1)
                    {
                        _referenceItem.Remove(pair.Key);
                    }
                    else
                    {
                        _referenceItem[pair.Key].RemoveAt(0);
                    }
                    break;
                }
            }
            return retur;
        }
        public bool Contains(Item item)
        {
            return _referenceItem.ContainsKey(item);
        }

        private static bool CheckSquareInInventory(int x1, int y1, int x2, int y2, bool[,] Contein)
        {
            for (int i = x1; i < x2; i++)
            {
                for (int j = y1; j < y2; j++)
                {
                    if (Contein[i, j]) { return false; }
                }
            }
            return true;
        }
        private static List<Item> ToListItem(CustomDictionary<Item, List<GamePoint>> dict)
        {
            List<Item> list = new List<Item>();
            foreach (var pair in dict)
            {
                for (int count = 0; count < pair.Value.Count; count++)
                {
                    list.Add(pair.Key);
                }
            }
            return list;
        }
        private static bool IsInclude(int sizeH, int sizeW, CustomDictionary<Item, List<GamePoint>> oldReference, out CustomDictionary<Item, List<GamePoint>> newReference)
        {
            return IsInclude(sizeH, sizeW, ToListItem(oldReference), out newReference);
        }
        private static bool IsInclude(int sizeH, int sizeW, List<Item> list, out CustomDictionary<Item, List<GamePoint>> newReference)
        {
            list.Sort();
            CustomDictionary<Item, List<GamePoint>> save = new CustomDictionary<Item, List<GamePoint>>();
            bool[,] contein = new bool[sizeH, sizeW];
            int[] indexContein = new int[sizeH];

            foreach (var item in list)
            {
                bool ItemIsAdd = false;
                for (int i = 0; i < sizeH - item.SizeH + 1; i++)
                {
                    if (ItemIsAdd)
                    {
                        while (indexContein[i] < sizeW && contein[i, indexContein[i]]) indexContein[i]++;
                        continue;
                    }
                    for (int j = indexContein[i]; j < sizeW - item.SizeW + 1; j++)
                    {
                        if (!ItemIsAdd && CheckSquareInInventory(i, j, i + item.SizeH, j + item.SizeW, contein))
                        {
                            for (int Item = i; Item < i + item.SizeH; Item++)
                            {
                                for (int jitem = j; jitem < j + item.SizeW; jitem++)
                                {
                                    contein[Item, jitem] = true;
                                }
                            }
                            if (save.ContainsKey(item)) save[item].Add(new GamePoint(i, j));
                            else save.Add(item, new List<GamePoint> { new GamePoint(i, j) });
                            ItemIsAdd = true;
                            i--;
                            break;
                        }
                    }
                }
                newReference = null;
                if (!ItemIsAdd) return false;
            }
            newReference = save;
            return true;
        }
    }
}
