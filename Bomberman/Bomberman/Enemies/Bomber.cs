using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Bomberman
{
    [DataContract()]
    public class Bomber : Enemy
    {
        Bomb bomb;

        [DataMember()]
        public Bomb @Bomb
        {
            get { return bomb; }
            set { bomb = value; }
        }

        Queue<int> track = new Queue<int>();

        [DataMember()]
        public Queue<int> @Track
        {
            get { return track; }
            set { track = value; }
        }

        public Bomber() : base()
        {
            Point p;
            int tries = 0;
            do
            {
                p = new Point(random.Next((int)Maze.Width - 1), random.Next((int)Maze.Height - 1));
                tries++;
            } while ((Math.Max(Math.Abs(p.X - Engine.Instance.Player.Position.X), Math.Abs(p.Y - Engine.Instance.Player.Position.Y)) < 4 ||
                !Engine.Instance.Maze.isPassable((uint)p.X, (uint)p.Y) ||
                Engine.Instance.Maze.freeSpace((uint)p.X, (uint)p.Y) < 5) &&
                tries < 1000000);

            if (tries < 1000000)
                position = p;

            speed = 0.0015f;
        }

        public Bomber(int x, int y)
            : base(x, y)
        {
            speed = 0.0015f;
        }

        public override void Load(ContentManager content)
        {
            tex = new Texture2D[1];
            tex[0] = content.Load<Texture2D>("ghost2");
        }

        protected override bool canPass(int x, int y)
        {
            if(bomb == null || bomb.state == Bomb.State.Dead)
                return Engine.Instance.Maze.isPassable((uint)x, (uint)y) && !Bomb.exist(x, y);
            else
                return Engine.Instance.Maze.isPassable((uint)x, (uint)y) && !Bomb.exist(x, y) &&
                    Math.Abs(x - bomb.Position.X) + Math.Abs(y - bomb.Position.Y) > 
                    Math.Abs(this.position.X - bomb.Position.X) + Math.Abs(this.position.Y - bomb.Position.Y);
        }

        private bool placeBomb()
        {
            for (int i = 0; i < 4; i++)
            {
                Point dir = dirs[i];
                Point p = add(position, dir);
                if ( canPass( p.X, p.Y ) )
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Point dir_p = dirs[j];
                        Point q = add(p, dir_p);
                        if (canPass(q.X, q.Y) && !q.Equals(position))
                        {
                            bomb = new Bomb(Position.X, Position.Y, 1, false);
                            Engine.Instance.AddObject(bomb);
                            Debug.WriteLine(i + " " + j);
                            track.Enqueue(i);
                            track.Enqueue(j);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        protected override void step()
        {
            Engine.Instance.Maze.Destroy((uint)position.X, (uint)position.Y);

            Point p = add(position, dirs[(int)faced]);
            if (track.Count > 0)
            {
                faced = (Faced)(track.Dequeue() % 4);
                offset = 1.0f;
            }
            else if ((!canPass(p.X, p.Y)) || random.Next(6) == 0)
            {
                if (Engine.Instance.Maze.Block[(uint)p.X, (uint)p.Y] == Obstacle.Instance && (bomb == null || bomb.state == Bomb.State.Dead))
                {
                    if (placeBomb())
                        return;
                }

                for (int it = (int)faced + (((int)faced + 1) % 2) + 1; it < (int)faced + (((int)faced + 1) % 2) + 5; it++)
                {
                    p = add(position, dirs[it % 4]);
                    if (canPass(p.X, p.Y))
                    {
                        faced = (Faced)(it % 4);
                        offset = 1.0f;
                        break;
                    }
                    else if (Engine.Instance.Maze.Block[(uint)p.X, (uint)p.Y] == Obstacle.Instance && (bomb == null || bomb.state == Bomb.State.Dead))
                    {
                        if (placeBomb())
                            return;
                    }
                }
            }
            else
            {
                offset = 1.0f;
            }
        }
    }
}
