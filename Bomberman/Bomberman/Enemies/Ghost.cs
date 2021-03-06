using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace Bomberman
{
    [DataContract()]
    public class Ghost : Enemy
    {
        public Ghost() : base()
        {
            speed = 0.0015f;
        }

        public Ghost(int x, int y)
            : base(x, y)
        {
            speed = 0.0015f;
        }

        public override void Load(ContentManager content)
        {
            tex = new Texture2D[1];
            tex[0] = content.Load<Texture2D>("ghost1");
        }

        protected override bool canPass(int x, int y)
        {
            bool is_bomb = false;
            foreach (var bomb in Engine.Instance.Bombs) { 
                if( bomb.Position.X == x && bomb.Position.Y == y ){
                    is_bomb = true;
                    break;
                }
            }
            return Engine.Instance.Maze.Block[(uint)x, (uint)y] != Wall.Instance && !is_bomb;
        }

        protected override void castAI()
        {
            if (Math.Max(Math.Abs(this.position.X - Engine.Instance.Player.Position.X), Math.Abs(this.position.Y - Engine.Instance.Player.Position.Y)) < 4)
            {
                Point p = findPath();
                if (p.Y > 0)
                    faced = Faced.South;
                else if (p.Y < 0)
                    faced = Faced.North;
                else if (p.X < 0)
                    faced = Faced.West;
                else if (p.X > 0)
                    faced = Faced.East;
                if (p.X == 0 && p.Y == 0)
                    step();
                else
                    offset = 1.0f;
            }
            else
            {
                step();
            }
        }
    }
}
