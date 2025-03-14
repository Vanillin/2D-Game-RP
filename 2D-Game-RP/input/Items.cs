﻿namespace TwoD_Game_RP
{
    public class NoteBook : Item
    {
        public NoteBook() : base("Записная книжка", "notebook", 0, 1, 1) { }
    }
    public class Carrot : ItemMedicine
    {
        public Carrot() : base("Морковь", "carrot", 0, 1, 1, 1) { }
    }
    public class Potato : ItemMedicine
    {
        public Potato() : base("Картошка", "potato", 0, 1, 1, 40) { }
    }
    public class DatailDrawwell : Item
    {
        public DatailDrawwell() : base("Деталь от колодца", "detailDrawwell", 0, 1, 1) { }
    }
    public class ScorpionPart : Item
    {
        public ScorpionPart() : base("Жало", "partScorpion", 0, 1, 1) { }
    }
    public class Water : ItemMedicine
    {
        public Water() : base("Вода", "water", 0, 1, 1, 70) { }
    }
}
