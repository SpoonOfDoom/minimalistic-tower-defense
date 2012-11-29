using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinimalisticTD
{
	/// <summary>
	/// A control used in the editor to configure <see cref="EnemyType"/> and level of a single <see cref="Enemy"/>.
	/// </summary>
	public class EnemyControl
	{
		/// <summary>
		/// EventArgs for clicks on the <see cref="LevelUpRectangle"/> and <see cref="LevelDownRectangle"/>.
		/// </summary>
		public class LevelClickEventArgs
		{
			/// <summary>
			/// The old value for <see cref="Level"/>.
			/// </summary>
			public int OldLevel;

			/// <summary>
			/// The new value for <see cref="Level"/>
			/// </summary>
			public int NewLevel;

			/// <summary>
			/// Initializes a new instance of the <see cref="LevelClickEventArgs"/> class.
			/// </summary>
			/// <param name="OldLevel">The old level.</param>
			/// <param name="NewLevel">The new level.</param>
			public LevelClickEventArgs(int OldLevel, int NewLevel)
			{
				this.OldLevel = OldLevel;
				this.NewLevel = NewLevel;
			}
		}

		/// <summary>
		/// EventArgs for clicks on the <see cref="TypeLeftRectangle"/> and <see cref="TypeRightRectangle"/>.
		/// </summary>
		public class TypeClickEventArgs
		{
			/// <summary>
			/// The old value for <see cref="Type"/>.
			/// </summary>
			public Enemy.EnemyType OldType;

			/// <summary>
			/// The new value for <see cref="Type"/>.
			/// </summary>
			public Enemy.EnemyType NewType;

			/// <summary>
			/// Initializes a new instance of the <see cref="TypeClickEventArgs"/> class.
			/// </summary>
			/// <param name="oldType">The old type.</param>
			/// <param name="newType">The new type.</param>
			public TypeClickEventArgs(Enemy.EnemyType oldType, Enemy.EnemyType newType)
			{
				OldType = oldType;
				NewType = newType;
			}
		}

		/// <summary>
		/// The <see cref="EnemyType"/> of the desired <see cref="Enemy"/>.
		/// </summary>
		private Enemy.EnemyType type;

		/// <summary>
		/// The level of the desired <see cref="Enemy"/>.
		/// </summary>
		public int Level;

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		public Enemy.EnemyType Type
		{
			get
			{
				return type;
			}

			set
			{
				type = value;
			}
		}

		public Vector2 ScreenLocation
		{
			get
			{
				return ScreenRectangle.Location.ToVector2();
			}
			set
			{
				this.ScreenRectangle.Location = value.ToPoint();
			}
		}

		/// <summary>
		/// The onscreen <see cref="Rectangle"/> in which this control is drawn.
		/// </summary>
		public Rectangle ScreenRectangle;

		/// <summary>
		/// The onscreen <see cref="Rectangle"/> which acts as the "Level Up" button.
		/// </summary>
		public Rectangle LevelUpRectangle;

		/// <summary>
		/// The onscreen <see cref="Rectangle"/> in which the current value of <see cref="Level"/> is drawn.
		/// </summary>
		public Rectangle LevelRectangle;

		/// <summary>
		/// The onscreen <see cref="Rectangle"/> which acts as the "Level Down" button.
		/// </summary>
		public Rectangle LevelDownRectangle;

		/// <summary>
		/// The onscreen <see cref="Rectangle"/> which acts as the "Type Left" button.
		/// </summary>
		public Rectangle TypeLeftRectangle;

		/// <summary>
		/// The onscreen <see cref="Rectangle"/> which acts as the "Type Right" button.
		/// </summary>
		public Rectangle TypeRightRectangle;

		/// <summary>
		/// EventHandler for the <see cref="LevelUpClick"/> and <see cref="LevelDownClick"/> events.
		/// </summary>
		/// <param name="sender">The sender, i.e. the <see cref="EnemyControl"/>.</param>
		/// <param name="e">The <see cref="MinimalisticTD.EnemyControl.LevelClickEventArgs"/> instance containing the event data, i.e. new and old level values.</param>
		public delegate void LevelClickEventHandler(object sender, LevelClickEventArgs e);

		/// <summary>
		/// EventHandler for the <see cref="TypeLeftClick"/> and <see cref="TypeRightClick"/> events.
		/// </summary>
		/// <param name="sender">The sender, i.e. the <see cref="EnemyControl"/>.</param>
		/// <param name="e">The <see cref="MinimalisticTD.EnemyControl.TypeClickEventArgs"/> instance containing the event data, i.e. new and old <see cref="EnemyType"/> values.</param>
		public delegate void TypeClickEventHandler(object sender, TypeClickEventArgs e);

		/// <summary>
		/// Occurs when the user clicks the <see cref="LevelUpRectangle"/>.
		/// </summary>
		public event LevelClickEventHandler LevelUpClick;

		/// <summary>
		/// Occurs when the user clicks the <see cref="LevelDownRectangle"/>.
		/// </summary>
		public event LevelClickEventHandler LevelDownClick;

		/// <summary>
		/// Occurs when the user clicks the <see cref="TypeLeftRectangle"/>.
		/// </summary>
		public event TypeClickEventHandler TypeLeftClick;

		/// <summary>
		/// Occurs when the user clicks the <see cref="TypeRightRectangle"/>.
		/// </summary>
		public event TypeClickEventHandler TypeRightClick;

		/// <summary>
		/// Initializes a new instance of the <see cref="EnemyControl"/> class.
		/// </summary>
		/// <param name="startPosition">The start position of the list of <see cref="EnemyControl"/>s.</param>
		/// <param name="index">The index of this <see cref="EnemyControl"/> in its list.</param>
		public EnemyControl(Point startPosition, int index)
		{
			int xPos = startPosition.X + (int)(index * (Map.TileWidth * 1.5) + 5);
			int yPos = startPosition.Y;
			Level = 1;
			ScreenRectangle = new Rectangle(xPos, yPos, (int)(Map.TileWidth * 1.5), (int)(Map.TileHeight * 1.5));
			LevelUpRectangle = new Rectangle(xPos + ScreenRectangle.Width / 3, yPos, ScreenRectangle.Width / 3, ScreenRectangle.Height / 3);
			LevelRectangle = new Rectangle(LevelUpRectangle.X, yPos + ScreenRectangle.Height / 3, ScreenRectangle.Width / 3, ScreenRectangle.Height / 3);
			LevelDownRectangle = new Rectangle(LevelUpRectangle.X, yPos + ScreenRectangle.Height * 2 / 3, ScreenRectangle.Width / 3, ScreenRectangle.Height / 3);

			TypeLeftRectangle = new Rectangle(xPos, LevelRectangle.Y, Map.TileWidth, Map.TileHeight / 3);
			TypeRightRectangle = new Rectangle(xPos + ScreenRectangle.Width * 2 / 3, LevelRectangle.Y, ScreenRectangle.Width / 3, ScreenRectangle.Height / 3);

			LevelUpClick += new LevelClickEventHandler(EnemyControl_LevelClick);
			LevelDownClick += new LevelClickEventHandler(EnemyControl_LevelClick);
			TypeRightClick += new TypeClickEventHandler(EnemyControl_TypeClick);
			TypeLeftClick += new TypeClickEventHandler(EnemyControl_TypeClick);
		}

		/// <summary>
		/// Handles the TypeClick event of the <see cref="EnemyControl"/>.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.EnemyControl.TypeClickEventArgs"/> instance containing the event data, i.e. new and old <see cref="EnemyType"/> values.</param>
		private void EnemyControl_TypeClick(object sender, EnemyControl.TypeClickEventArgs e)
		{
			Type = e.NewType;
		}

		/// <summary>
		/// Handles the LevelClick event of the <see cref="EnemyControl"/>.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.EnemyControl.LevelClickEventArgs"/> instance containing the event data, i.e. new and old level values.</param>
		private void EnemyControl_LevelClick(object sender, EnemyControl.LevelClickEventArgs e)
		{
			if (e.NewLevel > e.OldLevel)
			{
				if (Level < 5)
				{
					Level = e.NewLevel;
				}
			}
			else
			{
				if (Level > 1)
				{
					Level--;
				}
			}
		}

		/// <summary>
		/// Checks if the mouse click at the specified coordinates hit any of the reacting Rectangles and carries out appropriate actions.
		/// </summary>
		/// <param name="mouseX">The mouse X coordinate.</param>
		/// <param name="mouseY">The mouse Y coordinate.</param>
		/// <returns><code>true</code>, if any of the reacting Rectangles contains the coordinates, otherwise <code>false</code>.</returns>
		public bool HitClick(int mouseX, int mouseY)
		{
			if (LevelUpRectangle.Contains(mouseX, mouseY))
			{
				if (LevelUpClick != null)
				{
					LevelUpClick(this, new LevelClickEventArgs(Level, Level + 1));
					SoundManager.PlayMenuClick();
				}
				return true;
			}
			else if (LevelDownRectangle.Contains(mouseX, mouseY))
			{
				if (LevelDownClick != null)
				{
					LevelUpClick(this, new LevelClickEventArgs(Level, Level - 1));
					SoundManager.PlayMenuClick();
				}
				return true;
			}
			else if (TypeLeftRectangle.Contains(mouseX, mouseY))
			{
				if (TypeLeftClick != null)
				{
					int typeMax = Enum.GetNames(typeof(Enemy.EnemyType)).Length;
					int type = (int)Type;
					int newType = type - 1 < 0 ? typeMax - 1 : type - 1;
					TypeLeftClick(this, new TypeClickEventArgs(Type, (Enemy.EnemyType)newType));
					SoundManager.PlayMenuClick();
				}
			}
			else if (TypeRightRectangle.Contains(mouseX, mouseY))
			{
				if (TypeRightClick != null)
				{
					int typeMax = Enum.GetNames(typeof(Enemy.EnemyType)).Length;
					int type = (int)Type;
					int newType = (type + 1) % (typeMax);
					TypeRightClick(this, new TypeClickEventArgs(Type, (Enemy.EnemyType)newType));
					SoundManager.PlayMenuClick();
				}
			}
			return false;
		}

		/// <summary>
		/// Builds a creation string for an <see cref="Enemy"/> object.
		/// </summary>
		/// <returns>A creation string for an <see cref="Enemy"/> object.</returns>
		public string CreateStringInfo()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Type=" + Enum.GetName(typeof(Enemy.EnemyType), Type) + ",");
			sb.Append("Level=" + Level + ";");
			return sb.ToString();
		}

		/// <summary>
		/// Draws this <see cref="EnemyControl"/> to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawString(GameManager.IngameFont, "+", LevelUpRectangle.Location.ToVector2(), Color.Gold);
			spriteBatch.DrawString(GameManager.IngameFont, "->", TypeRightRectangle.Location.ToVector2(), Color.Gold);
			spriteBatch.DrawString(GameManager.IngameFont, Level.ToString(), LevelRectangle.Location.ToVector2(), Color.Gold);
			spriteBatch.DrawString(GameManager.IngameFont, "<-", TypeLeftRectangle.Location.ToVector2(), Color.Gold);
			spriteBatch.DrawString(GameManager.IngameFont, "-", LevelDownRectangle.Location.ToVector2(), Color.Gold);
		}
	}
}