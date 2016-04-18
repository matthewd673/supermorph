using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using neon2d;
using neon2d.Physics;

namespace Super_Morph
{
    class Program
    {

        public static Window win;
        public static Scene scene;

        //tiles
        public static Prop grass;
        public static Prop sand;
        public static Prop rock;

        //projectiles
        public static Prop bullet;
        public static Prop fireball;
        public static Prop heal;
        public static Prop friendly;

        //characters
        public static Prop quickCharacter;
        public static Prop shootUpLeftCharacter;
        public static Prop shootUpRightCharacter;
        public static Prop shootDownCharacter;
        public static Prop giantCharacter;

        //character related stuff
        public static Prop scepter;

        //ui junk
        public static Prop scoreSheet;
        public static Prop singlePlayerButton;
        public static Prop neon2dButton;
        public static Prop playAgainButton;
        public static Prop toMenuButton;

        public static Prop logo;

        //enemies
        public static Prop spitter;
        public static Prop firebelcher;
        public static Prop ancientone;
        public static Prop snakeeyes;
        public static Prop viper;

        //effects
        public static Prop explosionParticle;

        //character properties
        //legend:
        //0 = quick
        //1 = shoot
        //2 = super big
        public static int characterId = 1;
        public static Rect character;

        public static int characterSpeed = 4;
        public static int quickSpeed = 9;
        public static int shootSpeed = 4;
        public static int giantSpeed = 2;
        public static int characterHealth = 25;
        public static int maxHealth = 25;
        public static int giantBuff = 15;
        public static int maxGiantBuff = 15;

        //input stuff
        public static bool movingUp = false;
        public static bool movingDown = false;
        public static bool movingLeft = false;
        public static bool movingRight = false;

        public static bool bigDone = false;
        public static bool smallDone = false;
        public static bool giantDone = false;

        public static string switchNotice = "[E] to ghost";

        //move on (result screen) input
        public static bool shouldPlayAgain = false;
        public static bool shouldGoMenu = false;

        //title screen input
        public static bool goSingle = false;
        public static bool goNeon = false;

        //mouse input
        public static int mx;
        public static int my;

        //legend:
        //0 = title
        //1 = game
        //2 = results
        public static int sceneId = 0;

        public static int[,] map = new int[25, 25];
        public static ArrayList collisionmap = new ArrayList();
        public static ArrayList monsters = new ArrayList();
        public static ArrayList bullets = new ArrayList();
        public static ArrayList homing = new ArrayList();
        public static ArrayList explosions = new ArrayList();
        public static ArrayList friendlyBullets = new ArrayList();

        //monster spawns
        public static int spawnInterval = 70;
        public static int currentSpawnCount = 0;
        public static int intervalDecreaseTime = 0;
        public static int maxIntervalDecreaseTime = 3;
        public static int decreaseInterval = 10;

        //bullet spawns
        public static int spitterBulletSpawnInterval = 60;
        public static int firebelcherBulletSpawnInterval = 20;
        public static int ancientoneBulletSpawnInterval = 45;
        public static int snakeeyesBulletSpawnInterval = 10;
        public static int viperBulletSpawnInterval = 20;

        //statistics
        public static int bulletsShot;
        public static int monstersKilled;
        public static int hitsTaken;
        public static int timeAlive;
        public static int monstersSpawned;

        //game
        public static Game g;

