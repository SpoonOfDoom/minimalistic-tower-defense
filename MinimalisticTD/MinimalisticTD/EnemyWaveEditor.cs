using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MinimalisticTD
{
	/// <summary>
	/// An editor for enemy waves.
	/// </summary>
	public static class EnemyWaveEditor
	{
		/// <summary>
		/// The <see cref="Map"/> which is loaded in the <see cref="MapEditor"/> and to which this <see cref="EnemyWaveEditor"/> belongs.
		/// </summary>
		public static Map OwnerMap;

		/// <summary>
		/// The <see cref="Rectangle"/> in which the <see cref="WaveControl"/>s are drawn.
		/// </summary>
		public static Rectangle ScreenRectangle;

		/// <summary>
		/// Contains the <see cref="SelectableMenuItems"/> for increasing/decreasing the number of <see cref="WaveControl"/>s.
		/// </summary>
		public static List<SelectableMenuItem> MenuItems = new List<SelectableMenuItem>();

		/// <summary>
		/// The <see cref="WaveControl"/>s with which <see cref="EnemyWave"/>s for this <see cref="Map"/> are configured.
		/// </summary>
		public static List<WaveControl> WaveControls = new List<WaveControl>();

		/// <summary>
		/// Initializes the editor.
		/// </summary>
		/// <param name="map">The map whose waves will be edited.</param>
		public static void Initialize(Map map)
		{
			OwnerMap = map;
			if (OwnerMap.EnemyWaveInfo != string.Empty)
			{
				LoadWaveInfo();
			}
			int xPos = GameManager.MapRectangle.Left + 10;
			int yPos = Map.TileHeight * 4 / 3;
			Rectangle countUpRect = new Rectangle(xPos, yPos, Map.TileHeight * 2 / 3, Map.TileHeight * 2 / 3);
			Rectangle countDownRect = new Rectangle(xPos, yPos + countUpRect.Height + 5, Map.TileHeight * 2 / 3, Map.TileHeight * 2 / 3);
			SelectableMenuItem miWaveCountDown = new SelectableMenuItem(countDownRect, GameManager.IngameFont, "-", Color.Gold, Color.LightGray * 0.9f, Color.DarkGray);
			SelectableMenuItem miWaveCountUp = new SelectableMenuItem(countUpRect, GameManager.IngameFont, "+", Color.Gold, Color.LightGray * 0.9f, Color.DarkGray);
			miWaveCountUp.Click += new SelectableMenuItem.ClickEventHandler(miWaveCountUp_Click);
			miWaveCountDown.Click += new SelectableMenuItem.ClickEventHandler(miWaveCountDown_Click);
			ScreenRectangle = new Rectangle(miWaveCountUp.ScreenRectangle.Right + 5, yPos, GameManager.MapRectangle.Right - (miWaveCountUp.ScreenRectangle.Right + 5), miWaveCountDown.ScreenRectangle.Bottom - miWaveCountUp.ScreenRectangle.Top);
			MenuItems.Add(miWaveCountDown);
			MenuItems.Add(miWaveCountUp);
		}

		/// <summary>
		/// Loads the <see cref="EnemyWave"/> info from the <see cref="Map"/>.
		/// </summary>
		public static void LoadWaveInfo()
		{
			string[] waveStrings = OwnerMap.EnemyWaveInfo.Split('-').Where(s => s != string.Empty).ToArray();
			foreach (string wave in waveStrings)
			{
				WaveControl wc = new WaveControl();
				int xPos = WaveControls.Count > 0 ? WaveControls[WaveControls.Count - 1].TextureRectangle.Right + 5 : ScreenRectangle.Left + 5;
				int yPos = ScreenRectangle.Y + 3;
				wc.TextureLocation = new Vector2(xPos, yPos);
				wc.TextureClick += new WaveControl.TextureClickEventHandler(wc_TextureClick);
				string[] split = wave.Split('#').Where(s => s != string.Empty).ToArray();
				System.Diagnostics.Debug.Assert(split.Length == 2, "Something's wrong with the EnemyWave info.");
				string[] enemyStrings = split[1].Split(';').Where(s => s != string.Empty).ToArray();
				foreach (string enemy in enemyStrings)
				{
					string[] enemyInfo = enemy.Split(',').Where(s => s != string.Empty).ToArray();
					string typeString = enemyInfo[0].Split('=')[1];
					string levelString = enemyInfo[1].Split('=')[1];

					Enemy.EnemyType type = typeString.ToEnemyType();
					int level = levelString.ToInt();
					Point startLocation = new Point(wc.ButtonUpRectangle.Right, wc.ButtonUpRectangle.Top + 5);
					EnemyControl ec = new EnemyControl(startLocation, wc.enemyControls.Count);
					ec.Type = type;
					ec.Level = level;
					wc.enemyControls.Add(ec);
				}
				WaveControls.Add(wc);
			}
		}

		/// <summary>
		/// Handles the Click event of the miWaveCountDown control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		private static void miWaveCountDown_Click(object sender, ClickEventArgs e)
		{
			if (WaveControls.Count > 0)
			{
				WaveControls.RemoveAt(WaveControls.Count - 1);
			}
		}

		/// <summary>
		/// Handles the Click event of the miWaveCountUp control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		private static void miWaveCountUp_Click(object sender, ClickEventArgs e)
		{
			WaveControl wc = new WaveControl();
			int xPos = WaveControls.Count > 0 ? WaveControls[WaveControls.Count - 1].TextureRectangle.Right + 5 : ScreenRectangle.Left + 5;
			int yPos = ScreenRectangle.Y + 3;
			wc.TextureLocation = new Vector2(xPos, yPos);
			wc.TextureClick += new WaveControl.TextureClickEventHandler(wc_TextureClick);
			WaveControls.Add(wc);
		}

		/// <summary>
		/// Handles the TextureClick event of the wc control, i.e. when the <see cref="WaveControl"/> itself is clicked..
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="buttonUpArgs">The <see cref="MinimalisticTD.TextureClickEventArgs"/> instance containing the event data.</param>
		private static void wc_TextureClick(object sender, TextureClickEventArgs buttonUpArgs)
		{
			bool selected = !((WaveControl)sender).IsSelected;
			//First deactivate all WaveControls
			foreach (WaveControl wc in WaveControls)
			{
				wc.IsSelected = false;
			}
			//Now set the sender's value to the cached one from above. If the new value is true, it will be selected, otherwise nothing will happen.
			((WaveControl)sender).IsSelected = selected;
		}

		/// <summary>
		/// Handles the mouse input and checks if any control on <see cref="SelectableMenuItem"/> was hit..
		/// </summary>
		/// <param name="mouse">The MouseState.</param>
		/// <returns><code>true</code>, if a control or <see cref="SelectableMenuItem"/> was hit, otherwise <code>false</code>.</returns>
		public static bool ClickHit(MouseState mouse)
		{
			bool clickHit = false;
			if (mouse.LeftButton == ButtonState.Pressed) //Should be redundant, but I'm too lazy to remove it right now. Even though it would have taken less time and effort than writing this comment.
			{
				foreach (SelectableMenuItem smi in MenuItems)
				{
					clickHit = smi.ClickHit(mouse.X, mouse.Y);
					if (clickHit)
					{
						return true;
					}
				}
				foreach (WaveControl wc in WaveControls)
				{
					clickHit = wc.ClickHit(mouse.X, mouse.Y);
					if (clickHit)
					{
						return true;
					}
				}
			}
			return clickHit;
		}

		/// <summary>
		/// Builds a creation string including all <see cref="EnemyWave"/>s.
		/// </summary>
		/// <returns></returns>
		public static string CreateStringInfo()
		{
			if (WaveControls.Count > 0)
			{
				StringBuilder waveInfo = new StringBuilder();
				foreach (WaveControl wc in WaveControls)
				{
					waveInfo.Append(wc.CreateStringInfo());
				}
				return waveInfo.ToString();
			}
			else
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Draws the editor to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public static void Draw(SpriteBatch spriteBatch)
		{
			//TODO: Draw "Waves: {Number}" or something somewhere so the user knows what the +/- buttons are for.
			spriteBatch.Draw(GameManager.RectTexture, ScreenRectangle, Color.DarkGray * 0.8f);
			foreach (var item in MenuItems)
			{
				item.Draw(spriteBatch);
			}
			foreach (WaveControl wc in WaveControls)
			{
				wc.Draw(spriteBatch);
			}
		}
	}
}