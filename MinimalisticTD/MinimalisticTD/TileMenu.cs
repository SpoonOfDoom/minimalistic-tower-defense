using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MinimalisticTD
{
	/// <summary>
	/// A Menu spawned on a tile for buying, selling or upgrading towers.
	/// </summary>
	public class TileMenu
	{
		/// <summary>
		/// The type of menu.
		/// </summary>
		public enum MenuType
		{
			/// <summary>
			/// Buy menu. To buy towers.
			/// </summary>
			BuyMenu,

			/// <summary>
			/// Tower menu. To upgrade or sell towers.
			/// </summary>
			TowerMenu,

			/// <summary>
			/// No menu at all. Used for closing.
			/// </summary>
			None
		}

		/// <summary>
		/// The <see cref="Map"/> coordinates where this <see cref="TileMenu"/> is originates.
		/// </summary>
		public Vector2 Origin;

		/// <summary>
		/// The <see cref="MenuType"/> of this <see cref="TileMenu"/>.
		/// </summary>
		public MenuType Type;

		/// <summary>
		/// The <see cref="MenuItem"/>s this <see cref="TileMenu"/> contains.
		/// </summary>
		private List<MenuItem> menuItems = new List<MenuItem>();

		/// <summary>
		/// A struct for keeping up with the <see cref="TileMenu"/>'s items.
		/// </summary>
		private struct MenuItem
		{
			/// <summary>
			/// The position of this <see cref="MenuItem"/> in the <see cref="TileMenu"/> grid.
			/// </summary>
			public Vector2 Position;

			/// <summary>
			/// The text this <see cref="MenuItem"/> draws to screen.
			/// </summary>
			public string Text;

			/// <summary>
			/// The color this <see cref="MenuItem"/>'s <see cref="Text"/> is drawn in.
			/// </summary>
			public Color Color;

			/// <summary>
			/// The tooltip of this <see cref="MenuItem"/>. Contains slightly more detailed information about the item.
			/// </summary>
			public string ToolTip;

			/// <summary>
			/// Initializes a new instance of the <see cref="MenuItem"/> struct.
			/// </summary>
			/// <param name="position">The position in the menu grid.</param>
			/// <param name="text">The text.</param>
			/// <param name="color">The color.</param>
			public MenuItem(Vector2 position, string text, string tooltip, Color color)
			{
				Position = position;
				Text = text;
				ToolTip = tooltip;
				Color = color;
			}
		}

		/// <summary>
		/// The onscreen Rectangle of this <see cref="TileMenu"/>.
		/// </summary>
		public Rectangle ScreenRectangle;

		/// <summary>
		/// Initializes a new instance of the <see cref="TileMenu"/> class.
		/// </summary>
		/// <param name="mapCoordinates">The map coordinates.</param>
		public TileMenu(Vector2 mapCoordinates)
		{
			this.Origin = mapCoordinates;
			if (GameManager.CurrentMap.Tiles[(int)mapCoordinates.X, (int)mapCoordinates.Y] == Map.TileType.NormalEnvironment)
			{
				this.Type = MenuType.BuyMenu;
				Rectangle mapTileRect = new Rectangle((int)mapCoordinates.X * Map.TileWidth + GameManager.CurrentMap.MapOffsetX, (int)mapCoordinates.Y * Map.TileHeight + GameManager.CurrentMap.MapOffsetY, Map.TileWidth, Map.TileHeight);
				foreach (Tower tower in GameManager.PlayerTowers)
				{
					if (mapTileRect.Contains(new Point((int)tower.Center.X, (int)tower.Center.Y)))
					{
						this.Type = MenuType.TowerMenu;
					}
				}

				int screenX = (int)MathHelper.Clamp(0, mapTileRect.X - Map.TileWidth, (GameManager.CurrentMap.MapWidth - 3) * Map.TileWidth);
				int screenY = (int)MathHelper.Clamp(0, mapTileRect.Y - Map.TileHeight, (GameManager.CurrentMap.MapHeight - 3) * Map.TileHeight);
				ScreenRectangle = new Rectangle(screenX, screenY, Map.TileWidth * 3, Map.TileHeight * 3);

				AddMenuItems();
			}
			else
			{
				this.Type = MenuType.None;
			}
		}

		/// <summary>
		/// Adds appropriate menu items depending on what <see cref="MenuType"/> this is.
		/// </summary>
		private void AddMenuItems()
		{
			switch (Type)
			{
				case MenuType.BuyMenu:
					int x = 0;
					int y = 0;
					foreach (Tower.DamageType dt in Tower.LevelValues.Keys)
					{
						string damageType = System.Enum.GetName(typeof(Tower.DamageType), dt);
						menuItems.Add(new MenuItem(new Vector2(x, y), damageType.Substring(0, 1), damageType, Tower.TypeColors[dt]));
						x++;
						if (x > 2)
						{
							x = 0;
							y++;
						}
						if (y == 1 && x == 1) //only use the two outer tiles, the middle tile stays empty
						{
							x++;
						}
					}
					break;
				case MenuType.TowerMenu:
					menuItems.Add(new MenuItem(new Vector2(0, 0), "U", "Upgrade Tower", Color.White));
					menuItems.Add(new MenuItem(new Vector2(2, 0), "S", "Sell Tower", Color.White));
					break;
			}
			menuItems.Add(new MenuItem(new Vector2(1, 2), "X", "Close Menu", Color.Red));
		}

		/// <summary>
		/// Does the menu item action.
		/// </summary>
		/// <param name="itemPosition">The item position.</param>
		public void DoMenuItemAction(Vector2 itemPosition)
		{
			string itemText = this.menuItems.Find(item => item.Position.X == itemPosition.X && item.Position.Y == itemPosition.Y).Text;
			string itemTooltip = this.menuItems.Find(item => item.Position.X == itemPosition.X && item.Position.Y == itemPosition.Y).ToolTip;
			SoundManager.PlayMenuClick();
			if (Type == MenuType.BuyMenu)
			{
				if (itemTooltip == "Close Menu")
				{
					this.Type = MenuType.None;
				}
				else
				{
					foreach (string damageType in Enum.GetNames(typeof(Tower.DamageType)))
					{
						if (itemTooltip == damageType)
						{
							GameManager.TryBuyTower(Origin, (Tower.DamageType)Enum.Parse(typeof(Tower.DamageType), damageType));
							break;
						}
					}
				}
			}
			else if (Type == MenuType.TowerMenu)
			{
				switch (itemText)
				{
					case "U":
						//Upgrade Tower
						GameManager.TryUpgradeTower(Origin);
						break;

					case "S":
						//Sell Tower
						GameManager.TrySellTower(Origin);
						break;

					case "X":
						this.Type = MenuType.None;
						break;
				}
			}
		}

		/// <summary>
		/// Determines whether this menu and its items contain the mouse coordinates.
		/// </summary>
		/// <param name="x">The mouse x coordinate.</param>
		/// <param name="y">The mouse y coordinate.</param>
		/// <param name="clickedItem">The clicked item.</param>
		/// <returns>
		///   <c>true</c> if contains mouse coordinates; otherwise, <c>false</c>.
		/// </returns>
		public bool ClickHit(int x, int y, out Vector2 clickedItem)
		{
			if (ScreenRectangle.Contains(x, y))
			{
				foreach (MenuItem item in menuItems)
				{
					Rectangle rect = new Rectangle(ScreenRectangle.X + (int)item.Position.X * 32, ScreenRectangle.Y + (int)item.Position.Y * 32, 32, 32);
					if (rect.Contains(x, y))
					{
						clickedItem = new Vector2(item.Position.X, item.Position.Y);
						return true;
					}
				}
				clickedItem = new Vector2(-1, -1);
				return false;
			}
			else
			{
				this.Type = MenuType.None;
				clickedItem = new Vector2(-1, -1);
				return false;
			}
		}



		/// <summary>
		/// Draws this <see cref="TileMenu"/> to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			Helper.DrawBorderRectangle(spriteBatch, GameManager.RectTexture, ScreenRectangle, 2, Color.Yellow, Color.DarkGray * 0.75f);

			foreach (MenuItem item in menuItems)
			{
				Helper.DrawBorderRectangle(spriteBatch, GameManager.RectTexture, new Vector2(ScreenRectangle.X, ScreenRectangle.Y) + item.Position * 32, Map.TileWidth, Map.TileHeight, 2, Color.Black, item.Color * 0.5f);
				spriteBatch.DrawString(GameManager.IngameFont, item.Text, new Vector2(ScreenRectangle.X, ScreenRectangle.Y) + item.Position * 32, item.Color);
			}
			MouseState mouse = Mouse.GetState();
			if (GameManager.IsGameActive && ScreenRectangle.Contains(mouse.X, mouse.Y))
			{
				Vector2 mousePosition = new Vector2(mouse.X, mouse.Y);
				Vector2 clickedItem;
				int money;
				string itemText = string.Empty;
				string itemToolTip = string.Empty;

				foreach (MenuItem item in menuItems)
				{
					Rectangle rect = new Rectangle(ScreenRectangle.X + (int)item.Position.X * 32, ScreenRectangle.Y + (int)item.Position.Y * 32, 32, 32);
					if (rect.Contains(mouse.X, mouse.Y))
					{
						clickedItem = item.Position;
						itemToolTip = item.ToolTip;
						itemText = item.Text;
						break;
					}
				}

				if (itemToolTip != "Close Menu")
				{
					switch (Type)
					{
						case MenuType.BuyMenu:
							if (itemToolTip != string.Empty)
							{
								money = Tower.LevelValues[itemToolTip.ToDamageType()][1]["cost"].ToInt();
								Helper.DrawBorderRectangle(spriteBatch, GameManager.RectTexture, mousePosition + new Vector2(0, 18), 170, 34, 0, Color.Black * 0.8f, Color.Black * 0.8f);
								spriteBatch.DrawString(GameManager.IngameFontSmallBold, "Buy " + itemToolTip + " Tower", mousePosition + new Vector2(0, 15), Tower.TypeColors[itemToolTip.ToDamageType()]);
								spriteBatch.DrawString(GameManager.IngameFontSmallBold, "-" + money + " $", mousePosition + new Vector2(0, 30), Color.Red);
							}
							break;

						case MenuType.TowerMenu:
							if (itemToolTip == "Sell Tower")
							{
								spriteBatch.DrawString(GameManager.IngameFontSmallBold, itemToolTip, mousePosition + new Vector2(0, 15), Color.Yellow);
								money = GameManager.GetTowerSellGain(Origin);
								Helper.DrawBorderRectangle(spriteBatch, GameManager.RectTexture, mousePosition + new Vector2(0, 18), 170, 34, 0, Color.Black * 0.8f, Color.Black * 0.8f);
								spriteBatch.DrawString(GameManager.IngameFontSmallBold, "+" + money + " $", mousePosition + new Vector2(0, 30), Color.Green);
							}
							else if (itemToolTip == "Upgrade Tower")
							{
								money = GameManager.GetTowerUpgradeCost(Origin);
								float newRange;
								float currentRange = GameManager.GetTowerRanges(Origin, out newRange);
								Helper.DrawCircle(spriteBatch, Origin * Map.TileWidth + new Vector2(Map.TileWidth / 2), currentRange, Color.OrangeRed * 0.4f, 1, Color.OrangeRed * 0.7f);
								Helper.DrawCircle(spriteBatch, Origin * Map.TileWidth + new Vector2(Map.TileWidth / 2), newRange, Color.YellowGreen * 0.4f, 1, Color.YellowGreen * 0.7f);
								Helper.DrawBorderRectangle(spriteBatch, GameManager.RectTexture, mousePosition + new Vector2(0, 18), 170, 34, 0, Color.Black * 0.9f, Color.Black * 0.3f);
								spriteBatch.DrawString(GameManager.IngameFontSmallBold, itemToolTip, mousePosition + new Vector2(0, 15), Color.Yellow);
								spriteBatch.DrawString(GameManager.IngameFontSmallBold, "-" + money + " $", mousePosition + new Vector2(0, 45), Color.Red);
							}
							break;
					}
				}
				else
				{
					Helper.DrawBorderRectangle(spriteBatch, GameManager.RectTexture, mousePosition + new Vector2(0, 18), 170, 34, 0, Color.Black * 0.8f, Color.Black * 0.8f);
					spriteBatch.DrawString(GameManager.IngameFontSmallBold, itemToolTip, mousePosition + new Vector2(0, 15), Color.Purple);
				}
			}
		}
	}
}