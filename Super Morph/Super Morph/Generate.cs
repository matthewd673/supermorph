using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using neon2d.Physics;

namespace Super_Morph
{
    public class Generate
    {

        public static int[,] map = new int[25, 25];
        public static ArrayList collisionmap = new ArrayList();
        public static ArrayList monsters = new ArrayList();
        public static ArrayList bullets = new ArrayList();
        public static ArrayList homing = new ArrayList();
        public static ArrayList friendly = new ArrayList();
        public static ArrayList explosions = new ArrayList();
        public static int bulletSpeed = 5;

        public static void generateMap()
        {

            //generate map
            Random rng = new Random();
            for (int i = 0; i <= 24; i++)
            {
                for (int j = 0; j <= 24; j++)
                {
                    int tilevalue = rng.Next(38);
                    if (tilevalue > 33)
                    {
                        map[i, j] = 1;
                    }
                    else if (tilevalue == 1)
                    {
                        map[i, j] = 2;
                        collisionmap.Add(new Rect(i * 32, j * 32, 32, 32));
                    }
                    else
                    {
                        map[i, j] = 0;
                    }
                }
            }
            map[0, 0] = 0;
        }


        
        public static void spawnMonster()
        {
            bool canSpawn = true;
            Random rng = new Random();
            int monsx = rng.Next(801);
            int monsy = rng.Next(801);
            Rect monsLoc = new Rect(monsx, monsy, 24, 24);
            for (int i = 0; i <= collisionmap.Count - 1; i++)
            {
                if (collisionmap[i].GetType() == typeof(Rect))
                {
                    Rect placeholder = (Rect)collisionmap[i];
                    if (monsLoc.intersects(placeholder))
                    {
                        canSpawn = false;
                    }
                }
            }

            if (canSpawn)
            {
                int monsType = rng.Next(4);
                monsters.Add(new GameObjects.mons(monsx, monsy, monsType));
                if(Program.characterHealth > 0)
                {
                    Program.monstersSpawned++;
                }
                if(monsType == 1 || monsType == 2 || monsType == 3)
                {
                    shootBullets(monsx, monsy, monsType);
                }
                
            }

        }

        

        public static void shootBullets(int monsX, int monsY, int monsType)
        {
            switch(monsType)
            {

                case 0:
                    //homing bullets
                    homing.Add(new GameObjects.homingmissle(monsX, monsY, 6));
                    homing.Add(new GameObjects.homingmissle(monsX, monsY, 6));
                    homing.Add(new GameObjects.homingmissle(monsX, monsY, 6));
                    if (Program.characterHealth > 0)
                    {
                        Program.bulletsShot += 3;
                    }
                    break;

                case 1:
                    //fire bullets
                    bullets.Add(new GameObjects.bullet(monsX, monsY, 1, 0, 1, bulletSpeed));
                    bullets.Add(new GameObjects.bullet(monsX, monsY, 1, -1, 0, bulletSpeed));
                    bullets.Add(new GameObjects.bullet(monsX, monsY, 1, 0, -1, bulletSpeed));
                    bullets.Add(new GameObjects.bullet(monsX, monsY, 1, 1, 0, bulletSpeed));
                    if (Program.characterHealth > 0)
                    {
                        Program.bulletsShot += 4;
                    }
                    break;

                case 2:
                    //fire & healing bullets
                    bullets.Add(new GameObjects.bullet(monsX, monsY, 1, 1, 1, bulletSpeed));
                    bullets.Add(new GameObjects.bullet(monsX, monsY, 2, 1, 0, bulletSpeed));
                    bullets.Add(new GameObjects.bullet(monsX, monsY, 1, 1, -1, bulletSpeed));
                    bullets.Add(new GameObjects.bullet(monsX, monsY, 2, 0, -1, bulletSpeed));
                    bullets.Add(new GameObjects.bullet(monsX, monsY, 1, -1, -1, bulletSpeed));
                    bullets.Add(new GameObjects.bullet(monsX, monsY, 2, -1, 0, bulletSpeed));
                    bullets.Add(new GameObjects.bullet(monsX, monsY, 1, -1, 1, bulletSpeed));
                    if (Program.characterHealth > 0)
                    {
                        Program.bulletsShot += 7;
                    }
                    break;

                case 3:
                    //diagonal spray pattern
                    bullets.Add(new GameObjects.bullet(monsX, monsY, 1, -1, 1, bulletSpeed + 2));
                    bullets.Add(new GameObjects.bullet(monsX, monsY, 1, 1, 1, bulletSpeed + 2));
                    bullets.Add(new GameObjects.bullet(monsX, monsY, 1, 1, -1, bulletSpeed + 2));
                    bullets.Add(new GameObjects.bullet(monsX, monsY, 1, -1, -1, bulletSpeed + 2));
                    if (Program.characterHealth > 0)
                    {
                        Program.bulletsShot += 4;
                    }
                    break;
            }
        }

        public static void shootFriendly(int playerX, int playerY, int mouseX, int mouseY)
        {
            friendly.Add(new GameObjects.friendlyBullet(playerX, playerY, mouseX, mouseY, 8));
            Program.bulletsShot += 1;
        }

        public static GameObjects.friendlyBullet simulateFriendly(GameObjects.friendlyBullet f)
        {

            GameObjects.friendlyBullet tempBullet = f;

            //move it
            if(tempBullet.mx > tempBullet.x)
            {
                tempBullet.x += tempBullet.speed;
            }
            if(tempBullet.mx < tempBullet.x)
            {
                tempBullet.x -= tempBullet.speed;
            }
            if(tempBullet.my > tempBullet.y)
            {
                tempBullet.y += tempBullet.speed;
            }
            if(tempBullet.my < tempBullet.y)
            {
                tempBullet.y -= tempBullet.speed;
            }
            tempBullet.age++;

            return tempBullet;
        }

        public static void simulateBullets(GameObjects.bullet b)
        {

            b.age++;

            switch(b.xstrength)
            {
                case -1:
                    b.collisions.x -= b.speed;
                    break;

                case 1:
                    b.collisions.x += b.speed;
                    break;
            }

            switch(b.ystrength)
            {
                case -1:
                    b.collisions.y -= b.speed;
                    break;

                case 1:
                    b.collisions.y += b.speed;
                    break;
            }

        }
        
        public static void spawnExplosion(int x, int y, neon2d.Prop particle)
        {
            explosions.Add(new GameObjects.explosion(x, y));
        }

    }
}
