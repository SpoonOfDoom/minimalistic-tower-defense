using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinimalisticTD
{
	/// <summary>
	/// An <see cref="Enemy"/>.
	/// </summary>
	public class Enemy : Actor
	{
		/// <summary>
		/// A <see cref="Dictionary"/> which contains different values for different types and levels of an <see cref="Enemy"/>, things such as speed, health, armor, etc.
		/// </summary>
		public static Dictionary<EnemyType, Dictionary<int, Dictionary<string, float>>> LevelValues = new Dictionary<EnemyType, Dictionary<int, Dictionary<string, float>>>();

		/// <summary>
		/// A <see cref="Dictionary"/> which contains resistance values for different types and levels of an <see cref="Enemy"/>, e.g. resistance to ice, to physical damage, etc.
		/// </summary>
		public static Dictionary<EnemyType, Dictionary<int, Dictionary<Tower.DamageType, float>>> LevelResistance = new Dictionary<EnemyType, Dictionary<int, Dictionary<Tower.DamageType, float>>>();

		/// <summary>
		/// A <see cref="Dictionary"/> which contains different <see cref="Color"/>s for different types of an <see cref="Enemy"/>.
		/// </summary>
		public static Dictionary<EnemyType, Color> TypeColors = new Dictionary<EnemyType, Color>();

		/// <summary>
		/// This <see cref="Enemy"/>'s health points.
		/// </summary>
		private float health;

		/// <summary>
		/// This <see cref="Enemy"/>'s maximum health points.
		/// </summary>
		private float maxHealth;

		/// <summary>
		/// This <see cref="Enemy"/>'s armor.
		/// </summary>
		private float armor;

		/// <summary>
		/// How far this <see cref="Enemy"/> has to walk to the end tile.
		/// </summary>
		public float DistanceToEnd;

		/// <summary>
		/// How many points this <see cref="Enemy"/> is worth when killed.
		/// </summary>
		public float Points;

		/// <summary>
		/// How much money this <see cref="Enemy"/> is worth when killed.
		/// </summary>
		public float MoneyWorth;

		/// <summary>
		/// This <see cref="Enemy"/>'s level.
		/// </summary>
		private int level;

		/// <summary>
		/// The index of the current waypoint this <see cref="Enemy"/> is trying to reach.
		/// </summary>
		private int currentWayPoint;

		/// <summary>
		/// Measures the time that has passed since the last time damage over time was applied.
		/// </summary>
		private float timeSinceLastDamageTick = 0;

		/// <summary>
		/// The interval between single applications of damage over time.
		/// </summary>
		private float DamageTick = 1.5f;

		/// <summary>
		/// The waypoint this <see cref="Enemy"/> is currently trying to reach (i.e. the next tile of the path).
		/// </summary>
		public Vector2 WayPoint;

		/// <summary>
		/// The <see cref="EnemyType"/> of this <see cref="Enemy"/>.
		/// </summary>
		private EnemyType type;

		/// <summary>
		/// Gets or sets the <see cref="EnemyType"/> of this <see cref="Enemy"/>.
		/// </summary>
		/// <value>
		/// The <see cref="EnemyType"/>.
		/// </value>
		public EnemyType Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
				color = TypeColors[value];
			}
		}

		/// <summary>
		/// A <see cref="Queue"/> of damage over time values. Each <see cref="DamageTick"/> the appropriate damage is applied and removed from this <see cref="Queue"/>.
		/// </summary>
		public Queue<List<KeyValuePair<Tower.DamageType, float>>> ActiveDamageTicks = new Queue<List<KeyValuePair<Tower.DamageType, float>>>();

		/// <summary>
		/// The types an <see cref="Enemy"/> can have.
		/// </summary>
		public enum EnemyType
		{
			/// <summary>
			/// Standard type for an <see cref="Enemy"/>.
			/// </summary>
			Normal,

			/// <summary>
			/// This type of <see cref="Enemy"/> has a thick ice armor, but is slower.
			/// </summary>
			Ice
		}

		/// <summary>
		/// Gets or sets the level of this <see cref="Enemy"/>. It also reads all values from the <see cref="LevelValues"/> <see cref="Dictionary"/> that are appropriate for the level.
		/// </summary>
		/// <value>
		/// The level.
		/// </value>
		public int Level
		{
			get { return level; }
			set
			{
				level = value;
				speed = LevelValues[Type][value]["speed"];
				health = LevelValues[Type][value]["health"];
				maxHealth = LevelValues[Type][value]["health"];
				armor = LevelValues[Type][value]["armor"];
				Points = LevelValues[Type][value]["points"];
				MoneyWorth = LevelValues[Type][value]["moneyWorth"];
				resistance = LevelResistance[Type][value];
				int width = LevelValues[Type][value]["screenWidth"].ToInt();
				int height = LevelValues[Type][value]["screenHeight"].ToInt();
				screenRectangle = new Rectangle(screenRectangle.X, screenRectangle.Y, width, height);
			}
		}

		/// <summary>
		/// This dictionary contains resistance values against different <see cref="DamageType"/>s.
		/// </summary>
		private Dictionary<Tower.DamageType, float> resistance = new Dictionary<Tower.DamageType, float>();

		/// <summary>
		/// Gets the hitpoint percentage.
		/// </summary>
		public float HitpointPercentage
		{
			get { return health / maxHealth; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Enemy"/> class.
		/// </summary>
		/// <param name="location">The location where this <see cref="Enemy"/> will be created.</param>
		/// <param name="color">The color this <see cref="Enemy"/> will have.</param>
		/// <param name="width">The onscreen width.</param>
		/// <param name="height">The onscreen height.</param>
		/// <param name="health">The maximum and starting health of this <see cref="Enemy"/>.</param>
		/// <param name="armor">The armor of this <see cref="Enemy"/>.</param>
		/// <param name="speed">The speed of this <see cref="Enemy"/>.</param>
		/// <param name="moneyWorth">The money this <see cref="Enemy"/> is worth when killed.</param>
		/// <param name="points">The points this <see cref="Enemy"/> is worth when killed.</param>
		/// <param name="level">The level of this <see cref="Enemy"/>.</param>
		public Enemy(Vector2 location, EnemyType type, int level)
			: base(location)
		{
			Type = type;
			Level = level;
			currentWayPoint = 0;
			Center = location + new Vector2(screenRectangle.Width / 2, screenRectangle.Height / 2);
			if (GameManager.CurrentMap.TurningPoints.Count > currentWayPoint)
			{
				WayPoint = GameManager.CurrentMap.TurningPoints[currentWayPoint];
			}
			velocity = WayPoint - Center;
			velocity.Normalize();
		}

		/// <summary>
		/// Called by a <see cref="Shot"/> when it hits the <see cref="Enemy"/>.
		/// </summary>
		/// <param name="damageType">The <see cref="DamageType"/> of the damage.</param>
		/// <param name="level">The level of the <see cref="Tower"/> that has caused the damage.</param>
		/// <param name="shotColor">Color of the <see cref="Shot"/>.</param>
		public void Hit(Tower.DamageType damageType, int level, Color shotColor)
		{
			float damage = Tower.LevelValues[damageType][level]["damage"];
			float ticks = Tower.LevelValues[damageType][level]["ticks"];
			if (ticks > 1)
			{
				damage = damage / ticks;
				for (int i = 0; i < ticks - 1; i++)
				{
					if (i < ActiveDamageTicks.Count)
					{
						ActiveDamageTicks.ElementAt(i).Add(new KeyValuePair<Tower.DamageType, float>(damageType, damage));
					}
					else
					{
						List<KeyValuePair<Tower.DamageType, float>> list = new List<KeyValuePair<Tower.DamageType, float>>();
						list.Add(new KeyValuePair<Tower.DamageType, float>(damageType, damage));
						ActiveDamageTicks.Enqueue(list);
					}
				}
			}
			TakeDamage(damageType, damage, shotColor);
		}

		/// <summary>
		/// Takes the specified damage with the specified <see cref="DamageType"/>.
		/// </summary>
		/// <param name="damageType">The <see cref="DamageType"/>.</param>
		/// <param name="damage">The amount of damage.</param>
		/// <param name="shotColor">Color of the shot that has cause the damage.</param>
		/// <returns><code>true</code>, if the <see cref="Enemy"/> is still active (i.e. has hitpoints left) after this, otherwise <code>false</code>.</returns>
		public bool TakeDamage(Tower.DamageType damageType, float damage, Color? shotColor = null)
		{
			speed = LevelValues[Type][Level]["speed"];
			bool stunned = false;
			float speedModifier = 1;

			if (!stunned)
			{
				if (damageType == Tower.DamageType.Stun)
				{
					stunned = true;
				}
				else if (damageType == Tower.DamageType.Ice)
				{
					speedModifier -= speedModifier / 3; //I know, I know. Hard coded values are evil. But they'll do for now.
				}
				else if (damageType == Tower.DamageType.Poison)
				{
					speedModifier *= 0.9f; //I know, I know. Hard coded values are evil. But they'll do for now.
				}
			}
			if (stunned)
			{
				speed = 0;
			}
			else
			{
				speed *= speedModifier;
			}

			if (!shotColor.HasValue)
			{
				shotColor = Tower.TypeColors[damageType];
			}
			health -= (damage / (armor * LevelResistance[Type][Level][damageType]));
			timeSinceLastDamageTick = 0;
			if (health <= 0)
			{
				Color hitColor = Color.Lerp(color, shotColor.Value, 0.2f);
				IsActive = false;
				ParticleManager.AddExplosion(Center, hitColor, 23, 1.2f);
				GameManager.PlayerOne.AddKill((int)(Points + (DistanceToEnd / 10)), (int)MoneyWorth);
				return false;
			}
			else
			{
				Color hitColor = Color.Lerp(color, shotColor.Value, 0.8f);
				ParticleManager.AddHitEffect(Center, hitColor, 20, 1);
				return true;
			}
		}

		/// <summary>
		/// Applies the tick damage from the specified list.
		/// </summary>
		/// <param name="damageList">The damage list.</param>
		public void ApplyTickDamage(List<KeyValuePair<Tower.DamageType, float>> damageList)
		{
			foreach (var kvDamage in damageList)
			{
				if (!TakeDamage(kvDamage.Key, kvDamage.Value, null))
				{
					break;
				}
			}
		}

		/// <summary>
		/// Updates this enemy.
		/// </summary>
		/// <param name="gameTime">The <see cref="GameTime"/>.</param>
		public override void Update(GameTime gameTime)
		{
			if (IsActive)
			{
				//First check if there is any damage over time
				timeSinceLastDamageTick += (float)gameTime.ElapsedGameTime.TotalSeconds;
				if (timeSinceLastDamageTick >= DamageTick)
				{
					if (ActiveDamageTicks.Count > 0)
					{
						ApplyTickDamage(ActiveDamageTicks.Dequeue());
						//Check again if this enemy is still active - the tick damage might have killed him
						if (!IsActive)
						{
							return;
						}
					}
					else
					{
						speed = LevelValues[Type][Level]["speed"];
					}
				}
				if ((WayPoint - Center).Length() <= 1)
				{
					currentWayPoint++;
					if (GameManager.CurrentMap.TurningPoints.Count > currentWayPoint)
					{
						WayPoint = GameManager.CurrentMap.TurningPoints[currentWayPoint];
					}
					else
					{
						GameManager.PlayerOne.EnemyBreach();
						IsActive = false;
					}
					velocity = WayPoint - Center;
					velocity.Normalize();
				}
				DistanceToEnd -= (velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * speed).Length();
				base.Update(gameTime);
			}
		}

		/// <summary>
		/// Gets a creation string for this enemy.
		/// </summary>
		/// <returns>A string with all the information needed to create this enemy.</returns>
		public string GetStringInfo()
		{
			StringBuilder info = new StringBuilder();
			info.Append("Type=" + Enum.GetName(typeof(EnemyType), Type) + ",");
			info.Append("Level=" + this.level + ";");
			return info.ToString();
		}

		/// <summary>
		/// Draws the health bar to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public void DrawHealthBar(SpriteBatch spriteBatch)
		{
			Rectangle bgRect = new Rectangle(screenRectangle.X + 3, screenRectangle.Bottom + 1, screenRectangle.Width - 6, 5);
			int width = (int)((bgRect.Width - 2) * HitpointPercentage);
			Rectangle healthRect = new Rectangle(screenRectangle.X + 4, screenRectangle.Bottom + 2, width, 3);

			spriteBatch.Draw(GameManager.RectTexture, bgRect, Color.Black);
			spriteBatch.Draw(GameManager.RectTexture, healthRect, Color.DarkRed);
		}

		/// <summary>
		/// Draws the armor level to screen.
		/// </summary>
		/// <param name="spriteBatch">The <see cref="SpriteBatch"/> used for drawing to screen.</param>
		public void DrawArmorLevel(SpriteBatch spriteBatch)
		{
#if DEBUG
			spriteBatch.DrawString(GameManager.IngameFont, timeSinceLastDamageTick.ToString(), new Vector2(screenRectangle.Left - 3, screenRectangle.Top - 18), Color.Gold);
#endif
			spriteBatch.DrawString(GameManager.IngameFont, armor.ToString(), new Vector2(screenRectangle.Right - 3, screenRectangle.Bottom - 3), Color.Gold);
			
		}

		/// <summary>
		/// Draws this <see cref="Enemy"/> and its health and armor information to screen.
		/// </summary>
		/// <param name="spriteBatch">The <see cref="SpriteBatch"/> used for drawing to screen.</param>
		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			spriteBatch.DrawString(GameManager.IngameFont, this.level.ToString(), this.location, Color.DarkViolet);

			DrawHealthBar(spriteBatch);
			DrawArmorLevel(spriteBatch);
		}

		/// <summary>
		/// Adds an entry to the <see cref="LevelValues"/> Dictionary.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="level">The level.</param>
		/// <param name="health">The health.</param>
		/// <param name="armor">The armor.</param>
		/// <param name="moneyWorth">The money worth.</param>
		/// <param name="points">The points.</param>
		/// <param name="speed">The speed.</param>
		/// <param name="resistance">The resistance.</param>
		/// <param name="screenWidth">Width on the screen.</param>
		/// <param name="screenHeight">Height on the screen.</param>
		public static void AddLevelValues(EnemyType type, int level, float health, float armor, float moneyWorth, float points, float speed, Dictionary<Tower.DamageType, float> resistance, int screenWidth, int screenHeight)
		{
			Dictionary<string, float> values = new Dictionary<string, float>();
			values.Add("health", health);
			values.Add("armor", armor);
			values.Add("moneyWorth", moneyWorth);
			values.Add("points", points);
			values.Add("speed", speed);
			values.Add("screenWidth", screenWidth);
			values.Add("screenHeight", screenHeight);

			if (LevelValues.ContainsKey(type))
			{
				LevelValues[type].Add(level, values);
			}
			else
			{
				Dictionary<int, Dictionary<string, float>> levelsV = new Dictionary<int, Dictionary<string, float>>();
				levelsV.Add(level, values);
				LevelValues.Add(type, levelsV);
			}
			if (LevelResistance.ContainsKey(type))
			{
				LevelResistance[type].Add(level, resistance);
			}
			else
			{
				Dictionary<int, Dictionary<Tower.DamageType, float>> levelsR = new Dictionary<int, Dictionary<Tower.DamageType, float>>();
				levelsR.Add(level, resistance);
				LevelResistance.Add(type, levelsR);
			}
		}
	}
}