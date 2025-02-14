using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TwoD_Game_RP
{
    internal class Vertex : IEnumerable
    {
        public GamePoint Cord;
        public List<Vertex> Near = new List<Vertex>();
        public Vertex(GamePoint point)
        {
            Cord = point;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var point in Near)
            {
                yield return point;
            }
        }
    }
    internal class Graf : IEnumerable
    {
        public List<Vertex> Vertexes = new List<Vertex>();
        public Graf()
        { }
        public IEnumerator GetEnumerator()
        {
            foreach (var vertex in Vertexes)
            {
                yield return vertex;
            }
        }
        /// <summary>
        /// Поиск пути в ширину (Порядок важен)
        /// </summary>
        public List<GamePoint> SearchWidth(GamePoint start, GamePoint finish)
        {
            return SearchWidth_OutOfPoint(start, finish, new CustomSortedEnum<GamePoint>());
        }
        /// <summary>
        /// Поиск пути в ширину, избегая некоторые точки (Порядок важен)
        /// </summary>
        public List<GamePoint> SearchWidth_OutOfPoint(GamePoint start, GamePoint finish, CustomSortedEnum<GamePoint> deletedpoints)
        {
            if (start.Equals(finish))
            {
                return new List<GamePoint>();
            }
            List<GamePoint> Retur = new List<GamePoint>();
            Vertex StartVert = Find(start);
            Vertex FinishVert = Find(finish);

            if (StartVert == null || FinishVert == null)
            {
                throw new CustomException($"В графе отсутствует точка ({StartVert.Cord.X},{StartVert.Cord.Y};{FinishVert.Cord.X},{FinishVert.Cord.Y})");
            }

            List<Vertex> PartStart = new List<Vertex>() { StartVert };
            List<Vertex> PartFinish = new List<Vertex>() { FinishVert };
            Dictionary<Vertex, Vertex> Path = new Dictionary<Vertex, Vertex>
            {
                { StartVert, null },
                { FinishVert, null }
            };
            Queue<Vertex> Que = new Queue<Vertex>();
            Que.Enqueue(StartVert);
            Que.Enqueue(FinishVert);
            while (Que.Count > 0)
            {
                Vertex Now = Que.Dequeue();
                foreach (Vertex v in Now.Near)
                {
                    if (!v.Cord.Equals(finish) && deletedpoints.Contains(v.Cord))
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
                        Retur.Add(Now.Cord);
                        while (Now != null)
                        {
                            if (Path[Now] != null)
                                Retur.Add(Path[Now].Cord);
                            Now = Path[Now];
                        }
                        Retur.Reverse();

                        Retur.Add(save.Cord);
                        while (save != null)
                        {
                            if (Path[save] != null)
                                Retur.Add(Path[save].Cord);
                            save = Path[save];
                        }

                        Retur.Remove(StartVert.Cord);
                        if (deletedpoints.Contains(finish))
                        {
                            Retur.Remove(finish);
                        }
                        return Retur;
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
        /// <summary>
        /// Получение всех точек в прямоугольнике min-max и радиусе count (Порядок НЕ важен)
        /// </summary>
        public CustomSortedEnum<GamePoint> SearchCircle(GamePoint start, double count, int minX, int minY, int maxX, int maxY)
        {
            CustomSortedEnum<GamePoint> Retur = new CustomSortedEnum<GamePoint>();

            Queue<GamePoint> Que = new Queue<GamePoint>();
            Retur.Add(start);
            Que.Enqueue(start);

            while (Que.Count > 0)
            {
                GamePoint Now = Que.Dequeue();

                List<GamePoint> Near = new List<GamePoint>() {new GamePoint(Now.X+1, Now.Y), new GamePoint(Now.X, Now.Y+1),
                    new GamePoint(Now.X - 1, Now.Y), new GamePoint(Now.X, Now.Y-1) };
                foreach (GamePoint p in Near)
                {
                    if (!Retur.Contains(p))
                    {
                        if (p.Y < minY || p.Y >= maxY || p.X < minX || p.X >= maxX)
                        {
                            continue;
                        }
                        if (CalculateLen(start, p) > count)
                        {
                            continue;
                        }
                        Retur.Add(p);
                        Que.Enqueue(p);
                    }
                }
            }
            return Retur;
        }
        /// <summary>
        /// Получение всех точек в прямоугольнике min-max и радиусе count, учитывая углы видимости (Порядок НЕ важен)
        /// </summary>
        public CustomSortedEnum<GamePoint> SearchCircle_WithAngle(GamePoint start, double count, int minX, int minY, int maxX, int maxY)
        {
            return SearchCircle_WithAngleOutOfPoint(start, count, minX, minY, maxX, maxY, new CustomSortedEnum<GamePoint>());
        }
        /// <summary>
        /// Получение всех точек в прямоугольнике min-max и радиусе count, избегая некоторые точки и учитывая углы видимости (Порядок НЕ важен)
        /// </summary>
        public CustomSortedEnum<GamePoint> SearchCircle_WithAngleOutOfPoint(GamePoint start, double count, int minX, int minY, int maxX, int maxY, CustomSortedEnum<GamePoint> deletedpoints)
        {
            CustomSortedEnum<GamePoint> Retur = new CustomSortedEnum<GamePoint>();

            Queue<GamePoint> Que = new Queue<GamePoint>();
            Retur.Add(start);
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
                    if (!Retur.Contains(p))
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
                            if (this.Vertexes.Find(X => X.Cord.CompareTo(p) == 0) != null && !deletedpoints.Contains(p))
                            {
                                Que.Enqueue(p);
                                Retur.Add(p);
                                continue; //нет смысла обрабатывать преграду, так как уже не стена
                            }
                            if (!Retur.Contains(p)) //добавление стен
                                Retur.Add(p);
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
            return Retur;
        }
        public Vertex Find(GamePoint point)
        {
            foreach (Vertex p in Vertexes)
            {
                if (p.Cord.CompareTo(point) == 0)
                {
                    return p;
                }
            }
            return null;
        }
    }
    public class GamePoint : IComparable<GamePoint>, IComparable<(int X, int Y)>, IEquatable<GamePoint>, IEquatable<(int X, int Y)>
    {
        public int X;
        public int Y;
        public GamePoint(int x, int y)
        {
            X = x;
            Y = y;
        }
        public GamePoint((int x, int y) pair)
        {
            X = pair.x;
            Y = pair.y;
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
        public bool Equals(GamePoint other)
        {
            return (X == other.X && Y == other.Y);
        }
        public bool Equals((int X, int Y) other)
        {
            return (X == other.X && Y == other.Y);
        }
    }
}
