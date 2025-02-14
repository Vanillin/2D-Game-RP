namespace TwoD_Game_RP
{
    internal interface IAliveElement
    {
        int Health { get; }
        double HealthPercent { get; }
        bool IsAlive { get; }
        void MinusHealth(int health);
        void PlusHealth(int health);
    }
    internal class HealthSkelet : IAliveElement
    {
        private int _health;
        private int _maxHealth;
        public int Health => _health;
        public double HealthPercent => _health * 1.0 / _maxHealth;
        public bool IsAlive => _health > 0;
        public HealthSkelet(int health, int maxHealth)
        {
            _health = health;
            _maxHealth = maxHealth;
        }
        public void MinusHealth(int health)
        {
            _health -= health;
            if (_health < 0) _health = 0;
        }
        public void PlusHealth(int health)
        {
            _health += health;
            if (_health > _maxHealth) _health = _maxHealth;
        }
    }
}
