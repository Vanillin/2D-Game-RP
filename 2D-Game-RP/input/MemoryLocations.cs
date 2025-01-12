﻿using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace TwoD_Game_RP
{
    public class MemoryLocations
    {
        private static Location Garden;
        private static Location GetGarden(Player player, int sizeInventH, int sizeInventW)
        {
            if (Garden == null)
            {
                Garden = new Location("Двор", "Garden", 21, 26, 1, 1);
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

                Garden.AddSecondLayerWithCell(new WoodDoor(new GamePoint(4, 14), 0, false));
                Garden.AddSecondLayerWithCell(new WoodDoor(new GamePoint(7, 11), 90, false));
                Garden.AddSecondLayerWithCell(new WoodDoor(new GamePoint(14, 15), 0, true));
                Garden.AddSecondLayerWithCell(new WoodDoor(new GamePoint(16, 17), 90, false));

                Garden.AddSecondLayerWithCell(new Trash(new GamePoint(10, 7), 270, sizeInventH, sizeInventW, new List<Item>()));
                Garden.AddSecondLayerWithCell(new Trash(new GamePoint(13, 17), 90, sizeInventH, sizeInventW, new List<Item>()));
                Garden.AddSecondLayerWithCell(new Trash(new GamePoint(15, 9), 0, sizeInventH, sizeInventW, new List<Item>()
                    { new BloodPaper() }));

                //var kristina = new Kristina(new GamePoint(7, 14), 0);
                //kristina.EnqueueDownGlobalAction(new ActionWait(3, true));
                //kristina.EnqueueDownGlobalAction(new ActionMove(new GamePoint(7, 12), true));
                //kristina.EnqueueDownGlobalAction(new ActionMove(new GamePoint(7, 14), true));
                //kristina.EnqueueDownGlobalAction(new ActionWait(3, true));
                //kristina.EnqueueDownGlobalAction(new ActionMove(new GamePoint(5, 13), true));
                //kristina.EnqueueDownGlobalAction(new ActionWait(4, true));
                //kristina.EnqueueDownGlobalAction(new ActionMove(new GamePoint(7, 14), true));

                //Garden.AddFirstLayerWithCell(player);
                //Garden.AddFirstLayerWithCell(kristina);
                //Garden.AddFirstLayerWithCell(new Nura(new GamePoint(14, 0), 0));
                //Garden.AddFirstLayerWithCell(new Dead(new GamePoint(3, 5), 0));

                Garden.UpdateDisplay();
            }
            return Garden;
        }

        private static Location Eosha;
        public static Location GetEosha(Player player, double compressH, double compressW)
        {
            if (Eosha == null)
            {
                Eosha = new Location("Посёлок Еоша", "Eosha", 23, 32, compressH, compressW);

                Eosha.AddLocationCellsLayer(CreateLocation("EoshaFloor1"), -1);
                Eosha.AddLocationCellsLayer(CreateLocation("EoshaFloor2"), -1);
                Eosha.AddLocationCellsLayer(CreateLocation("EoshaWall1"), 0);
                Eosha.AddLocationCellsLayer(CreateLocation("EoshaObject1"), 0);
                Eosha.AddLocationCellsLayer(CreateLocation("EoshaObject2"), 0);

                Eosha.CreateDarkPictCell(Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"System/Dark.png"));
                Eosha.CreateShootPictCell(Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"System/Shoot.png"));
                Eosha.CreateGrafWatch();
                Eosha.CreateGrafMove();
                Eosha.CreateGrafAll();

                var girl = new Girl();
                var grandma = new Grandma();
                var grandpa = new Grandpa();

                player.Cord = new GamePoint(8, 22);
                Eosha.AddFirstLayerWithCell(player);
                Eosha.AddFirstLayerWithCell(girl);
                Eosha.AddFirstLayerWithCell(grandma);
                Eosha.AddFirstLayerWithCell(grandpa);

                Eosha.UpdateDisplay();
            }
            return Eosha;
        }
        private static IPictureCell[,] CreateLocation(string nameloca)
        {
            ReadLocation rl = Information.GetLocation(nameloca);
            IPictureCell[,] retur = new IPictureCell[rl.Height, rl.Wight];
            for (int i = 0; i < rl.Height; i++)
            {
                for (int j = 0; j < rl.Wight; j++)
                {
                    char pict = rl.Location[i][j];
                    if (rl.Description.ContainsKey(pict))
                    {
                        retur[i, j] = new StaticPicCell(Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"{rl.Description[pict]}.png"));
                        switch (rl.Rotation[i][j])
                        {
                            case '1': retur[i, j].Rotate = 90; break;
                            case '2': retur[i, j].Rotate = 180; break;
                            case '3': retur[i, j].Rotate = 270; break;
                        }
                    }
                }
            }
            return retur;
        }
    }
    public class ReadLocation
    {
        public CustomDictionary<char, string> Description;
        public int Height;
        public int Wight;
        public List<string> Location;
        public List<string> Rotation;
        public ReadLocation(int height, int wight, CustomDictionary<char, string> description, List<string> location, List<string> rotation)
        {
            Height = height;
            Wight = wight;
            Location = location;
            Rotation = rotation;
            Description = description;
        }
    }
}
