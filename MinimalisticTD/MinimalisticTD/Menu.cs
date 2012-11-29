using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinimalisticTD
{
	/// <summary>
	/// A menu.
	/// </summary>
	public class Menu
	{
		/// <summary>
		/// Specifies where an item will be anchored.
		/// </summary>
		public enum MenuItemAnchor
		{
			/// <summary>
			/// Item will be anchored in the top left.
			/// </summary>
			TopLeft,

			/// <summary>
			/// Item will be anchored in the top right.
			/// </summary>
			TopRight,

			/// <summary>
			/// Item will be anchored in the top center.
			/// </summary>
			TopCenter,

			/// <summary>
			/// Item will be anchored in the left center.
			/// </summary>
			LeftCenter,

			/// <summary>
			/// Item will be anchored in the center.
			/// </summary>
			Center,

			/// <summary>
			/// Item will be anchored in the right center.
			/// </summary>
			RightCenter,

			/// <summary>
			/// Item will be anchored in the bottom center.
			/// </summary>
			BottomCenter,

			/// <summary>
			/// Item will be anchored in the bottom left.
			/// </summary>
			BottomLeft,

			/// <summary>
			/// Item will be anchored in the bottom right.
			/// </summary>
			BottomRight
		}

		/// <summary>
		/// Specifies the orientation of this <see cref="Menu"/>'s items.
		/// </summary>
		public enum Orientation
		{
			/// <summary>
			/// Items are displayed in a horizontal row.
			/// </summary>
			Horizontal,

			/// <summary>
			/// Items are displayed in a vertical column.
			/// </summary>
			Vertical,

			/// <summary>
			/// Items are displayed in a grid.
			/// </summary>
			Grid
		}

		/// <summary>
		/// Specifies the <see cref="Orientation"/> in which the items are displayed.
		/// </summary>
		private Orientation orientation;

		/// <summary>
		/// The background color of this <see cref="Menu"/>.
		/// </summary>
		private Color? background;

		/// <summary>
		/// The onscreen location of this <see cref="Menu"/>.
		/// </summary>
		private Vector2 location;

		/// <summary>
		/// Specifies how many items are in one row of the grid. Used only when the <see cref="orientation"/> is set to <see cref="Orientation.Grid"/>.
		/// </summary>
		private int gridWidth;

		/// <summary>
		/// Specifies how many items are in one column of the grid. Used only when the <see cref="orientation"/> is set to <see cref="Orientation.Grid"/>.
		/// </summary>
		private int gridHeight;

		public List<SelectableMenuItem> Items = new List<SelectableMenuItem>();

		public Vector2 Location
		{
			get
			{
				return location;
			}
			set
			{
				Vector2 offset = value - location;
				this.location = value;
				foreach (SelectableMenuItem item in Items)
				{
					item.ScreenRectangle.Offset((int)offset.X, (int)offset.Y);
				}
			}
		}

		public Rectangle ScreenRectangle
		{
			get
			{
				if (this.orientation == Orientation.Horizontal)
				{
					int width = Items.Max(item => item.ScreenRectangle.Right) - Items.Min(item => item.ScreenRectangle.Left);
					int height = Items.Max(item => item.ScreenRectangle.Bottom) - Items.Min(item => item.ScreenRectangle.Top);
					return new Rectangle((int)location.X, (int)location.Y, width, height);
				}
				else if (this.orientation == Orientation.Vertical)
				{
					int width = Items.Max(item => item.ScreenRectangle.Right) - Items.Min(item => item.ScreenRectangle.Left);
					int height = Items.Max(item => item.ScreenRectangle.Bottom) - Items.Min(item => item.ScreenRectangle.Top);
					return new Rectangle((int)location.X, (int)location.Y, width, height);
				}
				else
				{
					int width = Items.Max(item => item.ScreenRectangle.Right) - Items.Min(item => item.ScreenRectangle.Left);
					int height = Items.Max(item => item.ScreenRectangle.Bottom) - Items.Min(item => item.ScreenRectangle.Top);
					return new Rectangle((int)location.X, (int)location.Y, width, height);
				}
			}
			set
			{
				Location = new Vector2(value.X, value.Y);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Menu"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="orientation">The orientation.</param>
		/// <param name="background">The background.</param>
		/// <param name="gridWidth">Width of the grid. This is optional.</param>
		/// <param name="gridHeight">Height of the grid. This is optional.</param>
		public Menu(Vector2 location, Orientation orientation, Color? background, int gridWidth = 3, int gridHeight = 3)
		{
			Location = location;
			this.orientation = orientation;
			this.gridWidth = gridWidth;
			this.gridHeight = gridHeight;
			if (background.HasValue)
			{
				this.background = background;
			}
		}

		/// <summary>
		/// Adds the specified <see cref="SelectableMenuItem"/> to this <see cref="Menu"/>.
		/// </summary>
		/// <param name="item">The <see cref="SelectableMenuItem"/> that will be added.</param>
		public void AddSelectableItem(SelectableMenuItem item)
		{
			if (orientation == Orientation.Horizontal)
			{
				int offset = 0;
				if (Items.Count > 0)
				{
					offset = Items[Items.Count - 1].ScreenRectangle.Right;
				}
				item.ScreenRectangle.Offset((offset + 5), 0);
				Items.Add(item);
			}
			else if (orientation == Orientation.Vertical)
			{
				item.ScreenRectangle.Offset(Location.X.ToInt(), (int)location.Y + Items.Count * ((int)item.TextSize.Y + 5));
				Items.Add(item);
			}
			else if (orientation == Orientation.Grid)
			{
				if (Items.Count == 0)
				{
					item.ScreenRectangle.Offset(5, 5);
					Items.Add(item);
					return;
				}
				//quick'n'dirty for 3x3 grids. I need to stop overengineering this stupid game!
				//TODO: un-quick'n'dirty this.
				int offsetX = ((Items.Count) % 3) * item.ScreenRectangle.Width + 5;
				int offsetY = (((Items.Count) / 3) % 3) * (item.ScreenRectangle.Height + (int)item.TextSize.Y) + 5;

				item.ScreenRectangle.Offset(offsetX, offsetY);
				Items.Add(item);
			}
		}

		/// <summary>
		/// Checks if the mouse click hit any of this <see cref="Menu"/>'s items.
		/// </summary>
		/// <param name="mouseX">The mouse X.</param>
		/// <param name="mouseY">The mouse Y.</param>
		/// <returns><c>true</c>, if any item is hit, otherwise <c>false</c>.</returns>
		public bool ClickHit(int mouseX, int mouseY)
		{
			foreach (SelectableMenuItem item in this.Items)
			{
				if (item.ClickHit(mouseX, mouseY))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Draws this <see cref="Menu"/> to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			if (background.HasValue)
			{
				Helper.DrawBorderRectangle(spriteBatch, GameManager.RectTexture, ScreenRectangle, 0, background.Value, background);
			}
			foreach (SelectableMenuItem item in Items)
			{
				item.Draw(spriteBatch);
			}
		}
	}
}