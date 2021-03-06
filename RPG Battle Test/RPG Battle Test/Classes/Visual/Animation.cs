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
    /// <summary>
    /// The base class for sprite animation
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// How the animation plays
        /// </summary>
        public enum AnimationTypes
        {
            Forward, Looping
        }

        public Texture SpriteSheet = null;
        protected Frame[] Frames = null;

        protected int MaxFrames = 1;
        protected int CurFrame = 0;
        protected bool AnimDone = false;

        protected float FrameRate = 15f;
        protected float PrevFrameTimer = 0f;

        /// <summary>
        /// The type of animation this is
        /// </summary>
        public AnimationTypes AnimationType { get; protected set; } = AnimationTypes.Forward;

        public Animation(Texture texture, float frameRate, params IntRect[] frameRects)
        {
            SpriteSheet = texture;
            MaxFrames = frameRects.Length;
            Frames = new Frame[MaxFrames];
            for (int i = 0; i < Frames.Length; i++)
            {
                Frames[i] = new Frame(SpriteSheet, frameRects[i]);
            }

            AnimationType = AnimationTypes.Forward;

            FrameRate = frameRate;
            ResetFrameDur();
        }

        public bool AnimationFinished => AnimDone;
        protected int MaxFrameIndex => MaxFrames - 1;

        public Vector2f Position
        {
            get
            {
                return Frames[0].FrameSprite.Position;
            }
            set
            {
                for (int i = 0; i < Frames.Length; i++)
                {
                    Frames[i].FrameSprite.Position = value;
                }
            }
        }

        protected virtual void Progress()
        {
            CurFrame++;
            if (CurFrame >= MaxFrames)
            {
                CurFrame = MaxFrameIndex;

                //Animation done
                AnimDone = true;
            }

            ResetFrameDur();
        }

        protected void ResetFrameDur()
        {
            PrevFrameTimer = GameCore.ActiveSeconds + (1f / FrameRate);
        }

        /// <summary>
        /// Resets the animation
        /// </summary>
        public virtual void Reset()
        {
            AnimDone = false;
            CurFrame = 0;
            ResetFrameDur();
        }

        public void Update()
        {
            if (AnimDone == false)
            {
                if (GameCore.ActiveSeconds >= PrevFrameTimer)
                {
                    Progress();
                }
            }
        }

        public void Draw(float depth)
        {
            GameCore.spriteSorter.Add(Frames[CurFrame], depth);
        }

        /// <summary>
        /// Animation frame
        /// </summary>
        protected sealed class Frame : Drawable
        {
            public IntRect TextureRect = new IntRect();
            public Sprite FrameSprite = null;

            public Frame(Texture texture, IntRect texrect)
            {
                TextureRect = texrect;

                FrameSprite = Helper.CreateSprite(texture, false, TextureRect);
                FrameSprite.Scale = new Vector2f(FrameSprite.Scale.X * 3f, FrameSprite.Scale.Y * 3f);
            }

            public void Draw(RenderTarget target, RenderStates states)
            {
                FrameSprite.Draw(target, states);
            }
        }
    }
}
