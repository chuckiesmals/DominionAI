using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionAI
{
    namespace Card
    {
        public enum Type
        {
            Action,
            Treasure,
            Victory
        }

        public class Card
        {
            public int _cost = 0;
            public int _worth = 0;
            public int _vp = 0;
            public int _supplyCount = 10;
            public string _name;
            public Type _type;

            void Action(Player player, Simulator sim)
            {
            }
        }

        #region Treasure
        public class Copper : Card
        {
            public Copper()
            {
                _worth = 1;
                _name = "Copper";
                _type = Type.Treasure;
            }
        }

        public class Silver : Card
        {
            public Silver()
            {
                _cost = 3;
                _worth = 2;
                _name = "Silver";
                _type = Type.Treasure;
            }
        }

        public class Gold : Card
        {
            public Gold()
            {
                _cost = 6;
                _worth = 3;
                _name = "Gold";
                _type = Type.Treasure;
            }
        }
        #endregion

        #region Victory Points
        public class Estate : Card
        {
            public Estate()
            {
                _cost = 2;
                _worth = 0;
                _vp = 1;
                _name = "Estate";
                _type = Type.Victory;
            }
        }

        public class Duchy : Card
        {
            public Duchy()
            {
                _cost = 5;
                _worth = 0;
                _vp = 3;
                _name = "Duchy";
                _type = Type.Victory;
            }
        }

        public class Province : Card
        {
            public Province()
            {
                _cost = 8;
                _worth = 0;
                _vp = 6;
                _name = "Province";
                _type = Type.Victory;
            }
        }
        #endregion
        
    }

}
