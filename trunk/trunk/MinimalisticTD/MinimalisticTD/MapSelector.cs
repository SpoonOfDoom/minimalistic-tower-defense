using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MinimalisticTD
{
	/// <summary>
	/// A class for selecting maps.
	/// </summary>
	public class MapSelector
	{
		/// <summary>
		/// The <see cref="MouseState"/> from the previous frame.
		/// </summary>
		private MouseState lastMouseState;

		/// <summary>
		/// Specifies how many <see cref="Map"/>s are shown on one page.
		/// </summary>
		private const int pageBreak = 9;

		/// <summary>
		/// Specifies in which mode the selected <see cref="Map"/> is loaded, e.g. in editor or as a custom <see cref="Map"/> for playing.
		/// </summary>
		private LoadMode loadMode;

		/// <summary>
		/// The <see cref="Map"/>s that can be selected from this <see cref="MapSelector"/>.
		/// </summary>
		public List<Map> Maps = new List<Map>();

		/// <summary>
		/// The <see cref="Menu"/> which displays the current page of <see cref="Maps"/>.
		/// </summary>
		public Menu MapMenu = new Menu(new Vector2(5, 20), Menu.Orientation.Grid, null);

		/// <summary>
		/// Specifies how the map will be opened.
		/// </summary>
		public enum LoadMode
		{
			/// <summary>
			/// Editing in the <see cref="MapEditor"/>.
			/// </summary>
			Editor,

			/// <summary>
			/// Playing as a campaign <see cref="Map"/>.
			/// </summary>
			PlayCampaign,

			/// <summary>
			/// Playing as a custom <see cref="Map"/>.
			/// </summary>
			PlayCustom
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MapSelector"/> class.
		/// </summary>
		/// <param name="folder">The folder from which maps will be loaded.</param>
		/// <param name="loadMode">How the maps will be opened.</param>
		public MapSelector(string folder, LoadMode loadMode)
		{
			this.loadMode = loadMode;
			lastMouseState = Mouse.GetState();
			BinaryFormatter bf = new BinaryFormatter();

			foreach (string file in Directory.GetFiles(folder))
			{
				try
				{
					FileInfo f = new FileInfo(file);
					using (FileStream fs = f.Open(FileMode.Open))
					{
						Map map = bf.Deserialize(fs) as Map;
						Maps.Add(map);
						fs.Close();
					}
				}
				catch (IOException)
				{
					Debug.Write(file + "could not be read.");
				}
			}

			int itemWidt = (GameManager.GameScreenRectangle.Width / 3) - 15;
			int itemHeight = (GameManager.GameScreenRectangle.Height / 3) - 15;

			foreach (Map map in Maps)
			{
				SelectableMenuItem smi = new SelectableMenuItem(new Rectangle(0, 0, itemWidt +5, itemHeight + 5), GameManager.IngameFontSmall, map.FilePath, Color.Blue, Color.DarkGray, Color.DarkOrange, map.Thumbnail);
				smi.Click += new SelectableMenuItem.ClickEventHandler(smi_Click);
				MapMenu.AddSelectableItem(smi);
			}
		}

		/// <summary>
		/// Handles the Click event of the <see cref="SelectableMenuItem"/> controls.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MinimalisticTD.ClickEventArgs"/> instance containing the event data.</param>
		private void smi_Click(object sender, ClickEventArgs e)
		{
			SelectableMenuItem smi = sender as SelectableMenuItem;
			Map m = Maps.Single(map => map.FilePath == smi.Text);
			MapEditor.LoadMap(m.FilePath, loadMode);
		}

		/// <summary>
		/// Handles the input.
		/// </summary>
		/// <param name="gameTime">The GameTime.</param>
		public bool ClickHit(GameTime gameTime)
		{
			MouseState mouse = Mouse.GetState();
			if (mouse.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
			{
				if (MapMenu.ClickHit(mouse.X, mouse.Y))
				{
					lastMouseState = mouse;
					return true;
				}
			}
			lastMouseState = mouse;
			return false;
		}

		/// <summary>
		/// Draws the map selector to screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch used for drawing to screen.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(GameManager.RectTexture, GameManager.GameScreenRectangle, Color.Purple);
			MapMenu.Draw(spriteBatch);
		}
	}
}