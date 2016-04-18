using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using neon2d;
using neon2d.Physics;

namespace Super_Morph
{
    public class GameObjects
    {
        public struct mons
        {
            public int x;
            public int y;
            public int type;
            public int spawnTime;
            public int health;

            public mons(int x, int y, int type)
            {
                this.x = x;
                this.y = y;
                this.type = type;
                this.spawnTime = 0;
                switch(type)
                {
                    case 0:
                        this.health = 5;
                        break;

                    case 1:
                        this.health = 15;
                        break;

                    case 2:
                        this.health = 10;
                        break;

                    case 3:
                        this.health = 15;
                        break;

                    default:
                        this.health = 10;
                        break;
                }
            }
        }

        public struct bullet
        {
            public int x;
            public int y;
            public int type;
            public int xstrength;
            public int ystrength;
            public int speed;
            public Rect collisions;
            public int age;

            public bullet(int x, int y, int type, int xs, int ys, int speed)
            {
                this.x = x;
                this.y = y;
                this.type = type;
                this.xstrength = xs;
                this.ystrength = ys;
                this.speed = speed;
                this.collisions = new Rect(x, y, 24, 24);
                this.age = 0;
            }
        }

        public struct friendlyBullet
        {
            public int x;
            public int y;
            public int mx;
            public int my;
            public int speed;
            public int age;

            public friendlyBullet(int x, int y, int mx, int my, int speed)
            {
                this.x = x;
                this.y = y;

                this.mx = mx;
                this.my = my;

                this.speed = speed;
                this.age = 0;
            }
        }

        public struct homingmissle
        {
            public int x;
            public int y;
            public int speed;
            public Rect collisions;
            public int age;

            public homingmissle(int x, int y, int speed)
            {
                this.x = x;
                this.y = y;
                this.speed = speed;
                this.collisions = new Rect(x, y, 24, 24);
                this.age = 0;
            }
        }

        public struct explosion
        {
            public int x;
            public int y;
            public int w;
            public int h;

            public explosion(int x, int y)
            {
                this.x = x;
                this.y = y;
                this.w = 32;
                this.h = 32;
            }
        }
    }
}
