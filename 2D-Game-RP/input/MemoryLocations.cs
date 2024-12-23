using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace TwoD_Game_RP
{
    public class MemoryLocations
    {
        private static Location Garden;
        public static Location GetGarden(Player player, int sizeInventH, int sizeInventW)
        {
            if (Garden == null)
            {
                Garden = new Location("Двор", "Garden", 21, 26);
                Garden.CreateDarkPictCell(Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"System/Dark.png"));
                Garden.AddLocationCellsLayer(CreateLocation("GardenFloors"), 0);
                Garden.AddLocationCellsLayer(CreateLocation("GardenFloors2"), 0);
                Garden.AddLocationCellsLayer(CreateLocation("GardenFloors3"), 0);
                Garden.AddLocationCellsLayer(CreateLocation("GardenWalls"), 0);
                Garden.AddLocationCellsLayer(CreateLocation("GardenObject1"), 0);
                Garden.AddLocationCellsLayer(CreateLocation("GardenAir"), 3);
                Garden.CreateGrafWatch();
                Garden.CreateGrafMove();
                Garden.CreateGrafAll();

                Garden.AddSecondLayerWithCell(new WoodDoor(new GamePoint(4, 14), '0', false));
                Garden.AddSecondLayerWithCell(new WoodDoor(new GamePoint(7, 11), '1', false));
                Garden.AddSecondLayerWithCell(new WoodDoor(new GamePoint(14, 15), '0', true));
                Garden.AddSecondLayerWithCell(new WoodDoor(new GamePoint(16, 17), '1', false));

                Garden.AddSecondLayerWithCell(new Trash(new GamePoint(10, 7), '3', sizeInventH, sizeInventW, new List<Item>()));
                Garden.AddSecondLayerWithCell(new Trash(new GamePoint(13, 17), '1', sizeInventH, sizeInventW, new List<Item>()));
                Garden.AddSecondLayerWithCell(new Trash(new GamePoint(15, 9), '0', sizeInventH, sizeInventW, new List<Item>()
                    { new BloodPaper() }));

                var kristina = new Kristina(new GamePoint(7, 14), '0');
                kristina.EnqueueDownGlobalAction(new ActionWait(3, true));
                kristina.EnqueueDownGlobalAction(new ActionMove(new GamePoint(7, 12), true));
                kristina.EnqueueDownGlobalAction(new ActionMove(new GamePoint(7, 14), true));
                kristina.EnqueueDownGlobalAction(new ActionWait(3, true));
                kristina.EnqueueDownGlobalAction(new ActionMove(new GamePoint(5, 13), true));
                kristina.EnqueueDownGlobalAction(new ActionWait(4, true));
                kristina.EnqueueDownGlobalAction(new ActionMove(new GamePoint(7, 14), true));

                Garden.AddFirstLayerWithCell(player);
                Garden.AddFirstLayerWithCell(kristina);
                Garden.AddFirstLayerWithCell(new Nura(new GamePoint(14, 0), '0'));
                Garden.AddFirstLayerWithCell(new Dead(new GamePoint(3, 5), '0'));

                Garden.UpdateDisplay();
            }
            return Garden;
        }
        private static IPictureCell[,] CreateLocation(string nameloca)
        {
            ReadLocation rl;
            using (var file = new FileStream(Path.Combine(ConfigurationManager.AppSettings["Levels"], nameloca + ".txt"), FileMode.Open))
            {
                var xml = new XmlSerializer(typeof(ReadLocation));
                rl = (ReadLocation)xml.Deserialize(file);
            }
            IPictureCell[,] retur = new IPictureCell[rl.height, rl.wight];
            Dictionary<char, string> dict = new Dictionary<char, string>();
            foreach (var v in rl.description) { dict.Add(v.Item1[0], v.Item2); }
            for (int i = 0; i < rl.height; i++)
            {
                for (int j = 0; j < rl.wight; j++)
                {
                    char pict = rl.location[i][j];
                    if (dict.ContainsKey(pict))
                    {
                        retur[i, j] = new StaticPicCell(Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"{dict[pict]}.png"));
                        switch (rl.rotation[i][j])
                        {
                            case '1': retur[i, j].Rotate90 = true; break;
                            case '2': retur[i, j].Rotate180 = true; break;
                            case '3': retur[i, j].Rotate270 = true; break;
                        }
                    }
                }
            }
            return retur;
        }
        public class ReadLocation
        {
            public List<(string c, string s)> description;
            public int height;
            public int wight;
            public string[] location;
            public List<string> rotation;
        }
    }
}
