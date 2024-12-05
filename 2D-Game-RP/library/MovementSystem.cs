using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TwoD_Game_RP
{
    public class Vertex : IEnumerable
    {
        public GamePoint Cord;
        public List<Vertex> Near = new List<Vertex>();
        public Vertex(GamePoint point)
        {
            Cord = point;
        }
        private Vertex(GamePoint point, List<Vertex> near)
        {
            Cord = point;
            Near = near;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var point in Near)
            {
                yield return point;
            }
        }
    }
    public class Graf : IEnumerable
    {
        public List<Vertex> Vertexes = new List<Vertex>();
        public Graf()
        { }
        private Graf(List<Vertex> list)
        {
            Vertexes = list;
        }
        public IEnumerator GetEnumerator()
        {
            foreach (var vertex in Vertexes)
            {
                yield return vertex;
            }
        }
        public List<GamePoint> SearchWidth(GamePoint start, GamePoint finish)
        {
            return SearchWidthWithoutSomePoint(start, finish, new SortedEnum<GamePoint>());
        }
        public List<GamePoint> SearchWidthWithoutSomePoint(GamePoint start, GamePoint finish, SortedEnum<GamePoint> deletedpoints)
        {
            if (start == finish || /*deletedpoints.Contains(start) ||*/ deletedpoints.Contains(finish))
            {
                return null;
            }
            List<GamePoint> retur = new List<GamePoint>();
            Vertex startVert = Find(start);
            Vertex finishVert = Find(finish);

            if (startVert == null || finishVert == null)
            {
                throw new Exception($"В графе отсутствует точка ({startVert.Cord.X},{startVert.Cord.Y};{finishVert.Cord.X},{finishVert.Cord.Y})");
            }

            List<Vertex> PartStart = new List<Vertex>() { startVert };
            List<Vertex> PartFinish = new List<Vertex>() { finishVert };
            Dictionary<Vertex, Vertex> Path = new Dictionary<Vertex, Vertex>
            {
                { startVert, null },
                { finishVert, null }
            };
            Queue<Vertex> Que = new Queue<Vertex>();
            Que.Enqueue(startVert);
            Que.Enqueue(finishVert);
            while (Que.Count > 0)
            {
                Vertex Now = Que.Dequeue();
                foreach (Vertex v in Now.Near)
                {
                    if (deletedpoints.Contains(v.Cord))
                    {
                        continue;
                    }
                    if ((PartStart.Contains(Now) && PartFinish.Contains(v)) ||
                            (PartStart.Contains(v) && PartFinish.Contains(Now)))
                    {
                        Vertex save = v;
                        if (PartFinish.Contains(Now))
                        {
                            save = Now;
                            Now = v;
                        }
                        retur.Add(Now.Cord);
                        while (Now != null)
                        {
                            if (Path[Now] != null)
                                retur.Add(Path[Now].Cord);
                            Now = Path[Now];
                        }
                        retur.Reverse();

                        retur.Add(save.Cord);
                        while (save != null)
                        {
                            if (Path[save] != null)
                                retur.Add(Path[save].Cord);
                            save = Path[save];
                        }

                        retur.Remove(startVert.Cord);
                        return retur;
                    }
                    else if (!PartStart.Contains(v) && !PartFinish.Contains(v))
                    {
                        if (PartStart.Contains(Now))
                            PartStart.Add(v);
                        else
                            PartFinish.Add(v);
                        Path.Add(v, Now);
                        Que.Enqueue(v);
                    }
                }
            }
            return null;
        }        
        public SortedEnum<GamePoint> SearchSee(GamePoint start, int count)
        {
            //if (Find(start) == null)
            //{
            //    //throw new Exception($"В графе отсутствует точка ({start.X},{start.Y})");
            //    return new SortedEnum<GamePoint>() { start };
            //}
            //GamePoint startVert = Find(start).Cord;

            SortedEnum<GamePoint> retur = new SortedEnum<GamePoint>();

            Dictionary<GamePoint, int> Length = new Dictionary<GamePoint, int> { { start, 0 } };
            Queue<GamePoint> Que = new Queue<GamePoint>();
            Que.Enqueue(start);

            List<double[]> SavedObject = new List<double[]>(); //0 - max //1 - min
            bool KeyPI = true;

            while (Que.Count > 0)
            {
                GamePoint Now = Que.Dequeue();
                List<GamePoint> Near = new List<GamePoint>() {new GamePoint(Now.X+1, Now.Y), new GamePoint(Now.X, Now.Y+1),
                    new GamePoint(Now.X - 1, Now.Y), new GamePoint(Now.X, Now.Y-1) };
                foreach (GamePoint p in Near)
                {
                    if (!Length.ContainsKey(p))
                    {
                        if (Length[Now] + 1 > count)
                        {
                            return retur;
                        }

                        double CurX = p.X - start.X;
                        double CurY = p.Y - start.Y;
                        double NewAngle = Math.Atan2(CurX, CurY);

                        if (this.Vertexes.Find(X => X.Cord.CompareTo(p) == 0) != null)
                        {
                            bool RadianKey = true;
                            foreach (double[] o in SavedObject)
                            {
                                if (RadianKey)
                                {
                                    if (o[0] > NewAngle && NewAngle > o[1])
                                    {
                                        RadianKey = false;
                                    }
                                }
                            }
                            if (RadianKey) //добавляет все видимые точки
                            {
                                Length.Add(p, Length[Now] + 1);
                                retur.Add(p);
                                Que.Enqueue(p);
                            }
                        }
                        else
                        {
                            List<double> Angles = new List<double> { Math.Atan2(CurX +0.5, CurY +0.5), Math.Atan2(CurX +0.5, CurY -0.5),
                            Math.Atan2(CurX -0.5, CurY +0.5), Math.Atan2(CurX -0.5, CurY -0.5) };
                            double MaxNewAngle = Angles.Max();
                            double MinNewAngle = Angles.Min();

                            if (NewAngle == Math.PI)
                            {
                                if (KeyPI)
                                {
                                    SavedObject.Add(new double[2] { Math.Atan2(CurX - 0.5, CurY + 0.5), -3.2 });
                                    SavedObject.Add(new double[2] { 3.2, Math.Atan2(CurX + 0.5, CurY + 0.5) });
                                    KeyPI = false;
                                }
                                continue;
                            }

                            bool AddNewAngle = true;
                            foreach (double[] o in SavedObject)
                            {

                                if (AddNewAngle)
                                {
                                    bool MaxInside = (o[0] > MaxNewAngle && MaxNewAngle > o[1]);
                                    bool MinInside = (o[0] > MinNewAngle && MinNewAngle > o[1]);

                                    if (MaxInside && MinInside)
                                    {
                                        AddNewAngle = false;
                                    }
                                    else if (MaxInside && !MinInside)
                                    {
                                        o[1] = MinNewAngle;
                                        AddNewAngle = false;
                                    }
                                    else if (!MaxInside && MinInside)
                                    {
                                        o[0] = MaxNewAngle;
                                        AddNewAngle = false;
                                    }
                                }
                            }
                            if (AddNewAngle)
                            {
                                SavedObject.Add(new double[2] { MaxNewAngle, MinNewAngle });
                            }
                        }
                    }
                }
            }
            return retur;
        }
        public SortedEnum<GamePoint> SearchSeeWithBlocks(GamePoint start, int count, int minX, int minY, int maxX, int maxY)
        {
            //if (Find(start) == null)
            //{
            //    //throw new Exception($"В графе отсутствует точка ({start.X},{start.Y})");
            //    return new SortedEnum<GamePoint>() { start };
            //}
            //GamePoint startVert = Find(start).Cord;

            SortedEnum<GamePoint> retur = new SortedEnum<GamePoint>();

            Dictionary<GamePoint, int> Length = new Dictionary<GamePoint, int> { { start, 0 } };
            Queue<GamePoint> Que = new Queue<GamePoint>();
            retur.Add(start);
            Que.Enqueue(start);

            List<double[]> SavedObject = new List<double[]>(); //0 - max //1 - min
            bool KeyPI = true;

            while (Que.Count > 0)
            {
                GamePoint Now = Que.Dequeue();

                List<GamePoint> Near = new List<GamePoint>() {new GamePoint(Now.X+1, Now.Y), new GamePoint(Now.X, Now.Y+1),
                    new GamePoint(Now.X - 1, Now.Y), new GamePoint(Now.X, Now.Y-1) };
                foreach (GamePoint p in Near)
                {
                    if (!Length.ContainsKey(p))
                    {
                        if (Length[Now] + 1 > count)
                        {
                            return retur;
                        }
                        if (p.Y < minY || p.Y >= maxY || p.X < minX || p.X >= maxX)
                        {
                            continue;
                        }

                        double CurX = p.X - start.X;
                        double CurY = p.Y - start.Y;
                        List<double> Angles = new List<double> { Math.Atan2(CurX +0.5, CurY +0.5), Math.Atan2(CurX +0.5, CurY -0.5),
                            Math.Atan2(CurX -0.5, CurY +0.5), Math.Atan2(CurX -0.5, CurY -0.5) };

                        bool FinalKey = false;
                        foreach (var v in Angles)
                        {
                            bool RadianKey = true;
                            foreach (double[] o in SavedObject)
                            {
                                if (o[0] > v && v > o[1])
                                {
                                    RadianKey = false;
                                    break;
                                }
                            }
                            FinalKey = FinalKey || RadianKey;
                            if (FinalKey)
                            {
                                break;
                            }
                        }
                        
                        if (FinalKey) //добавляет все видимые точки
                        {
                            if (this.Vertexes.Find(X => X.Cord.CompareTo(p) == 0) != null)
                            {
                                Length.Add(p, Length[Now] + 1);
                                Que.Enqueue(p);
                                retur.Add(p);
                                continue; //нет смысла обрабатывать преграду, так как уже не стена
                            }
                            if (!retur.Contains(p))
                                retur.Add(p);
                        }

                        double MaxNewAngle = Angles.Max();
                        double MinNewAngle = Angles.Min();

                        if (Math.Atan2(CurX, CurY) == Math.PI)
                        {
                            if (KeyPI)
                            {
                                SavedObject.Add(new double[2] { Math.Atan2(CurX - 0.5, CurY + 0.5), -3.2 });
                                SavedObject.Add(new double[2] { 3.2, Math.Atan2(CurX + 0.5, CurY + 0.5) });
                                KeyPI = false;
                            }
                            continue;
                        }

                        bool AddNewAngle = true;
                        foreach (double[] o in SavedObject)
                        {

                            if (AddNewAngle)
                            {
                                bool MaxInside = (o[0] > MaxNewAngle && MaxNewAngle > o[1]);
                                bool MinInside = (o[0] > MinNewAngle && MinNewAngle > o[1]);

                                if (MaxInside && MinInside)
                                {
                                    AddNewAngle = false;
                                }
                                else if (MaxInside && !MinInside)
                                {
                                    o[1] = MinNewAngle;
                                    AddNewAngle = false;
                                }
                                else if (!MaxInside && MinInside)
                                {
                                    o[0] = MaxNewAngle;
                                    AddNewAngle = false;
                                }
                            }
                        }
                        if (AddNewAngle)
                        {
                            SavedObject.Add(new double[2] { MaxNewAngle, MinNewAngle });
                        }

                    }
                }
            }
            return retur;
        }
        private double CalculateLen(GamePoint a, GamePoint b)
        {
            if (a.X == b.X)
            {
                return Math.Abs(a.Y - b.Y);
            }
            if (a.Y == b.Y)
            {
                return Math.Abs(a.X - b.X);
            }
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
        public SortedEnum<GamePoint> SearchSeeInCircle(GamePoint start, int count, int minX, int minY, int maxX, int maxY)
        {
            //if (Find(start) == null)
            //{
            //    //throw new Exception($"В графе отсутствует точка ({start.X},{start.Y})");
            //    return new SortedEnum<GamePoint>() { start };
            //}
            //GamePoint startVert = Find(start).Cord;

            SortedEnum<GamePoint> retur = new SortedEnum<GamePoint>();

            Queue<GamePoint> Que = new Queue<GamePoint>();
            retur.Add(start);
            Que.Enqueue(start);

            while (Que.Count > 0)
            {
                GamePoint Now = Que.Dequeue();

                List<GamePoint> Near = new List<GamePoint>() {new GamePoint(Now.X+1, Now.Y), new GamePoint(Now.X, Now.Y+1),
                    new GamePoint(Now.X - 1, Now.Y), new GamePoint(Now.X, Now.Y-1) };
                foreach (GamePoint p in Near)
                {
                    if (!retur.Contains(p))
                    {
                        if (CalculateLen(start, p) > count)
                        {
                            continue;
                        }
                        if (p.Y < minY || p.Y >= maxY || p.X < minX || p.X >= maxX)
                        {
                            continue;
                        }
                        retur.Add(p);
                        Que.Enqueue(p);
                    }
                }
            }
            return retur;
        }
        public SortedEnum<GamePoint> SearchSeeInCircleWithBlocks(GamePoint start, int count, int minX, int minY, int maxX, int maxY)
        {
            return SearchSeeInCircleWithBlocksWithousSomePoint(start, count, minX, minY, maxX, maxY, new SortedEnum<GamePoint>());
        }
        public SortedEnum<GamePoint> SearchSeeInCircleWithBlocksWithousSomePoint(GamePoint start, int count, int minX, int minY, int maxX, int maxY, SortedEnum<GamePoint> deletedpoints)
        {
            //if (Find(start) == null)
            //{
            //    //throw new Exception($"В графе отсутствует точка ({start.X},{start.Y})");
            //    return new SortedEnum<GamePoint>() { start };
            //}
            //GamePoint startVert = Find(start).Cord;

            SortedEnum<GamePoint> retur = new SortedEnum<GamePoint>();

            Queue<GamePoint> Que = new Queue<GamePoint>();
            retur.Add(start);
            Que.Enqueue(start);

            List<double[]> SavedObject = new List<double[]>(); //0 - max //1 - min
            bool KeyPI = true;

            while (Que.Count > 0)
            {
                GamePoint Now = Que.Dequeue();

                List<GamePoint> Near = new List<GamePoint>() {new GamePoint(Now.X+1, Now.Y), new GamePoint(Now.X, Now.Y+1),
                    new GamePoint(Now.X - 1, Now.Y), new GamePoint(Now.X, Now.Y-1) };
                foreach (GamePoint p in Near)
                {
                    if (!retur.Contains(p))
                    {
                        if (CalculateLen(start, p) > count)
                        {
                            continue;
                        }
                        if (p.Y < minY || p.Y >= maxY || p.X < minX || p.X >= maxX)
                        {
                            continue;
                        }

                        double CurX = p.X - start.X;
                        double CurY = p.Y - start.Y;
                        List<double> Angles = new List<double> { Math.Atan2(CurX +0.5, CurY +0.5), Math.Atan2(CurX +0.5, CurY -0.5),
                            Math.Atan2(CurX -0.5, CurY +0.5), Math.Atan2(CurX -0.5, CurY -0.5) };

                        bool FinalKey = false;
                        foreach (var v in Angles)
                        {
                            bool RadianKey = true;
                            foreach (double[] o in SavedObject)
                            {
                                if (o[0] > v && v > o[1])
                                {
                                    RadianKey = false;
                                    break;
                                }
                            }
                            FinalKey = FinalKey || RadianKey;
                            if (FinalKey)
                            {
                                break;
                            }
                        }

                        if (FinalKey) //добавляет все видимые точки
                        {
                            if (this.Vertexes.Find(X => X.Cord.CompareTo(p)==0) != null && !deletedpoints.Contains(p))
                            {
                                //Length.Add(p, Length[Now] + 1);
                                Que.Enqueue(p);
                                retur.Add(p);
                                continue; //нет смысла обрабатывать преграду, так как уже не стена
                            }
                            if (!retur.Contains(p)) //добавление стен
                                retur.Add(p);
                        }

                        double MaxNewAngle = Angles.Max();
                        double MinNewAngle = Angles.Min();

                        if (CurX == 0 && CurY < 0) // Math.Atan2(CurX, CurY) == Math.PI)
                        {
                            if (KeyPI)
                            {
                                SavedObject.Add(new double[2] { Math.Atan2(CurX - 0.5, CurY + 0.5), -3.2 });
                                SavedObject.Add(new double[2] { 3.2, Math.Atan2(CurX + 0.5, CurY + 0.5) });
                                KeyPI = false;
                            }
                            continue;
                        }

                        bool AddNewAngle = true;
                        foreach (double[] o in SavedObject)
                        {

                            if (AddNewAngle)
                            {
                                bool MaxInside = (o[0] > MaxNewAngle && MaxNewAngle > o[1]);
                                bool MinInside = (o[0] > MinNewAngle && MinNewAngle > o[1]);

                                if (MaxInside && MinInside)
                                {
                                    AddNewAngle = false;
                                }
                                else if (MaxInside && !MinInside)
                                {
                                    o[1] = MinNewAngle;
                                    AddNewAngle = false;
                                }
                                else if (!MaxInside && MinInside)
                                {
                                    o[0] = MaxNewAngle;
                                    AddNewAngle = false;
                                }
                            }
                        }
                        if (AddNewAngle)
                        {
                            SavedObject.Add(new double[2] { MaxNewAngle, MinNewAngle });
                        }

                    }
                }
            }
            return retur;
        }
        public Vertex Find(GamePoint point)
        {
            foreach (Vertex p in Vertexes)
            {
                if (p.Cord.CompareTo(point)==0)
                {
                    return p;
                }
            }
            return null;
        }
    }
    public class GamePoint : IComparable<GamePoint>, IComparable<(int X, int Y)>
    {
        public int X;
        public int Y;
        public GamePoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int CompareTo(GamePoint other)
        {
            return CompareTo((other.X, other.Y));
        }
        public int CompareTo((int X, int Y) other)
        {
            if (X.CompareTo(other.X) < 0)
            {
                return -1;
            }
            if (X.CompareTo(other.X) > 0)
            {
                return 1;
            }
            if (Y.CompareTo(other.Y) < 0)
            {
                return -1;
            }
            if (Y.CompareTo(other.Y) > 0)
            {
                return 1;
            }
            return 0;
        }
    }
}
