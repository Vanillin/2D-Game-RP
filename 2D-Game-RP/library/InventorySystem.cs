using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace TwoD_Game_RP
{
    public class Inventory
    {
        int maxSizeH;
        int maxSizeW;
        DictionaryWithEqual<Item, List<GamePoint>> referenceItem;
        public DictionaryWithEqual<Item, List<GamePoint>> ReferenceItem => referenceItem;
        public int MaxSizeH => maxSizeH;
        public int MaxSizeW => maxSizeW;
        private Inventory(int maxH, int maxW, DictionaryWithEqual<Item, List<GamePoint>> reference)
        {
            this.maxSizeH = maxH;
            this.maxSizeW = maxW;
            this.referenceItem = reference;
        }
        private Inventory(int maxH, int maxW)
        {
            this.maxSizeH = maxH;
            this.maxSizeW = maxW;
            this.referenceItem = new DictionaryWithEqual<Item, List<GamePoint>>();
        }
        public Item SearchItem(int H, int W)
        {
            foreach (var pair in referenceItem)
            {
                foreach (var point in  pair.Value)
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
        public static Inventory Creating(int maxH, int maxW, List<Item> list)
        {
            if (list.Count == 0)
            {
                return new Inventory(maxH, maxW);
            }
            if (Is_Include(maxH, maxW, list, out DictionaryWithEqual<Item, List<GamePoint>> newReference))
            {
                return new Inventory(maxH, maxW, newReference);
            }

            throw new Exception("Инвентарь не вмещает в себя столько предметов");
        }
        public bool Add(Item item)
        {
            DictionaryWithEqual<Item, List<GamePoint>> newReference = referenceItem;
            if (newReference.ContainsKey(item))
            {
                newReference[item].Add(null);
            }
            else
            {
                newReference.Add(item, new List<GamePoint>() { null });
            }

            if (Is_Include(maxSizeH, maxSizeW, newReference, out newReference))
            {
                referenceItem = newReference;
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Remove(Item item)
        {
            if (!referenceItem.ContainsKey(item))
            {
                return;
            }
            else if (referenceItem[item].Count == 1)
            {
                referenceItem.Remove(item);
            }
            else
            {
                referenceItem[item].RemoveAt(0);
            }
            Is_Include(maxSizeH, maxSizeW, referenceItem, out referenceItem);
        }
        public bool Contains(Item item)
        {
            return referenceItem.ContainsKey(item);
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
        private static bool Is_Include(int sizeH, int sizeW, DictionaryWithEqual<Item, List<GamePoint>> oldReference, out DictionaryWithEqual<Item, List<GamePoint>> newReference)
        {
            DictionaryWithEqual<Item, List<GamePoint>> save = new DictionaryWithEqual<Item, List<GamePoint>>();
            bool[,] Contein = new bool[sizeH, sizeW];
            int[] IndexContein = new int[sizeH];

            foreach (var pair in oldReference)
            {
                for (int count = 0; count < pair.Value.Count; count++)
                {
                    bool ItemIsAdd = false;
                    for (int i = 0; i < sizeH - pair.Key.SizeH + 1; i++)
                    {
                        if (ItemIsAdd)
                        {
                            while (IndexContein[i] < sizeW && Contein[i, IndexContein[i]]) IndexContein[i]++;
                            continue;
                        }
                        for (int j = IndexContein[i]; j < sizeW - pair.Key.SizeW + 1; j++)
                        {
                            if (!ItemIsAdd && CheckSquareInInventory(i, j, i + pair.Key.SizeH, j + pair.Key.SizeW, Contein))
                            {
                                for (int Item = i; Item < i + pair.Key.SizeH; Item++)
                                {
                                    for (int jitem = j; jitem < j + pair.Key.SizeW; jitem++)
                                    {
                                        Contein[Item, jitem] = true;
                                    }
                                }
                                if (save.ContainsKey(pair.Key)) save[pair.Key].Add(new GamePoint(i, j));
                                else save.Add(pair.Key, new List<GamePoint> { new GamePoint(i, j) });
                                ItemIsAdd = true;
                                i--;
                                break;
                            }
                        }
                    }
                    newReference = null;
                    if (!ItemIsAdd) return false;
                }                
            }
            newReference = save;
            return true;
        }
        private static bool Is_Include(int sizeH, int sizeW, List<Item> list, out DictionaryWithEqual<Item, List<GamePoint>> newReference)
        {
            DictionaryWithEqual<Item, List<GamePoint>> save = new DictionaryWithEqual<Item, List<GamePoint>>();
            bool[,] Contein = new bool[sizeH, sizeW];
            int[] IndexContein = new int[sizeH];
            list.Sort();

            foreach (var item in list)
            {
                bool ItemIsAdd = false;
                for (int i = 0; i < sizeH - item.SizeH + 1; i++)
                {
                    if (ItemIsAdd)
                    {
                        while (IndexContein[i] < sizeW && Contein[i, IndexContein[i]]) IndexContein[i]++;
                        continue;
                    }
                    for (int j = IndexContein[i]; j < sizeW - item.SizeW + 1; j++)
                    {
                        if (!ItemIsAdd && CheckSquareInInventory(i, j, i + item.SizeH, j + item.SizeW, Contein))
                        {
                            for (int Item = i; Item < i + item.SizeH; Item++)
                            {
                                for (int jitem = j; jitem < j + item.SizeW; jitem++)
                                {
                                    Contein[Item, jitem] = true;
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
