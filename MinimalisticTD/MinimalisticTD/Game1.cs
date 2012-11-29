using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MinimalisticTD
{
	/// <summary>
	/// The main class for the game.
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		/// <summary>
		/// The GraphicsDeviceManager of the game.
		/// </summary>
		private GraphicsDeviceManager graphics;

		/// <summary>
		/// The <see cref="SpriteBatch"/> used for drawing to screen throughout the game.
		/// </summary>
		private SpriteBatch spriteBatch;

		/// <summary>
		/// The current <see cref="GameState"/>.
		/// </summary>
		private GameState gameState;

		/// <summary>
		/// The current <see cref="GameType"/>.
		/// </summary>
		private GameType gameType;

		/// <summary>
		/// The MouseState from the previous frame.
		/// </summary>
		private MouseState lastMouseState;

		/// <summary>
		/// The KeyboardState from the previous frame.
		/// </summary>
		private KeyboardState lastKeyState;

		/// <summary>
		/// The <see cref="Menu"/> used in the title screen.
		/// </summary>
		private static Menu MainMenu;

		/// <summary>
		/// The <see cref="Menu"/> used when the game is paused.
		/// </summary>
		private static Menu PauseMenu;

		/// <summary>
		/// The <see cref="Menu"/> used when the current map is over.
		/// </summary>
		private static Menu MapOverMenu;

		/// <summary>
		/// The <see cref="Menu"/> used when the editor is paused.
		/// </summary>
		private static Menu EditorMenu;

		/// <summary>
		/// The background color with which the GraphicsDevice is cleared each frame.
		/// </summary>
		private Color BackGroundColor = Color.SteelBlue;

		/// <summary>
		/// The current frames per second (calculated).
		/// </summary>
		public float FPS = 0;

		/// <summary>
		/// The string used for drawing <see cref="FPS"/> information to screen.
		/// </summary>
		private string fps;

		/// <summary>
		/// How much time has passed since the current <see cref="Map"/> was won or lost.
		/// </summary>
		private float timeSinceMapOver = 0;

		/// <summary>
		/// The delay between winning/losing the <see cref="Map"/> and showing the <see cref="MapOverMenu"/>.
		/// </summary>
		private float mapOverDelay = 4;

		#region BackgroundText Rectangles

		/// <summary>
		/// The size of individual blocks used in the creation of the title screen background.
		/// </summary>
		private static int blockSize = 64;

		/// <summary>
		/// The left part of the "M" in the background.
		/// </summary>
		private static Rectangle M_Left = new Rectangle(blockSize * 2, 0, blockSize, blockSize * 5);

		/// <summary>
		/// The right part of the "M" in the background.
		/// </summary>
		private static Rectangle M_Right = new Rectangle(M_Left.X + (blockSize * 4), M_Left.Y, blockSize, blockSize * 5);

		/// <summary>
		/// The middle part of the "M" in the background.
		/// </summary>
		private static Rectangle M_Middle = new Rectangle(M_Left.X + (blockSize * 2), M_Left.Y + (blockSize * 2), blockSize, blockSize * 2);

		/// <summary>
		/// The part between left and middle parts of the "M" in the background.
		/// </summary>
		private static Rectangle M_MiddleLeft = new Rectangle(M_Left.X + (blockSize), M_Left.Y + blockSize, blockSize, blockSize * 2);

		/// <summary>
		/// The part between middle and right parts of the "M" in the background.
		/// </summary>
		private static Rectangle M_MiddleRight = new Rectangle(M_Left.X + (blockSize * 3), M_Left.Y + blockSize, blockSize, blockSize * 2);

		/// <summary>
		/// The top part of the "T" in the background.
		/// </summary>
		private static Rectangle T_Top = new Rectangle(M_Left.X - blockSize, M_Left.Bottom, blockSize * 3, blockSize);

		/// <summary>
		/// The bottom part of the "T" in the background.
		/// </summary>
		private static Rectangle T_Bottom = new Rectangle(T_Top.X + (blockSize), T_Top.Y + blockSize, blockSize, blockSize * 4);

		/// <summary>
		/// The left part of the "D" in the background.
		/// </summary>
		private static Rectangle D_Left = new Rectangle(T_Top.Right + blockSize, T_Top.Y, blockSize, blockSize * 5);

		/// <summary>
		/// The right part of the "D" in the background.
		/// </summary>
		private static Rectangle D_Right = new Rectangle(D_Left.X + (blockSize * 3), D_Left.Y + blockSize, blockSize, blockSize * 3);

		/// <summary>
		/// The top part of the "D" in the background.
		/// </summary>
		private static Rectangle D_Top = new Rectangle(D_Left.X + blockSize, D_Left.Y, blockSize * 2, blockSize);

		/// <summary>
		/// The bottom part of the "D" in the background.
		/// </summary>
		private static Rectangle D_Bottom = new Rectangle(D_Left.X + blockSize, D_Left.Bottom - blockSize, blockSize * 2, blockSize);

		#endregion BackgroundText Rectangles

		/// <summary>
		/// Contains copyright information which is drawn to screen.
		/// </summary>
		private const string copyright = "(C)2012 Christian Syska, Spoonforge";

		/// <summary>
		/// Contains the Url to Spoonforge which is drawn to screen.
		/// </summary>
		private const string url = "http://www.spoonforge.com";

		/// <summary>
		/// The (clickable) Rectangle containing <see cref="copyright"/> and <see cref="url"/> strings.
		/// </summary>
		private Rectangle urlRectangle;

		/// <summary>
		/// Possible states the game can be in.
		/// </summary>
		private enum GameState
		{
			/// <summary>
			/// Currently in main menu / title screen.
			/// </summary>
			Menu,

			/// <summary>
			/// Currently playing a map.
			/// </summary>
			Playing,

			/// <summary>
			/// Paused the game while playing a map.
			/// </summary>
			Pause,

			/// <summary>
			/// The map is won.
			/// </summary>
			MapWon,

			/// <summary>
			/// The map is lost.
			/// </summary>
			GameOver,

			/// <summary>
			/// Currently in the <see cref="MapEditor"/>.
			/// </summary>
			Editor,

			/// <summary>
			/// The <see cref="MapEditor"/> is paused.
			/// </summary>
			EditorPause
		}

		/// <summary>
		/// The possible types of game when playing a map.
		/// </summary>
		private enum GameType
		{
			/// <summary>
			/// A campaing <see cref="Map"/> was chosen.
			/// </summary>
			Campaign,

			/// <summary>
			/// The <see cref="Map"/> was randomly generated.
			/// </summary>
			Random,

			/// <summary>
			/// A custom <see cref="Map"/> was picked.
			/// </summary>
			Custom
		}

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Ermöglicht dem Spiel, alle Initialisierungen durchzuführen, die es benötigt, bevor die Ausführung gestartet wird.
		/// Hier können erforderliche Dienste abgefragt und alle nicht mit Grafiken
		/// verbundenen Inhalte geladen werden.  Bei Aufruf von base.Initialize werden alle Komponenten aufgezählt
		/// sowie initialisiert.
		/// </summary>
		protected override void Initialize()
		{
			IsMouseVisible = true;
			graphics.PreferredBackBufferHeight = Map.TileHeight * 20 + Map.TileHeight * 2 / 3;
			graphics.PreferredBackBufferWidth = Map.TileWidth * 20;
			graphics.ApplyChanges();
			gameState = GameState.Menu;
			BackGroundColor = Color.SteelBlue;
			lastMouseState = Mouse.GetState();
			lastKeyState = Keyboard.GetState();

			base.Initialize();
		}

		/// <summary>
		/// Initializes the menus.
		/// </summary>
		private void InitializeMenus()
		{
			InitializeMainMenu();
			InitializePauseMenu();
			InitializeEditorMenu();
		}

		/// <summary>
		/// Initializes the main menu.
		/// </summary>
		private void InitializeMainMenu()
		{
			MainMenu = new Menu(new Vector2((graphics.PreferredBackBufferWidth / 2) - 150, 100), Menu.Orientation.Vertical, Color.Black * 0.4f);
			SelectableMenuItem miNewGame = new SelectableMenuItem(new Rectangle(0, 0, 300, 60), GameManager.MenuFontBig, "New Game", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);
			SelectableMenuItem miCustomMap = new SelectableMenuItem(new Rectangle(0, 0, 300, 60), GameManager.MenuFontBig, "Custom Map", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);
			SelectableMenuItem miRandom = new SelectableMenuItem(new Rectangle(0, 0, 300, 60), GameManager.MenuFontBig, "Random Map", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);
			SelectableMenuItem miEditor = new SelectableMenuItem(new Rectangle(0, 0, 300, 60), GameManager.MenuFontBig, "Editor", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);
			SelectableMenuItem miQuit = new SelectableMenuItem(new Rectangle(0, 0, 300, 60), GameManager.MenuFontBig, "Exit Game", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);

			miNewGame.Click += new SelectableMenuItem.ClickEventHandler(miNewGame_Click);
			miCustomMap.Click += new SelectableMenuItem.ClickEventHandler(miCustomMap_Click);
			miRandom.Click += new SelectableMenuItem.ClickEventHandler(miRandom_Click);
			miEditor.Click += new SelectableMenuItem.ClickEventHandler(miEditor_Click);
			miQuit.Click += new SelectableMenuItem.ClickEventHandler(miQuit_Click);

			MainMenu.AddSelectableItem(miNewGame);
			MainMenu.AddSelectableItem(miCustomMap);
			MainMenu.AddSelectableItem(miRandom);
			MainMenu.AddSelectableItem(miEditor);
			MainMenu.AddSelectableItem(miQuit);
		}

		/// <summary>
		/// Initializes the pause menu.
		/// </summary>
		private void InitializePauseMenu()
		{
			PauseMenu = new Menu(new Vector2(100, 20), Menu.Orientation.Vertical, Color.Black * 0.9f);
			SelectableMenuItem miResumeGame = new SelectableMenuItem(new Rectangle(0, 0, 300, 60), GameManager.MenuFontBig, "Resume", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);
			SelectableMenuItem miReturnToMenu = new SelectableMenuItem(new Rectangle(0, 0, 300, 60), GameManager.MenuFontBig, "Return To Menu", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);
			SelectableMenuItem miExit = new SelectableMenuItem(new Rectangle(0, 0, 300, 60), GameManager.MenuFontBig, "Exit Game", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);
			SelectableMenuItem miSaveMap = new SelectableMenuItem(new Rectangle(0, 0, 300, 60), GameManager.MenuFontBig, "Save Map", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);

			miResumeGame.Click += new SelectableMenuItem.ClickEventHandler(miResumeGame_Click);
			miReturnToMenu.Click += new SelectableMenuItem.ClickEventHandler(miReturnToMenu_Click);
			miExit.Click += new SelectableMenuItem.ClickEventHandler(miQuit_Click);
			miSaveMap.Click += new SelectableMenuItem.ClickEventHandler(miSaveMap_Click);

			PauseMenu.AddSelectableItem(miResumeGame);
			PauseMenu.AddSelectableItem(miReturnToMenu);
			PauseMenu.AddSelectableItem(miExit);
			PauseMenu.AddSelectableItem(miSaveMap);
		}

		/// <summary>
		/// Initializes the editor menu.
		/// </summary>
		private void InitializeEditorMenu()
		{
			EditorMenu = new Menu(new Vector2(100, 20), Menu.Orientation.Vertical, Color.Black * 0.9f);
			SelectableMenuItem miResume = new SelectableMenuItem(new Rectangle(0, 0, 300, 60), GameManager.MenuFontBig, "Resume", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);
			SelectableMenuItem miReturnToMenu = new SelectableMenuItem(new Rectangle(0, 0, 300, 60), GameManager.MenuFontBig, "Return To Menu", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);
			SelectableMenuItem miExit = new SelectableMenuItem(new Rectangle(0, 0, 300, 60), GameManager.MenuFontBig, "Exit Game", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);

			miResume.Click += new SelectableMenuItem.ClickEventHandler(miResumeEditor_Click);
			miReturnToMenu.Click += new SelectableMenuItem.ClickEventHandler(miReturnToMenu_Click);
			miExit.Click += new SelectableMenuItem.ClickEventHandler(miQuit_Click);

			EditorMenu.AddSelectableItem(miResume);
			EditorMenu.AddSelectableItem(miReturnToMenu);
			EditorMenu.AddSelectableItem(miExit);
		}

		/// <summary>
		/// Initializes the map over menu.
		/// </summary>
		/// <param name="mapState">State of the map.</param>
		private void InitializeMapOverMenu(Map.MapState mapState)
		{
			MapOverMenu = new Menu(new Vector2(100, 200), Menu.Orientation.Vertical, Color.Black * 0.9f);
			int itemWidth = 300;
			int itemHeight = 60;
			SelectableMenuItem miRestartMap = new SelectableMenuItem(new Rectangle(0, 0, itemWidth, itemHeight), GameManager.MenuFontBig, "Restart Map", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);
			miRestartMap.Click += new SelectableMenuItem.ClickEventHandler(miRestartMap_Click);
			MapOverMenu.AddSelectableItem(miRestartMap);

			if (gameType == GameType.Random)
			{
				SelectableMenuItem miSaveMap = new SelectableMenuItem(new Rectangle(0, 0, itemWidth, itemHeight), GameManager.MenuFontBig, "Save Map", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);
				miSaveMap.Click += new SelectableMenuItem.ClickEventHandler(miSaveMap_Click);
				MapOverMenu.AddSelectableItem(miSaveMap);

				SelectableMenuItem miNewRandomMap = new SelectableMenuItem(new Rectangle(0, 0, itemWidth, itemHeight), GameManager.MenuFontBig, "New Random Map", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);
				miNewRandomMap.Click += new SelectableMenuItem.ClickEventHandler(miNewRandomMap_Click);
				MapOverMenu.AddSelectableItem(miNewRandomMap);
			}

			if (gameType == GameType.Custom)
			{
				SelectableMenuItem miChooseCustomMap = new SelectableMenuItem(new Rectangle(0, 0, itemWidth, itemHeight), GameManager.MenuFontBig, "Choose New Map", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);
				miChooseCustomMap.Click += new SelectableMenuItem.ClickEventHandler(miChooseCustomMap_Click);
				MapOverMenu.AddSelectableItem(miChooseCustomMap);
			}

			if (gameType == GameType.Campaign)
			{
				SelectableMenuItem miChooseCampaignMap = new SelectableMenuItem(new Rectangle(0, 0, itemWidth, itemHeight), GameManager.MenuFontBig, "Choose New Map", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);
				miChooseCampaignMap.Click += new SelectableMenuItem.ClickEventHandler(miChooseCampaignMap_Click);
				MapOverMenu.AddSelectableItem(miChooseCampaignMap);
			}

			SelectableMenuItem miReturnToMenu = new SelectableMenuItem(new Rectangle(0, 0, itemWidth, itemHeight), GameManager.MenuFontBig, "Return To Menu", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);
			miReturnToMenu.Click += new SelectableMenuItem.ClickEventHandler(miReturnToMenu_Click);
			MapOverMenu.AddSelectableItem(miReturnToMenu);

			SelectableMenuItem miExit = new SelectableMenuItem(new Rectangle(0, 0, itemWidth, itemHeight), GameManager.MenuFontBig, "Exit Game", Color.Orange, null, Color.Yellow, Menu.MenuItemAnchor.Center);
			miExit.Click += new SelectableMenuItem.ClickEventHandler(miQuit_Click);
			MapOverMenu.AddSelectableItem(miExit);
		}

		/// <summary>
		/// Handles the Click event of the miRestartMap control. Restarts the current <see cref="Map"/>.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		private void miRestartMap_Click(object sender, ClickEventArgs e)
		{
			GameManager.ResetMap();
			gameState = GameState.Playing;
			BackGroundColor = Color.Black;
		}

		/// <summary>
		/// Handles the Click event of the miNewRandomMap control. Generates a new random <see cref="Map"/> and starts it.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		private void miNewRandomMap_Click(object sender, ClickEventArgs e)
		{
			GameManager.StartRandomMap();
			gameState = GameState.Playing;
			BackGroundColor = Color.Black;
		}

		/// <summary>
		/// Handles the Click event of the miChooseCustomMap control. Lets the player choose another custom <see cref="Map"/>.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		private void miChooseCustomMap_Click(object sender, ClickEventArgs e)
		{
			gameState = GameState.Menu;
			miCustomMap_Click(sender, e);
		}

		/// <summary>
		/// Handles the Click event of the miChooseCampaignMap control. Lets the player choose another campaign <see cref="Map"/>.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		private void miChooseCampaignMap_Click(object sender, ClickEventArgs e)
		{
			miNewGame_Click(sender, e);
		}

		/// <summary>
		/// Handles the Click event of the miCustomMap control. Lets the player select a custom map.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		private void miCustomMap_Click(object sender, ClickEventArgs e)
		{
			MapEditor.MapSelectorCustom = new MapSelector("CustomMaps", MapSelector.LoadMode.PlayCustom);
			gameType = GameType.Custom;
		}

		/// <summary>
		/// Handles the Click event of the miSaveMap control. Saves the current <see cref="Map"/> as a custom <see cref="Map"/> that can later be played again or edited.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		private void miSaveMap_Click(object sender, ClickEventArgs e)
		{
			MapEditor.SaveMap(GameManager.CurrentMap);
		}

		/// <summary>
		/// Handles the Click event of the miReturnToMenu control. Returns to the main menu.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		private void miReturnToMenu_Click(object sender, ClickEventArgs e)
		{
			gameState = GameState.Menu;
			BackGroundColor = Color.SteelBlue;
			GameManager.CurrentMap = null;
			MapEditor.CurrentMap = null;
		}

		/// <summary>
		/// Handles the Click event of the miResumeGame control. Unpauses the game.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		private void miResumeGame_Click(object sender, ClickEventArgs e)
		{
			gameState = GameState.Playing;
			BackGroundColor = Color.Black;
		}

		/// <summary>
		/// Handles the Click event of the miResumeEditor control. Unpauses the <see cref="MapEditor"/>.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		private void miResumeEditor_Click(object sender, ClickEventArgs e)
		{
			gameState = GameState.Editor;
			BackGroundColor = Color.Black;
		}

		/// <summary>
		/// Handles the Click event of the miEditor control. Starts the map editor.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		private void miEditor_Click(object sender, ClickEventArgs e)
		{
			gameState = GameState.Editor;
			BackGroundColor = Color.Black;
			MapEditor.Initialize();
		}

		/// <summary>
		/// Handles the Click event of the miNewGame control. Will at some point let the player choose one of the campaign maps, but right now it just changes the background color of the main menu.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		private void miNewGame_Click(object sender, ClickEventArgs e)
		{
			int r = GameManager.rand.Next(0, 256);
			int g = GameManager.rand.Next(0, 256);
			int b = GameManager.rand.Next(0, 256);
			BackGroundColor = new Color(r, g, b);
			gameType = GameType.Campaign;
		}

		/// <summary>
		/// Handles the Click event of the miQuit control. Exits the game.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		private void miQuit_Click(object sender, ClickEventArgs e)
		{
			Exit();
		}

		/// <summary>
		/// Handles the Click event of the miRandom control. Generates a new random map and starts it.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		private void miRandom_Click(object sender, ClickEventArgs e)
		{
			gameState = GameState.Playing;
			BackGroundColor = Color.Black;
			gameType = GameType.Random;
			GameManager.StartRandomMap();
		}

		/// <summary>
		/// LoadContent wird einmal pro Spiel aufgerufen und ist der Platz, wo
		/// Ihr gesamter Content geladen wird.
		/// </summary>
		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			GameManager.Initialize(Content, GraphicsDevice);
			GameManager.PlayerDataRectangle = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, Map.TileHeight * 2 / 3);
			GameManager.MapRectangle = new Rectangle(0, Map.TileHeight * 2 / 3, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
			GameManager.GameScreenRectangle = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
			SoundManager.Initialize(Content);
			InitializeMenus();

			Vector2 copyrightSize = GameManager.IngameFontSmall.MeasureString(copyright);
			Vector2 urlSize = GameManager.IngameFontSmall.MeasureString(url);

			int ypos = (int)(graphics.PreferredBackBufferHeight - copyrightSize.Y - urlSize.Y);

			urlRectangle = new Rectangle(5, ypos - 2, (int)Math.Max(copyrightSize.X, urlSize.X), (int)(copyrightSize.Y + urlSize.Y));

			fps = Math.Round(FPS, 2).ToString();
		}

		/// <summary>
		/// Ermöglicht dem Spiel die Ausführung der Logik, wie zum Beispiel Aktualisierung der Welt,
		/// Überprüfung auf Kollisionen, Erfassung von Eingaben und Abspielen von Ton.
		/// </summary>
		/// <param name="gameTime">Bietet einen Schnappschuss der Timing-Werte.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			{
				this.Exit();
			}
			MouseState mouse = Mouse.GetState();
			KeyboardState keyState = Keyboard.GetState();

			switch (gameState)
			{
				case GameState.Menu:
					if (gameType == GameType.Custom && MapEditor.MapSelectorCustom != null && MapEditor.CurrentMap == null)
					{
						if (MapEditor.MapSelectorCustom.ClickHit(gameTime))
						{
							GameManager.StartMap();
							gameState = GameState.Playing;
							BackGroundColor = Color.Black;
						}
					}
					else if (mouse.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released && IsActive)
					{
						if (GameManager.CurrentMap != null)
						{
							GameManager.StartMap();
							gameState = GameState.Playing;
							BackGroundColor = Color.Black;
						}

						else if (!MainMenu.ClickHit(mouse.X, mouse.Y))
						{
							if (urlRectangle.Contains(mouse.X, mouse.Y))
							{
								System.Diagnostics.Process p = System.Diagnostics.Process.Start(url);
							}
						}
					}
					break;
				case GameState.Playing:
					if (GameManager.RequestPause)
					{
						gameState = GameState.Pause;
						GameManager.RequestPause = false;
						break;
					}
					if (GameManager.CurrentMap.MapWon == Map.MapState.Won)
					{
						gameState = GameState.MapWon;
						InitializeMapOverMenu(GameManager.CurrentMap.MapWon);
						break;
					}
					else if (GameManager.CurrentMap.MapWon == Map.MapState.Lost)
					{
						gameState = GameState.GameOver;
						InitializeMapOverMenu(GameManager.CurrentMap.MapWon);
						break;
					}
					GameManager.IsGameActive = IsActive;
					GameManager.Update(gameTime);
					ParticleManager.Update(gameTime);
					break;
				case GameState.Pause:
					if (mouse.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released && IsActive)
					{
						PauseMenu.ClickHit(mouse.X, mouse.Y);
					}
					break;
				case GameState.MapWon:
					timeSinceMapOver += (float)gameTime.ElapsedGameTime.TotalSeconds;
					ParticleManager.Update(gameTime);
					if (timeSinceMapOver > mapOverDelay)
					{
						if (IsActive && mouse.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
						{
							if (MapOverMenu.ClickHit(mouse.X, mouse.Y))
							{
								timeSinceMapOver = 0;
							}
						}
					}
					break;
				case GameState.GameOver:
					timeSinceMapOver += (float)gameTime.ElapsedGameTime.TotalSeconds;
					ParticleManager.Update(gameTime);
					if (timeSinceMapOver > mapOverDelay)
					{
						if (IsActive && mouse.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
						{
							if (MapOverMenu.ClickHit(mouse.X, mouse.Y))
							{
								timeSinceMapOver = 0;
							}
						}
					}
					break;
				case GameState.Editor:
					if (keyState.IsKeyDown(Keys.Escape) && lastKeyState.IsKeyUp(Keys.Escape))
					{
						gameState = GameState.EditorPause;
					}
					else if (mouse.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released && IsActive)
					{
						MapEditor.HandleInput(mouse);
					}
					break;
				case GameState.EditorPause:
					if (mouse.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released && IsActive)
					{
						EditorMenu.ClickHit(mouse.X, mouse.Y);
					}
					break;
			}
			lastMouseState = mouse;
			lastKeyState = keyState;
			base.Update(gameTime);
		}

		/// <summary>
		/// Dies wird aufgerufen, wenn das Spiel selbst zeichnen soll.
		/// </summary>
		/// <param name="gameTime">Bietet einen Schnappschuss der Timing-Werte.</param>
		protected override void Draw(GameTime gameTime)
		{
			FPS = (float)(1 / gameTime.ElapsedGameTime.TotalSeconds);
			fps = FPS.ToString().Substring(0, 4) + " FPS";
			GraphicsDevice.Clear(BackGroundColor);
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

			if (gameState == GameState.Menu)
			{
				if (gameType == GameType.Custom && MapEditor.MapSelectorCustom != null)
				{
					MapEditor.MapSelectorCustom.Draw(spriteBatch);
				}
				else
				{
					DrawMTDBackground(spriteBatch);
					MainMenu.Draw(spriteBatch);
					DrawCopyright(spriteBatch);
				}
			}
			if (gameState == GameState.Playing)
			{
				GameManager.Draw(spriteBatch);
				ParticleManager.Draw(spriteBatch);
			}
			if (gameState == GameState.Editor || gameState == GameState.EditorPause)
			{
				MapEditor.Draw(spriteBatch);
			}
			if (gameState == GameState.EditorPause)
			{
				spriteBatch.Draw(GameManager.RectTexture, GameManager.GameScreenRectangle, Color.Black * 0.5f);
				EditorMenu.Draw(spriteBatch);
			}
			if (gameState == GameState.Pause)
			{
				GameManager.Draw(spriteBatch);
				ParticleManager.Draw(spriteBatch);
				spriteBatch.Draw(GameManager.RectTexture, GameManager.GameScreenRectangle, Color.Black * 0.5f);
				PauseMenu.Draw(spriteBatch);
			}

			if (gameState == GameState.MapWon || gameState == GameState.GameOver)
			{
				GameManager.Draw(spriteBatch);
				ParticleManager.Draw(spriteBatch);
				string text = gameState == GameState.MapWon ? "YOU WON!" : "YOU LOST!";
				Vector2 center = GameManager.MenuFontBig.MeasureString(text);
				center = center / 2;
				Vector2 textLocation = GameManager.GameScreenRectangle.Center.ToVector2();
				float scale = (float)(2 * MathHelper.Clamp(timeSinceMapOver, 0, 1.5f));
				//spriteBatch.DrawString(GameManager.MenuFontBig, text, textLocation, Color.White);
				spriteBatch.DrawString(GameManager.MenuFontBig, text, textLocation, Color.White, 0, center, scale, SpriteEffects.None, 0.5f);
				if (timeSinceMapOver > mapOverDelay)
				{
					MapOverMenu.Draw(spriteBatch);
				}
			}

			spriteBatch.DrawString(GameManager.IngameFont, fps, new Vector2(GameManager.GameScreenRectangle.Right - GameManager.IngameFont.MeasureString(fps).X, GameManager.GameScreenRectangle.Height - GameManager.IngameFont.MeasureString(fps).Y), Color.Gold);
			Helper.Draw(spriteBatch, gameTime);
			spriteBatch.End();
			base.Draw(gameTime);
		}

		/// <summary>
		/// Draws the <see cref="copyright"/> and <see cref="url"/> to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public void DrawCopyright(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawString(GameManager.IngameFontSmall, copyright, urlRectangle.Location.ToVector2(), Color.Gold);
			spriteBatch.DrawString(GameManager.IngameFontSmall, url, urlRectangle.Location.ToVector2() + new Vector2(0, urlRectangle.Height / 2), Color.Gold);
		}

		/// <summary>
		/// Draws the main menu background to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public void DrawMTDBackground(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(GameManager.RectTexture, M_Left, Color.Orange);
			spriteBatch.Draw(GameManager.RectTexture, M_Right, Color.Orange);
			spriteBatch.Draw(GameManager.RectTexture, M_Middle, Color.Orange);
			spriteBatch.Draw(GameManager.RectTexture, M_MiddleLeft, Color.Orange);
			spriteBatch.Draw(GameManager.RectTexture, M_MiddleRight, Color.Orange);

			spriteBatch.Draw(GameManager.RectTexture, T_Top, Color.OrangeRed);
			spriteBatch.Draw(GameManager.RectTexture, T_Bottom, Color.OrangeRed);

			spriteBatch.Draw(GameManager.RectTexture, D_Left, Color.OrangeRed);
			spriteBatch.Draw(GameManager.RectTexture, D_Right, Color.OrangeRed);
			spriteBatch.Draw(GameManager.RectTexture, D_Top, Color.OrangeRed);
			spriteBatch.Draw(GameManager.RectTexture, D_Bottom, Color.OrangeRed);
		}
	}
}