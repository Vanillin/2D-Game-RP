using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Game_STALKER_Exclusion_Zone
{
    public class Vertex : IEnumerable, ICloneable
    {
        public Point Cord = new Point();
        public List<Vertex> Near = new List<Vertex>();
        public Vertex(Point point)
        {
            Cord = point;
        }
        private Vertex(Point point, List<Vertex> near)
        {
            Cord = point;
            Near = near;
        }
        public object Clone()
        {
            return new Vertex(Cord, Near);
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var point in Near)
            {
                yield return point;
            }
        }
    }
    public class Graf : IEnumerable, ICloneable
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
        public object Clone()
        {
            List<Vertex> copy = new List<Vertex>();
            foreach (var vertex in Vertexes)
            {
                copy.Add((Vertex)vertex.Clone());
            }
            return new Graf(copy);
        }
        public List<Point> SearchWidth(Point start, Point finish)
        {
            return SearchWidthWithoutSomePoint(start, finish, new List<Point>(0));
        }
        public List<Point> SearchWidthWithoutSomePoint(Point start, Point finish, List<Point> deletedpoints)
        {
            if (start == finish || deletedpoints.Contains(start) || deletedpoints.Contains(finish))
            {
                return null;
            }
            List<Point> retur = new List<Point>();
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
        List<Point> SearchAround(Point start, int count)
        {
            List<Point> retur = new List<Point>();
            Vertex startVert = Find(start);

            if (startVert == null)
            {
                throw new Exception($"В графе отсутствует точка ({startVert.Cord.X},{startVert.Cord.Y})");
            }

            Dictionary<Vertex, int> Length = new Dictionary<Vertex, int> { { startVert, 0 } };
            Queue<Vertex> Que = new Queue<Vertex>();
            Que.Enqueue(startVert);
            while (Que.Count > 0)
            {
                Vertex Now = Que.Dequeue();
                foreach (Vertex v in Now.Near)
                {
                    if (!Length.ContainsKey(v))
                    {
                        if (Length[Now] + 1 > count)
                        {
                            return retur;
                        }
                        Length.Add(v, Length[Now] + 1);
                        retur.Add(v.Cord);
                        Que.Enqueue(v);
                    }
                }
            }
            return retur;
        }
        public List<Point> SearchSee(Point start, int count)
        {
            return new List<Point>();

            List<Point> retur = new List<Point>();
            Point startVert = Find(start).Cord;

            if (startVert == null)
            {
                throw new Exception($"В графе отсутствует точка ({startVert.X},{startVert.Y})");
            }

            Dictionary<Point, int> Length = new Dictionary<Point, int> { { startVert, 0 } };
            Queue<Point> Que = new Queue<Point>();
            Que.Enqueue(startVert);

            List<double[]> SavedObject = new List<double[]>(); //0 - max //1 - min
            bool KeyPI = true;

            while (Que.Count > 0)
            {
                Point Now = Que.Dequeue();
                List<Point> Near = new List<Point>() {new Point(Now.X+1, Now.Y), new Point(Now.X, Now.Y+1),
                    new Point(Now.X - 1, Now.Y), new Point(Now.X, Now.Y-1) };
                foreach (Point p in Near)
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

                        if (this.Vertexes.Find(X => X.Cord == p) != null)
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
        public Vertex Find(Point point)
        {
            foreach (Vertex p in Vertexes)
            {
                if (p.Cord == point)
                {
                    return p;
                }
            }
            return null;
        }
    }
}
