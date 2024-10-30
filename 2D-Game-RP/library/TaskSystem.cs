using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoD_Game_RP
{
    public class Task
    {
        public string SystemName { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public double CordOnMapX { get; set; }
        public double CordOnMapY { get; set; }
        public Task(string systemName, string name, string secondName, double cordOnMapX, double cordOnMapY)
        {
            this.Name = name;
            this.SystemName = systemName;
            this.SecondName = secondName;
            this.CordOnMapX = cordOnMapX;
            this.CordOnMapY = cordOnMapY;
        }
        private Task() { } //ДЛЯ СЕРИАЛИЗАЦИИ НЕ ТРОЖЖЖЖ
    }
}
