using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinimalisticTD
{
	/// <summary>
	/// Class with different helper methods. Also draws messages. It's a mess, really.
	/// </summary>
	public static class Helper
	{
		/// <summary>
		/// Remaining time the message will be shown.
		/// </summary>
		public static float messageTimeRemaining = 0;

		/// <summary>
		/// How long the message stays on screen (starting value).
		/// </summary>
		public static float messageTimeStart = 0;

		/// <summary>
		/// The text of the message.
		/// </summary>
		public static string messageText = string.Empty;

		/// <summary>
		/// The location the message will be drawn to.
		/// </summary>
		public static Vector2 messageLocation = new Vector2(10, GameManager.GameScreenRectangle.Height - 10);

		/// <summary>
		/// Draws the message to screen if appropriate.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		/// <param name="gameTime">The GameTime.</param>
		public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			if (messageTimeRemaining > 0 && messageText != string.Empty)
			{
				float progress = messageTimeRemaining / messageTimeStart;
				int width = GameManager.IngameFont.MeasureString(messageText).X.ToInt();
				int height = GameManager.IngameFont.MeasureString(messageText).Y.ToInt();
				DrawBorderRectangle(spriteBatch, GameManager.RectTexture, new Rectangle((int)messageLocation.X, (int)messageLocation.Y, width, height), 0, null, Color.DarkGray * 0.7f * progress);
				spriteBatch.DrawString(GameManager.IngameFont, messageText, messageLocation, Color.Gold * progress);

				messageTimeRemaining = Math.Max(messageTimeRemaining - (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
			}
			if (messageTimeRemaining <= 0)
			{
				messageText = string.Empty;
			}
		}

		/// <summary>
		/// Draws a rectangle with border to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		/// <param name="texture">The texture.</param>
		/// <param name="location">The location.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		/// <param name="borderThickness">The border thickness.</param>
		/// <param name="borderColor">Color of the border.</param>
		/// <param name="fill">Color of the fill.</param>
		public static void DrawBorderRectangle(SpriteBatch spriteBatch, Texture2D texture, Vector2 location, int width, int height, int borderThickness, Color borderColor, Color? fill)
		{
			Rectangle left, right, up, down;
			left = new Rectangle((int)location.X, (int)location.Y, borderThickness, height);
			right = new Rectangle((int)location.X + width - borderThickness, (int)location.Y, borderThickness, height);
			up = new Rectangle((int)location.X, (int)location.Y, width, borderThickness);
			down = new Rectangle((int)location.X, (int)location.Y + height - borderThickness, width, borderThickness);

			if (fill.HasValue)
			{
				spriteBatch.Draw(texture, new Rectangle((int)location.X, (int)location.Y, width, height), fill.Value);
			}
			spriteBatch.Draw(texture, left, borderColor);
			spriteBatch.Draw(texture, right, borderColor);
			spriteBatch.Draw(texture, up, borderColor);
			spriteBatch.Draw(texture, down, borderColor);
		}

		/// <summary>
		/// Draws a rectangle with border to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		/// <param name="texture">The texture.</param>
		/// <param name="destinationRectangle">The destination rectangle.</param>
		/// <param name="borderThickness">The border thickness.</param>
		/// <param name="borderColor">Color of the border.</param>
		/// <param name="fill">Color of the fill.</param>
		public static void DrawBorderRectangle(SpriteBatch spriteBatch, Texture2D texture, Rectangle destinationRectangle, int borderThickness, Color? borderColor, Color? fill)
		{
			Rectangle left, right, up, down;
			left = new Rectangle(destinationRectangle.X, destinationRectangle.Y, borderThickness, destinationRectangle.Height);
			right = new Rectangle(destinationRectangle.X + destinationRectangle.Width - borderThickness, destinationRectangle.Y, borderThickness, destinationRectangle.Height);
			up = new Rectangle(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, borderThickness);
			down = new Rectangle(destinationRectangle.X, destinationRectangle.Y + destinationRectangle.Height - borderThickness, destinationRectangle.Width, borderThickness);

			if (fill.HasValue)
			{
				spriteBatch.Draw(texture, destinationRectangle, fill.Value);
			}
			if (borderColor.HasValue)
			{
				spriteBatch.Draw(texture, left, borderColor.Value);
				spriteBatch.Draw(texture, right, borderColor.Value);
				spriteBatch.Draw(texture, up, borderColor.Value);
				spriteBatch.Draw(texture, down, borderColor.Value);
			}
		}

		/// <summary>
		/// Draws a circle with the specified center, radius and colors.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		/// <param name="center">The center.</param>
		/// <param name="radius">The range.</param>
		/// <param name="filling">Color of the filling.</param>
		/// <param name="edge">Color of the edge.</param>
		public static void DrawCircle(SpriteBatch spriteBatch, Vector2 center, float radius, Color filling, int lineThickness, Color? edge = null)
		{
			if (radius < 1)
			{
				return;
			}
			int outerRectXStart = (int)Math.Max(center.X - radius, 0);
			int outerRectXEnd = (int)Math.Min(center.X + radius, GameManager.GameScreenRectangle.Width);
			int outerRectYStart = (int)Math.Max(center.Y - radius, 0);
			int outerRectYEnd = (int)Math.Min(center.Y + radius, GameManager.GameScreenRectangle.Height);

			float innerRange = radius - lineThickness;
			float v = (float)Math.Ceiling(Math.Sqrt(innerRange * innerRange * 2) / 2); //calculate the half length of a side of the biggest square in this circle. We'll draw this square seperately, so we don't need to check the pixels inside that
			Vector2 innerRectTopLeft = center + new Vector2(-v);
			Vector2 innerRectBottomRight = center + new Vector2(v);

			spriteBatch.Draw(GameManager.RectTexture,
				new Rectangle((int)innerRectTopLeft.X, (int)innerRectTopLeft.Y, (int)(innerRectBottomRight.X - innerRectTopLeft.X), (int)(innerRectBottomRight.Y - innerRectTopLeft.Y)),
				filling);

			for (int x = outerRectXStart; x <= outerRectXEnd; x++)
			{
				for (int y = outerRectYStart; y <= outerRectYEnd; y++)
				{
					if (x >= innerRectTopLeft.X && x < innerRectBottomRight.X && y >= innerRectTopLeft.Y && y < innerRectBottomRight.Y)
					{
						continue;
					}
					Vector2 point = new Vector2(x, y);
					float pointDistance = (center - point).Length() - radius;
					if (pointDistance <= 1)
					{
						if (lineThickness == 0 || pointDistance < -lineThickness)
						{
							spriteBatch.Draw(GameManager.RectTexture, new Rectangle(x, y, 1, 1), filling);
						}
						else
						{
							spriteBatch.Draw(GameManager.RectTexture, new Rectangle(x, y, 1, 1), edge.Value);
						}
					}
				}
			}
		}

		//TODO: Move to Game1 class, so that it can easily access elapsed time and is accessible from every GameState
		/// <summary>
		/// Shows the message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="location">The location.</param>
		/// <param name="duration">The duration.</param>
		public static void ShowMessage(string message, Vector2 location, float duration)
		{
			//TODO: Build a message queue!
			messageTimeStart = duration;
			messageTimeRemaining = duration;
			messageText = message;
			messageLocation = location;
		}

		/// <summary>
		/// Gets the name of Variable passed as an expression (for example: GetName(() => myVar)).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expr">The expr.</param>
		/// <returns></returns>
		public static string GetName<T>(System.Linq.Expressions.Expression<Func<T>> expr)
		{
			var body = (System.Linq.Expressions.MemberExpression)expr.Body;
			return body.Member.Name;
		}

		/// <summary>
		/// Creates a new, blank, rectangular <see cref="Texture2D"/> instance with the specified width and height.
		/// </summary>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		/// <returns>A new, blank, rectangular <see cref="Texture2D"/> instance with the specified width and height.</returns>
		public static Texture2D CreateRectangleTexture(int width, int height)
		{
			Texture2D tex = new Texture2D(GameManager.ActiveGraphicsDevice, width, height);
			Color[] colorData = new Color[width * height];
			for (int i = 0; i < width * height; i++)
			{
				colorData[i] = Color.White;
			}
			tex.SetData(colorData);
			return tex;
		}
	}
}