using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MinimalisticTD
{
	/// <summary>
	/// A tower placed by the <see cref="Player"/>.
	/// </summary>
	public class Tower : Actor
	{
		/// <summary>
		/// Contains information for all <see cref="DamageType"/>s and levels a <see cref="Tower"/> can have.
		/// </summary>
		public static Dictionary<DamageType, Dictionary<int, Dictionary<string, float>>> LevelValues = new Dictionary<DamageType, Dictionary<int, Dictionary<string, float>>>();

		/// <summary>
		/// Contains a color for each <see cref="DamageType"/>.
		/// </summary>
		public static Dictionary<DamageType, Color> TypeColors = new Dictionary<DamageType, Color>();

		/// <summary>
		/// The type of damage a tower can cause.
		/// </summary>
		public enum DamageType
		{
			/// <summary>
			/// Classic physical damage. Pure blunt force.
			/// </summary>
			Physical,

			/// <summary>
			/// Ice damage. Slows and/or freezes the enemy. Effective against fire. Useless against ice.
			/// </summary>
			Ice,

			/// <summary>
			/// Fire damage. Burns the enemy over time, but quickens them due to panic(?). Effective against ice, useless against fire.
			/// </summary>
			Fire,

			/// <summary>
			/// Poison damage. Relatively low initial damage, but does damage over time (if the enemy is not immune).
			/// </summary>
			Poison,

			/// <summary>
			/// Stun damage. Low initial damage, but has a chance to stun the enemy.
			/// </summary>
			Stun
		}

		/// <summary>
		/// The location on the <see cref="Map"/> grid.
		/// </summary>
		public Vector2 MapLocation;

		/// <summary>
		/// How much it costs to buy this <see cref="Tower"/>.
		/// </summary>
		public int Cost;

		/// <summary>
		/// The range of this <see cref="Tower"/> in <see cref="Map"/> pixels.
		/// </summary>
		public float Range;

		/// <summary>
		/// The damage this <see cref="Tower"/> does when it fires a <see cref="Shot"/>.
		/// </summary>
		private float damage;

		/// <summary>
		/// The current level of this <see cref="Tower"/>.
		/// </summary>
		private int level;

		/// <summary>
		/// The amount of money the <see cref="Player"/> gains when this <see cref="Tower"/> is sold.
		/// </summary>
		public int SellGain;

		/// <summary>
		/// How much it costs to upgrade this <see cref="Tower"/> to the next level.
		/// </summary>
		public int UpgradeCost;

		/// <summary>
		/// The time that has passed since this <see cref="Tower"/> last fired a <see cref="Shot"/>.
		/// </summary>
		private float timeSinceLastShot = 6;

		/// <summary>
		/// The delay between shots.
		/// </summary>
		private float shotDelay = 1;

		/// <summary>
		/// Specifies whether this <see cref="Tower"/> draws its <see cref="Range"/> to screen as a circle.
		/// </summary>
		public bool ShowRange;

		/// <summary>
		/// The <see cref="DamageType"/> of this <see cref="Tower"/>.
		/// </summary>
		private DamageType type;

		/// <summary>
		/// Gets or sets the <see cref="DamageType"/>.
		/// </summary>
		/// <value>
		/// The <see cref="DamageType"/>.
		/// </value>
		public DamageType Type
		{
			get { return type; }
			set
			{
				type = value;
				color = TypeColors[value];
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="level"/>.
		/// </summary>
		/// <value>
		/// The <see cref="level"/>.
		/// </value>
		public int Level
		{
			get { return level; }
			set
			{
				level = value;
				Cost = LevelValues[Type][value]["cost"].ToInt();
				Range = LevelValues[Type][value]["range"] * Map.TileWidth;
				damage = LevelValues[Type][value]["damage"];
				UpgradeCost = LevelValues[Type][value]["upgradeCost"].ToInt();
				SellGain = LevelValues[Type][value]["sellGain"].ToInt();
				shotDelay = LevelValues[Type][value]["shotDelay"];

				int screenWidth = LevelValues[Type][value]["screenWidth"].ToInt();
				int screenHeight = LevelValues[Type][value]["screenHeight"].ToInt();
				screenRectangle = new Rectangle((int)location.X, (int)location.Y, screenWidth, screenHeight);
				Center = MapLocation * Map.TileWidth + new Vector2(Map.TileWidth / 2, Map.TileHeight / 2) + new Vector2(GameManager.CurrentMap.MapOffsetX, GameManager.CurrentMap.MapOffsetY);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Tower"/> class.
		/// </summary>
		/// <param name="location">The location.</param>
		/// <param name="type">The <see cref="DamageType"/>.</param>
		/// <param name="mapLocation">The map location.</param>
		public Tower(Vector2 location, DamageType type, Vector2 mapLocation)
			: base(location)
		{
			Type = type;
			Level = 1;
			this.MapLocation = mapLocation;
			this.Center = location + new Vector2(Map.TileWidth / 2, Map.TileHeight / 2);
		}

		/// <summary>
		/// Chooses an <see cref="Enemy"/> to shoot at and fires the <see cref="Shot"/> if there is an active <see cref="Enemy"/> in <see cref="Range"/>.
		/// </summary>
		public void PickTarget()
		{
			Enemy target = null;

			//first find the enemies in range, and from them the one that's closest to the end
			foreach (Enemy enemy in GameManager.Enemies.FindAll(enemy => (this.Center - enemy.Center).Length() <= this.Range && enemy.IsActive))
			{
				if (target == null)
				{
					target = enemy;
					continue;
				}
				if (enemy.DistanceToEnd < target.DistanceToEnd)
				{
					target = enemy;
				}
			}
			if (target != null)
			{
				Shot shot = new Shot(this, Center, color, screenRectangle.Width / 2, screenRectangle.Height / 2, 110, target);
				GameManager.Shots.Add(shot);
			}
		}

		/// <summary>
		/// Upgrades this Tower.
		/// </summary>
		public void Upgrade()
		{
			if (level < 5)
			{
				SoundManager.PlayUpgradeTower();
				Level++;
			}
		}

		/// <summary>
		/// Draws the range circle for this tower to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		/// <param name="lineThickness">The line thickness.</param>
		public void DrawRangeCircle(SpriteBatch spriteBatch, int lineThickness)
		{
			Helper.DrawCircle(spriteBatch, Center, Range, Color.OrangeRed * 0.4f, lineThickness, Color.OrangeRed * 0.7f);
		}

		/// <summary>
		/// Draws this <see cref="Tower"/> to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public override void Draw(SpriteBatch spriteBatch)
		{
			if (ShowRange)
			{
				DrawRangeCircle(spriteBatch, 1);
				ShowRange = false;
			}
			base.Draw(spriteBatch);
			spriteBatch.DrawString(GameManager.IngameFont, this.level.ToString(), this.screenRectangle.Location.ToVector2(), Color.DarkOliveGreen);
		}

		/// <summary>
		/// Updates this <see cref="Tower"/>.
		/// </summary>
		/// <param name="gameTime">The <see cref="GameTime"/>.</param>
		public override void Update(GameTime gameTime)
		{
			timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (timeSinceLastShot >= shotDelay)
			{
				PickTarget();
				timeSinceLastShot = 0;
			}
		}

		/// <summary>
		/// Adds the specified values to the <see cref="LevelValues"/> dictionary.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="level">The level.</param>
		/// <param name="damage">The damage.</param>
		/// <param name="ticks">The ticks.</param>
		/// <param name="shotDelay">The shot delay.</param>
		/// <param name="range">The range.</param>
		/// <param name="cost">The cost.</param>
		/// <param name="upgradeCost">The upgrade cost.</param>
		/// <param name="sellGain">The sell gain.</param>
		/// <param name="screenWidth">Width of the screen.</param>
		/// <param name="screenHeight">Height of the screen.</param>
		public static void AddLevelValues(DamageType type, int level, float damage, float ticks, float shotDelay, float range, float cost, float upgradeCost, float sellGain, int screenWidth, int screenHeight)
		{
			Dictionary<string, float> values = new Dictionary<string, float>();
			values.Add("damage", damage);
			values.Add("ticks", ticks);
			values.Add("shotDelay", shotDelay);
			values.Add("range", range);
			values.Add("cost", cost);
			values.Add("upgradeCost", upgradeCost);
			values.Add("sellGain", sellGain);
			values.Add("screenWidth", screenWidth);
			values.Add("screenHeight", screenHeight);
			if (LevelValues.ContainsKey(type))
			{
				LevelValues[type].Add(level, values);
			}
			else
			{
				Dictionary<int, Dictionary<string, float>> levels = new Dictionary<int, Dictionary<string, float>>();
				levels.Add(level, values);
				LevelValues.Add(type, levels);
			}
		}
	}
}