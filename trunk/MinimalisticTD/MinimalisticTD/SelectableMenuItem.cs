using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinimalisticTD
{
	/// <summary>
	/// The EventArgs class for the <see cref="Click"/> event of the <see cref="SelectableMenuItem"/>.
	/// </summary>
	public class ClickEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ClickEventArgs"/> class.
		/// </summary>
		/// <param name="s">The <see cref="Text"/> of the <see cref="SelectableMenuItem"/> sender.</param>
		public ClickEventArgs(string s)
		{
			Text = s;
		}

		/// <summary>
		/// The Text of the <see cref="SelectableMenuItem"/> sender.
		/// </summary>
		public String Text { get; private set; } // readonly
	}

	/// <summary>
	/// A menu item that can be clicked. Can be used independently or put into a <see cref="Menu"/>.
	/// </summary>
	public class SelectableMenuItem
	{
		/// <summary>
		/// The on screen <see cref="Rectangle"/> of this <see cref="SelectableMenuItem"/>.
		/// </summary>
		public Rectangle ScreenRectangle;

		/// <summary>
		/// The text this <see cref="SelectableMenuItem"/> displays.
		/// </summary>
		public string Text;

		/// <summary>
		/// The <see cref="Color"/> this <see cref="SelectableMenuItem"/>'s <see cref="Text"/> is drawn in.
		/// </summary>
		private Color color;

		/// <summary>
		/// The background color of this <see cref="SelectableMenuItem"/>.
		/// </summary>
		private Color? backgroundColor;

		/// <summary>
		/// The color of the border around this <see cref="SelectableMenuItem"/>.
		/// </summary>
		private Color? borderColor;

		/// <summary>
		/// The <see cref="SpriteFont"/> this <see cref="SelectableMenuItem"/>'s <see cref="Text"/> uses.
		/// </summary>
		private SpriteFont font;

		/// <summary>
		/// The offset used to position the <see cref="Text"/> correctly.
		/// </summary>
		private Vector2 textOffset;

		/// <summary>
		/// The texture that is used in addition or instead of the <see cref="Text"/>.
		/// </summary>
		private Texture2D texture;

		/// <summary>
		/// Specifies whether this <see cref="SelectableMenuItem"/> is visible.
		/// </summary>
		public bool IsVisible = true;

		/// <summary>
		/// Gets the size of the <see cref="Text"/>.
		/// </summary>
		/// <value>
		/// The size of the <see cref="Text"/>.
		/// </value>
		public Vector2 TextSize
		{
			get { return font.MeasureString(Text); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableMenuItem"/> class.
		/// </summary>
		/// <param name="screenRectangle">The <see cref="ScreenRectangle"/>.</param>
		/// <param name="font">The <see cref="font"/>.</param>
		/// <param name="text">The <see cref="Text"/>.</param>
		/// <param name="color">The color of the <see cref="Text"/>.</param>
		/// <param name="background">The <see cref="backgroundColor"/>.</param>
		/// <param name="border">The <see cref="borderColor"/>.</param>
		/// <param name="texture">The <see cref="texture"/>.</param>
		public SelectableMenuItem(Rectangle screenRectangle, SpriteFont font, string text, Color color, Color? background, Color? border, Texture2D texture = null)
		{
			ScreenRectangle = screenRectangle;
			this.color = color;
			if (background.HasValue)
			{
				this.backgroundColor = background;
			}
			if (border.HasValue)
			{
				this.borderColor = border;
			}
			this.font = font;
			Text = text;
			if (texture != null)
			{
				this.texture = texture;
			}
			textOffset = Vector2.Zero;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableMenuItem"/> class.
		/// </summary>
		/// <param name="screenRectangle">The <see cref="ScreenRectangle"/>.</param>
		/// <param name="font">The <see cref="font"/>.</param>
		/// <param name="text">The <see cref="Text"/>.</param>
		/// <param name="color">The color of the <see cref="Text"/>.</param>
		/// <param name="background">The <see cref="backgroundColor"/>.</param>
		/// <param name="border">The <see cref="borderColor"/>.</param>
		/// <param name="anchor">The <see cref="Menu.MenuItemAnchor"/> defining how text is offset inside the rectangle.</param>
		/// <param name="texture">The <see cref="texture"/>.</param>
		public SelectableMenuItem(Rectangle screenRectangle, SpriteFont font, string text, Color color, Color? background, Color? border, Menu.MenuItemAnchor anchor, Texture2D texture = null)
			: this(screenRectangle, font, text, color, background, border, texture)
		{
			textOffset = new Vector2(screenRectangle.Width - font.MeasureString(text).X, 0);
			float xOffsetFull = screenRectangle.Width - font.MeasureString(text).X;
			float xOffsetHalf = xOffsetFull / 2;
			float yOffsetFull = screenRectangle.Height - font.MeasureString(text).Y;
			float yOffsetHalf = yOffsetFull / 2;

			switch (anchor)
			{
				case Menu.MenuItemAnchor.TopLeft:
					textOffset = Vector2.Zero;
					break;
				case Menu.MenuItemAnchor.TopRight:
					textOffset = new Vector2(xOffsetFull, 0);
					break;
				case Menu.MenuItemAnchor.TopCenter:
					textOffset = new Vector2(xOffsetHalf, 0);
					break;
				case Menu.MenuItemAnchor.LeftCenter:
					textOffset = new Vector2(0, yOffsetHalf);
					break;
				case Menu.MenuItemAnchor.Center:
					textOffset = new Vector2(xOffsetHalf, yOffsetHalf);
					break;
				case Menu.MenuItemAnchor.RightCenter:
					textOffset = new Vector2(xOffsetFull, yOffsetHalf);
					break;
				case Menu.MenuItemAnchor.BottomCenter:
					textOffset = new Vector2(xOffsetHalf, yOffsetFull);
					break;
				case Menu.MenuItemAnchor.BottomLeft:
					textOffset = new Vector2(0, yOffsetFull);
					break;
				case Menu.MenuItemAnchor.BottomRight:
					textOffset = new Vector2(xOffsetFull, yOffsetFull);
					break;
			}
		}

		/// <summary>
		/// The EventHandler for the <see cref="Click"/> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		public delegate void ClickEventHandler(object sender, ClickEventArgs e);

		/// <summary>
		/// Occurs when this <see cref="SelectableMenuItem"/> is clicked.
		/// </summary>
		public event ClickEventHandler Click;

		/// <summary>
		/// Raises the <see cref="Click"/> event.
		/// </summary>
		protected virtual void RaiseClickEvent()
		{
			if (Click != null)
				Click(this, new ClickEventArgs(Text));
		}

		/// <summary>
		/// Checks if the mouse click on the given coordinates hit this <see cref="SelectableMenuItem"/>'s <see cref="ScreenRectangle"/> and raises the <see cref="Click"/> event if that is the case.
		/// </summary>
		/// <param name="mouseX">The mouse X.</param>
		/// <param name="mouseY">The mouse Y.</param>
		/// <returns><c>true</c>, if this <see cref="ScreenRectangle"/> contains the specified coordinates, otherwise <c>false</c>.</returns>
		public bool ClickHit(int mouseX, int mouseY)
		{
			if (ScreenRectangle.Contains(mouseX, mouseY) && this.IsVisible)
			{
				RaiseClickEvent();
				SoundManager.PlayMenuClick();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Draws this <see cref="SelectableMenuItem"/> to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			if (this.IsVisible)
			{
				if (backgroundColor.HasValue || borderColor.HasValue)
				{
					Helper.DrawBorderRectangle(spriteBatch, GameManager.RectTexture, ScreenRectangle, 2, borderColor.Value, backgroundColor);
				}
				if (texture != null)
				{
					spriteBatch.Draw(texture, ScreenRectangle, Color.White);
					spriteBatch.DrawString(font, Text, new Vector2(ScreenRectangle.X, ScreenRectangle.Bottom + 2) + textOffset, color);
				}
				else
				{
					spriteBatch.DrawString(font, Text, new Vector2(ScreenRectangle.X, ScreenRectangle.Y) + textOffset, color);
				}
			}
		}
	}
}