using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinimalisticTD
{
	/// <summary>
	/// A wave of enemies.
	/// </summary>
	public class EnemyWave
	{
		/// <summary>
		/// Starting number of <see cref="Enemy"/> instances in this <see cref="EnemyWave"/>.
		/// </summary>
		public int EnemyCount;

		/// <summary>
		/// The <see cref="Enemy"/> instances this <see cref="EnemyWave"/> contains and releases over time.
		/// </summary>
		public Queue<Enemy> Enemies = new Queue<Enemy>();

		/// <summary>
		/// The amount of time that has passed since the last <see cref="Enemy"/> from the <see cref="Enemies"/> queue was released.
		/// </summary>
		private float timeSinceLastEnemy = 2;

		/// <summary>
		/// The delay between single <see cref="Enemy"/> releases.
		/// </summary>
		public float enemyDelay = 2;

		/// <summary>
		/// The average level of this <see cref="EnemyWave"/>. Used if the <see cref="Enemy"/> instances are created randomly.
		/// </summary>
		private int averageLevel = 1;

		/// <summary>
		/// Determines whether this <see cref="EnemyWave"/> is already/still active.
		/// </summary>
		public bool IsActive = false;

		//data for drawing the info circle:
		/// <summary>
		/// Where the circle for this <see cref="EnemyWave"/> will be drawn.
		/// </summary>
		public Vector2 InfoLocation;

		/// <summary>
		/// The radius of this <see cref="EnemyWave"/>'s info circle.
		/// </summary>
		public float Radius;

		/// <summary>
		/// Gets the percentage of enemies still alive.
		/// </summary>
		public float Percentage
		{
			get { return (this.Enemies.Count + GameManager.Enemies.Count) / EnemyCount; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnemyWave"/> class.
		/// </summary>
		/// <param name="enemyCount">The enemy count.</param>
		/// <param name="enemyDelay">The enemy delay.</param>
		/// <param name="index">The index.</param>
		public EnemyWave(int enemyCount, float enemyDelay, int index)
		{
			this.enemyDelay = enemyDelay;
			Radius = 10;
			EnemyCount = enemyCount;
			for (int i = 0; i < enemyCount; i++)
			{
				int level = GameManager.rand.Next(Math.Max(1, averageLevel - 2), averageLevel + 2);
				Enemy en = new Enemy(GameManager.CurrentMap.StartLocation * Map.TileWidth + new Vector2(GameManager.CurrentMap.MapOffsetX, GameManager.CurrentMap.MapOffsetY), Enemy.EnemyType.Normal, level);
				en.Center = (GameManager.CurrentMap.StartLocation * Map.TileWidth) + new Vector2(Map.TileWidth / 2, Map.TileHeight / 2) + new Vector2(GameManager.CurrentMap.MapOffsetX, GameManager.CurrentMap.MapOffsetY);
				Enemies.Enqueue(en);
			}

			//Info circle:
			int centerX = (int)(GameManager.MapRectangle.Right - (Radius * 2 + Radius / 2));
			int centerY = (int)MathHelper.Clamp(Radius / 2 + (index * (Radius * 2 + Radius / 2)), 0, GameManager.MapRectangle.Bottom);
			InfoLocation = new Vector2(centerX, centerY);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnemyWave"/> class.
		/// </summary>
		public EnemyWave()
		{
		}

		/// <summary>
		/// Sends this <see cref="EnemyWave"/> by setting <see cref="IsActive"/> to <code>true</code>.
		/// </summary>
		public void SendWave()
		{
			IsActive = true;
			SoundManager.PlayWaveStart();
		}

		/// <summary>
		/// Updates the wave to check if there are enemies left and sends them if necessary.
		/// </summary>
		/// <param name="gameTime">The game time used for updating.</param>
		public void Update(GameTime gameTime)
		{
			if (IsActive)
			{
				timeSinceLastEnemy += (float)gameTime.ElapsedGameTime.TotalSeconds;
				if (Enemies.Count == 0)
				{
					IsActive = false;
					return;
				}
				if (timeSinceLastEnemy >= enemyDelay)
				{
					Enemy e = Enemies.Dequeue();
					e.DistanceToEnd = GameManager.CurrentMap.PathLength * Map.TileWidth;
					GameManager.Enemies.Add(e);
					timeSinceLastEnemy = 0;
				}
			}
		}

		/// <summary>
		/// Draws the wave information to the screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			float colorOffset = IsActive ? 1 : 0.6f;
			Color circleColor = Color.Lerp(Color.Red, Color.Green, Percentage);
			Helper.DrawCircle(spriteBatch, InfoLocation, Radius, circleColor * 0.85f * colorOffset, 1, circleColor * 0.95f * colorOffset);
		}

		/// <summary>
		/// Generates a creation string for this wave.
		/// </summary>
		/// <returns>A string with all the information needed to create this wave.</returns>
		public string GetStringInfo()
		{
			StringBuilder info = new StringBuilder("enemyDelay=" + enemyDelay + "#");
			foreach (string s in Enemies.Select(e => e.GetStringInfo()))
			{
				info.Append(s);
			}
			info.Append("-");
			return info.ToString();
		}
	}
}