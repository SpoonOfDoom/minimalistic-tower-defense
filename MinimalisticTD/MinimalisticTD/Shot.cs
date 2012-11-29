using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinimalisticTD
{
	/// <summary>
	/// A shot fired by a <see cref="Tower"/>.
	/// </summary>
	public class Shot : Actor
	{
		/// <summary>
		/// The current rotation of this <see cref="Shot"/>.
		/// </summary>
		private float rotation = 0;

		/// <summary>
		/// The current scale of this <see cref="Shot"/>.
		/// </summary>
		private Vector2 scale;

		/// <summary>
		/// The <see cref="Tower"/> that fired this <see cref="Shot"/>.
		/// </summary>
		public Tower Parent;

		/// <summary>
		/// The <see cref="Enemy"/> this <see cref="Shot"/> is trying to hit.
		/// </summary>
		public Enemy Target;

		/// <summary>
		/// The amount of damage this <see cref="Shot"/> carries in different <see cref="Tower.DamageType"/>s.
		/// </summary>
		private Dictionary<Tower.DamageType, float> damage = new Dictionary<Tower.DamageType, float>();

		/// <summary>
		/// If there is any damage over time, this contains information about how many ticks each <see cref="Tower.DamageType"/> in this <see cref="Shot"/> should do.
		/// </summary>
		private Dictionary<Tower.DamageType, int> damageTicks = new Dictionary<Tower.DamageType, int>();

		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		/// <value>
		/// The location.
		/// </value>
		public Vector2 Location
		{
			get { return new Vector2(screenRectangle.X, screenRectangle.Y); }
			set { this.screenRectangle = new Rectangle((int)value.X, (int)value.Y, screenRectangle.Width, screenRectangle.Height); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Shot"/> class.
		/// </summary>
		/// <param name="location">The location.</param>
		/// <param name="color">The color.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		/// <param name="speed">The speed.</param>
		/// <param name="target">The target.</param>
		/// <param name="damage">The damage.</param>
		public Shot(Tower parent, Vector2 location, Color color, int width, int height, float speed, Enemy target)
			: base(location, color, width, height)
		{
			Parent = parent;
			Vector2 direction = target.Center - Center;
			direction.Normalize();
			rotation = (float)Math.Atan2(direction.Y, direction.X);
			scale = new Vector2(screenRectangle.Width * 2, screenRectangle.Height * 0.5f);
			this.speed = speed;
			this.damage.Add(Parent.Type, Tower.LevelValues[Parent.Type][Parent.Level]["damage"]);
			this.damageTicks.Add(Parent.Type, Tower.LevelValues[Parent.Type][Parent.Level]["ticks"].ToInt());
			this.velocity = speed * direction;
			this.Target = target;
		}

		/// <summary>
		/// Adds additional damage to this shot. Can be used for buffed <see cref="Tower"/>s or similar things.
		/// </summary>
		/// <param name="damageType">Type of the damage.</param>
		/// <param name="damage">The damage.</param>
		/// <param name="ticks">The ticks.</param>
		public void AddDamage(Tower.DamageType damageType, float damage, int ticks)
		{
			if (this.damage.ContainsKey(damageType))
			{
				this.damage[damageType] += damage;
				this.damageTicks[damageType] = Math.Max(this.damageTicks[damageType], (this.damageTicks[damageType] + ticks) / 2);
			}
			else
			{
				this.damage.Add(damageType, damage);
			}
		}

		/// <summary>
		/// Updates this <see cref="Shot"/>.
		/// </summary>
		/// <param name="gameTime">The <see cref="GameTime"/>.</param>
		public override void Update(GameTime gameTime)
		{
			if (IsActive) // still on screen?
			{
				if (!GameManager.MapRectangle.Intersects(screenRectangle))
				{
					IsActive = false;
					return;
				}
				//Calculate new direction because the enemy has likely moved by now

				Vector2 newDirection = Target.IsActive ? Target.Center - Center : velocity;
				newDirection.Normalize();
				rotation = (float)Math.Atan2(newDirection.Y, newDirection.X);
				velocity = speed * newDirection * (float)gameTime.ElapsedGameTime.TotalSeconds;
				if (!Target.IsActive)
				{
					color *= 0.95f;
					if (color.A < 0.001)
					{
						IsActive = false;
					}
				}
				if (CollidesWith(Target))
				{
					if (Target.IsActive)
					{
						foreach (var kv in damage)
						{
							//Hit with each assigned damage type and its value.
							//TODO: Create a new damage class that can handle damagetype, damage amount, damage over time, etc.
							Target.Hit(Parent.Type, Parent.Level, color);
						}
					}
					else
					{
						ParticleManager.AddHitEffect(location, color, 22, 1);
					}
					IsActive = false;
				}
				base.Update(gameTime);
			}
		}

		/// <summary>
		/// Draws this <see cref="Shot"/> to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(GameManager.RectTexture, location, null, color, rotation, GameManager.RectTexture.Bounds.Center.ToVector2(), scale, SpriteEffects.None, 0);
		}
	}
}