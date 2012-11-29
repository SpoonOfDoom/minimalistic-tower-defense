using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MinimalisticTD
{
	/// <summary>
	/// The map editor.
	/// </summary>
	public static class MapEditor
	{
		/// <summary>
		/// Specifies whether the <see cref="CurrentMap"/> is an existing <see cref="Map"/> that was loaded or a new one.
		/// </summary>
		private static bool existingMap = false;

		/// <summary>
		/// Contains the <see cref="Map"/>' FilePath, if the <see cref="CurrentMap"/> is an <see cref="existingMap"/>.
		/// </summary>
		private static string existingPath = "";

		/// <summary>
		/// The <see cref="Map"/> that's currently being edited.
		/// </summary>
		public static Map CurrentMap;

		/// <summary>
		/// The <see cref="Menu"/> from which the user selects different <see cref="Map.TileType"/>s to place and can save or load the <see cref="Map"/>.
		/// </summary>
		private static Menu TileMenu;

		/// <summary>
		/// The currently selected <see cref="Map.TileType"/> that will be placed at a click.
		/// </summary>
		private static Map.TileType ActiveTileType = Map.TileType.NormalEnvironment;

		/// <summary>
		/// The <see cref="MapSelector"/> used for choosing which <see cref="Map"/> to load.
		/// </summary>
		public static MapSelector MapSelectorCustom = null;

		/// <summary>
		/// Initializes the map editor.
		/// </summary>
		public static void Initialize()
		{
			CurrentMap = new Map(20, 20);
			InitializeGUI();
		}

		/// <summary>
		/// Initializes the GUI.
		/// </summary>
		private static void InitializeGUI()
		{
			TileMenu = new Menu(new Vector2(0, 0), Menu.Orientation.Horizontal, Color.Black * 0.7f);
			foreach (string type in Enum.GetNames(typeof(Map.TileType)))
			{
				string text;
				switch (type)
				{
					case "PathStart":
						text = "Start";
						break;
					case "PathEnd":
						text = "End";
						break;
					case "Path":
						text = "Path";
						break;
					case "NormalEnviroment":
						text = "Env";
						break;
					case "NonPlaceableEnviroment":
						text = "Blocked";
						break;
					default:
						text = "Undefined";
						break;
				}
				SelectableMenuItem smi = new SelectableMenuItem(new Rectangle(0, 0, (int)GameManager.IngameFont.MeasureString(text).X, 20), GameManager.IngameFont, text, Color.Yellow, null, Color.Orange);
				smi.Click += new SelectableMenuItem.ClickEventHandler(TileMenuItem_Click);
				TileMenu.AddSelectableItem(smi);
			}

			SelectableMenuItem smiSave = new SelectableMenuItem(new Rectangle(0, 0, (int)GameManager.IngameFont.MeasureString("Save Map").X, 20), GameManager.IngameFont, "Save Map", Color.Yellow, null, Color.Orange);
			smiSave.Click += new SelectableMenuItem.ClickEventHandler(TileMenuItem_Click);
			TileMenu.AddSelectableItem(smiSave);

			SelectableMenuItem smiLoad = new SelectableMenuItem(new Rectangle(0, 0, (int)GameManager.IngameFont.MeasureString("Load Map").X, 20), GameManager.IngameFont, "Load Map", Color.Yellow, null, Color.Orange);
			smiLoad.Click += new SelectableMenuItem.ClickEventHandler(TileMenuItem_Click);
			TileMenu.AddSelectableItem(smiLoad);

			EnemyWaveEditor.Initialize(CurrentMap);
		}

		/// <summary>
		/// Handles the Click event of the TileMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data. Used to determine the button that was clicked.</param>
		private static void TileMenuItem_Click(object sender, ClickEventArgs e)
		{
			switch (e.Text)
			{
				case "Start":
					ActiveTileType = Map.TileType.PathStart;
					break;
				case "End":
					ActiveTileType = Map.TileType.PathEnd;
					break;
				case "Path":
					ActiveTileType = Map.TileType.Path;
					break;
				case "Env":
					ActiveTileType = Map.TileType.NormalEnvironment;
					break;
				case "Blocked":
					ActiveTileType = Map.TileType.NonPlaceableEnvironment;
					break;
				case "Save Map":
					SaveMap();
					break;
				case "Load Map":
					MapSelectorCustom = new MapSelector("CustomMaps", MapSelector.LoadMode.Editor);
					break;
			}
		}

		/// <summary>
		/// Handles the mouse input.
		/// </summary>
		/// <param name="mouse">The MouseState.</param>
		public static void HandleInput(MouseState mouse)
		{
			if (MapSelectorCustom != null)
			{
				if (MapSelectorCustom.MapMenu.ScreenRectangle.Contains(mouse.X, mouse.Y))
				{
					MapSelectorCustom.MapMenu.ClickHit(mouse.X, mouse.Y);
				}
			}
			else
			{
				if (TileMenu.ScreenRectangle.Contains(mouse.X, mouse.Y))
				{
					TileMenu.ClickHit(mouse.X, mouse.Y);
				}
				else if (EnemyWaveEditor.ClickHit(mouse))
				{
					return;
				}
				else
				{
					int mapPositionX = (mouse.X - CurrentMap.MapOffsetX) / Map.TileWidth;
					int mapPositionY = (mouse.Y - CurrentMap.MapOffsetY) / Map.TileHeight;

					if (ActiveTileType == Map.TileType.PathStart)
					{
						for (int x = 0; x < CurrentMap.MapWidth; x++)
						{
							for (int y = 0; y < CurrentMap.MapHeight; y++)
							{
								if (CurrentMap.Tiles[x, y] == Map.TileType.PathStart)
								{
									CurrentMap.Tiles[x, y] = Map.TileType.NormalEnvironment;
								}
							}
						}
						CurrentMap.StartLocation = new Vector2(mapPositionX, mapPositionY);
					}
					else if (ActiveTileType == Map.TileType.PathEnd)
					{
						for (int x = 0; x < CurrentMap.MapWidth; x++)
						{
							for (int y = 0; y < CurrentMap.MapHeight; y++)
							{
								if (CurrentMap.Tiles[x, y] == Map.TileType.PathEnd)
								{
									CurrentMap.Tiles[x, y] = Map.TileType.NormalEnvironment;
								}
							}
						}
						CurrentMap.EndLocation = new Vector2(mapPositionX, mapPositionY);
					}
					CurrentMap.Tiles[mapPositionX, mapPositionY] = ActiveTileType;
					CurrentMap.Playable = CurrentMap.FindPath();
				}
			}
		}

		/// <summary>
		/// Loads the map.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="loadMode">The load mode. Determines whether the map is opened for editing or other usage.</param>
		/// <returns></returns>
		public static void LoadMap(string filePath, MapSelector.LoadMode loadMode)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream fs = new FileStream(filePath, FileMode.Open);
			Map map = formatter.Deserialize(fs) as Map;
			fs.Close();
			if (loadMode == MapSelector.LoadMode.Editor)
			{
				CurrentMap = map;
				EnemyWaveEditor.Initialize(CurrentMap);
				MapSelectorCustom = null;
				existingMap = true;
				existingPath = filePath;
			}
			else if (loadMode == MapSelector.LoadMode.PlayCustom || loadMode == MapSelector.LoadMode.PlayCampaign)
			{
				GameManager.CurrentMap = map;
				map.LoadEnemiesFromString();
				MapSelectorCustom = null;
			}
		}

		/// <summary>
		/// Saves the <see cref="CurrentMap"/>.
		/// </summary>
		public static void SaveMap()
		{
			CurrentMap.EnemyWaveInfo = EnemyWaveEditor.CreateStringInfo();
			CurrentMap.Validate();
#if CAMPAIGN
			CurrentMap.IsCampaign = true;
#endif
			CurrentMap.thumbnailColors = new Color[CurrentMap.MapWidth * CurrentMap.MapHeight];
			int i = 0;
			for (int x = 0; x < CurrentMap.MapWidth; x++)
			{
				for (int y = 0; y < CurrentMap.MapHeight; y++)
				{
					int tileInt = (int)CurrentMap.Tiles[x, y];
					if (tileInt < 3)
					{
						CurrentMap.thumbnailColors[i] = Color.SandyBrown;
					}
					else if (tileInt == 3)
					{
						CurrentMap.thumbnailColors[i] = Color.DarkGreen;
					}
					else if (tileInt == 4)
					{
						CurrentMap.thumbnailColors[i] = Color.Brown;
					}
					else
					{
						CurrentMap.thumbnailColors[i] = Color.Black;
					}

					i++;
				}
			}

			string filePath;
			if (existingMap)
			{
				filePath = existingPath;
			}
			else
			{
				int mapCount = 0;
				if (!Directory.Exists("CustomMaps"))
				{
					Directory.CreateDirectory("CustomMaps");
					mapCount = 1;
				}
				else
				{
					mapCount = Directory.GetFiles("CustomMaps").Count() + 1;
				}
				filePath = @"CustomMaps\Map" + mapCount.ToString().PadLeft(4, '0') + Map.FileTypeString;
				while (File.Exists(filePath))
				{
					mapCount++;
					filePath = @"CustomMaps\Map" + mapCount.ToString().PadLeft(4, '0') + Map.FileTypeString;
				}
			}
			CurrentMap.FilePath = filePath;
			BinaryFormatter formatter = new BinaryFormatter();
			using (FileStream fs = new System.IO.FileStream(filePath, FileMode.Create))
			{
				formatter.Serialize(fs, CurrentMap);
				fs.Close();
			}
			existingMap = true;
			existingPath = filePath;
			Helper.ShowMessage("Map saved!", new Vector2(GameManager.GameScreenRectangle.Center.X, GameManager.GameScreenRectangle.Center.Y), 2.5f);
		}

		/// <summary>
		/// Saves the specified map. Can be called from anywhere.
		/// </summary>
		/// <param name="map">The map to be saved.</param>
		public static void SaveMap(Map map)
		{
			Map storedMap = null;
			bool storedExistingMap = false;
			string storedExistingPath = string.Empty;

			if (CurrentMap != null)
			{
				storedMap = CurrentMap;
				storedExistingMap = existingMap;
				storedExistingPath = existingPath;
			}
			existingMap = false;
			existingPath = string.Empty;
			CurrentMap = map;
			SaveMap();

			if (storedMap != null)
			{
				CurrentMap = storedMap;
				existingMap = storedExistingMap;
				existingPath = storedExistingPath;
			}
			else
			{
				CurrentMap = null;
			}
		}

		/// <summary>
		/// Draws the map and the editor GUI to screen.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch used for drawing to screen.</param>
		public static void Draw(SpriteBatch spriteBatch)
		{
			if (MapSelectorCustom != null)
			{
				MapSelectorCustom.Draw(spriteBatch);
			}
			else
			{
				CurrentMap.Draw(spriteBatch);
				Color tileColor;
				string tileText = "";

				switch (ActiveTileType)
				{
					case Map.TileType.PathStart:
						tileColor = Color.SandyBrown;
						tileText = "S";
						break;
					case Map.TileType.PathEnd:
						tileColor = Color.SandyBrown;
						tileText = "E";
						break;
					case Map.TileType.Path:
						tileColor = Color.SandyBrown;
						break;
					case Map.TileType.NormalEnvironment:
						tileColor = Color.DarkGreen;
						break;
					case Map.TileType.NonPlaceableEnvironment:
						tileColor = Color.Brown;
						break;
					default:
						tileColor = Color.Red;
						tileText = "ERR";
						break;
				}
				MouseState mouse = Mouse.GetState();
				if (GameManager.MapRectangle.Contains(mouse.X, mouse.Y))
				{
					int positionX = mouse.X - ((mouse.X - CurrentMap.MapOffsetX) % Map.TileWidth);
					int positionY = mouse.Y - ((mouse.Y - CurrentMap.MapOffsetY) % Map.TileHeight);
					Helper.DrawBorderRectangle(spriteBatch, GameManager.RectTexture, new Rectangle(positionX, positionY, Map.TileWidth, Map.TileHeight), 4, Color.Yellow, tileColor);
					spriteBatch.DrawString(GameManager.IngameFont, tileText, new Vector2(positionX + Map.TileWidth / 3, positionY), Color.Black);
				}
				EnemyWaveEditor.Draw(spriteBatch);
				TileMenu.Draw(spriteBatch);
			}
		}
	}
}