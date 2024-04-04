using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.DirectWrite;
using SharpDX.MediaFoundation;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Transactions;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace space_invaders
{
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		//mus
		MouseState mus = Mouse.GetState();
		MouseState gammalMus = Mouse.GetState();

		//tangent
		KeyboardState tangentBord = Keyboard.GetState();
		KeyboardState gammaltTangentbord = Keyboard.GetState();

		//random
		Random slump = new Random();
		Random random = new Random();

		//ship
		Rectangle ship;
		Texture2D shipBild;

		//eld
		Rectangle eld;
		Rectangle eld2;
		Texture2D eldBild;

		//variabler för tid
		int update = 0;
		int update2 = 0;
		int updatesTillSväng = 60;
		int updateAnimation = 350;
		float elapsedShotTime = 0f;
		float shotCooldown = 0.7f;

		//fiender
		Texture2D enemy1Bild;
		Texture2D enemy2Bild;
		Texture2D enemy3Bild;
		List<Rectangle> enemy1Positioner = new List<Rectangle>();
		List<Rectangle> enemy2Positioner = new List<Rectangle>();
		List<Rectangle> enemy3Positioner = new List<Rectangle>();

		Texture2D enemy1_2Bild;
		Texture2D enemy3_2Bild;
		Texture2D enemy2_2Bild;

		//skott
		Texture2D skottBild;
		List<Rectangle> skottRect = new List<Rectangle>();
		List<Rectangle> invaderBullets = new List<Rectangle>();

		//temp
		Rectangle tempRect;

		//barrier
		Texture2D barrierBild;
		Rectangle barrier1;
		Rectangle barrier2;
		Rectangle barrier3;
		Rectangle barrier4;
		SpriteFont times;
		int timer1 = 30;
		int timer2 = 30;
		int timer3 = 30;
		int timer4 = 30;
		string text1 = "30";
		string text2 = "30";
		string text3 = "30";
		string text4 = "30";
		bool barrier1Bool = true;
		bool barrier2Bool = true;
		bool barrier3Bool = true;
		bool barrier4Bool = true;

		//meny sida
		Texture2D logoBild;
		Texture2D playBild;
		Rectangle playRect;

		//scen varibel
		int scen = 0;

		//stjärnor
		Texture2D stars;

		//UFO
		Texture2D ufoBild;
		List<Rectangle> ufoRect = new List<Rectangle>();

		//liv
		Texture2D heartsBild;
		List<Rectangle> heartsRect = new List<Rectangle>();

		//musik och ljudeffekter
		Song musik;
		SoundEffect invaderSkott;
		SoundEffect playerSkott;
		SoundEffect playerHit;
		SoundEffect ufoSpawn;
		SoundEffect enemyHit;
		SoundEffect gameOver;

		bool moveLeft = true;
		bool moveDown = false;

		//skeppträffad
		bool skeppTräffad = false;


        public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			//res
			graphics.PreferredBackBufferWidth = 1720;
			graphics.PreferredBackBufferHeight = 1070;
			graphics.IsFullScreen = false;
		}

		protected override void Initialize()
		{
			base.Initialize();
			LäggTillInvaders();
			SkapaHearts();
		}

		private void SkapaHearts()
		{
			for (int i = 0; i < 3; i++)
			{
				heartsRect.Add(new Rectangle(heartsBild.Width + 50 * i + graphics.PreferredBackBufferWidth / 2 - 150, 100, heartsBild.Width, heartsBild.Height));
			}
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			shipBild = Content.Load<Texture2D>("spaceship");
			eldBild = Content.Load<Texture2D>("eld");
			enemy1Bild = Content.Load<Texture2D>("enemy1");
			enemy1_2Bild = Content.Load<Texture2D>("enemy1_2");
			enemy2Bild = Content.Load<Texture2D>("enemy2");
			enemy2_2Bild = Content.Load<Texture2D>("enemy2_2");
			enemy3Bild = Content.Load<Texture2D>("enemy3");
			enemy3_2Bild = Content.Load<Texture2D>("enemy3_2");
			skottBild = Content.Load<Texture2D>("bullet");
			barrierBild = Content.Load<Texture2D>("barrier");
			ufoBild = Content.Load<Texture2D>("UFO");
			playBild = Content.Load<Texture2D>("play");
			logoBild = Content.Load<Texture2D>("logo");
			stars = Content.Load<Texture2D>("stars");
			heartsBild = Content.Load<Texture2D>("hearts");
			times = Content.Load<SpriteFont>("times");

			ship = new Rectangle((graphics.PreferredBackBufferWidth / 2) - shipBild.Width, 900, shipBild.Width, shipBild.Height);
			eld = new Rectangle(((graphics.PreferredBackBufferWidth / 2) + 31)- shipBild.Width, 975, eldBild.Width + 4, eldBild.Height + 4);
			eld2 = new Rectangle(((graphics.PreferredBackBufferWidth / 2) + 55) - shipBild.Width, 975, eldBild.Width + 4, eldBild.Height + 4);
			
			//musik
			musik = Content.Load<Song>("musik");
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play(musik);
			MediaPlayer.Volume = 0.1f;

			//ljudeffekter
			invaderSkott = Content.Load<SoundEffect>("invaderSkott");
			playerSkott = Content.Load<SoundEffect>("playerSkott");
			ufoSpawn = Content.Load<SoundEffect>("ufoSpawn");
			enemyHit = Content.Load<SoundEffect>("enemyHit");
			playerHit = Content.Load<SoundEffect>("playerHit");
			gameOver = Content.Load<SoundEffect>("gameOver");

			
			float volume = 0.0f;
			SoundEffectInstance soundInstance = playerSkott.CreateInstance();
			soundInstance.Volume = volume;
		}


		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			//tagent
			gammaltTangentbord = tangentBord;
			tangentBord = Keyboard.GetState();

			//mus
			gammalMus = mus;
			mus = Mouse.GetState();

			switch (scen)
			{
				case 0:
					UppdateraMeny();
					MusikVolym();
					break;

				case 1:
					Flytta();
					ShipGräns();
					SkottTid(gameTime);
					Skott();
					InvaderRörelse();
					InvaderSkott();
					InvaderSkottRörelse();
					UFO();
					UfoRörelse();
					BarrierTräff1();
					BarrierTräff2();
					BarrierTräff3();
					BarrierTräff4();
					UppdateraSpel();
					TräffaRymdSkepp();
					break;
			}

			base.Update(gameTime);
		}

		private void TräffaRymdSkepp()
		{
			for (int i = heartsRect.Count - 1; i >= 0; i--)
			{
				for (int j = invaderBullets.Count - 1; j >= 0; j--)
				{
					if (invaderBullets[j].Intersects(ship))
					{
						playerHit.Play();
						heartsRect.RemoveAt(i);
						invaderBullets.RemoveAt(j);
						invaderBullets.Clear();
						System.Threading.Thread.Sleep(500);
						skeppTräffad = !skeppTräffad;
						i--;
						break;
					}
				}
			}
        }

		protected override void Draw(GameTime gameTime)
		{
			spriteBatch.Begin();
			GraphicsDevice.Clear(Color.Black);
			
			switch (scen)
			{
				case 0:
					GraphicsDevice.Clear(Color.Black);
					spriteBatch.Draw(stars, new Rectangle(0, 0, stars.Width, stars.Height), Color.White);
					spriteBatch.Draw(logoBild, new Vector2(210, 50), Color.White);
					spriteBatch.Draw(playBild, playRect = new Rectangle(graphics.PreferredBackBufferWidth/2 - 300, 650, playBild.Width-100, playBild.Height-100), Color.LightGoldenrodYellow);
					
					break;
				case 1:
					GraphicsDevice.Clear(Color.Black);
					spriteBatch.Draw(stars, new Rectangle(0, 0, stars.Width, stars.Height), Color.White);

					spriteBatch.Draw(shipBild, ship, Color.White);
					EldAnimation();

					if (!barrier1Bool)
					{
						spriteBatch.Draw(barrierBild, barrier1 = new Rectangle(100, 600, 0, 0), Color.DarkGreen);
						spriteBatch.DrawString(times, text1, new Vector2(-10, 0), Color.White);
					}
					else
					{
						spriteBatch.Draw(barrierBild, barrier1 = new Rectangle(200, 700, 120, 80), Color.DarkGreen);
						spriteBatch.DrawString(times, text1, new Vector2(250, 725), Color.White);
					}
					if (!barrier2Bool)
					{
						spriteBatch.Draw(barrierBild, barrier2 = new Rectangle(100, 600, 0, 0), Color.DarkGreen);
						spriteBatch.DrawString(times, text2, new Vector2(-10, 0), Color.White);
					}
					else
					{
						spriteBatch.Draw(barrierBild, barrier2 = new Rectangle(600, 700, 120, 80), Color.DarkGreen);
						spriteBatch.DrawString(times, text2, new Vector2(650, 725), Color.White);
					}
					if (!barrier3Bool)
					{
						spriteBatch.Draw(barrierBild, barrier3 = new Rectangle(100, 600, 0, 0), Color.DarkGreen);
						spriteBatch.DrawString(times, text3, new Vector2(-10, 0), Color.White);
					}
					else
					{
						spriteBatch.Draw(barrierBild, barrier3 = new Rectangle(1000, 700, 120, 80), Color.DarkGreen);
						spriteBatch.DrawString(times, text3, new Vector2(1050, 725), Color.White);
					}
					if (!barrier4Bool)
					{
						spriteBatch.Draw(barrierBild, barrier4 = new Rectangle(100, 600, 0, 0), Color.DarkGreen);
						spriteBatch.DrawString(times, text4, new Vector2(-10, 0), Color.White);
					}
					else
					{
						spriteBatch.Draw(barrierBild, barrier4 = new Rectangle(1400, 700, 120, 80), Color.DarkGreen);
						spriteBatch.DrawString(times, text4, new Vector2(1450, 725), Color.White);
					}


					bool enemy1Animation = true;
					updateAnimation--;
					if (updateAnimation < 0)
					{
						updateAnimation = 350;
						enemy1Animation = !enemy1Animation;
					}

					foreach (Rectangle enemy1 in enemy1Positioner)
					{
						if (updateAnimation < 350 / 2)
						{
							spriteBatch.Draw(enemy1_2Bild, enemy1, Color.DarkGreen);
						}
						else
						{
							spriteBatch.Draw(enemy1Bild, enemy1, Color.DarkGreen);
						}
					}

					bool enemy2Animation = true;
					updateAnimation--;
					if (updateAnimation < 0)
					{
						updateAnimation = 350;
						enemy2Animation = !enemy2Animation;
					}

					foreach (Rectangle enemy2 in enemy2Positioner)
					{
						if (updateAnimation < 350 / 2)
						{
							spriteBatch.Draw(enemy2_2Bild, enemy2, Color.Yellow);
						}
						else
						{
							spriteBatch.Draw(enemy2Bild, enemy2, Color.Yellow);
						}
					}

					bool enemy3Animation = true;
					updateAnimation--;
					if (updateAnimation < 0)
					{
						updateAnimation = 350;
						enemy3Animation = !enemy3Animation;
					}

					foreach (Rectangle enemy3 in enemy3Positioner)
					{
						if (updateAnimation < 350 / 2)
						{
							spriteBatch.Draw(enemy3_2Bild, enemy3, Color.White);
						}
						else
						{
							spriteBatch.Draw(enemy3Bild, enemy3, Color.White);
						}
					}


					foreach (Rectangle skott in skottRect)
					{
						spriteBatch.Draw(skottBild, skott, Color.White);
					}

					foreach (Rectangle skott in invaderBullets)
					{
						spriteBatch.Draw(skottBild, skott, Color.Orange);
					}

					foreach (Rectangle ufo in ufoRect)
					{
						spriteBatch.Draw(ufoBild, ufo, Color.White);
					}

					foreach (Rectangle heart in heartsRect)
					{
						spriteBatch.Draw(heartsBild, heart, Color.White);
					}
					break;
			}
			spriteBatch.End();
			base.Draw(gameTime);
		}

		private void EldAnimation()
		{
			update++;
			if (update >= 30)
			{
				spriteBatch.Draw(eldBild, eld, Color.White);
				spriteBatch.Draw(eldBild, eld2, Color.White);
				update2++;
				if (update2 >= 30)
				{
					update = 0;
					update2 = 0;
				}
			}
		}

		private void Flytta()
		{
			if (tangentBord.IsKeyDown(Keys.A) == true)
			{
				ship.X += -5;
				eld.X += -5;
				eld2.X += -5;
			}
			if (tangentBord.IsKeyDown(Keys.D) == true)
			{
				ship.X += 5;
				eld.X += 5;
				eld2.X += 5;
			}
		}

		private void ShipGräns()
		{
			if (ship.X > graphics.PreferredBackBufferWidth)
			{
				ship.X = -50;
				eld.X = -50 + 31;
				eld2.X = -50 + 55;
			}

			if (ship.X < -50)
			{
				ship.X = graphics.PreferredBackBufferWidth;
				eld.X = graphics.PreferredBackBufferWidth + 31;
				eld2.X = graphics.PreferredBackBufferWidth + 55;
			}
		}

		private void LäggTillInvaders()
		{
			for (int x = 0; x < 8; x++)
			{
				enemy1Positioner.Add(new Rectangle(100*x + 500, 400, 90, 60));
			}

			for (int x = 0; x < 8; x++)
			{
				enemy2Positioner.Add(new Rectangle(95 * x + 520, 300, 60, 40));
			}

			for (int x = 0; x < 8; x++)
			{
				enemy3Positioner.Add(new Rectangle(85 * x + 550, 200, 60, 40));
			}
		}

		private void InvaderRörelse()
		{
			int leftmostInvaderIndex = 0;
			int rightmostInvaderIndex1 = enemy1Positioner.Count - 1;
			int rightmostInvaderIndex2 = enemy2Positioner.Count - 1;
			int rightmostInvaderIndex3 = enemy3Positioner.Count - 1;

			updatesTillSväng--;
			if (updatesTillSväng < 0)
			{
				updatesTillSväng = 60;
                if (moveLeft)
				{
					if (leftmostInvaderIndex >= 0 && leftmostInvaderIndex < enemy1Positioner.Count &&
						enemy1Positioner[leftmostInvaderIndex].X <= 0)
					{
						moveDown = true;
						moveLeft = false;
					}
					else if (leftmostInvaderIndex >= 0 && leftmostInvaderIndex < enemy2Positioner.Count &&
							 enemy2Positioner[leftmostInvaderIndex].X <= 0)
					{
						moveDown = true;
						moveLeft = false;
					}
					else if (leftmostInvaderIndex >= 0 && leftmostInvaderIndex < enemy3Positioner.Count &&
							 enemy3Positioner[leftmostInvaderIndex].X <= 0)
					{
						moveDown = true;
						moveLeft = false;
					}
					else
					{
						moveDown = false;
					}
				}
				else if (rightmostInvaderIndex1 >= 0 && rightmostInvaderIndex1 < enemy1Positioner.Count &&
						 enemy1Positioner[rightmostInvaderIndex1].Right >= GraphicsDevice.Viewport.Width ||
						 rightmostInvaderIndex2 >= 0 && rightmostInvaderIndex2 < enemy2Positioner.Count &&
						 enemy2Positioner[rightmostInvaderIndex2].Right >= GraphicsDevice.Viewport.Width ||
						 rightmostInvaderIndex3 >= 0 && rightmostInvaderIndex3 < enemy3Positioner.Count &&
						 enemy3Positioner[rightmostInvaderIndex3].Right >= GraphicsDevice.Viewport.Width)
				{
					moveLeft = true;
					moveDown = true;
				}
				else
				{
					moveDown = false;
				}

				if (moveDown)
				{
					for (int i = 0; i < enemy1Positioner.Count; i++)
					{
						tempRect = enemy1Positioner[i];
						tempRect.Y += 30;
						enemy1Positioner[i] = tempRect;
					}

					for (int i = 0; i < enemy2Positioner.Count; i++)
					{
						tempRect = enemy2Positioner[i];
						tempRect.Y += 30;
						enemy2Positioner[i] = tempRect;
					}

					for (int i = 0; i < enemy3Positioner.Count; i++)
					{
						tempRect = enemy3Positioner[i];
						tempRect.Y += 30;
						enemy3Positioner[i] = tempRect;
					}
				}
				else
				{
					for (int i = 0; i < enemy1Positioner.Count; i++)
					{
						tempRect = enemy1Positioner[i];
						if (moveLeft)
						{
							tempRect.X -= 50;
						}
						else
						{
							tempRect.X += 50;
						}
						enemy1Positioner[i] = tempRect;
					}

					for (int i = 0; i < enemy2Positioner.Count; i++)
					{
						tempRect = enemy2Positioner[i];
						if (moveLeft)
						{
							tempRect.X -= 50;
						}
						else
						{
							tempRect.X += 50;
						}
						enemy2Positioner[i] = tempRect;
					}

					for (int i = 0; i < enemy3Positioner.Count; i++)
					{
						tempRect = enemy3Positioner[i];
						if (moveLeft)
						{
							tempRect.X -= 50;
						}
						else
						{
							tempRect.X += 50;
						}
						enemy3Positioner[i] = tempRect;
					}
				}
			}
        }

		private void Skott()
		{
			if (tangentBord.IsKeyDown(Keys.Space) == true && gammaltTangentbord.IsKeyUp(Keys.Space) && elapsedShotTime >= shotCooldown)
			{
				skottRect.Add(new Rectangle(ship.X + 45, 900, 5, 20));
				playerSkott.Play();
				elapsedShotTime = 0f;
			}        
		}
		void SkottTid(GameTime gameTime)
		{
			float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			elapsedShotTime += deltaTime;
		}

		private void InvaderSkott()
		{
			for (int i = 0; i < enemy1Positioner.Count; i++)
			{
				Rectangle tempInvader = enemy1Positioner[i];

				if (random.Next(1200) < 1) 
				{
					Rectangle bullet = new Rectangle(tempInvader.X + tempInvader.Width / 2, tempInvader.Y + tempInvader.Height, 5, 15);
					invaderBullets.Add(bullet);
					invaderSkott.Play();
				}
			}

			for (int i = 0; i < enemy2Positioner.Count; i++)
			{
				Rectangle tempInvader = enemy2Positioner[i];
				if (random.Next(1200) < 1)  
				{
					Rectangle bullet = new Rectangle(tempInvader.X + tempInvader.Width / 2, tempInvader.Y + tempInvader.Height, 5, 15);
					invaderBullets.Add(bullet);
					invaderSkott.Play();
				}
			}

			for (int i = 0; i < enemy3Positioner.Count; i++)
			{
				Rectangle tempInvader = enemy3Positioner[i];
				if (random.Next(1200) < 1) 
				{
					Rectangle bullet = new Rectangle(tempInvader.X + tempInvader.Width / 2, tempInvader.Y + tempInvader.Height, 5,15);
					invaderBullets.Add(bullet);
					invaderSkott.Play();
				}
			}
		}

		private void InvaderSkottRörelse()
		{
			for (int i = 0; i < invaderBullets.Count; i++)
			{
				Rectangle bullet = invaderBullets[i];
				bullet.Y += 5;

				if (bullet.Y > graphics.PreferredBackBufferHeight)
				{
					invaderBullets.RemoveAt(i);
					i--;
				}
				else
				{
					invaderBullets[i] = bullet;
				}
			}

			for (int i = skottRect.Count - 1; i >= 0; i--)
			{
				tempRect = skottRect[i];
				tempRect.Y -= 10;
				skottRect[i] = tempRect;

				if (skottRect[i].Y < 0)
				{
					skottRect.RemoveAt(i);
				}
				else
				{
					bool intersected = false;
					for (int j = enemy1Positioner.Count - 1; j >= 0; j--)
					{
						if (skottRect[i].Intersects(enemy1Positioner[j]))
						{
							skottRect.RemoveAt(i);
							enemy1Positioner.RemoveAt(j);
							enemyHit.Play();
							intersected = true;
							break;
						}
					}

					if (intersected)
					{
						continue;
					}

					for (int j = enemy2Positioner.Count - 1; j >= 0; j--)
					{
						if (skottRect[i].Intersects(enemy2Positioner[j]))
						{
							skottRect.RemoveAt(i);
							enemy2Positioner.RemoveAt(j);
							enemyHit.Play();
							intersected = true;
							break;
						}
					}

					if (intersected)
					{
						continue;
					}

					for (int j = ufoRect.Count - 1; j >= 0; j--)
					{
						if (skottRect[i].Intersects(ufoRect[j]))
						{
							skottRect.RemoveAt(i);
							ufoRect.RemoveAt(j);
							enemyHit.Play();
							intersected = true;
							break;
						}
					}

					if (intersected)
					{
						continue;
					}

					for (int j = enemy3Positioner.Count - 1; j >= 0; j--)
					{
						if (skottRect[i].Intersects(enemy3Positioner[j]))
						{
							skottRect.RemoveAt(i);
							enemy3Positioner.RemoveAt(j);
							enemyHit.Play();
							break;
						}
					}
				}
			}
		}

		private void UFO()
		{
			if (random.Next(3000) < 1 && ufoRect.Count == 0)
			{
				ufoSpawn.Play();
				int xPosition = 0; // börjar på vänster sida
				for (int i = 0; i < ufoRect.Count; i++)
				{
					Rectangle tempUFO = ufoRect[i];
					if (random.Next(0, 2) == 0)
					{
						xPosition = graphics.PreferredBackBufferWidth; // höger sida
						tempUFO.X -= 2;
					}
					else if (xPosition == 0)
					{
						tempUFO.X += 2;
					}
					ufoRect[i] = tempUFO;

				}
				ufoRect.Add(new Rectangle(xPosition, 75, 90, 60));
			}
		}

		private void UfoRörelse()
		{
			for (int i = 0; i < ufoRect.Count; i++)
			{
				ufoRect[i] = new Rectangle(ufoRect[i].X + 2, ufoRect[i].Y, ufoRect[i].Width, ufoRect[i].Height); // flyttar med 2 pixlar

				if (ufoRect[i].X > graphics.PreferredBackBufferWidth || ufoRect[i].X + ufoRect[i].Width < 0)
				{
					ufoRect.RemoveAt(i);
					i--;
				}
			}
		}

		private void BarrierTräff1()
		{
			for (int i = invaderBullets.Count - 1; i >= 0; i--)
			{
				if (invaderBullets[i].Y > graphics.PreferredBackBufferHeight)
				{
					invaderBullets.RemoveAt(i);
				}
				else
				{
					bool intersected = false; 
					if (invaderBullets[i].Intersects(barrier1))
					{
						invaderBullets.RemoveAt(i);
						timer1--;
						text1 = "" + timer1;
						intersected = true;
					}

					if (intersected)
					{
						continue;
					}
				}
			}
			for (int i = skottRect.Count - 1; i >= 0; i--)
			{
				if (skottRect[i].Y > graphics.PreferredBackBufferHeight)
				{
					skottRect.RemoveAt(i);
				}
				else
				{
					bool intersected = false; 
					if (skottRect[i].Intersects(barrier1))
					{
						skottRect.RemoveAt(i);
						timer1--;
						text1 = "" + timer1;
						intersected = true;
					}

					if (intersected)
					{
						continue;
					}

				}
			}
			if (timer1 == 0)
			{
				barrier1Bool = false;
			}
		}

		private void BarrierTräff2()
		{
			for (int i = invaderBullets.Count - 1; i >= 0; i--)
			{
				if (invaderBullets[i].Y > graphics.PreferredBackBufferHeight)
				{
					invaderBullets.RemoveAt(i);
				}
				else
				{
					bool intersected = false;
					if (invaderBullets[i].Intersects(barrier2))
					{
						invaderBullets.RemoveAt(i);
						timer2--;
						text2 = "" + timer2;
						intersected = true;
					}

					if (intersected)
					{
						continue; 
					}

				}
			}
			for (int i = skottRect.Count - 1; i >= 0; i--)
			{
				if (skottRect[i].Y > graphics.PreferredBackBufferHeight)
				{
					skottRect.RemoveAt(i);
				}
				else
				{
					bool intersected = false; 
					if (skottRect[i].Intersects(barrier2))
					{
						skottRect.RemoveAt(i);
						timer2--;
						text2 = "" + timer2;
						intersected = true;
					}

					if (intersected)
					{
						continue; 
					}

				}
			}
			if (timer2 == 0)
			{
				barrier2Bool = false;
			}
		}

		private void BarrierTräff3()
		{
			for (int i = invaderBullets.Count - 1; i >= 0; i--)
			{
				if (invaderBullets[i].Y > graphics.PreferredBackBufferHeight)
				{
					invaderBullets.RemoveAt(i);
				}
				else
				{
					bool intersected = false;
					if (invaderBullets[i].Intersects(barrier3))
					{
						invaderBullets.RemoveAt(i);
						timer3--;
						text3 = "" + timer3;
						intersected = true;
					}

					if (intersected)
					{
						continue; 
					}

				}
			}
			for (int i = skottRect.Count - 1; i >= 0; i--)
			{
				if (skottRect[i].Y > graphics.PreferredBackBufferHeight)
				{
					skottRect.RemoveAt(i);
				}
				else
				{
					bool intersected = false; 
					if (skottRect[i].Intersects(barrier3))
					{
						skottRect.RemoveAt(i);
						timer3--;
						text3 = "" + timer3;
						intersected = true;
					}

					if (intersected)
					{
						continue;
					}

				}
			}
			if (timer3 == 0)
			{
				barrier3Bool = false;
			}

		}

		private void BarrierTräff4()
		{
			for (int i = invaderBullets.Count - 1; i >= 0; i--)
			{
				if (invaderBullets[i].Y > graphics.PreferredBackBufferHeight)
				{
					invaderBullets.RemoveAt(i);
				}
				else
				{
					bool intersected = false; 
					if (invaderBullets[i].Intersects(barrier4))
					{
						invaderBullets.RemoveAt(i);
						timer4--;
						text4 = "" + timer4;
						intersected = true;
					}
					if (intersected)
					{
						continue; 
					}
				}
			}
			for (int i = skottRect.Count - 1; i >= 0; i--)
			{
				if (skottRect[i].Y > graphics.PreferredBackBufferHeight)
				{
					skottRect.RemoveAt(i);
				}
				else
				{
					bool intersected = false; 
					if (skottRect[i].Intersects(barrier4))
					{
						skottRect.RemoveAt(i);
						timer4--;
						text4 = "" + timer4;
						intersected = true;
					}
					if (intersected)
					{
						continue;
					}
				}
			}
			if (timer4 == 0)
			{
				barrier4Bool = false;
			}
		}

		bool VänsterMusTryckt()
		{
			if (mus.LeftButton == ButtonState.Pressed && gammalMus.LeftButton == ButtonState.Released)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		void BytScen(int nyscen)
		{
			scen = nyscen;
		}

		void UppdateraMeny()
		{
			if (VänsterMusTryckt() && playRect.Contains(mus.Position) || tangentBord.IsKeyDown(Keys.Space) == true && gammaltTangentbord.IsKeyUp(Keys.Space))
			{
				BytScen(1);
			}
		}

		void UppdateraSpel()
		{
			bool isEmpty1 = !enemy1Positioner.Any();
			bool isEmpty2 = !enemy2Positioner.Any();
			bool isEmpty3 = !enemy3Positioner.Any();
			if (isEmpty1 && isEmpty2 && isEmpty3)
			{
				LäggTillInvaders();
			}
			else if (heartsRect.Count == 0 || tangentBord.IsKeyDown(Keys.Q) == true && gammaltTangentbord.IsKeyUp(Keys.Q))
			{
				BytScen(0);
				gameOver.Play();
				heartsRect.Clear();
				enemy1Positioner.Clear();
				enemy2Positioner.Clear();
				enemy3Positioner.Clear();
				SkapaHearts();
				LäggTillInvaders();
				ufoRect.Clear();
				timer1 = 30;
				timer2 = 30;
				timer3 = 30;
				timer4 = 30;
				text1 = "30";
				text2 = "30";
				text3 = "30";
				text4 = "30";
			}
		}

		private void MusikVolym()
		{
			if (Keyboard.GetState().IsKeyDown(Keys.M) && gammaltTangentbord.IsKeyUp(Keys.M))
			{
				MediaPlayer.Volume -= 0.1f; 
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.K) && gammaltTangentbord.IsKeyUp(Keys.K))
			{
				MediaPlayer.Volume += 0.1f;
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.L) && gammaltTangentbord.IsKeyUp(Keys.L))
			{
				MediaPlayer.Volume = 0.0f;
			}
		}

		private void Fixa()
		{
			//skjuter mer desto färre
			//inställnings meny
			//bakgrund
			//font
			//ljudeffekter
			//fixa bilderna
			//ufo som ger "powerup"
			//fixa namngivning och kommentera kod
		}
	}
} 