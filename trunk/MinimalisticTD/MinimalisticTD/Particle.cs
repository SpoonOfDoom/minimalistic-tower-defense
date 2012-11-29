using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MinimalisticTD
{
	/// <summary>
	/// A simple particle.Nothing more than displaying colored pixels that don't collide or otherwise interact with anything. Looks pretty, though.
	/// </summary>
	public class Particle : Actor
	{
		/// <summary>
		/// The standard width of a single <see cref="Particle"/>.
		/// </summary>
		public const int ParticleWidth = 1;

		/// <summary>
		/// The standard height of a single <see cref="Particle"/>.
		/// </summary>
		public const int ParticleHeight = 1;

		/// <summary>
		/// Specifies how long this <see cref="Particle"/> has been on screen already.
		/// </summary>
		private float screenTime = 0;

		/// <summary>
		/// The maximum duration this <see cref="Particle"/> stays on screen.
		/// </summary>
		private float maxDuration;

		/// <summary>
		/// The <see cref="Color"/> this <see cref="Particle"/> has at the beginning.
		/// </summary>
		private Color startColor;

		/// <summary>
		/// The <see cref="Color"/> this <see cref="Particle"/> will have at the end of its lifetime.
		/// </summary>
		private Color endColor;

		/// <summary>
		/// Specifies how much of this <see cref="Particle"/>'s <see cref="maxDuration"/> has already passed.
		/// </summary>
		private float lerpProgress
		{
			get { return screenTime / maxDuration; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Particle"/> class.
		/// </summary>
		/// <param name="location">The location of this <see cref="Particle"/>.</param>
		/// <param name="color">The color of this <see cref="Particle"/>.</param>
		/// <param name="direction">The direction of this <see cref="Particle"/>.</param>
		/// <param name="speed">The speed of this <see cref="Particle"/>.</param>
		/// <param name="duration">The duration this <see cref="Particle"/> will stay on screen.</param>
		public Particle(Vector2 location, Color color, Vector2 direction, float speed, float duration)
			: base(location, color, ParticleWidth + GameManager.rand.Next(0, 3), ParticleHeight + GameManager.rand.Next(0, 3)) //randomize the width and height to change things up a bit
		{
			this.speed = speed;
			velocity = direction;
			velocity.Normalize();
			velocity = velocity * speed;
			maxDuration = duration;
			startColor = color;
			endColor = Color.DarkGray;
		}

		/// <summary>
		/// Creates a list of <see cref="Particle"/>s.
		/// </summary>
		/// <param name="count">How many <see cref="Particle"/>s will be created.</param>
		/// <param name="location">The location where each <see cref="Particle"/> will be created.</param>
		/// <param name="color">The color the created <see cref="Particle"/>s will have.</param>
		/// <param name="avgSpeed">The avg speed.</param>
		/// <param name="avgDuration">Duration of the avg.</param>
		/// <returns></returns>
		public static List<Particle> CreateParticles(int count, Vector2 location, Color color, float avgSpeed, float avgDuration)
		{
			List<Particle> particles = new List<Particle>();
			for (int i = 0; i < count; i++)
			{
				int x = GameManager.rand.Next(-50, 50);
				int y = GameManager.rand.Next(-50, 50);
				Vector2 direction = new Vector2(x, y);
				particles.Add(new Particle(location, color, direction, avgSpeed + GameManager.rand.Next(-5, +6), avgDuration + (float)GameManager.rand.NextDouble()));
			}
			return particles;
		}

		/// <summary>
		/// Updates this <see cref="Particle"/>.
		/// </summary>
		/// <param name="gameTime">The <see cref="GameTime"/>.</param>
		public override void Update(GameTime gameTime)
		{
			screenTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
			color = Color.Lerp(startColor, endColor, lerpProgress) * (1 - lerpProgress);
			if (screenTime >= maxDuration)
			{
				IsActive = false;
			}
			if (!GameManager.MapRectangle.Intersects(screenRectangle))
			{
				IsActive = false;
			}
			base.Update(gameTime);
		}
	}
}