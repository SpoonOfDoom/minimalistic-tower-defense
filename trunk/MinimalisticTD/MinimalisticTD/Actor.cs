using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinimalisticTD
{
	/// <summary>
	/// Base class for everything that behaves in some way or another.
	/// </summary>
	public class Actor
	{
		protected int ID;

		/// <summary>
		/// The onscreen Rectangle of this <see cref="Actor"/>.
		/// </summary>
		protected Rectangle screenRectangle;

		/// <summary>
		/// The <see cref="Color"/> in which this <see cref="Actor"/> will be drawn.
		/// </summary>
		protected Color color;

		/// <summary>
		/// The direction of movement of this <see cref="Actor"/>.
		/// </summary>
		protected Vector2 velocity;

		/// <summary>
		/// The speed of movement of this <see cref="Actor"/>
		/// </summary>
		protected float speed;

		/// <summary>
		/// The current onscreen location of this <see cref="Actor"/>, based on the upper left corner.
		/// </summary>
		protected Vector2 location;

		/// <summary>
		/// Is this <see cref="Actor"/> still active, or can it be removed?
		/// </summary>
		public bool IsActive;

		/// <summary>
		/// Initializes a new instance of the <see cref="Actor"/> class.
		/// </summary>
		/// <param name="location">The location where this <see cref="Actor"/> will be created.</param>
		/// <param name="color">The color this <see cref="Actor"/> will have.</param>
		/// <param name="screenWidth">Width on the screen.</param>
		/// <param name="screenHeight">Height on the screen.</param>
		public Actor(Vector2 location, Color color, int screenWidth, int screenHeight)
		{
			ID = GameManager.rand.Next(1000000);
			this.location = location;
			screenRectangle = new Rectangle((int)location.X, (int)location.Y, screenWidth, screenHeight);
			this.color = color;
			IsActive = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Actor"/> class. This constructor can be used for <see cref="Tower"/> or <see cref="Enemy"/> instances, which get their screen width and height from other sources.
		/// </summary>
		/// <param name="location">The location where this <see cref="Actor"/> will be created.</param>
		/// <param name="color">The color this <see cref="Actor"/> will have.</param>
		public Actor(Vector2 location)
		{
			ID = GameManager.rand.Next(1000000);
			this.location = location;
			IsActive = true;
		}

		/// <summary>
		/// Gets or sets the center of this <see cref="Actor"/>.
		/// </summary>
		/// <value>
		/// The center.
		/// </value>
		public Vector2 Center
		{
			get { return new Vector2(screenRectangle.X + screenRectangle.Width / 2, screenRectangle.Y + screenRectangle.Height / 2); }
			set { screenRectangle = new Rectangle((int)value.X - screenRectangle.Width / 2, (int)value.Y - screenRectangle.Width / 2, screenRectangle.Width, screenRectangle.Height); }
		}

		/// <summary>
		/// Checks if this <see cref="Actor"/> collides with the specified other <see cref="Actor"/>.
		/// </summary>
		/// <param name="otherActor">The other <see cref="Actor"/>.</param>
		/// <returns><c>True</c>, if their screenRectangles intersect; otherwise <c>false</c>.</returns>
		public virtual bool CollidesWith(Actor otherActor)
		{
			return screenRectangle.Intersects(otherActor.screenRectangle);
		}

		/// <summary>
		/// Updates this <see cref="Actor"/>.
		/// </summary>
		/// <param name="gameTime">The <see cref="GameTime"/>.</param>
		public virtual void Update(GameTime gameTime)
		{
			float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
			velocity.Normalize();
			if (speed > 0)
			{
				velocity = velocity * elapsed * speed;
				location += velocity;
				screenRectangle.X = (int)location.X;
				screenRectangle.Y = (int)location.Y;
			}
			
		}

		/// <summary>
		/// Draws this <see cref="Actor"/> to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public virtual void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(GameManager.RectTexture, screenRectangle, color);
		}
	}
}