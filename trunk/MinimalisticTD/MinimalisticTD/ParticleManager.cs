using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinimalisticTD
{
	/// <summary>
	/// This class manages <see cref="Particle"/>s and <see cref="Explosion"/>s, creating, updating and drawing them.
	/// </summary>
	public static class ParticleManager
	{
		/// <summary>
		/// A list of active <see cref="Explosion"/>s.
		/// </summary>
		private static List<Explosion> Explosions = new List<Explosion>();

		/// <summary>
		/// A list of active <see cref="Particle"/>s.
		/// </summary>
		public static List<Particle> Particles = new List<Particle>();

		/// <summary>
		/// Adds an <see cref="Explosion"/> at the specified location.
		/// </summary>
		/// <param name="location">The location.</param>
		/// <param name="color">The color.</param>
		/// <param name="avgSpeed">The average speed of the <see cref="Explosion"/>'s <see cref="Particle"/>s.</param>
		/// <param name="avgDuration">The average duration of the <see cref="Explosion"/>'s <see cref="Particle"/>s.</param>
		public static void AddExplosion(Vector2 location, Color color, float avgSpeed, float avgDuration)
		{
			Explosion ex = new Explosion(location, color, GameManager.rand.Next(250, 450), avgSpeed, avgDuration);
			Explosions.Add(ex);
			SoundManager.PlayExplosion();
		}

		/// <summary>
		/// Adds a hit effect (i.e. a number of <see cref="Particle"/>s) at the specified location.
		/// </summary>
		/// <param name="location">The location.</param>
		/// <param name="color">The color.</param>
		/// <param name="avgSpeed">The average speed of the created <see cref="Particle"/>s.</param>
		/// <param name="avgDuration">The average Duration of the created <see cref="Particle"/>s.</param>
		public static void AddHitEffect(Vector2 location, Color color, float avgSpeed, float avgDuration)
		{
			int particleCount = GameManager.rand.Next(15, 25);
			Particles.AddRange(Particle.CreateParticles(particleCount, location, color, avgSpeed, avgDuration));
			SoundManager.PlayHit();
		}

		/// <summary>
		/// Updates this all items in the <see cref="Explosions"/> and <see cref="Particles"/> lists.
		/// </summary>
		/// <param name="gameTime">The <see cref="GameTime"/>.</param>
		public static void Update(GameTime gameTime)
		{
			for (int i = Explosions.Count - 1; i >= 0; i--)
			{
				if (Explosions[i].IsActive)
				{
					Explosions[i].Update(gameTime);
				}
				else
				{
					Explosions[i] = null;
					Explosions.RemoveAt(i);
				}
			}

			for (int i = Particles.Count - 1; i >= 0; i--)
			{
				if (Particles[i].IsActive)
				{
					Particles[i].Update(gameTime);
				}
				else
				{
					Particles[i] = null;
					Particles.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Draws the items from the <see cref="Explosions"/> and <see cref="Particles"/> lists to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public static void Draw(SpriteBatch spriteBatch)
		{
			foreach (Explosion explosion in Explosions)
			{
				if (explosion.IsActive)
				{
					explosion.Draw(spriteBatch);
				}
			}

			foreach (Particle particle in Particles)
			{
				if (particle.IsActive)
				{
					particle.Draw(spriteBatch);
				}
			}
		}
	}
}