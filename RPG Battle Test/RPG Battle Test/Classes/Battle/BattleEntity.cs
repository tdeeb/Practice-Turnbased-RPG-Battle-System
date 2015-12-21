﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using SFML.Audio;

namespace RPG_Battle_Test
{
    public abstract class BattleEntity
    {
        public enum EntityTypes
        {
            None, Player, Enemy
        }

        public Vector2f Position
        {
            get
            {
                return EntitySprite.Position;
            }
            set
            {
                EntitySprite.Position = value;
            }
        }
        
        public Sprite EntitySprite = null;
        public int NumActions = 1;

        //Stats
        public string Name = "ERROR";
        public int CurHP { get; protected set; } = 10;
        public int CurMP { get; protected set; } = 0;
        public int MaxHP { get; protected set; } = 10;
        public int MaxMP { get; protected set; } = 0;
        public int Attack { get; protected set; } = 5;
        public int MagicAtk { get; protected set; } = 0;
        public int Defense { get; protected set; } = 0;
        public int MagicDef { get; protected set; } = 0;
        public int Speed { get; protected set; } = 1;

        public readonly Color TurnColor = Color.Cyan;
        public EntityTypes EntityType { get; protected set; } = EntityTypes.None;

        public BattleEntity()
        {
            
        }

        public static bool IsEntityEnemy(BattleEntity entity) => entity.EntityType == EntityTypes.Enemy;
        public static bool IsEntityPlayer(BattleEntity entity) => entity.EntityType == EntityTypes.Player;

        public bool IsEnemy => EntityType == EntityTypes.Enemy;
        public bool IsPlayer => EntityType == EntityTypes.Player;

        public bool IsTurn => this == BattleManager.Instance.CurrentEntityTurn;
        public bool IsDead => CurHP <= 0;

        //The entity being targeted in battle
        public BattleEntity Target { get; protected set; } = null;

        //Battle-turn related methods
        public virtual void StartTurn()
        {
            
        }

        public virtual void OnTurnEnd()
        {
            
        }

        public void EndTurn()
        {
            Target = null;
            BattleManager.Instance.TurnEnd();
            OnTurnEnd();
        }

        /// <summary>
        /// Performs a battle command
        /// </summary>
        public void UseCommand(BattleCommand command)
        {
            command.Perform();
        }

        /// <summary>
        /// Calculates the total damage this entity will deal to another entity
        /// </summary>
        /// <returns>The amount of total damage this entity will deal</returns>
        private int CalculateDamageDealt()
        {
            return Helper.Clamp(Attack, 0, int.MaxValue);
        }

        /// <summary>
        /// Calculates damage reduction based on this entity's Defense. Cannot go below 0
        /// </summary>
        /// <param name="damage">The damage value from the damage source</param>
        /// <returns>The amount of total damage dealt to this entity</returns>
        private int CalculateDamageReduction(int damage)
        {
            return Helper.Clamp(damage - Defense, 0, int.MaxValue);
        }

        /// <summary>
        /// Deals damage to an entity
        /// </summary>
        /// <param name="entity">The entity to deal damage to. Minimum damage is 0</param>
        public void AttackEntity(BattleEntity entity)
        {
            entity.TakeDamage(CalculateDamageDealt());

            Debug.Log(Name + " attacked " + entity.Name + "!");
        }

        public void TakeDamage(int damage)
        {
            int totaldamage = CalculateDamageReduction(damage);
            CurHP -= totaldamage;

            Debug.Log($"{Name} received {totaldamage} damage!");
        }

        //Battle-combat related methods
        /// <summary>
        /// Restores an Entity's HP and MP values by the given amount
        /// </summary>
        /// <param name="hp">The amount of HP to restore</param>
        /// <param name="mp">The amount of MP to restore</param>
        public void Restore(uint hp, uint mp)
        {
            CurHP = Helper.Clamp(CurHP + (int)hp, 0, MaxHP);
            CurMP = Helper.Clamp(CurMP + (int)mp, 0, MaxMP);
        }

        /// <summary>
        /// The update called during the entity's turn; for input and other stuff
        /// </summary>
        public virtual void TurnUpdate()
        {

        }

        /// <summary>
        /// This update is for animations, status effects, and so on; nothing turn related
        /// </summary>
        public virtual void Update()
        {
            
        }

        public virtual void Draw()
        {
            if (IsDead == false)
            {
                GameCore.spriteSorter.Add(EntitySprite, Constants.BASE_ENTITY_LAYER + (Position.Y / 1000f));
            }
        }
    }
}