        public static void start()
        {

            win.gamewindow.MouseClick += Gamewindow_MouseClick;

            string prefix = Environment.CurrentDirectory + @"\RESOURCES\";
            grass = new Prop(new Bitmap(prefix + "grass.png"), 32, 32);
            sand = new Prop(new Bitmap(prefix + "sand.png"), 32, 32);
            rock = new Prop(new Bitmap(prefix + "rock.png"), 32, 32);
            quickCharacter = new Prop(new Bitmap(prefix + "quick.png"), 24, 24);
            shootUpLeftCharacter = new Prop(new Bitmap(prefix + "shoot-up-left.png"), 32, 32);
            shootUpRightCharacter = new Prop(new Bitmap(prefix + "shoot-up-right.png"), 32, 32);
            shootDownCharacter = new Prop(new Bitmap(prefix + "shoot-down.png"), 32, 32);
            giantCharacter = new Prop(new Bitmap(prefix + "giant.png"), 32, 32);
            bullet = new Prop(new Bitmap(prefix + "bullet.png"), 24, 24);
            fireball = new Prop(new Bitmap(prefix + "fireball.png"), 24, 24);
            heal = new Prop(new Bitmap(prefix + "heal.png"), 24, 24);
            spitter = new Prop(new Bitmap(prefix + "spitter.png"), 32, 32);
            firebelcher = new Prop(new Bitmap(prefix + "firebelcher.png"), 32, 32);
            ancientone = new Prop(new Bitmap(prefix + "ancientone.png"), 32, 32);
            snakeeyes = new Prop(new Bitmap(prefix + "snakeeyes.png"), 32, 32);
            viper = new Prop(new Bitmap(prefix + "viper.png"), 32, 32);
            explosionParticle = new Prop(new Bitmap(prefix + "explosion.png"), 32, 32);
            friendly = new Prop(new Bitmap(prefix + "friendly.png"), 32, 32);
            scepter = new Prop(new Bitmap(prefix + "scepter.png"), 16, 20);
            scoreSheet = new Prop(new Bitmap(prefix + "score-sheet.png"), 672, 672);
            singlePlayerButton = new Prop(new Bitmap(prefix + "singleplayer-button.png"), 124, 32);
            neon2dButton = new Prop(new Bitmap(prefix + "neon2d-button.png"), 124, 32);
            playAgainButton = new Prop(new Bitmap(prefix + "play-again-button.png"), 124, 32);
            toMenuButton = new Prop(new Bitmap(prefix + "to-menu-button.png"), 124, 32);
            logo = new Prop(new Bitmap(prefix + "big-logo.png"), 510, 190);

            character = new Rect(0, 0, 24, 24);

            Generate.generateMap();
            map = Generate.map;
            collisionmap = Generate.collisionmap;
            Generate.spawnMonster();
            monsters = Generate.monsters;
            friendlyBullets = Generate.friendly;

            Sound music = new Sound(prefix + "music.wav");
            scene.playSound(music, true);
        }

        private static void Gamewindow_MouseClick(object sender, MouseEventArgs e)
        {
            triggerClick();
        }

        public static void update()
        {

            switch (sceneId)
            {
                case 0:
                    characterHealth = 0;
                    titleScreen();
                    break;

                case 1:
                    if(spawnInterval == 47)
                    {
                        //47 is very specific, so it won't accidentally trigger
                        spawnInterval = 70;
                    }
                    gameScreen();
                    break;

                case 2:
                    resultScreen(monstersKilled, bulletsShot);
                    break;

                default:
                    titleScreen();
                    break;
            }
        }

        static void Main(string[] args)
        {
            win = new Window(800, 800, "Super Morph");
            scene = new Scene(win);
            g = new Game(win, scene, new Action(update));
            start();
            g.runGame();
        }

        public static void titleScreen()
        {
            titleInput();
            gameScreen();
            scene.render(logo, 125, 50);
            scene.render(singlePlayerButton, 338, 350);
            scene.render(neon2dButton, 338, 460);
        }

        public static void titleInput()
        {
            if(scene.readKeyDown(Keys.D1))
            {
                goSingle = true;
            }
            if(scene.readKeyDown(Keys.D2))
            {
                //multiplayer stuff
                goNeon = true;
            }
            
            //reset everything
            if (scene.keyUp())
            {
                goSingle = false;
                goNeon = false;
            }

            //interpret input
            if(goSingle)
            {
                resetGame();
                gameScreen();
            }
            if(goNeon)
            {
                Process.Start("https://neon2d.github.io");
            }
        }

