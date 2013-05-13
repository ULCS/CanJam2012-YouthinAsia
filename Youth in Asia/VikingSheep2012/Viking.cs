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
    class Viking
    {
        Vector2 pos;
        Vector2 vel;
        Texture2D fall;
        Texture2D jump;
        Texture2D run;
        Texture2D stay;
        SpriteBatch spriteBatch;
        PlayerIndex index;
        KeyboardState kbs;
        KeyboardState old_kbs;
        GamePadState gps;
        GamePadState old_gps;
        const float acceleration = 0.84f;
        const float friction = 0.21f;
        const float gravity = 0.63f;
        const float maxSpeed = 10.5f;
        const float jumpSpeed = 26.0f;
        const float releaseSpeed = 10.5f;
        float animt = 0.0f;
        byte state = 0;
        byte sprite = 0;
        bool onGround = true;
        bool jumping = false;
        bool faceRight = true;

        public Viking(Vector2 _pos, Vector2 _vel, Texture2D _fall, Texture2D _jump, Texture2D _run, Texture2D _stay, SpriteBatch _spriteBatch, PlayerIndex _index)
        {
            pos = _pos;
            vel = _vel;
            fall = _fall;
            jump = _jump;
            run = _run;
            stay = _stay;
            spriteBatch = _spriteBatch;
            index = _index;
        }

        public void NewIndex(PlayerIndex _index)
        {
            index = _index;
        }

        public Vector2 GetPos()
        {
            return pos;
        }

        public Vector2 GetVel()
        {
            return vel;
        }

        public Vector3 GetState()
        {
            byte fRnum = 0;
            if (faceRight)
                fRnum = 1;
            return new Vector3(state, sprite, fRnum);
        }

        public void SetPos(Vector2 _pos)
        {
            pos = _pos;
        }

        public void SetVel(Vector2 _vel)
        {
            vel = _vel;
        }

        public void SetGround(bool _onGround)
        {
            onGround = _onGround;
        }

        public void SetState(Vector3 _state)
        {
            state = (byte)_state.X;
            sprite = (byte)_state.Y;
            animt = 0;
            faceRight = (_state.Z == 1);
            if (state > 1)
                onGround = false;
        }

        private void Accelerate(float amount)
        {
            vel.X += acceleration * amount;
            if (vel.X > maxSpeed)
                vel.X = maxSpeed;
            if (vel.X < -maxSpeed)
                vel.X = -maxSpeed;
        }

        private void Jump()
        {
            vel.Y = -jumpSpeed;
            onGround = false;
            jumping = true;
        }

        public byte Update(Sheep[] sheep)
        {
            byte hitType = 255;
            kbs = Keyboard.GetState();
            gps = GamePad.GetState(index);
            bool flag = true;
            if (kbs.IsKeyDown(Keys.A) || kbs.IsKeyDown(Keys.Left) || gps.IsButtonDown(Buttons.DPadLeft))
            {
                Accelerate(-1.0f);
                if (!onGround || vel.X < 0)
                    faceRight = false;
                flag = false;
            }
            if (kbs.IsKeyDown(Keys.D) || kbs.IsKeyDown(Keys.Right) || gps.IsButtonDown(Buttons.DPadRight))
            {
                Accelerate(1.0f);
                if (!onGround || vel.X > 0)
                    faceRight = true;
                flag = false;
            }
            if (flag)
            {
                if (Math.Abs(gps.ThumbSticks.Left.X) > 0.1f)
                {
                    Accelerate(gps.ThumbSticks.Left.X);
                    if (!onGround)
                    {
                        if (gps.ThumbSticks.Left.X < 0)
                            faceRight = false;
                        else
                            faceRight = true;
                    }
                    else
                    {
                        if (vel.X < 0)
                            faceRight = false;
                        else
                            faceRight = true;
                    }
                }
            }
            if (vel.X > 0)
            {
                vel.X -= friction;
                if (vel.X < 0)
                    vel.X = 0;
            }
            else if (vel.X < 0)
            {
                vel.X += friction;
                if (vel.X > 0)
                    vel.X = 0;
            }
            if (onGround)
            {
                if ((kbs.IsKeyDown(Keys.W) || kbs.IsKeyDown(Keys.Up) || kbs.IsKeyDown(Keys.Space) || gps.IsButtonDown(Buttons.A)) && old_kbs.IsKeyUp(Keys.W) && old_kbs.IsKeyUp(Keys.Up) && old_kbs.IsKeyUp(Keys.Space) && old_gps.IsButtonUp(Buttons.A))
                    Jump();
            }
            else
            {
                vel.Y += gravity;
                if (jumping)
                {
                    if (vel.Y > -releaseSpeed)
                        jumping = false;
                    else if (kbs.IsKeyUp(Keys.W) && kbs.IsKeyUp(Keys.Up) && kbs.IsKeyUp(Keys.Space) && gps.IsButtonUp(Buttons.A))
                    {
                        vel.Y = -releaseSpeed;
                        jumping = false;
                    }
                }
            }
            pos += vel;
            if (pos.Y > 0)
            {
                pos.Y = 0;
                vel.Y = 0;
                onGround = true;
            }
            if (pos.X > 640)
                pos.X -= 1280;
            if (pos.X < -640)
                pos.X += 1280;
            if (onGround)
            {
                if (Math.Abs(vel.X) < 0.1f)
                    state = 0;
                else
                {
                    if (state == 1)
                    {
                        animt += Math.Abs(vel.X);
                        if (animt > 42)
                        {
                            sprite++;
                            if (sprite > 4)
                                sprite -= 4;
                            animt = 0;
                        }
                    }
                    else
                    {
                        state = 1;
                        sprite = 1;
                        animt = 0;
                    }
                }
            }
            else
            {
                if (vel.Y < 0)
                {
                    if (state == 2)
                    {
                        animt += 1;
                        if (animt > 5)
                        {
                            sprite++;
                            if (sprite > 2)
                                sprite = 2;
                            animt = 0;
                        }
                    }
                    else
                    {
                        state = 2;
                        sprite = 0;
                        animt = 0;
                    }
                }
                else
                {
                    if (state == 3)
                    {
                        animt += 1;
                        if (animt > 5)
                        {
                            sprite = (byte)(1 - sprite);
                            animt = 0;
                        }
                    }
                    else
                    {
                        state = 3;
                        sprite = 0;
                        animt = 0;
                    }
                }
            }
            for (byte i = 0; i < 42; i++)
            {
                Vector2 tempPos = sheep[i].GetPos();
                if (pos.X - tempPos.X > 640)
                    tempPos.X += 1280;
                if (tempPos.X - pos.X > 640)
                    tempPos.X -= 1280;
                if (Math.Abs(pos.X - tempPos.X) < 84 && pos.Y > tempPos.Y && pos.Y - 84 < tempPos.Y && vel.Y > 0 && sheep[i].GetExists())
                {
                    sheep[i].Activate();
                    vel.Y = -jumpSpeed;
                    hitType = sheep[i].GetBehaviour();
                }
            }
            old_kbs = kbs;
            old_gps = gps;
            return hitType;
        }

        public void Draw(Vector2 c_pos, bool fullScreen)
        {
            float scale = 1.0f;
            if (!fullScreen)
            {
                c_pos -= new Vector2(310, 192);
                scale = 0.5f;
            }
            SpriteEffects se = SpriteEffects.FlipHorizontally;
            if (faceRight)
                se = SpriteEffects.None;
            switch (state)
            {
                case 0:
                    spriteBatch.Draw(run, (pos - c_pos) * scale, new Rectangle(2, 0, 163, 305), Color.White, 0, new Vector2(82, 285), scale * 0.5f, se, 0);
                    spriteBatch.Draw(run, (pos + new Vector2(1280, 0) - c_pos) * scale, new Rectangle(2, 0, 167, 305), Color.White, 0, new Vector2(82, 285), scale * 0.5f, se, 0);
                    spriteBatch.Draw(run, (pos - new Vector2(1280, 0) - c_pos) * scale, new Rectangle(2, 0, 167, 305), Color.White, 0, new Vector2(82, 285), scale * 0.5f, se, 0);
                    break;
                case 1:
                    spriteBatch.Draw(run, (pos - c_pos) * scale, new Rectangle(172 * sprite + 2, 0, 163, 305), Color.White, 0, new Vector2(82, 285), scale * 0.5f, se, 0);
                    spriteBatch.Draw(run, (pos + new Vector2(1280, 0) - c_pos) * scale, new Rectangle(172 * sprite + 2, 0, 163, 305), Color.White, 0, new Vector2(82, 285), scale * 0.5f, se, 0);
                    spriteBatch.Draw(run, (pos - new Vector2(1280, 0) - c_pos) * scale, new Rectangle(172 * sprite + 2, 0, 163, 305), Color.White, 0, new Vector2(82, 285), scale * 0.5f, se, 0);
                    break;
                case 2:
                    spriteBatch.Draw(jump, (pos - c_pos) * scale, new Rectangle(172 * sprite + 2, 0, 163, 305), Color.White, 0, new Vector2(82, 285), scale * 0.5f, se, 0);
                    spriteBatch.Draw(jump, (pos + new Vector2(1280, 0) - c_pos) * scale, new Rectangle(172 * sprite + 2, 0, 163, 305), Color.White, 0, new Vector2(82, 285), scale * 0.5f, se, 0);
                    spriteBatch.Draw(jump, (pos - new Vector2(1280, 0) - c_pos) * scale, new Rectangle(172 * sprite + 2, 0, 163, 305), Color.White, 0, new Vector2(82, 285), scale * 0.5f, se, 0);
                    break;
                case 3:
                    spriteBatch.Draw(fall, (pos - c_pos) * scale, new Rectangle(172 * sprite + 2, 0, 163, 305), Color.White, 0, new Vector2(82, 285), scale * 0.5f, se, 0);
                    spriteBatch.Draw(fall, (pos + new Vector2(1280, 0) - c_pos) * scale, new Rectangle(172 * sprite + 2, 0, 163, 305), Color.White, 0, new Vector2(82, 285), scale * 0.5f, se, 0);
                    spriteBatch.Draw(fall, (pos - new Vector2(1280, 0) - c_pos) * scale, new Rectangle(172 * sprite + 2, 0, 163, 305), Color.White, 0, new Vector2(82, 285), scale * 0.5f, se, 0);
                    break;
            }
        }
    }
}
