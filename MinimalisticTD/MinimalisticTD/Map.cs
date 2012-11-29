using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinimalisticTD
{
	/// <summary>
	/// A map made of tiles of different <see cref="TileType"/>s.
	/// </summary>
	[Serializable]
	public class Map
	{
		/// <summary>
		/// The filetype extension used when saving <see cref="Map"/>s.
		/// </summary>
		public const string FileTypeString = ".MTDM";

		/// <summary>
		/// Creates a thumbnail <see cref="Texture2D"/> for displaying in the <see cref="MapSelector"/>.
		/// </summary>
		public Texture2D Thumbnail
		{
			get
			{
				Texture2D texture = new Texture2D(GameManager.ActiveGraphicsDevice, MapWidth, MapHeight);
				texture.SetData(thumbnailColors);
				return texture;
			}
		}

		/// <summary>
		/// Contains the necessary <see cref="Color"/> data for creating the <see cref="Thumbnail"/>.
		/// </summary>
		public Color[] thumbnailColors;

		/// <summary>
		/// The filepath of this <see cref="Map"/> (if it's saved to disk).
		/// </summary>
		public string FilePath;

		/// <summary>
		/// Offset of the <see cref="Map"/> in the X direction in pixels.
		/// </summary>
		public int MapOffsetX = 0;

		/// <summary>
		/// Offset of the <see cref="Map"/> in the Y direction in pixels.
		/// </summary>
		public int MapOffsetY = 0;

		/// <summary>
		/// Specifies whether this is a campaign map.
		/// </summary>
		public bool IsCampaign = false;

		/// <summary>
		/// The position (in <see cref="Map"/> coordinates) of the start tile.
		/// </summary>
		public Vector2 StartLocation;

		/// <summary>
		/// The position (in <see cref="Map"/> coordinates) of the end tile.
		/// </summary>
		public Vector2 EndLocation;

		/// <summary>
		/// Specifies whether this <see cref="Map"/> is playable, i.e. if there's a path from <see cref="StartLocation"/> to <see cref="EndLocation"/> and if there are <see cref="EnemyWave"/>s.
		/// </summary>
		public bool Playable = false;

		/// <summary>
		/// A list of navigation points an <see cref="Enemy"/> uses to find his way around the <see cref="Map"/>.
		/// </summary>
		public List<Vector2> TurningPoints = new List<Vector2>();

		/// <summary>
		/// Used in <see cref="Map"/> generating. Specifies in which <see cref="Direction"/> to go first from the <see cref="StartLocation"/> tile.
		/// </summary>
		public Direction StartDirection;

		/// <summary>
		/// Contains information for creating the <see cref="EnemyWaves"/> of this map.
		/// </summary>
		public string EnemyWaveInfo = string.Empty;

		/// <summary>
		/// Contains the <see cref="EnemyWave"/>s the player has to fight off on this <see cref="Map"/>. Is not serialized because not always needed; instead it's created with the <see cref="EnemyWaveInfo"/> string when necessary.
		/// </summary>
		[NonSerialized]
		public List<EnemyWave> EnemyWaves = new List<EnemyWave>();

		/// <summary>
		/// The index of the currently active <see cref="EnemyWave"/>.
		/// </summary>
		public int CurrentWave = -1;

		/// <summary>
		/// Possible types of tiles.
		/// </summary>
		public enum TileType
		{
			/// <summary>
			/// Starting tile. Enemies spawn here.
			/// </summary>
			PathStart,

			/// <summary>
			/// The end tile. This is the tile enemies have to reach.
			/// </summary>
			PathEnd,

			/// <summary>
			/// A normal path tile. Enemies can walk here.
			/// </summary>
			Path,

			/// <summary>
			/// A normal environment tile. Towers can be placed here.
			/// </summary>
			NormalEnvironment,

			/// <summary>
			/// An environment tile where towers cannot be placed.
			/// </summary>
			NonPlaceableEnvironment
		}

		/// <summary>
		/// Directions used during map creation.
		/// </summary>
		public enum Direction { Up, Right, Down, Left, None }

		/// <summary>
		/// Specifies if the <see cref="Map"/> is currently running or if it has been lost or won.
		/// </summary>
		public enum MapState
		{
			/// <summary>
			/// The <see cref="Map"/> has been lost.
			/// </summary>
			Lost,

			/// <summary>
			/// The <see cref="Map"/> has been won.
			/// </summary>
			Won,

			/// <summary>
			/// The <see cref="Map"/> is currently being played.
			/// </summary>
			Running
		}

		/// <summary>
		/// The width of a single map tile.
		/// </summary>
		public const int TileWidth = 32;

		/// <summary>
		/// The height of a single map tile.
		/// </summary>
		public const int TileHeight = 32;

		/// <summary>
		/// The width of the <see cref="Map"/> in tiles.
		/// </summary>
		public int MapWidth;

		/// <summary>
		/// The height of the <see cref="Map"/> in tiles.
		/// </summary>
		public int MapHeight;

		/// <summary>
		/// The length of the <see cref="Map"/>'s path in tiles.
		/// </summary>
		public int PathLength;

		/// <summary>
		/// Standard color for <see cref="TileType.NormalEnvironment"/> tiles.
		/// </summary>
		public static Color NormalEnvironmentColor = Color.DarkGreen;

		/// <summary>
		/// Standard color for <see cref="TileType.NonPlaceableEnvironment"/> tiles.
		/// </summary>
		public static Color NonPlaceableEnvironmentColor = Color.Brown;

		/// <summary>
		/// Standard color for <see cref="TileType.Path"/>, <see cref="TileType.PathStart"/> and <see cref="TileType.PathEnd"/> tiles.
		/// </summary>
		public static Color PathColor = Color.SandyBrown;

		/// <summary>
		/// The grid of tiles this <see cref="Map"/> consists of. Each tile has a <see cref="TileType"/>.
		/// </summary>
		public TileType[,] Tiles;

		/// <summary>
		/// The time that has passed since either the <see cref="Map"/> was started or since the last <see cref="EnemyWave"/> was sent.
		/// </summary>
		private float timeSinceLastWave = 0;

		/// <summary>
		/// Indicates the <see cref="Map"/>'s <see cref="MapState"/>, i.e. whether the <see cref="Map"/> is won, lost, or still running.
		/// </summary>
		public MapState MapWon = MapState.Running;

		/// <summary>
		/// The delay between single <see cref="EnemyWave"/>s.
		/// </summary>
		public float WaveDelay = 5;

		/// <summary>
		/// The starting amount of money the <see cref="Player"/> has on this map.
		/// </summary>
		public int StartingMoney = 200;

		/// <summary>
		/// The starting amount of lives the <see cref="Player"/> has on this map.
		/// </summary>
		public int StartingLives = 10;

		/// <summary>
		/// Initializes a new instance of the <see cref="Map"/> class.
		/// </summary>
		/// <param name="width">The width of the <see cref="Map"/> in tiles.</param>
		/// <param name="height">The height of the <see cref="Map"/> in tiles.</param>
		public Map(int width, int height)
		{
			MapWidth = width;
			MapHeight = height;
			Tiles = new TileType[MapWidth, MapHeight];
			StartLocation = new Vector2(-1, -1);
			EndLocation = new Vector2(-1, -1);
			MapOffsetX = GameManager.MapRectangle.X;
			MapOffsetY = GameManager.MapRectangle.Y;
			for (int x = 0; x < MapWidth; x++)
			{
				for (int y = 0; y < MapHeight; y++)
				{
					Tiles[x, y] = TileType.NormalEnvironment;
				}
			}
		}

		/// <summary>
		/// Validates this instance. Checks if there's a path from <see cref="StartLocation"/> to <see cref="EndLocation"/> and if there are <see cref="EnemyWaves"/> on this <see cref="Map"/>.
		/// </summary>
		public void Validate()
		{
			Playable = FindPath() && EnemyWaveInfo != string.Empty;
		}

		/// <summary>
		/// Finds the path from start to end tile. Also fills the <see cref="TurningPoints"/> list for this <see cref="Map"/>.
		/// </summary>
		/// <returns><code>true</code>, if a path is found, otherwise <code>false</code>.</returns>
		public bool FindPath()
		{
			//TODO: pathfinding magic goes here and keeps track of the distance
			TurningPoints.Clear();

			if (StartLocation.X < 0 || EndLocation.X < 0)
			{
				//Either StartLocation, EndLocation or both are not set (or at least not on the map, which would be even more worrying)
				return false;
			}
			bool existStartTile = false;
			bool existEndTile = false;
			foreach (TileType tile in Tiles)
			{
				if (tile == TileType.PathStart)
				{
					existStartTile = true;
				}
				if (tile == TileType.PathEnd)
				{
					existEndTile = true;
				}
				if (existEndTile && existStartTile)
				{
					break;
				}
			}
			if (!(existStartTile && existEndTile))
			{
				return false;
			}
			Vector2 currentMapPosition = StartLocation;
			Direction dirFrom = Direction.None;

			while (currentMapPosition != EndLocation)
			{
				if (TurningPoints.Count > Tiles.Length)
				{
					// something's wrong, abort
					return false;
				}
				int dirCount = 0;
				Direction dirTo = CheckNextDirection(currentMapPosition, dirFrom, out dirCount);
				if (dirCount > 1)
				{
					//there is more than one possible direction to move, which we will prohibit for the time being.
					return false;
				}
				switch (dirTo)
				{
					case Direction.Up:
						currentMapPosition += new Vector2(0, -1);
						//TurningPoints.Add(new Vector2(currentX * TileWidth + TileWidth / 2 + MapOffsetX, currentY * TileHeight + TileHeight / 2 + MapOffsetY));
						TurningPoints.Add(currentMapPosition * TileWidth + new Vector2(TileWidth / 2 + MapOffsetX, TileHeight / 2 + MapOffsetY));
						break;
					case Direction.Right:
						currentMapPosition += new Vector2(1, 0);
						TurningPoints.Add(currentMapPosition * TileWidth + new Vector2(TileWidth / 2 + MapOffsetX, TileHeight / 2 + MapOffsetY));
						break;
					case Direction.Down:
						currentMapPosition += new Vector2(0, 1);
						TurningPoints.Add(currentMapPosition * TileWidth + new Vector2(TileWidth / 2 + MapOffsetX, TileHeight / 2 + MapOffsetY));
						break;
					case Direction.Left:
						currentMapPosition += new Vector2(-1, 0);
						TurningPoints.Add(currentMapPosition * TileWidth + new Vector2(TileWidth / 2 + MapOffsetX, TileHeight / 2 + MapOffsetY));
						break;
					case Direction.None:
						currentMapPosition += new Vector2(0, 0);
						TurningPoints.Add(currentMapPosition * TileWidth + new Vector2(TileWidth / 2 + MapOffsetX, TileHeight / 2 + MapOffsetY));
						break;
				}
				dirFrom = dirTo.Opposite();
			}

			return true;
		}

		/// <summary>
		/// Checks the next direction.
		/// </summary>
		/// <param name="start">The start tile.</param>
		/// <param name="dirFrom">The direction from which you come.</param>
		/// <param name="moveCount">The move count.</param>
		/// <returns></returns>
		public Direction CheckNextDirection(Vector2 start, Direction dirFrom, out int moveCount)
		{
			moveCount = 0;
			Direction dirTo = Direction.None;

			if (start.Y > 0 && dirFrom != Direction.Up)
			{
				if (Tiles[(int)start.X, (int)start.Y - 1].IsPath())
				{
					dirTo = Direction.Up;
					moveCount++;
				}
			}
			if (start.X > 0 && dirFrom != Direction.Left)
			{
				if (Tiles[(int)start.X - 1, (int)start.Y].IsPath())
				{
					dirTo = Direction.Left;
					moveCount++;
				}
			}
			if (start.Y < MapHeight - 1 && dirFrom != Direction.Down)
			{
				if (Tiles[(int)start.X, (int)start.Y + 1].IsPath())
				{
					dirTo = Direction.Down;
					moveCount++;
				}
			}
			if (start.X < MapWidth - 1 && dirFrom != Direction.Right)
			{
				if (Tiles[(int)start.X + 1, (int)start.Y].IsPath())
				{
					dirTo = Direction.Right;
					moveCount++;
				}
			}

			return dirTo;
		}

		/// <summary>
		/// Generates a random map and its enemy waves.
		/// </summary>
		/// <param name="startLocation">The start location.</param>
		/// <param name="pathLength">Length of the path.</param>
		/// <param name="waveCount">The wave count.</param>
		public void GenerateMap(Vector2 startLocation, int pathLength, int waveCount)
		{
			PathLength = pathLength;
			StartLocation = startLocation;
			GenerateMap(pathLength, GameManager.rand.Next(4));
			for (int i = 0; i < waveCount; i++)
			{
				EnemyWave ew = new EnemyWave(GameManager.rand.Next(5, 10), 2, this.EnemyWaves.Count);
				EnemyWaveInfo += ew.GetStringInfo();
				this.EnemyWaves.Add(ew);
			}
			Playable = true;
		}

		/// <summary>
		/// Generates a random map (without enemy waves).
		/// </summary>
		/// <param name="pathLength">Length of the path.</param>
		/// <param name="startDirection">The start direction.</param>
		public void GenerateMap(int pathLength, int startDirection)
		{
			int straightCount = 0;
			bool retry = false;

			Direction dir = (Direction)(startDirection % 4);
			Direction from = dir.Opposite();
		StuckInLoop:
			int loopDetect = -1;
			if (retry)
			{
				for (int x = 0; x < MapWidth; x++)
				{
					for (int y = 0; y < MapHeight; y++)
					{
						Tiles[x, y] = TileType.NormalEnvironment;
					}
				}
			}
			TurningPoints.Clear();
			Tiles[(int)StartLocation.X, (int)StartLocation.Y] = TileType.PathStart;
			int currentX = (int)StartLocation.X;
			int currentY = (int)StartLocation.Y;
			for (int i = 0; i < pathLength; i++)
			{
				bool turn = GameManager.rand.Next(100) > 75;
			NewDirection:
				if (turn && straightCount >= 2)
				{
					if (GameManager.rand.Next(0, 100) >= 50)
					{
						dir = dir.TurnLeft();
					}
					else
					{
						dir = dir.TurnRight();
					}
					straightCount = 0;
				}

				switch (dir)
				{
					case Direction.Up:
						if (currentY - 1 >= 0 && !Tiles[currentX, currentY - 1].IsPath())
						{
							currentY--;

							TurningPoints.Add(new Vector2(currentX * TileWidth + TileWidth / 2 + MapOffsetX, currentY * TileHeight + TileHeight / 2 + MapOffsetY));
						}
						else
						{
							turn = true;
							loopDetect++;
							if (loopDetect >= 10)
							{
								//must be stuck, let's try again
								retry = true;
								goto StuckInLoop;
							}
							goto NewDirection;
						}
						break;
					case Direction.Down:
						if (currentY + 1 < MapHeight && !Tiles[currentX, currentY + 1].IsPath())
						{
							currentY++;
							TurningPoints.Add(new Vector2(currentX * TileWidth + TileWidth / 2 + MapOffsetX, currentY * TileHeight + TileHeight / 2 + MapOffsetY));
						}
						else
						{
							turn = true;
							loopDetect++;
							if (loopDetect >= 10)
							{
								//must be stuck, let's try again
								retry = true;
								goto StuckInLoop;
							}
							goto NewDirection;
						}
						break;
					case Direction.Left:
						if (currentX - 1 >= 0 && !Tiles[currentX - 1, currentY].IsPath())
						{
							currentX--;
							TurningPoints.Add(new Vector2(currentX * TileWidth + TileWidth / 2 + MapOffsetX, currentY * TileHeight + TileHeight / 2 + MapOffsetY));
						}
						else
						{
							turn = true;
							loopDetect++;
							if (loopDetect >= 10)
							{
								//must be stuck, let's try again
								retry = true;
								goto StuckInLoop;
							}
							goto NewDirection;
						}
						break;
					case Direction.Right:
						if (currentX + 1 < MapWidth && !Tiles[currentX + 1, currentY].IsPath())
						{
							currentX++;
							TurningPoints.Add(new Vector2(currentX * TileWidth + TileWidth / 2 + MapOffsetX, currentY * TileHeight + TileHeight / 2 + MapOffsetY));
						}
						else
						{
							turn = true;
							loopDetect++;
							if (loopDetect >= 4)
							{
								//must be stuck, let's try again
								retry = true;
								goto StuckInLoop;
							}
							goto NewDirection;
						}
						break;
				}
				Tiles[currentX, currentY] = TileType.Path;
				loopDetect = -1;
				straightCount++;
				if (i == 0)
				{
					StartDirection = dir;
				}
			}
			Tiles[currentX, currentY] = TileType.PathEnd;
			EndLocation = new Vector2(currentX, currentY);
			if (!TurningPoints.Any(tp => tp.X == currentX * TileWidth + TileWidth / 2 + MapOffsetX && tp.Y == currentY * TileHeight + TileHeight / 2 + MapOffsetY))
			{
				TurningPoints.Add(new Vector2(currentX * TileWidth + TileWidth / 2 + MapOffsetX, currentY * TileHeight + TileHeight / 2 + MapOffsetY));
			}
		}

		/// <summary>
		/// Resets this instance.
		/// </summary>
		public void Reset()
		{
			CurrentWave = -1;
			EnemyWaves.Clear();
			LoadEnemiesFromString();
			MapWon = MapState.Running;
		}

		/// <summary>
		/// Loads the <see cref="EnemyWave"/>s from the <see cref="EnemyWaveInfo"/> string.
		/// </summary>
		public void LoadEnemiesFromString()
		{
			List<EnemyWave> waves = new List<EnemyWave>();
			string[] waveStrings = EnemyWaveInfo.Split('-').Where(s => s != string.Empty).ToArray();
			foreach (string wave in waveStrings)
			{
				EnemyWave ew = new EnemyWave();
				string[] split = wave.Split('#').Where(s => s != string.Empty).ToArray();
				System.Diagnostics.Debug.Assert(split.Length == 2, "Something's wrong with the EnemyWave info.");
				ew.enemyDelay = split[0].Split('=')[1].ToFloat();
				string[] enemyStrings = split[1].Split(';').Where(s => s != string.Empty).ToArray();
				foreach (string enemy in enemyStrings)
				{
					string[] enemyInfo = enemy.Split(',').Where(s => s != string.Empty).ToArray();
					string typeString = enemyInfo[0].Split('=')[1];
					string levelString = enemyInfo[1].Split('=')[1];

					Enemy.EnemyType type = typeString.ToEnemyType();
					int level = levelString.ToInt();
					Enemy en = new Enemy(StartLocation * Map.TileWidth + new Vector2(TileWidth / 2 + MapOffsetX, TileHeight / 2 + MapOffsetY), type, level);
					en.Center = (GameManager.CurrentMap.StartLocation * Map.TileWidth) + new Vector2(Map.TileWidth / 2, Map.TileHeight / 2) + new Vector2(MapOffsetX, MapOffsetY);
					ew.Enemies.Enqueue(en);
				}
				ew.EnemyCount = ew.Enemies.Count;
				int centerX = (int)(GameManager.MapRectangle.Right - (ew.Radius * 2 + ew.Radius / 2));
				int centerY = (int)MathHelper.Clamp(ew.Radius / 2 + (waves.Count * (ew.Radius * 2 + ew.Radius / 2)), 0, GameManager.MapRectangle.Bottom);
				ew.InfoLocation = new Vector2(centerX, centerY);
				waves.Add(ew);
			}
			EnemyWaves = waves;
		}

		/// <summary>
		/// Updates the map and the enemy waves.
		/// </summary>
		/// <param name="gameTime">The game time.</param>
		public void Update(GameTime gameTime)
		{
			if (!EnemyWaves.Exists(wave => wave.Enemies.Count > 0) && GameManager.Enemies.Count <= 0)
			{
				MapWon = MapState.Won;
				SoundManager.PlayWin();
				return;
			}
			if (timeSinceLastWave >= WaveDelay)
			{
				CurrentWave++;
				timeSinceLastWave = 0;
				if (EnemyWaves.Count > CurrentWave)
				{
					EnemyWaves[CurrentWave].SendWave();
				}
			}
			foreach (EnemyWave wave in EnemyWaves.FindAll(wave => wave.IsActive))
			{
				wave.Update(gameTime);
			}
			if (CurrentWave < 0)
			{
				timeSinceLastWave += (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
			else if (CurrentWave < EnemyWaves.Count && CurrentWave >= 0 && EnemyWaves[CurrentWave].Enemies.Count <= 0)
			{
				timeSinceLastWave += (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
		}

		/// <summary>
		/// Draws this <see cref="Map"/> to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			Microsoft.Xna.Framework.Input.MouseState mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
			int positionX = Math.Max((mouse.X - MapOffsetX) / Map.TileWidth, 0);
			int positionY = Math.Max((mouse.Y - MapOffsetY) / Map.TileHeight, 0);
			Color tileColor = Color.White;
			if (positionX < MapWidth && positionY < MapHeight && GameManager.IsGameActive)
			{
				TileType t = Tiles[positionX, positionY];
				switch (t)
				{
					case TileType.PathStart:
						tileColor = PathColor;
						break;
					case TileType.PathEnd:
						tileColor = PathColor;
						break;
					case TileType.Path:
						tileColor = PathColor;
						break;
					case TileType.NormalEnvironment:
						tileColor = NormalEnvironmentColor;
						break;
					case TileType.NonPlaceableEnvironment:
						tileColor = NonPlaceableEnvironmentColor;
						break;
					default:
						tileColor = Color.White;
						break;
				}
			}

			for (int x = 0; x < MapWidth; x++)
			{
				for (int y = 0; y < MapHeight; y++)
				{
					Rectangle screenRectangle = new Rectangle((x * TileWidth) + MapOffsetX, (y * TileHeight) + MapOffsetY, TileWidth, TileHeight);
					Rectangle innerRectangle = new Rectangle(screenRectangle.X + 1, screenRectangle.Y + 1, screenRectangle.Width - 2, screenRectangle.Height - 2);
					switch (Tiles[x, y])
					{
						case TileType.PathStart:
							spriteBatch.Draw(GameManager.RectTexture, screenRectangle, Color.Black);
							spriteBatch.Draw(GameManager.RectTexture, innerRectangle, PathColor);
							spriteBatch.DrawString(GameManager.IngameFont, "S", new Vector2(x * Map.TileWidth + Map.TileWidth / 3 + MapOffsetX, y * Map.TileHeight + MapOffsetY), Color.Black);
							break;
						case TileType.PathEnd:
							spriteBatch.Draw(GameManager.RectTexture, screenRectangle, Color.Black);
							spriteBatch.Draw(GameManager.RectTexture, innerRectangle, PathColor);
							spriteBatch.DrawString(GameManager.IngameFont, "E", new Vector2(x * Map.TileWidth + Map.TileWidth / 3 + MapOffsetX, y * Map.TileHeight + MapOffsetY), Color.Black);
							break;
						case TileType.Path:
							spriteBatch.Draw(GameManager.RectTexture, screenRectangle, Color.Black);
							spriteBatch.Draw(GameManager.RectTexture, innerRectangle, PathColor);
							break;
						case TileType.NormalEnvironment:
							spriteBatch.Draw(GameManager.RectTexture, screenRectangle, NormalEnvironmentColor);
							break;
						case TileType.NonPlaceableEnvironment:
							spriteBatch.Draw(GameManager.RectTexture, screenRectangle, NonPlaceableEnvironmentColor);
							break;
					}
				}
			}
			if (GameManager.MapRectangle.Contains(mouse.X, mouse.Y))
			{
				Helper.DrawBorderRectangle(spriteBatch, GameManager.RectTexture, new Rectangle(positionX * Map.TileWidth + MapOffsetX, positionY * Map.TileHeight + MapOffsetY, Map.TileWidth, Map.TileHeight), 4, Color.Yellow, tileColor);
			}
		}

		/// <summary>
		/// Draws this <see cref="Map"/>'s <see cref="EnemyWave"/> info to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public void DrawWaves(SpriteBatch spriteBatch)
		{
			foreach (EnemyWave wave in EnemyWaves)
			{
				wave.Draw(spriteBatch);
			}
		}

		/// <summary>
		/// Draws a thumbnail of the map to the screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		/// <param name="location">The location.</param>
		/// <param name="scale">The scale.</param>
		public void DrawThumbnail(SpriteBatch spriteBatch, Vector2 location, float scale = 1)
		{
			spriteBatch.Draw(Thumbnail, location, null, Color.White, 0, new Vector2(Thumbnail.Width / 2, Thumbnail.Height / 2), scale, SpriteEffects.None, 0.3f);
			spriteBatch.DrawString(GameManager.IngameFont, FilePath, new Vector2(location.X * scale, (Thumbnail.Bounds.Bottom * scale) + 5), Color.Blue, 0, new Vector2(GameManager.IngameFont.MeasureString(FilePath).X / 2, GameManager.IngameFont.MeasureString(FilePath).Y / 2), scale, SpriteEffects.None, 0.2f);
		}
	}
}