        public static void gameScreen()
        {

            if (sceneId == 1)
            {
                checkInput();
            }
            if(characterHealth > 0)
            {
                timeAlive++;
            }

            //render stuff
            for(int i = 0; i <= 24; i++)
            {
                for(int j = 0; j <= 24; j++)
                {
                    switch (map[i, j])
                    {
                        case 0:
                            scene.render(grass, i * 32, j * 32);
                            break;

                        case 1:
                            scene.render(sand, i * 32, j * 32);
                            break;

                        case 2:
                            scene.render(grass, i * 32, j * 32);
                            scene.render(rock, i * 32, j * 32);
                            break;

                        default:
                            scene.render(grass, i * 32, j * 32);
                            break;
                    }
                }
            }

            //shoot bullets (monsters)
            for(int i = 0; i <= monsters.Count - 1; i++)
            {
                if (monsters[i].GetType() == typeof(GameObjects.mons))
                {
                    GameObjects.mons placeholder = (GameObjects.mons)monsters[i];
                    switch(placeholder.type)
                    {
                        case 0:
                            if (placeholder.spawnTime == spitterBulletSpawnInterval)
                            {
                                Generate.shootBullets(placeholder.x, placeholder.y, 0);
                                placeholder.spawnTime = -1;
                            }
                            break;

                        case 1:
                            if (placeholder.spawnTime == firebelcherBulletSpawnInterval)
                            {
                                Generate.shootBullets(placeholder.x, placeholder.y, 1);
                                placeholder.spawnTime = -1;
                            }
                            break;

                        case 2:
                            if (placeholder.spawnTime == ancientoneBulletSpawnInterval)
                            {
                                Generate.shootBullets(placeholder.x, placeholder.y, 2);
                                placeholder.spawnTime = -1;
                            }
                            break;

                        case 3:
                            if (placeholder.spawnTime == snakeeyesBulletSpawnInterval)
                            {
                                Generate.shootBullets(placeholder.x, placeholder.y, 3);
                                placeholder.spawnTime = -1;
                            }
                            break;

                        case 4:
                            if (placeholder.spawnTime == viperBulletSpawnInterval)
                            {
                                Generate.shootBullets(placeholder.x, placeholder.y, 4);
                                placeholder.spawnTime = -1;
                            }
                            break;
                    }
                    placeholder.spawnTime++;

                    //finally, write values
                    monsters[i] = placeholder;
                }
            }

            if (characterHealth > 0)
            {
                if (characterId == 0)
                {
                    scene.render(quickCharacter, (int)character.x, (int)character.y);
                }
                if (characterId == 1)
                {
                    if (my > character.y)
                    {
                        scene.render(shootDownCharacter, (int)character.x, (int)character.y);
                    }
                    else
                    {
                        if (mx < character.x)
                        {
                            scene.render(shootUpLeftCharacter, (int)character.x, (int)character.y);
                        }
                        if (mx > character.x)
                        {
                            scene.render(shootUpRightCharacter, (int)character.x, (int)character.y);
                        }
                    }
                }
                if (characterId == 2)
                {
                    scene.render(giantCharacter, (int)character.x, (int)character.y);
                }
            }

            //simulate bullets
            bullets = Generate.bullets;
            homing = Generate.homing;

            for (int i = 0; i <= bullets.Count - 1; i++)
            {
                if (bullets[i].GetType() == typeof(GameObjects.bullet))
                {
                    GameObjects.bullet placeholder = (GameObjects.bullet)bullets[i];
                    Generate.simulateBullets(placeholder);
                    switch (placeholder.type)
                    {
                        case 0:
                            if (placeholder.age > 60)
                            {
                                bullets.Remove(placeholder);
                            }
                            else
                            {
                                scene.render(bullet, (int)placeholder.collisions.x, (int)placeholder.collisions.y);
                                placeholder.age++;
                                //schreiben die wert
                                bullets[i] = placeholder;
                                Generate.bullets = bullets;
                            }
                            break;

                        case 1:
                            if (placeholder.age > 60)
                            {
                                bullets.Remove(placeholder);
                                placeholder.age++;
                            }
                            else
                            {
                                scene.render(fireball, (int)placeholder.collisions.x, (int)placeholder.collisions.y);
                                placeholder.age++;
                                //schreiben die wert
                                bullets[i] = placeholder;
                                Generate.bullets = bullets;
                            }
                            break;

                        case 2:
                            if (placeholder.age > 60)
                            {
                                bullets.Remove(placeholder);
                            }
                            else
                            {
                                scene.render(heal, (int)placeholder.collisions.x, (int)placeholder.collisions.y);
                                placeholder.age++;
                                //schreiben die wert
                                bullets[i] = placeholder;
                                Generate.bullets = bullets;
                            }
                            break;
                    }
                }
                
            }

            //homing bullets
            for (int i = 0; i <= homing.Count - 1; i++)
            {
                if (homing[i].GetType() == typeof(GameObjects.homingmissle))
                {
                    GameObjects.homingmissle placeholder = (GameObjects.homingmissle)homing[i];
                    simulateHoming(placeholder);
                    if (placeholder.age > 30)
                    {
                        bullets.Remove(placeholder);
                        //save this too
                        Generate.homing = homing;
                    }
                    else
                    {
                        scene.render(bullet, (int)placeholder.collisions.x, (int)placeholder.collisions.y);
                        placeholder.age++;
                        //schreiben die wert
                        homing[i] = placeholder;
                        Generate.homing = homing;
                    }
                }

            }

            //check bullet collisions

            //with a character
            for (int i = 0; i <= bullets.Count - 1; i++)
            {
                if(bullets[i].GetType() == typeof(GameObjects.bullet))
                {
                    GameObjects.bullet b = (GameObjects.bullet)bullets[i];
                    if(b.collisions.intersects(character))
                    {
                        bullets.Remove(b);
                        Generate.bullets = bullets;
                        if (b.type == 0 || b.type == 1)
                        {
                            if (characterId != 2)
                            {
                                characterHealth -= 5;
                            }
                            else
                            {
                                if (giantBuff > 0)
                                {
                                    giantBuff -= 5;
                                }
                                else
                                {
                                    characterHealth -= 5;
                                }
                            }
                            if (characterHealth > 0)
                            {
                                hitsTaken++;
                            }
                        }
                        if (b.type == 2)
                        {
                            if (characterHealth > 0)
                            {
                                characterHealth += 5;
                                if (characterHealth > 25)
                                {
                                    characterHealth = 25;
                                }
                            }
                        }
                    }
                }
            }
            //now for homing
            for (int i = 0; i <= homing.Count - 1; i++)
            {
                if(homing[i].GetType() == typeof(GameObjects.homingmissle))
                {
                    GameObjects.homingmissle h = (GameObjects.homingmissle)homing[i];
                    if(h.collisions.intersects(character) && h.age < 31)
                    {
                        homing.Remove(h);
                        h.age = 1000;
                        if (i < homing.Count)
                        {
                            homing[i] = h;
                        }
                        Generate.homing = homing;
                        if (characterId != 2)
                        {
                            characterHealth -= 5;
                        }
                        else
                        {
                            if (giantBuff > 0)
                            {
                                giantBuff -= 5;
                            }
                            else
                            {
                                characterHealth -= 5;
                            }
                        }
                        if (characterHealth > 0)
                        {
                            hitsTaken++;
                        }
                    }
                }
            }
            //back to regular bullets (with rocks)
            for (int i = 0; i <= bullets.Count - 1; i++)
            {
                for (int j = 0; j <= collisionmap.Count - 1; j++)
                {
                    if (collisionmap[j].GetType() == typeof(Rect))
                    {
                        Rect rock = (Rect)collisionmap[j];
                        if (i < bullets.Count)
                        {
                            if (bullets[i].GetType() == typeof(GameObjects.bullet))
                            {
                                GameObjects.bullet b = (GameObjects.bullet)bullets[i];
                                if (b.collisions.intersects(rock))
                                {
                                    Generate.spawnExplosion((int)b.collisions.x, (int)b.collisions.y, explosionParticle);
                                    bullets.Remove(b);
                                }
                            }
                        }
                    }
                }
            }
            //and the same thing for homing
            for (int i = 0; i <= homing.Count - 1; i++)
            {
                for (int j = 0; j <= collisionmap.Count - 1; j++)
                {
                    if (collisionmap[j].GetType() == typeof(Rect))
                    {
                        Rect rock = (Rect)collisionmap[j];
                        if (i < homing.Count)
                        {
                            if(homing[i].GetType() == typeof(GameObjects.homingmissle))
                            {
                                GameObjects.homingmissle h = (GameObjects.homingmissle)homing[i];
                                if(h.collisions.intersects(rock))
                                {
                                    Generate.spawnExplosion((int)h.collisions.x, (int)h.collisions.y, explosionParticle);
                                    homing.Remove(h);
                                    Generate.homing = homing;
                                }
                            }
                        }
                    }
                }
            }
            //friendly bullet collisions
            //with rocks
            for (int i = 0; i <= friendlyBullets.Count - 1; i++)
            {
                for (int j = 0; j <= collisionmap.Count - 1; j++)
                {
                    if (collisionmap[j].GetType() == typeof(Rect))
                    {
                        Rect rock = (Rect)collisionmap[j];
                        if (i < friendlyBullets.Count)
                        {
                            if(friendlyBullets[i].GetType() == typeof(GameObjects.friendlyBullet))
                            {
                                GameObjects.friendlyBullet f = (GameObjects.friendlyBullet)friendlyBullets[i];
                                Rect friendlyCollisions = new Rect(f.x, f.y, 32, 32);
                                if(friendlyCollisions.intersects(rock))
                                {
                                    Generate.spawnExplosion((int)friendlyCollisions.x, (int)friendlyCollisions.y, explosionParticle);
                                    friendlyBullets.Remove(f);
                                    Generate.friendly = friendlyBullets;
                                }
                            }
                        }
                    }
                }
            }

            //with doodz
            if (friendlyBullets.Count > 0)
            {
                for (int i = 0; i <= friendlyBullets.Count - 1; i++)
                {
                    for (int j = 0; j <= monsters.Count - 1; j++)
                    {
                        if (monsters[j].GetType() == typeof(GameObjects.mons))
                        {
                            GameObjects.mons tempMons = (GameObjects.mons)monsters[j];
                            Rect monsCollisions = new Rect(tempMons.x, tempMons.y, 32, 32);
                            if (i < friendlyBullets.Count)
                            {
                                if (friendlyBullets[i].GetType() == typeof(GameObjects.friendlyBullet))
                                {
                                    GameObjects.friendlyBullet f = (GameObjects.friendlyBullet)friendlyBullets[i];
                                    Rect friendlyCollisions = new Rect(f.x, f.y, 32, 32);
                                    if (friendlyCollisions.intersects(monsCollisions))
                                    {
                                        Generate.spawnExplosion(f.x, f.y, explosionParticle);
                                        friendlyBullets.Remove(f);
                                        tempMons.health -= 5;
                                        monsters[j] = tempMons;
                                        if (tempMons.health == 0)
                                        {
                                            monsters.Remove(tempMons);
                                            monstersKilled++;
                                        }
                                        Generate.friendly = friendlyBullets;
                                        Generate.monsters = monsters;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //render friendly bullets
            if(friendlyBullets.Count > 0)
            {
                for (int i = 0; i <= friendlyBullets.Count - 1; i++)
                {
                    if(friendlyBullets[i].GetType() == typeof(GameObjects.friendlyBullet))
                    {
                        GameObjects.friendlyBullet placeholder = (GameObjects.friendlyBullet)friendlyBullets[i];
                        if (placeholder.age < 21)
                        {
                            placeholder = Generate.simulateFriendly(placeholder);
                            scene.render(friendly, placeholder.x, placeholder.y);
                            //save it all back
                            friendlyBullets[i] = placeholder;
                            Generate.friendly = friendlyBullets;
                        }
                        else
                        {
                            friendlyBullets.Remove(placeholder);
                            //save it, its all yours my friend
                            Generate.friendly = friendlyBullets;
                        }
                    }
                }
            }

            //spawn monsters
            if (currentSpawnCount == spawnInterval)
            {
                currentSpawnCount = -1;
                Generate.spawnMonster();
                if (intervalDecreaseTime == maxIntervalDecreaseTime)
                {
                    intervalDecreaseTime = -1;
                    spawnInterval -= decreaseInterval;
                }
            }

            currentSpawnCount++;
            intervalDecreaseTime++;

            //render monsters
            for (int i = 0; i <= monsters.Count - 1; i++)
            {
                if (monsters[i].GetType() == typeof(GameObjects.mons))
                {
                    GameObjects.mons placeholder = (GameObjects.mons)monsters[i];
                    switch(placeholder.type)
                    {
                        case 0:
                            scene.render(spitter, placeholder.x, placeholder.y);
                            break;

                        case 1:
                            scene.render(firebelcher, placeholder.x, placeholder.y);
                            break;

                        case 2:
                            scene.render(ancientone, placeholder.x, placeholder.y);
                            break;

                        case 3:
                            scene.render(snakeeyes, placeholder.x, placeholder.y);
                            break;

                        case 4:
                            scene.render(viper, placeholder.x, placeholder.y);
                            break;

                        default:
                            Console.WriteLine("something went wrong :/");
                            break;
                    }
                    if (sceneId == 1)
                    {
                        scene.render(new Shape.Rectangle(placeholder.x, placeholder.y - 5, placeholder.health * 5, 1), 5, Brushes.Red);
                    }
                }
            }

            //EXPLOSIONS!
            explosions = Generate.explosions;

            if (explosions.Count > 0)
            {
                for (int i = 0; i < explosions.Count; i++)
                {
                    if (explosions[i].GetType() == typeof(GameObjects.explosion))
                    {
                        GameObjects.explosion explo = (GameObjects.explosion)explosions[i];
                        if(explo.w > 0)
                        {
                            explo.w -= 4;
                            explo.h -= 4;
                            explo.x += 2;
                            explo.y += 2;
                            Prop tempExplo = explosionParticle;
                            tempExplo.propWidth = explo.w;
                            tempExplo.propHeight = explo.h;
                            scene.render(tempExplo, explo.x, explo.y);
                            //save our changes
                            explosions[i] = explo;
                        }
                        else
                        {
                            explosions.Remove(explo);
                        }
                    }
                }
            }

            //ui stuff
            if (characterHealth > 0 && sceneId == 1)
            {
                scene.render(new Shape.Rectangle(0, 0, 250, 1), 25, Brushes.DarkRed);
                scene.render(new Shape.Rectangle(0, 0, characterHealth * 10, 1), 25, Brushes.Red);
                scene.render("HEALTH: " + characterHealth + " / " + maxHealth, 0, 0, Brushes.Black);

                if (characterId == 2)
                {
                    scene.render(new Shape.Rectangle(0, 25, 150, 1), 15, Brushes.DarkCyan);
                    scene.render(new Shape.Rectangle(0, 25, giantBuff * 10, 1), 15, Brushes.Cyan);
                    scene.render("SHIELD: " + giantBuff + " / " + maxGiantBuff, 0, 20, Brushes.Black);
                }

                if (characterId == 1)
                {
                    scene.render(scepter, mx, my);
                }

                if (characterId == 2)
                {
                    scene.render(switchNotice, 0, 35, Brushes.Black);
                }
                else
                {
                    scene.render(switchNotice, 0, 20, Brushes.Black);
                }
            }
            //check if ded
            if(characterHealth <= 0 && sceneId == 1)
            {
                sceneId = 2;
            }
        }

        public static void checkInput()
        {

            mx = scene.getMouseX();
            my = scene.getMouseY();

            //movement
            //note the specially-coded diagonal movement
            if (scene.readKeyDown(Keys.W))
            {
                movingUp = true;
            }
            if (scene.readKeyDown(Keys.W) && scene.readKeyDown(Keys.A))
            {
                movingUp = true;
                movingLeft = true;
            }
            if (scene.readKeyDown(Keys.W) && scene.readKeyDown(Keys.D))
            {
                movingUp = true;
                movingRight = true;
            }
            if (scene.readKeyDown(Keys.A))
            {
                movingLeft = true;
            }
            if (scene.readKeyDown(Keys.A) && scene.readKeyDown(Keys.S))
            {
                movingLeft = true;
                movingDown = true;
            }
            if (scene.readKeyDown(Keys.S))
            {
                movingDown = true;
            }
            if (scene.readKeyDown(Keys.S) && scene.readKeyDown(Keys.D))
            {
                movingDown = true;
                movingRight = true;
            }
            if (scene.readKeyDown(Keys.D))
            {
                movingRight = true;
            }

            //change size
            if (scene.readKeyDown(Keys.D1))
            {
                bigDone = false;
                becomeBig();
            }
            if (scene.readKeyDown(Keys.D2))
            {
                smallDone = false;
                becomeSmall();
            }
            if(scene.readKeyDown(Keys.D3))
            {
                giantDone = false;
                becomeGiant();
            }

            //reset everything
            if (scene.keyUp())
            {
                movingUp = false;
                movingLeft = false;
                movingDown = false;
                movingRight = false;
                bigDone = true;
                smallDone = true;
            }

            //calculating movement
            if (characterHealth != 0)
            {
                if (movingUp)
                {
                    if (characterId == 1)
                    {
                        //we make a clone of the guy
                        Rect tempGuy = character;
                        //the clone does the movement
                        tempGuy.y -= characterSpeed;
                        //we look through all the rocks
                        for (int i = 0; i <= collisionmap.Count - 1; i++)
                        {
                            //we grab each rock
                            if (collisionmap[i].GetType() == typeof(Rect))
                            {
                                Rect rock = (Rect)collisionmap[i];
                                //we see if the movement (done on tempGuy) would intersect the rock
                                //note: the movement is done above
                                if (tempGuy.intersects(rock))
                                {
                                    //reset tempGuy (so he's one with the character)
                                    tempGuy = character;
                                    //get the difference between tempguy and the rock
                                    int yDiff = (int)tempGuy.y - (int)rock.y;
                                    //set it to the distance or something
                                    character.y += (32 - yDiff);
                                }
                            }
                        }
                    }
                    else
                    {
                        character.y -= characterSpeed;
                    }
                }
                if (movingLeft)
                {
                    if (characterId != 0)
                    {
                        //we make a clone of the guy
                        Rect tempGuy = character;
                        //the clone does the movement
                        tempGuy.x -= characterSpeed;
                        //we look through all the rocks
                        for (int i = 0; i <= collisionmap.Count - 1; i++)
                        {
                            //we grab each rock
                            if (collisionmap[i].GetType() == typeof(Rect))
                            {
                                Rect rock = (Rect)collisionmap[i];
                                //we see if the movement (done on tempGuy) would intersect the rock
                                //note: the movement is done above
                                if (tempGuy.intersects(rock))
                                {
                                    //reset tempGuy (so he's one with the character)
                                    tempGuy = character;
                                    //get the difference between tempguy and the rock
                                    int xDiff = (int)tempGuy.x - (int)rock.x;
                                    //set it to the distance or something
                                    character.x += (32 - xDiff);
                                }
                            }
                        }
                    }
                    else
                    {
                        character.x -= characterSpeed;
                    }
                }
                if (movingDown)
                {
                    if (characterId != 0)
                    {
                        //we make a clone of the guy
                        Rect tempGuy = character;
                        //the clone does the movement
                        tempGuy.y += characterSpeed;
                        //we look through all the rocks
                        for (int i = 0; i <= collisionmap.Count - 1; i++)
                        {
                            //we grab each rock
                            if (collisionmap[i].GetType() == typeof(Rect))
                            {
                                Rect rock = (Rect)collisionmap[i];
                                //we see if the movement (done on tempGuy) would intersect the rock
                                //note: the movement is done above
                                if (tempGuy.intersects(rock))
                                {
                                    //set it to the distance or something
                                    character.y = rock.y - 32;
                                }
                            }
                        }
                    }
                    else
                    {
                        character.y += characterSpeed;
                    }
                }
                if (movingRight)
                {
                    if (characterId != 0)
                    {
                        //we make a clone of the guy
                        Rect tempGuy = character;
                        //the clone does the movement
                        tempGuy.x += characterSpeed;
                        //we look through all the rocks
                        for (int i = 0; i <= collisionmap.Count - 1; i++)
                        {
                            //we grab each rock
                            if (collisionmap[i].GetType() == typeof(Rect))
                            {
                                Rect rock = (Rect)collisionmap[i];
                                //we see if the movement (done on tempGuy) would intersect the rock
                                //note: the movement is done above
                                if (tempGuy.intersects(rock))
                                {
                                    //set it to the distance or something
                                    character.x = rock.x - 32;
                                }
                            }
                        }
                    }
                    else
                    {
                        character.x += characterSpeed;
                    }
                }

                if(character.x < 0)
                {
                    character.x = 0;
                }
                if(character.y < 0)
                {
                    character.y = 0;
                }
            }
            
        }

        public static void becomeBig()
        {
            if (!bigDone)
            {
                characterId = 1;
                characterSpeed = shootSpeed;
                character.width = 32;
                character.height = 32;
                switchNotice = "[2] to ghost, [3] to shield-up";
            }
        }

        public static void becomeSmall()
        {
            if (!smallDone)
            {
                characterId = 0;
                characterSpeed = quickSpeed;
                character.width = 24;
                character.height = 24;
                switchNotice = "[1] to solidify, [3] to shield-up";
            }
        }

        public static void becomeGiant()
        {
            if(!giantDone)
            {
                characterId = 2;
                characterSpeed = giantSpeed;
                character.width = 32;
                character.height = 32;
                switchNotice = "[1] to attack, [2] to ghost";
            }
        }

        public static void triggerClick()
        {
            if (characterId == 1 && characterHealth > 0)
            {
                Generate.shootFriendly((int)character.x, (int)character.y, mx, my);
            }
        }

        public static void simulateHoming(GameObjects.homingmissle h)
        {
            if (character.x > h.collisions.x)
            {
                h.collisions.x += h.speed;
            }
            if (character.x < h.collisions.x)
            {
                h.collisions.x -= h.speed;
            }
            if(character.y > h.collisions.y)
            {
                h.collisions.y += h.speed;
            }
            if(character.y < h.collisions.y)
            {
                h.collisions.y -= h.speed;
            }
        }

        public static GameObjects.bullet calculateViperStrengths(GameObjects.mons m)
        {
            int monsX = m.x;
            int monsY = m.y;

            int strengthX = 0;
            int strengthY = 0;

            if (character.x > monsX)
            {
                strengthX = 1;
            }
            if (character.x < monsX)
            {
                strengthX = -1;
            }
            if (character.y > monsY)
            {
                strengthY = 1;
            }
            if (character.y < monsY)
            {
                strengthY = -1;
            }

            //put it all together
            GameObjects.bullet final = new GameObjects.bullet(monsX, monsY, 1, strengthX, strengthY, Generate.bulletSpeed);

            return final;
        }


        public static void resultScreen(int kills, int bullets)
        {

            if (sceneId == 2)
            {
                moveOnInput();

                if (character != null) //this should go away after testing
                {
                    character.x = 0;
                    character.y = 0;
                }
                spawnInterval = 47;
                gameScreen();
                int score;
                if (hitsTaken > 0)
                {
                    score = (int)(timeAlive / hitsTaken) + kills;
                }
                else
                {
                    score = timeAlive + kills;
                }

                scene.render(scoreSheet, 64, 64);

                Font scoreFont = new Font(FontFamily.GenericMonospace, 32f);

                scene.render("Final Score: " + score.ToString(), 80, 150, Brushes.Black, scoreFont);
                scene.render("Bullets Spawned: " + bulletsShot.ToString(), 80, 250, Brushes.Black, scoreFont);
                scene.render("Monsters Spawned: " + monstersSpawned.ToString(), 80, 350, Brushes.Black, scoreFont);
                scene.render("Monsters Slayed: " + monstersKilled.ToString(), 80, 450, Brushes.Black, scoreFont);
                scene.render("Times Injured: " + hitsTaken.ToString(), 80, 550, Brushes.Black, scoreFont);

                scene.render(playAgainButton, 600, 650);
                scene.render(toMenuButton, 80, 650);
            }

        }

        public static void moveOnInput()
        {
            if(scene.readKeyDown(Keys.Right))
            {
                shouldPlayAgain = true;
            }
            if(scene.readKeyDown(Keys.Left))
            {
                shouldGoMenu = true;
            }

            //reset key presses
            if(scene.keyUp())
            {
                shouldPlayAgain = false;
            }

            //navigate
            if(shouldPlayAgain)
            {
                resetGame();
            }
            if(shouldGoMenu)
            {
                goMenu();
            }

        }

        public static void resetGame()
        {
            Generate.bullets.Clear();
            Generate.collisionmap.Clear();
            Generate.explosions.Clear();
            Generate.friendly.Clear();
            Generate.homing.Clear();
            for(int i = 0; i <= 24; i++)
            {
                for(int j = 0; j <= 24; j++)
                {
                    map[i, j] = 0;
                }
            }
            Generate.monsters.Clear();
            start();
            characterHealth = 25;
            giantBuff = 15;
            character.x = 0;
            character.y = 0;
            bigDone = false;
            becomeBig();
            bigDone = true;
            sceneId = 1;
        }

        public static void goMenu()
        {
            resetGame();
            characterHealth = 0;
            sceneId = 0;
            Console.WriteLine(sceneId);
        }
    }
}
