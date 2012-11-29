using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinimalisticTD
{
	#region EventArg Classes

	/// <summary>
	/// The EventArgs for the <see cref="ButtonClick"/> event.
	/// </summary>
	public class ButtonClickEventArgs
	{
		/// <summary>
		/// Specifies what button was pressed, i.e. "up" or "down".
		/// </summary>
		public string Button;

		/// <summary>
		/// Initializes a new instance of the <see cref="ButtonClickEventArgs"/> class.
		/// </summary>
		/// <param name="button">The button.</param>
		public ButtonClickEventArgs(string button)
		{
			this.Button = button;
		}
	}

	/// <summary>
	/// The EventArgs for the <see cref="TextureClick"/> event.
	/// </summary>
	public class TextureClickEventArgs
	{
	}

	#endregion EventArg Classes

	/// <summary>
	/// A control used for editing an <see cref="EnemyWave"/>.
	/// </summary>
	public class WaveControl
	{
		/// <summary>
		/// The rectangle containing the <see cref="EnemyControl"/>s (and other controls) of this <see cref="WaveControl"/>.
		/// </summary>
		public Rectangle ControlRectangle;

		/// <summary>
		/// The rectangle containing this <see cref="WaveControl"/>'s texture. Used for selecting/deselecting this <see cref="WaveControl"/>.
		/// </summary>
		public Rectangle TextureRectangle;

		/// <summary>
		/// The rectangle acting as the button for increasing the <see cref="Enemy"/> count.
		/// </summary>
		public Rectangle ButtonUpRectangle;

		/// <summary>
		/// The rectangle acting as the button for decreasing the <see cref="Enemy"/> count.
		/// </summary>
		public Rectangle ButtonDownRectangle;

		/// <summary>
		/// Specifies whether this <see cref="WaveControl"/> is currently selected.
		/// </summary>
		public bool IsSelected = false;

		/// <summary>
		/// The list of <see cref="EnemyControl"/>s belonging to this <see cref="WaveControl"/>.
		/// </summary>
		public List<EnemyControl> enemyControls = new List<EnemyControl>();

		/// <summary>
		/// Gets or sets the <see cref="TextureRectangle"/> location.
		/// </summary>
		/// <value>
		/// The <see cref="TextureRectangle"/> location.
		/// </value>
		public Vector2 TextureLocation
		{
			get { return TextureRectangle.Location.ToVector2(); }
			set { TextureRectangle.Location = value.ToPoint(); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WaveControl"/> class.
		/// </summary>
		public WaveControl()
		{
			int xPos = (int)(GameManager.GameScreenRectangle.Width * 0.05);
			int yPos = GameManager.GameScreenRectangle.Height * 2 / 3;
			ControlRectangle = new Rectangle(xPos, yPos, (int)(GameManager.GameScreenRectangle.Width * 0.9), Map.TileHeight * 2);
			TextureRectangle = new Rectangle(0, 0, Map.TileWidth * 2 / 3, Map.TileHeight * 2 / 3);
			ButtonUpRectangle = new Rectangle(xPos + 2, yPos, Map.TileWidth * 2 / 3, Map.TileHeight * 2 / 3);
			ButtonDownRectangle = new Rectangle(xPos + 2, ButtonUpRectangle.Bottom + 3, Map.TileWidth * 2 / 3, Map.TileHeight * 2 / 3);

			this.ButtonClick += new ButtonClickEventHandler(Button_Click);
		}

		/// <summary>
		/// Handles the Click event of the Button control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="buttonClickArgs">The <see cref="MinimalisticTD.ButtonClickEventArgs"/> instance containing the event data.</param>
		private void Button_Click(object sender, ButtonClickEventArgs buttonClickArgs)
		{
			if (buttonClickArgs.Button == "up")
			{
				Point startLocation = new Point(ButtonUpRectangle.Right, ButtonUpRectangle.Top + 5);
				enemyControls.Add(new EnemyControl(startLocation, enemyControls.Count));
			}
			else if (buttonClickArgs.Button == "down")
			{
				if (enemyControls.Count > 0)
				{
					enemyControls.RemoveAt(enemyControls.Count - 1);
				}
			}
		}

		/// <summary>
		/// The EventHandler for the <see cref="ButtonClick"/> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="buttonClickArgs">The <see cref="MinimalisticTD.ButtonClickEventArgs"/> instance containing the event data.</param>
		public delegate void ButtonClickEventHandler(object sender, ButtonClickEventArgs buttonClickArgs);

		/// <summary>
		/// The EventHandler for the <see cref="TextureClick"/> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="textureClickArgs">The <see cref="MinimalisticTD.TextureClickEventArgs"/> instance containing the event data.</param>
		public delegate void TextureClickEventHandler(object sender, TextureClickEventArgs textureClickArgs);

		/// <summary>
		/// Occurs when the <see cref="ButtonUpRectangle"/> or <see cref="ButtonDownRectangle"/> is clicked.
		/// </summary>
		public event ButtonClickEventHandler ButtonClick;

		/// <summary>
		/// Occurs when <see cref="TextureRectangle"/> is clicked.
		/// </summary>
		public event TextureClickEventHandler TextureClick;

		/// <summary>
		/// Checks if any control of this <see cref="WaveControl"/> was hit by the click at the specified coordinates.
		/// </summary>
		/// <param name="mouseX">The mouse X.</param>
		/// <param name="mouseY">The mouse Y.</param>
		/// <returns><c>true</c>, if any active control contains the coordinates, otherwise <c>false</c>.</returns>
		public bool ClickHit(int mouseX, int mouseY)
		{
			if (TextureRectangle.Contains(mouseX, mouseY))
			{
				if (TextureClick != null)
				{
					TextureClick(this, new TextureClickEventArgs());
					return true;
				}
			}

			if (IsSelected)
			{
				if (ButtonUpRectangle.Contains(mouseX, mouseY))
				{
					if (ButtonClick != null)
					{
						ButtonClick(this, new ButtonClickEventArgs("up"));
						return true;
					}
				}
				else if (ButtonDownRectangle.Contains(mouseX, mouseY))
				{
					if (ButtonClick != null)
					{
						ButtonClick(this, new ButtonClickEventArgs("down"));
						return true;
					}
				}
				if (enemyControls.Count > 0)
				{
					foreach (EnemyControl ec in enemyControls)
					{
						if (ec.HitClick(mouseX, mouseY))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Builds a creation string for an <see cref="EnemyWave"/> object.
		/// </summary>
		/// <returns>A creation string for an <see cref="EnemyWave"/> object.</returns>
		public string CreateStringInfo()
		{
			if (enemyControls.Count > 0)
			{
				StringBuilder sb = new StringBuilder("enemyDelay=2#");
				foreach (EnemyControl ec in enemyControls)
				{
					sb.Append(ec.CreateStringInfo());
				}
				sb.Append("-");
				return sb.ToString();
			}
			else
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Draws the texture (i.e. the activation button) to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public void DrawTexture(SpriteBatch spriteBatch)
		{
			float opaq = IsSelected ? 0.95f : 0.6f;
			spriteBatch.Draw(GameManager.RectTexture, TextureRectangle, Color.Purple * opaq);
			spriteBatch.DrawString(GameManager.IngameFont, "W", TextureRectangle.Location.ToVector2(), Color.Gold * opaq);
		}

		/// <summary>
		/// Draws the controls to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public void DrawControls(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(GameManager.RectTexture, ControlRectangle, Color.DarkMagenta * 0.9f);
			Helper.DrawBorderRectangle(spriteBatch, GameManager.RectTexture, ButtonUpRectangle, 1, Color.DarkGray, Color.DarkGray);
			Helper.DrawBorderRectangle(spriteBatch, GameManager.RectTexture, ButtonDownRectangle, 1, Color.DarkGray, Color.DarkGray);
			spriteBatch.DrawString(GameManager.IngameFont, "-", ButtonDownRectangle.Location.ToVector2(), Color.Gold);
			spriteBatch.DrawString(GameManager.IngameFont, "+", ButtonUpRectangle.Location.ToVector2(), Color.Gold);

			foreach (EnemyControl ec in enemyControls)
			{
				ec.Draw(spriteBatch);
			}
		}

		/// <summary>
		/// Draws this <see cref="WaveControl"/> to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			DrawTexture(spriteBatch);
			if (IsSelected)
			{
				DrawControls(spriteBatch);
			}
		}
	}
}