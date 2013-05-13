using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace VikingSheep2012
{
    class Sheep
    {
        Vector2 pos;
        Vector2 vel;
        Texture2D gfx;
        SpriteBatch spriteBatch;
        byte type;
        byte timer;
        bool faceRight;
        bool exists = true;
        bool active;


        public Sheep(Vector2 _pos, Vector2 _vel, Texture2D _gfx, SpriteBatch _spriteBatch, byte _type, byte _timer, bool _faceRight)
        {
            Change(_pos, _vel, _gfx, _type, _timer, _faceRight);
            spriteBatch = _spriteBatch;
        }

        public void Change(Vector2 _pos, Vector2 _vel, Texture2D _gfx, byte _type, byte _timer, bool _faceRight)
        {
            pos = _pos;
            vel = _vel;
            type = _type;
            timer = _timer;
            gfx = _gfx;
            faceRight = _faceRight;
            if (vel.X > 0)
                faceRight = true;
            if (vel.X < 0)
                faceRight = false;
        }

        public Vector2 GetPos()
        {
            return pos;
        }

        public Vector2 GetVel()
        {
            return vel;
        }

        public bool GetExists()
        {
            return exists;
        }

        public Vector4 GetState()
        {
            int existsNum = 0;
            if (exists)
                existsNum = 1;
            int activeNum = 0;
            if (active)
                activeNum = 1;
            return new Vector4(type, timer, existsNum, activeNum);
        }

        public byte GetBehaviour()
        {
            return type;
        }

        public void SetPos(Vector2 _pos)
        {
            pos = _pos;
        }

        public void SetVel(Vector2 _vel)
        {
            vel = _vel;
        }

        public void SetState(Vector4 _state)
        {
            type = (byte)_state.X;
            timer = (byte)_state.Y;
            exists = (_state.Z == 1);
            active = (_state.W == 1);
        }

        public void SetGfx(Texture2D _gfx)
        {
            gfx = _gfx;
        }

        public void Activate()
        {
            switch (type)
            {
                case 2:
                case 3:
                    active = true;
                    break;
            }
        }

        public void Explode(Viking viking)
        {
            Vector2 diff = (pos + new Vector2(0, 60)) - viking.GetPos();
            Vector2 explvel = Vector2.Zero;
            if (diff.Length() < 441)
                explvel = (diff / diff.Length()) * (441 - diff.Length()) * 0.05f;
            viking.SetVel(viking.GetVel() - explvel);
            exists = false;
            active = false;
        }

        public bool Update(Viking viking)
        {
            bool pop = false;
            pos += vel;
            if (pos.X > 640)
                pos.X -= 1280;
            if (pos.X < -640)
                pos.X += 1280;
            if (pos.Y > -60)
            {
                pos.Y = -60;
                vel.Y = -vel.Y * 0.95f;
            }
            switch (type)
            {
                case 2:
                    if (active)
                    {
                        timer--;
                        if (timer == 0)
                        {
                            Explode(viking);
                            pop = true;
                        }
                    }
                    break;
                case 3:
                    if (active)
                        vel.Y += 0.42f;
                    break;
            }
            return pop;
        }

        public void Draw(Vector2 c_pos, bool fullScreen)
        {
            if (exists)
            {
                float scale = 1.0f;
                if (!fullScreen)
                {
                    c_pos -= new Vector2(310, 192);
                    scale = 0.5f;
                }
                SpriteEffects se = SpriteEffects.None;
                if (faceRight)
                    se = SpriteEffects.FlipHorizontally;
                int sprite = 0;
                if (type == 2 && timer < 15)
                {
                    switch (timer)
                    {
                        case 1:
                        case 2:
                        case 3:
                            sprite = 4;
                            break;
                        case 4:
                        case 5:
                        case 6:
                            sprite = 3;
                            break;
                        case 7:
                        case 8:
                        case 9:
                            sprite = 2;
                            break;
                        case 10:
                        case 11:
                        case 12:
                            sprite = 1;
                            break;
                    }
                }
                spriteBatch.Draw(gfx, (pos - c_pos) * scale, new Rectangle(sprite * 170, 0, 170, 121), Color.White, 0, new Vector2(85, 0), scale * 0.5f, se, 0);
                spriteBatch.Draw(gfx, (pos + new Vector2(1280, 0) - c_pos) * scale, new Rectangle(sprite * 170, 0, 170, 121), Color.White, 0, new Vector2(85, 0), scale * 0.5f, se, 0);
                spriteBatch.Draw(gfx, (pos - new Vector2(1280, 0) - c_pos) * scale, new Rectangle(sprite * 170, 0, 170, 121), Color.White, 0, new Vector2(85, 0), scale * 0.5f, se, 0);
            }
        }
    }
}
