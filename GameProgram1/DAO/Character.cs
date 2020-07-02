using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol1;

namespace GameProgram1.DAO
{
    class Character
    {
        /*
        public int CID { get; set; }//角色ID
        public int level{ get; set; } //等级
        public int exp { get; set; } //经验
        public int cost { get; set; } //费用
        public float life { get; set; }//生命
        public string name { get; set; } //名字
        
        public int type { get; set; }
        public int attack_pow { get; set; }//攻击力
        public int defend { get; set; }//防御力
        public float attack_area { get; set; } //攻击范围
        public float attack_interval { get; set; } //攻击间隔

        */
        private SocketGameProtocol1.CharacterData character;

        public Character(SocketGameProtocol1.CharacterData data)
        {
            character = data;
        }

        public SocketGameProtocol1.CharacterData GetCharacterData
        {
            get
            {
                return character;
            }           
        }

    }
}
