using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoD_Game_RP
{
    public class Phrase
    {
        public string Index { get; set; }
        public string Dialog { get; set; }
        public Phrase(string index, string dialog)
        {
            this.Index = index;
            this.Dialog = dialog;
        }
        private Phrase() { } //ДЛЯ СЕРИАЛИЗАЦИИ НЕ ТРОЖЖЖЖ
    }
}
