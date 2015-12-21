﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    public class DamageItem : Item
    {
        public int Damage = 0;

        public DamageItem(string name, int damage) : base(name)
        {
            Damage = damage;

            TypeList.Add(ItemTypes.Damage, true);
        }

        protected override void OnUse(BattleEntity entity)
        {
            entity.TakeDamage(Damage);
        }
    }
}
