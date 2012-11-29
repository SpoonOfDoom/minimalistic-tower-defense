using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MinimalisticTD
{
	/// <summary>
	/// Handles input, general game flow, contains globally used references and all sorts of stuff.
	/// </summary>
	public static class GameManager
	{
		/// <summary>
		/// Specifies if the game is currently active
		/// </summary>
		public static bool IsGameActive;

		/// <summary>
		/// The active GraphicsDevice.
		/// </summary>
		public static GraphicsDevice ActiveGraphicsDevice;

		/// <summary>
		/// The complete Rectangle to which the game is drawn.
		/// </summary>
		public static Rectangle GameScreenRectangle;

		/// <summary>
		/// Used for all calculations that involve randomly generated numbers.
		/// </summary>
		public static Random rand = new Random();

		/// <summary>
		/// The one and only texture used throughout the game.
		/// </summary>
		public static Texture2D RectTexture;

		/// <summary>
		/// The standard ingame font.
		/// </summary>
		public static SpriteFont IngameFont;

		/// <summary>
		/// A small ingame font.
		/// </summary>
		public static SpriteFont IngameFontSmall;

		/// <summary>
		/// A small, bold ingame font.
		/// </summary>
		public static SpriteFont IngameFontSmallBold;

		/// <summary>
		/// A bigger font used for menus.
		/// </summary>
		public static SpriteFont MenuFontBig;

		/// <summary>
		/// The list of <see cref="Tower"/>s the <see cref="Player"/> has.
		/// </summary>
		public static List<Tower> PlayerTowers = new List<Tower>();

		/// <summary>
		/// Contains every <see cref="Enemy"/> that's currently active.
		/// </summary>
		public static List<Enemy> Enemies = new List<Enemy>();

		/// <summary>
		/// Contains all currently active <see cref="Shot"/>s.
		/// </summary>
		public static List<Shot> Shots = new List<Shot>();

		/// <summary>
		/// The <see cref="Map"/> that's currently being played.
		/// </summary>
		public static Map CurrentMap;

		/// <summary>
		/// The current <see cref="Player"/>.
		/// </summary>
		public static Player PlayerOne;

		/// <summary>
		/// Specifies if the <see cref="GameManager"/> wants to pause the game. Used to communicate with the <see cref="Game1"/> class.
		/// </summary>
		public static bool RequestPause = false;

		/// <summary>
		/// Specifies if the <see cref="CurrentMap"/> is a randomly generated map.
		/// </summary>
		public static bool IsRandomMap = false;

		/// <summary>
		/// The menu with which the <see cref="Player"/> can buy, sell or upgrade <see cref="Tower"/>s.
		/// </summary>
		private static TileMenu tileMenu;

		/// <summary>
		/// The Rectangle in which the <see cref="Player"/>'s data is drawn.
		/// </summary>
		public static Rectangle PlayerDataRectangle;

		/// <summary>
		/// The part of the screen that contains the <see cref="Map"/>.
		/// </summary>
		public static Rectangle MapRectangle;

		/// <summary>
		/// Position where the <see cref="Player"/>'s score is drawn.
		/// </summary>
		private static Vector2 scoreDrawPosition = new Vector2(5, 0);

		/// <summary>
		/// Position where the <see cref="Player"/>'s number of lives is drawn.
		/// </summary>
		private static Vector2 livesDrawPosition = new Vector2(180, 0);

		/// <summary>
		/// Position where the <see cref="Player"/>'s money is drawn.
		/// </summary>
		private static Vector2 moneyDrawPosition = new Vector2(380, 0);

		/// <summary>
		/// The MouseState from the previous frame.
		/// </summary>
		private static MouseState lastMouseState;

		/// <summary>
		/// The KeyboardState from the previous frame.
		/// </summary>
		private static KeyboardState lastKeyboardState;

		/// <summary>
		/// The GamePadState from the previous frame.
		/// </summary>
		private static GamePadState lastGamePadState;

		/// <summary>
		/// Initializes the GameManager.
		/// </summary>
		/// <param name="content">The ContentManager used for loading content.</param>
		/// <param name="graphicsDevice">The active GraphicsDevice.</param>
		public static void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
		{
			ActiveGraphicsDevice = graphicsDevice;
			RectTexture = Helper.CreateRectangleTexture(1, 1);
			IngameFont = content.Load<SpriteFont>(@"Fonts\IngameFont");
			IngameFontSmall = content.Load<SpriteFont>(@"Fonts\IngameFontSmall");
			IngameFontSmallBold = content.Load<SpriteFont>(@"Fonts\IngameFontSmallBold");
			MenuFontBig = content.Load<SpriteFont>(@"Fonts\MenuFontBig");
			PlayerOne = new Player();
			lastMouseState = Mouse.GetState();
			lastKeyboardState = Keyboard.GetState();
			lastGamePadState = GamePad.GetState(PlayerIndex.One);
			ReadEnemyInfo("EnemyLevelInfo.xml");
			ReadTowerInfo("TowerLevelInfo.xml");
		}

		/// <summary>
		/// Reads the enemy info from Xml.
		/// </summary>
		/// <param name="path">The path.</param>
		private static void ReadEnemyInfo(string path)
		{
			// using (var stream = typeof(GameManager).Assembly.GetManifestResourceStream("MinimalisticTD." + path))
			using (var stream = File.Open(path))
			{
				using (var eInforeader = System.Xml.XmlReader.Create(stream))
				{
					while (!eInforeader.EOF)
					{
						if (eInforeader.ReadToFollowing("EnemyType"))
						{
							Enemy.EnemyType type = eInforeader.GetAttribute("Type").ToEnemyType();
							Color color = eInforeader.GetAttribute("Color").ToColor();
							Enemy.TypeColors.Add(type, color);
							using (var levelReader = eInforeader.ReadSubtree())
							{
								while (levelReader.ReadToFollowing("Level"))
								{
									//levelReader.ReadToFollowing("Level");
									int level = levelReader.GetAttribute("Value").ToInt();
									float health = levelReader.GetAttribute("Health").ToFloat();
									float armor = levelReader.GetAttribute("Armor").ToFloat(); //TOTHINK: Do we really need armor, or could we just use PhysicalResistance to achieve the same effect? How about a general resistance stat? How does this effect other resistances?
									float moneyWorth = levelReader.GetAttribute("MoneyWorth").ToFloat();
									float points = levelReader.GetAttribute("Points").ToFloat();
									float speed = levelReader.GetAttribute("Speed").ToFloat();
									int screenWidth = levelReader.GetAttribute("ScreenWidth").ToInt();
									int screenHeight = levelReader.GetAttribute("ScreenHeight").ToInt();
									Dictionary<Tower.DamageType, float> resistance = new Dictionary<Tower.DamageType, float>();

									//See if there's any resistance mentioned.
									foreach (string dType in Enum.GetNames(typeof(Tower.DamageType)))
									{
										try
										{
											string attribute = dType + "Resistance";
											float res = Math.Max(levelReader.GetAttribute(attribute).ToFloat(), 0.01f); //ensure it is greater than zero, so it doesn't get ugly when we divide by it
											resistance.Add((Tower.DamageType)Enum.Parse(typeof(Tower.DamageType), dType), res);
										}
										catch (Exception)
										{
											resistance.Add((Tower.DamageType)Enum.Parse(typeof(Tower.DamageType), dType), 1); //default resistance is 1 - this means, it won't affect the calculation. Double damage would be 0.5, half damage 2 etc.
										}
									}
									Enemy.AddLevelValues(type, level, health, armor, moneyWorth, points, speed, resistance, screenWidth, screenHeight);
								}
								levelReader.Close();
							}
						}
					}
					eInforeader.Close();
				}
				stream.Close();
			}
		}

		/// <summary>
		/// Reads the tower info from Xml.
		/// </summary>
		/// <param name="xmlPath">The path.</param>
		private static void ReadTowerInfo(string xmlPath)
		{
			// using (var stream = typeof(GameManager).Assembly.GetManifestResourceStream("MinimalisticTD." + xmlPath))
			using (var stream = File.Open(xmlPath))
			{
				using (var tInforeader = System.Xml.XmlReader.Create(stream))
				{
					while (!tInforeader.EOF)
					{
						if (tInforeader.ReadToFollowing("TowerType"))
						{
							Tower.DamageType type = tInforeader.GetAttribute("Type").ToDamageType();
							Color color = tInforeader.GetAttribute("Color").ToColor();
							Tower.TypeColors.Add(type, color);

							using (var levelReader = tInforeader.ReadSubtree())
							{
								while (levelReader.ReadToFollowing("Level"))
								{
									int level = levelReader.GetAttribute("Value").ToInt();
									float damage = levelReader.GetAttribute("Damage").ToFloat();
									float ticks = levelReader.GetAttribute("Ticks").ToFloat();
									float shotDelay = levelReader.GetAttribute("ShotDelay").ToFloat();
									float range = levelReader.GetAttribute("Range").ToFloat();
									float cost = levelReader.GetAttribute("Cost").ToFloat();
									float upgradeCost = levelReader.GetAttribute("UpgradeCost").ToFloat();
									float sellGain = levelReader.GetAttribute("SellGain").ToFloat();
									int screenWidth = levelReader.GetAttribute("ScreenWidth").ToInt();
									int screenHeight = levelReader.GetAttribute("ScreenHeight").ToInt();
									Tower.AddLevelValues(type, level, damage, ticks, shotDelay, range, cost, upgradeCost, sellGain, screenWidth, screenHeight);
								}
								levelReader.Close();
							}
						}
					}
					tInforeader.Close();
				}
				stream.Close();
			}
		}

		/// <summary>
		/// Starts the <see cref="Map"/>.
		/// </summary>
		public static void StartMap()
		{
			PlayerOne = new Player();
			PlayerOne.Money = CurrentMap.StartingMoney;
			PlayerOne.Lives = CurrentMap.StartingLives;
			lastMouseState = Mouse.GetState();
			lastKeyboardState = Keyboard.GetState();
			lastGamePadState = GamePad.GetState(PlayerIndex.One);
		}

		/// <summary>
		/// Starts a game with random <see cref="Map"/>.
		/// </summary>
		public static void StartRandomMap()
		{
			IsRandomMap = true;
			CurrentMap = new Map(20, 20);
			int startX = rand.Next(2, 18);
			int startY = rand.Next(2, 18);
			CurrentMap.GenerateMap(new Vector2(startX, startY), 15, 4);
			PlayerOne = new Player();
			PlayerOne.Money = CurrentMap.StartingMoney;
			PlayerOne.Lives = CurrentMap.StartingLives;
			lastMouseState = Mouse.GetState();
			lastKeyboardState = Keyboard.GetState();
			lastGamePadState = GamePad.GetState(PlayerIndex.One);
			ResetMap();
		}

		/// <summary>
		/// Resets the <see cref="CurrentMap"/>.
		/// </summary>
		public static void ResetMap()
		{
			CurrentMap.Reset();
			PlayerTowers.Clear();
			Enemies.Clear();
			PlayerOne.Money = CurrentMap.StartingMoney;
			PlayerOne.Lives = CurrentMap.StartingLives;
			PlayerOne.Score = 0;
		}

		/// <summary>
		/// Handles the input from all devices.
		/// </summary>
		/// <param name="mouse">The MouseState.</param>
		/// <param name="keyboard">The KeyboardState.</param>
		/// <param name="pad">The GamePadState.</param>
		public static void HandleInput(MouseState mouse, KeyboardState keyboard, GamePadState pad)
		{
			HandleMouse(mouse);
			HandleKeyboard(keyboard);
			HandleGamepad(pad);
		}

		/// <summary>
		/// Handles the gamepad input.
		/// </summary>
		/// <param name="pad">The GamePadState.</param>
		public static void HandleGamepad(GamePadState pad)
		{
			//TODO: Handle gamepad input
			lastGamePadState = pad;
		}

		/// <summary>
		/// Handles the keyboard input.
		/// </summary>
		/// <param name="keyboard">The KeyboardState.</param>
		public static void HandleKeyboard(KeyboardState keyboard)
		{
			if (keyboard.IsKeyDown(Keys.Escape) && lastKeyboardState.IsKeyUp(Keys.Escape))
			{
				RequestPause = true;
			}
			lastKeyboardState = keyboard;
		}

		/// <summary>
		/// Handles the mouse input.
		/// </summary>
		/// <param name="mouse">The MouseState.</param>
		public static void HandleMouse(MouseState mouse)
		{
			if (!GameManager.GameScreenRectangle.Contains(mouse.X, mouse.Y))
			{
				return;
			}
			Vector2 mousePosition = new Vector2((mouse.X - CurrentMap.MapOffsetX) / Map.TileWidth, (mouse.Y - CurrentMap.MapOffsetY) / Map.TileHeight);
			if (PlayerTowers.Exists(tower => tower.MapLocation == mousePosition))
			{
				PlayerTowers.Find(tower => tower.MapLocation == mousePosition).ShowRange = true;
			}
			if (mouse.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
			{
				if (tileMenu != null)
				{
					Vector2 menuItem = new Vector2();
					if (tileMenu.ClickHit(mouse.X, mouse.Y, out menuItem))
					{
						tileMenu.DoMenuItemAction(menuItem);
					}
				}
				else if (CurrentMap.Tiles[(int)mousePosition.X, (int)mousePosition.Y].IsPath())
				{
					tileMenu = null;
					return;
				}
				else
				{
					tileMenu = new TileMenu(mousePosition);
				}
			}
			lastMouseState = mouse;
		}

		/// <summary>
		/// Updates the map, enemies, shots and towers. Also grabs input and handles it if appropriate.
		/// </summary>
		/// <param name="gameTime">The GameTime.</param>
		public static void Update(GameTime gameTime)
		{
			for (int i = Shots.Count - 1; i >= 0; i--)
			{
				if (Shots[i].IsActive)
				{
					Shots[i].Update(gameTime);
				}
				else
				{
					Shots.RemoveAt(i);
				}
			}

			for (int i = Enemies.Count - 1; i >= 0; i--)
			{
				if (Enemies[i].IsActive)
				{
					Enemies[i].Update(gameTime);
				}
				else
				{
					Enemies.RemoveAt(i);
				}
			}

			for (int i = PlayerTowers.Count - 1; i >= 0; i--)
			{
				if (PlayerTowers[i].IsActive)
				{
					PlayerTowers[i].Update(gameTime);
				}
				else
				{
					PlayerTowers.RemoveAt(i);
				}
			}

			if (IsGameActive)
			{
				HandleInput(Mouse.GetState(), Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
			}
			if (tileMenu != null && tileMenu.Type == TileMenu.MenuType.None)
			{
				tileMenu = null;
			}
			CurrentMap.Update(gameTime);
			if (CurrentMap.MapWon == Map.MapState.Won)
			{
				//Message won't go away, but I don't care about that right now.
				//Helper.ShowMessage("All Enemies defeated!", GameManager.GameScreenRectangle.Center.ToVector2(), 10);
			}
		}

		/// <summary>
		/// Draws the map, enemies, towers, shots, menus and player data to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public static void Draw(SpriteBatch spriteBatch)
		{
			CurrentMap.Draw(spriteBatch);
			CurrentMap.DrawWaves(spriteBatch);
			foreach (Enemy enemy in Enemies)
			{
				enemy.Draw(spriteBatch);
			}
			foreach (Tower tower in PlayerTowers)
			{
				tower.Draw(spriteBatch);
			}
			foreach (Shot shot in Shots)
			{
				shot.Draw(spriteBatch);
			}

			if (tileMenu != null && tileMenu.Type != TileMenu.MenuType.None)
			{
				tileMenu.Draw(spriteBatch);
			}

			DrawPlayerData(spriteBatch);
		}

		/// <summary>
		/// Draws a rectangle containing the player data to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public static void DrawPlayerData(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(GameManager.RectTexture, PlayerDataRectangle, Color.Black * 0.75f);
			spriteBatch.DrawString(IngameFont, "Score: " + PlayerOne.Score.ToString(), scoreDrawPosition, Color.Gold);
			spriteBatch.DrawString(IngameFont, "Lives: " + PlayerOne.Lives.ToString(), livesDrawPosition, Color.Gold);
			spriteBatch.DrawString(IngameFont, "Money: " + PlayerOne.Money.ToString(), moneyDrawPosition, Color.Gold);
		}

		/// <summary>
		/// Tries to buy a tower at the specified location.
		/// </summary>
		/// <param name="Origin">The location where the tower will be placed.</param>
		/// <param name="cost">The cost of purchasing the tower.</param>
		public static void TryBuyTower(Vector2 Origin, Tower.DamageType type)
		{
			if (PlayerTowers.Any(tower => tower.MapLocation == Origin))
			{
				return;
			}
			int cost = Tower.LevelValues[type][1]["cost"].ToInt();
			if (PlayerOne.Money >= cost)
			{
				PlayerOne.Money -= cost;
				PlayerTowers.Add(new Tower(Origin * Map.TileWidth + new Vector2(CurrentMap.MapOffsetX, CurrentMap.MapOffsetY), type, Origin));
				SoundManager.PlayBuyTower();
				tileMenu.Type = TileMenu.MenuType.None;
			}
		}

		/// <summary>
		/// Tries to upgrade the tower at the specified location.
		/// </summary>
		/// <param name="Origin">The location of the tower that will be upgraded.</param>
		/// <param name="cost">The cost of this upgrade.</param>
		public static void TryUpgradeTower(Vector2 Origin)
		{
			Tower tower = PlayerTowers.Find(t => t.MapLocation == Origin);
			int cost = Tower.LevelValues[tower.Type][tower.Level]["upgradeCost"].ToInt();
			if (PlayerOne.Money >= cost)
			{
				PlayerOne.Money -= cost;
				//PlayerTowers.Find(tower => tower.MapLocation == Origin).Upgrade();
				tower.Upgrade();
				tileMenu.Type = TileMenu.MenuType.None;
			}
		}

		/// <summary>
		/// Tries to sell the tower at the specified location.
		/// </summary>
		/// <param name="Origin">The location of the tower that will be sold.</param>
		/// <param name="gain">The money gained by selling this tower.</param>
		public static void TrySellTower(Vector2 Origin)
		{
			Tower tower = PlayerTowers.Find(t => t.MapLocation == Origin);
			PlayerOne.Money += Tower.LevelValues[tower.Type][tower.Level]["sellGain"].ToInt();
			//PlayerTowers.Remove(PlayerTowers.Find(t => t.MapLocation == Origin));
			tower.IsActive = false;
			SoundManager.PlaySellTower();
			tileMenu.Type = TileMenu.MenuType.None;
		}

		public static int GetTowerSellGain(Vector2 location)
		{
			return PlayerTowers.Find(tower => tower.MapLocation == location).SellGain;
		}

		public static int GetTowerUpgradeCost(Vector2 location)
		{
			return PlayerTowers.Find(tower => tower.MapLocation == location).UpgradeCost;
		}

		public static float GetTowerRanges(Vector2 location, out float newRange)
		{
			Tower tower = PlayerTowers.Find(t => t.MapLocation == location);
			newRange = Tower.LevelValues[tower.Type][tower.Level + 1]["range"] * Map.TileWidth;
			return tower.Range;
		}
	}
}