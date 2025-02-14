namespace TwoD_Game_RP
{
    internal interface IHaveGun
    {
        void ChangeGunInHandAndInBack();
        Gun GetGunInHand();
        void DropGunInHand();
        Gun GetGunInBack();
        void DropGunInBack();
        void GiveGunInHand(Gun gun);
        void GiveGunInBack(Gun gun);
    }
    internal class InventoryGun : IHaveGun
    {
        Gun _gunInHand;
        Gun _gunInBack;
        public InventoryGun()
        {
            _gunInHand = null;
            _gunInBack = null;
        }

        public void ChangeGunInHandAndInBack()
        {
            (_gunInHand, _gunInBack) = (_gunInBack, _gunInHand);
        }
        public Gun GetGunInHand()
        {
            return _gunInHand;
        }
        public void GiveGunInHand(Gun gun)
        {
            _gunInHand = gun;
        }
        public void DropGunInHand()
        {
            _gunInHand = null;
        }
        public Gun GetGunInBack()
        {
            return _gunInBack;
        }
        public void GiveGunInBack(Gun gun)
        {
            _gunInBack = gun;
        }
        public void DropGunInBack()
        {
            _gunInBack = null;
        }
    }
}
