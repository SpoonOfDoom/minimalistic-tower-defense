using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinimalisticTD
{
	/// <summary>
	/// An explosion caused by a destroyed enemy. Displays a number of <see cref="Particle"/>s.
	/// </summary>
	public class Explosion : Actor
	{
		/// <summary>
		/// The list of <see cref="Particle"/>s which belong to this <see cref="Explosion"/>.
		/// </summary>
		private List<Particle> particles = new List<Particle>();

		/// <summary>
		/// Initializes a new instance of the <see cref="Explosion"/> class.
		/// </summary>
		/// <param name="location">The location where this <see cref="Explosion"/> is created.</param>
		/// <param name="color">The color of this <see cref="Explosion"/>'s <see cref="Particle"/>s.</param>
		/// <param name="particleAmount">The amount of <see cref="Particle"/>s this <see cref="Explosion"/> has.</param>
		public Explosion(Vector2 location, Color color, int particleAmount, float avgSpeed, float avgDuration)
			: base(location)
		{
			particles.AddRange(Particle.CreateParticles(particleAmount, location, color, avgSpeed, avgDuration));
		}

		/// <summary>
		/// Updates this <see cref="Explosion"/> and its <see cref="Particle"/>s.
		/// </summary>
		/// <param name="gameTime">The <see cref="GameTime"/>.</param>
		public override void Update(GameTime gameTime)
		{
			for (int i = particles.Count - 1; i >= 0; i--)
			{
				if (particles[i].IsActive)
				{
					particles[i].Update(gameTime);
				}
				else
				{
					particles.RemoveAt(i);
				}
			}
			if (particles.Count == 0)
			{
				IsActive = false;
			}
		}

		/// <summary>
		/// Draws this <see cref="Explosion"/> and its <see cref="Particle"/>s to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public override void Draw(SpriteBatch spriteBatch)
		{
			foreach (Particle particle in particles)
			{
				particle.Draw(spriteBatch);
			}
		}
	}
}