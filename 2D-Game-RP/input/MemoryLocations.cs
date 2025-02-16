using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace TwoD_Game_RP
{
    public class MemoryLocations
    {
        private static Location Eosha;
        public static Location GetEosha(PlayerSkelet player, int lenWatch, double compressH, double compressW)
        {
            if (Eosha == null)
            {
                List<(string, GamePoint)> transit = new List<(string, GamePoint)>()
                {
                    ("Mine", new GamePoint(10,0)),
                    ("Mine", new GamePoint(11,0)),
                    ("Mine", new GamePoint(12,0)),
                    ("Mine", new GamePoint(13,0)),
                    ("Mine", new GamePoint(14,0)),
                    ("Mine", new GamePoint(15,0)),
                    ("UnderEosha", new GamePoint(3,13)),
                };
                Eosha = new Location("Булгар", "Eosha", 23, 32, compressH, compressW, transit);

                Eosha.AddLocationCellsLayer(CreateLocation("EoshaFloor1"), -1);
                Eosha.AddLocationCellsLayer(CreateLocation("EoshaFloor2"), -1);
                Eosha.AddLocationCellsLayer(CreateLocation("EoshaWall1"), 0);
                Eosha.AddLocationCellsLayer(CreateLocation("EoshaObject1"), 0);
                Eosha.AddLocationCellsLayer(CreateLocation("EoshaObject2"), 0);

                Eosha.CreateDarkPictCell(Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"System/Dark.png"));
                //Eosha.CreateShootPictCell(Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"System/Shoot.png"));
                Eosha.CreateGrafWatch();
                Eosha.CreateGrafMove();
                Eosha.CreateGrafAll();

                var girl = new Girl(lenWatch);
                //var grandma = new Grandma(lenWatch);
                var grandpa = new Grandpa(lenWatch);

                Eosha.AddFirstLayerWithCell(new BoxSkelet("", new GamePoint(3, 18), true, 2, 2, new List<Item>() { new Water() }));
                Eosha.AddFirstLayerWithCell(new BoxSkelet("", new GamePoint(6, 13), true, 2, 2));
                Eosha.AddFirstLayerWithCell(new BoxSkelet("", new GamePoint(9, 25), true, 2, 2));
                Eosha.AddFirstLayerWithCell(new BoxSkelet("", new GamePoint(16, 3), true, 2, 2));
                Eosha.AddFirstLayerWithCell(new BoxSkelet("", new GamePoint(17, 3), true, 2, 2));

                player.GPoint = new GamePoint(8, 22);
                Eosha.AddFirstLayerWithCell(player);
                Eosha.AddFirstLayerWithCell(girl);
                //Eosha.AddFirstLayerWithCell(grandma);
                Eosha.AddFirstLayerWithCell(grandpa);

                Eosha.UpdateDisplay();
            }
            return Eosha;
        }
        private static Location Mine;
        public static Location GetMine(int lenWatch, double compressH, double compressW)
        {
            if (Mine == null)
            {
                List<(string, GamePoint)> transit = new List<(string, GamePoint)>()
                {
                    ("Eosha", new GamePoint(33,26)),
                    ("Eosha", new GamePoint(32,26)),
                    ("Eosha", new GamePoint(31,26)),
                    ("Eosha", new GamePoint(30,26)),
                };
                Mine = new Location("sdfsdfsdf", "Mine", 34, 27, compressH, compressW, transit);

                Mine.AddLocationCellsLayer(CreateLocation("MineFloor1"), -1);
                Mine.AddLocationCellsLayer(CreateLocation("MineFloor2"), -1);
                Mine.AddLocationCellsLayer(CreateLocation("MineWall1"), 0);
                Mine.AddLocationCellsLayer(CreateLocation("MineWall2"), 0);
                Mine.AddLocationCellsLayer(CreateLocation("MineObject1"), 0);

                Mine.AddFirstLayerWithCell(new BoxSkelet("", new GamePoint(4, 8), true, 2, 2, new List<Item>() { new DatailDrawwell() }));
                Mine.AddFirstLayerWithCell(new BoxSkelet("", new GamePoint(20, 23), true, 2, 2, new List<Item>() { new Potato() }));
                Mine.AddFirstLayerWithCell(new BoxSkelet("", new GamePoint(19, 25), true, 2, 2));
                Mine.AddFirstLayerWithCell(new BoxSkelet("", new GamePoint(20, 1), true, 2, 2));
                Mine.AddFirstLayerWithCell(new BoxSkelet("", new GamePoint(21, 1), true, 2, 2));

                Mine.AddFirstLayerWithCell(new Scorpion(new GamePoint(15, 18), lenWatch));
                Mine.AddFirstLayerWithCell(new Scorpion(new GamePoint(6, 9), lenWatch));
                Mine.AddFirstLayerWithCell(new Scorpion(new GamePoint(7, 14), lenWatch));

                Mine.CreateGrafWatch();
                Mine.CreateGrafMove();
                Mine.CreateGrafAll();

                Mine.UpdateDisplay();
            }
            return Mine;
        }
        private static Location UnderEosha;
        public static Location GetUnderEosha(double compressH, double compressW)
        {
            if (UnderEosha == null)
            {
                List<(string, GamePoint)> transit = new List<(string, GamePoint)>()
                {
                    ("Eosha", new GamePoint(1,5)),
                };
                UnderEosha = new Location("", "UnderEosha", 5, 7, compressH, compressW, transit);

                UnderEosha.AddLocationCellsLayer(CreateLocation("UnderEoshaFloor1"), -1);
                UnderEosha.AddLocationCellsLayer(CreateLocation("UnderEoshaWall1"), 0);
                UnderEosha.AddLocationCellsLayer(CreateLocation("UnderEoshaObject1"), 0);

                UnderEosha.AddFirstLayerWithCell(new BoxSkelet("", new GamePoint(1, 1), true, 2, 2, new List<Item>() { new SmallToz() }));

                UnderEosha.CreateGrafWatch();
                UnderEosha.CreateGrafMove();
                UnderEosha.CreateGrafAll();

                UnderEosha.UpdateDisplay();
            }
            return UnderEosha;
        }
        private static IPicture[,] CreateLocation(string nameloca) //change creating ISomePicture on ISomePicture\IPicture, because not some dark texture _sD_. Example Floor textures
        {
            ReadLocation rl = Information.GetLocation(nameloca);
            IPicture[,] retur = new IPicture[rl.Height, rl.Wight];
            for (int i = 0; i < rl.Height; i++)
            {
                for (int j = 0; j < rl.Wight; j++)
                {
                    char pict = rl.Location[i][j];
                    if (rl.Description.ContainsKey(pict))
                    {
                        retur[i, j] = new MapPicture(Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"{rl.Description[pict]}.png"));
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
