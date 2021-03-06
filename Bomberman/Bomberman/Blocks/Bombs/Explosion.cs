using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Bomberman.Bombs
{
    [DataContract()]
    public class Explosion : GameObject
    {
        //static Texture2D tex;
        private Texture2D tex;
        int range;
        Bomb bomb;
        List<Point> fire = new List<Point>();
        static Point[] dirs = new Point[] { new Point(0, 1), new Point(0, -1), new Point(-1, 0), new Point(1, 0) };

        Point position;

        [DataMember()]
        public Point Position
        {
            set
            {
                position = value;
            }
            get { return position; }
        }

        [DataMember()]
        public List<Point> Fire{
            get { return fire; }
            set { fire = value; }
        }
        public void Load(ContentManager content)
        {
            tex = content.Load<Texture2D>("explosion");
        }
        private void Destroy(int x, int y)
        {
            //Debug.WriteLine(Engine.Instance.Maze.Destroy((uint)x, (uint)y));
            //if (Engine.Instance.Maze.Destroy((uint)x, (uint)y))
            //{
            //    Engine.Instance.ScoreHolder.DestroyedObstacle();
            //}
            Engine.Instance.Maze.Destroy((uint)x, (uint)y);
            foreach (var en in Engine.Instance.Enemies)
            {
                if (en.Position.X == x && en.Position.Y == y)
                {
                    en.IsDead = true;
                    if (bomb.playered)
                        Engine.Instance.ScoreHolder.KilledEnemy();
                }
            }
        }

        public Explosion(Bomb b)
        {
            x = b.Position.X;
            y = b.Position.Y;
            position = new Point(b.Position.X, b.Position.Y);
            this.range = b.range;
            bomb = b;

            Destroy(x, y);
            fire.Add(new Point(x, y));

            for (int i = 0; i < dirs.Length; i++)
            {
                for (int j = 1; j <= range; j++)
                {
                    int dx = x + j * dirs[i].X;
                    int dy = y + j*dirs[i].Y;
                    if( dx >= Maze.Width || dx < 0 || dy >= Maze.Height || dy < 0 )
                        break;
                    if (!Engine.Instance.Maze.isSolid((uint)dx, (uint)dy))
                    {
                        fire.Add(new Point(dx, dy));
                    }
                    else {
                        break;
                    }

                    if (!Engine.Instance.Maze.isPassable((uint)dx, (uint)dy))
                    {
                        Engine.Instance.Maze.Destroy((uint)dx, (uint)dy);
                        //break;
                    }
                    else
                    {
                        Destroy(dx, dy);
                    }
                }
            }
        }

        public override void Update(GameTime gt)
        {
        }


        public override void Draw(SpriteBatch spriteBatch, ContentManager contentManager)
        {
            //if (tex == null || tex.GraphicsDevice != spriteBatch.GraphicsDevice)
            Load(contentManager);

             Point point = StdGameScaler.Instance.cast(position);
            spriteBatch.Draw(tex, new Rectangle(point.X * Maze.BlockWidth, point.Y * Maze.BlockHeight, Maze.BlockWidth, Maze.BlockHeight), Color.White);
            foreach (Point p in fire)
            {
                point = StdGameScaler.Instance.cast(p);
                spriteBatch.Draw(tex, new Rectangle(point.X * Maze.BlockWidth, point.Y * Maze.BlockHeight, Maze.BlockWidth, Maze.BlockHeight), Color.White);
            }
        }
    }
}
