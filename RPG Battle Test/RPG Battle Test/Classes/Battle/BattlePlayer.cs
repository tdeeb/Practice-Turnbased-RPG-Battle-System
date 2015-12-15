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
using static RPG_Battle_Test.BattlePlayer.Characters;

namespace RPG_Battle_Test
{
    public class BattlePlayer : BattleEntity
    {
        public enum Characters
        {
            None, CecilK, CecilP
        }

        /// <summary>
        /// Possible BattleActions for players to have.
        /// Every character has Attack, Item, Magic, and Run, but more can be obtained or specific to characters (Ex. Steal).
        /// </summary>
        public enum BattleActions
        {
            Attack, Item, Magic, Steal, Run
        }

        /// <summary>
        /// The main battle menu
        /// </summary>
        public static BattleMenu MainBattleMenu = null;
        public static BattleMenu ItemMenu = null;

        public static Stack<BattleMenu> Menus = null;

        public static BattleMenu CurrentMenu => Menus.Peek();

        protected const float ArrowVerticalDist = 100f;

        public Characters Character { get; private set; } = Characters.None;
        protected Sprite Arrow = null;
        protected int? CurSelection = null;

        public BattlePlayer(Characters character)
        {
            EntityType = EntityTypes.Player;
            
            IntRect rect = new IntRect(5, 55, 16, 24);

            switch (character)
            {
                case CecilK:
                    Name = "CecilK";
                    Speed = 3;
                    MaxHP = 30;
                    MaxMP = 0;
                    Attack = 7;
                    Defense = 3;
                    break;
                case CecilP:
                    Name = "CecilP";
                    Speed = 1;
                    MaxHP = 25;
                    MaxMP = 10;
                    Attack = 5;
                    Defense = 1;
                    MagicAtk = 3;
                    MagicDef = 2;
                    rect = new IntRect(7, 44, 16, 24);
                    break;
            }

            CurHP = MaxHP;
            CurMP = MaxMP;

            EntitySprite = Helper.CreateSprite(new Texture(Constants.ContentPath + Name + ".png"), false, rect);
            EntitySprite.Position = new Vector2f(GameCore.GameWindow.Size.X - GameCore.GameWindow.Size.X / 4f, GameCore.GameWindow.Size.Y / 2f);
            EntitySprite.Scale *= 3f;

            Arrow = Helper.CreateSprite(new Texture(Constants.ContentPath + "Arrow.png"), false);
        }

        /// <summary>
        /// Called when the battle is started
        /// </summary>
        public static void OnBattleStart()
        {
            Menus = new Stack<BattleMenu>();
            MainBattleMenu = new BattleMenu(new Vector2f(40f, GameCore.GameWindow.Size.Y - 150), new Vector2f(100, 40), BattleMenu.MenuTypes.Vertical);
            ItemMenu = new BattleMenu(new Vector2f(40f, GameCore.GameWindow.Size.Y - 150), new Vector2f(100, 40), BattleMenu.MenuTypes.Vertical, () => Menus.Pop());

            Menus.Push(MainBattleMenu);
        }

        public override void StartTurn()
        {
            base.StartTurn();
            BattleMenu.Active = true;

            //Set the basic options
            MainBattleMenu.SetOptions(new BattleMenu.MenuOption("Attack", AttackSelect), new BattleMenu.MenuOption("Item", ItemSelect));
            ItemMenu.SetOptions(new BattleMenu.MenuOption("Poison", null));
        }

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
            BattleMenu.Active = false;
            while (Menus.Count > 1)
            {
                Menus.Pop();
            }
        }

        public override void TurnUpdate()
        {
            base.TurnUpdate();

            if (CurSelection.HasValue)
            {
                //Cancel selection
                if (Input.PressedKey(Keyboard.Key.X))
                {
                    CurSelection = null;
                    BattleManager.Instance.HeaderBox.SetText(Name + "'s turn!");
                    return;
                }
                else
                {
                    //Move up a selection
                    if (Input.PressedKey(Keyboard.Key.Up))
                    {
                        do CurSelection = Helper.Wrap(CurSelection.Value - 1, 0, BattleManager.Instance.Enemies.Count - 1);
                        while (BattleManager.Instance.GetEnemy(CurSelection.Value).IsDead == true);
                        Arrow.Position = new Vector2f(BattleManager.Instance.GetEnemy(CurSelection.Value).Position.X, BattleManager.Instance.GetEnemy(CurSelection.Value).Position.Y - ArrowVerticalDist);
                        BattleManager.Instance.HeaderBox.SetText("Attack " + BattleManager.Instance.Enemies[CurSelection.Value].Name + "?");
                    }
                    //Move down a selection
                    if (Input.PressedKey(Keyboard.Key.Down))
                    {
                        do CurSelection = Helper.Wrap(CurSelection.Value + 1, 0, BattleManager.Instance.Enemies.Count - 1);
                        while (BattleManager.Instance.GetEnemy(CurSelection.Value).IsDead == true);
                        Arrow.Position = new Vector2f(BattleManager.Instance.GetEnemy(CurSelection.Value).Position.X, BattleManager.Instance.GetEnemy(CurSelection.Value).Position.Y - ArrowVerticalDist);
                        BattleManager.Instance.HeaderBox.SetText("Attack " + BattleManager.Instance.Enemies[CurSelection.Value].Name + "?");
                    }
                }
            }

            if (Input.PressedKey(Keyboard.Key.Z) && CurSelection.HasValue == true)
            {
                AttackEntity(BattleManager.Instance.GetEnemy(CurSelection.Value));
                CurSelection = null;
                EndTurn();
                return;
            }

            if (CurSelection.HasValue == false)
                CurrentMenu.Update();
        }

        protected void AttackSelect()
        {
            for (int i = 0; i < BattleManager.Instance.Enemies.Count; i++)
            {
                BattleEnemy enemy = BattleManager.Instance.Enemies[i];
                if (enemy.IsDead == false)
                {
                    CurSelection = i;
                    Arrow.Position = new Vector2f(enemy.Position.X, enemy.Position.Y - ArrowVerticalDist);
                    BattleManager.Instance.HeaderBox.SetText("Attack " + BattleManager.Instance.Enemies[i].Name + "?");
                    break;
                }
            }
        }

        protected void ItemSelect()
        {
            Menus.Push(ItemMenu);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
            if (CurSelection.HasValue)
            {
                GameCore.spriteSorter.Add(Arrow, Constants.BASE_UI_LAYER + .03f);
                //Arrow.Draw(GameCore.GameWindow, RenderStates.Default);
            }
        }
    }
}
