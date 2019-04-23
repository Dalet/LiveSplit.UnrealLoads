using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UnrealLoads.Games;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UnrealLoads
{
	public partial class UnrealLoadsSettings : UserControl
	{
		public bool AutoStart { get; set; }
		public bool AutoReset { get; set; }
		public bool AutoSplitOnMapChange { get; set; }
		public bool AutoSplitOncePerMap { get; set; }
		public bool DbgShowMap { get; set; }

		public IList<Map> Maps => (IList<Map>) mapBindingSource.List;

		const bool DEFAULT_AUTOSTART = true;
		const bool DEFAULT_AUTORESET = true;
		const bool DEFAULT_AUTOSPLITONMAPCHANGE = false;
		const bool DEFAULT_AUTOSPLITONCEPERMAP = true;
		const bool DEFAULT_SPLITONLEAVE = false;

		LiveSplitState _state;

		public UnrealLoadsSettings(LiveSplitState state)
		{
			Dock = DockStyle.Fill;
			InitializeComponent();

			_state = state;

			dgvMapSet.EnabledChanged += DgvMapSet_EnabledChanged;

			cbGame.DataSource = GameMemory.SupportedGames.Select(s => s.GetType())
				.OrderBy(t => t.Name)
				.ToList();
			cbGame.DisplayMember = nameof(Type.Name);

			chkAutoStart.DataBindings.Add(nameof(CheckBox.Checked), this, nameof(AutoStart), false, DataSourceUpdateMode.OnPropertyChanged);
			chkAutoReset.DataBindings.Add(nameof(CheckBox.Checked), this, nameof(AutoReset), false, DataSourceUpdateMode.OnPropertyChanged);
			chkSplitOnNewMap.DataBindings.Add(nameof(CheckBox.Checked), this, nameof(AutoSplitOnMapChange), false, DataSourceUpdateMode.OnPropertyChanged);
			chkSplitOncePerMap.DataBindings.Add(nameof(CheckBox.Checked), this, nameof(AutoSplitOncePerMap), false, DataSourceUpdateMode.OnPropertyChanged);
			chkSplitOncePerMap.DataBindings.Add(nameof(CheckBox.Enabled), chkSplitOnNewMap, nameof(CheckBox.Checked), false, DataSourceUpdateMode.OnPropertyChanged);
			gbMapWhitelist.DataBindings.Add(nameof(GroupBox.Enabled), chkSplitOnNewMap, nameof(CheckBox.Checked), false, DataSourceUpdateMode.OnPropertyChanged);
			chkDbgShowMap.DataBindings.Add(nameof(CheckBox.Checked), this, nameof(DbgShowMap), false, DataSourceUpdateMode.OnPropertyChanged);

			// defaults
			AutoStart = DEFAULT_AUTOSTART;
			AutoReset = DEFAULT_AUTORESET;
			AutoSplitOnMapChange = DEFAULT_AUTOSPLITONMAPCHANGE;
			AutoSplitOncePerMap = DEFAULT_AUTOSPLITONCEPERMAP;
			cbGame.SelectedItem = SearchGameSupport(_state.Run.GameName)?.GetType() ?? GameMemory.SupportedGames[0].GetType();

#if DEBUG
			chkDbgShowMap.Visible = true;
#endif
		}

		void DgvMapSet_EnabledChanged(object sender, EventArgs e)
		{
			// https://stackoverflow.com/questions/8715459/disabling-or-greying-out-a-datagridview
			// give it disabled look (it doesn't by default)
			var dgv = (DataGridView)sender;
			if (!dgv.Enabled)
			{
				dgv.DefaultCellStyle.BackColor = SystemColors.Control;
				dgv.DefaultCellStyle.ForeColor = SystemColors.GrayText;
				splitOnEnterDataGridViewCheckBoxColumn.CellTemplate.Style.ForeColor
					= splitOnLeaveDataGridViewCheckBoxColumn.CellTemplate.Style.ForeColor
					= Color.DarkGray;
			}
			else
			{
				dgv.DefaultCellStyle.BackColor = SystemColors.Window;
				dgv.DefaultCellStyle.ForeColor = SystemColors.ControlText;
				splitOnEnterDataGridViewCheckBoxColumn.CellTemplate.Style.ForeColor
					= splitOnLeaveDataGridViewCheckBoxColumn.CellTemplate.Style.ForeColor
					= Color.White;
			}
		}

		static GameSupport SearchGameSupport(string name)
		{
			name = name.ToLowerInvariant();

			var game = GameMemory.SupportedGames.FirstOrDefault(g =>
				g.GetType().Name.ToLowerInvariant() == name
					|| g.GameNames.Any(n => n.ToLowerInvariant() == name)
			);

			if (game != null)
				return game;

			return GameMemory.SupportedGames.FirstOrDefault(g =>
				g.GameNames.Any(n => name.Contains(n.ToLowerInvariant()))
			);
		}

		public XmlNode GetSettings(XmlDocument doc)
		{
			XmlElement settingsNode = doc.CreateElement("Settings");

			settingsNode.AppendChild(SettingsHelper.ToElement(doc, "Version", Assembly.GetExecutingAssembly().GetName().Version.ToString(3)));
			settingsNode.AppendChild(SettingsHelper.ToElement(doc, "AutoStart", AutoStart));
			settingsNode.AppendChild(SettingsHelper.ToElement(doc, "AutoReset", AutoReset));
			settingsNode.AppendChild(SettingsHelper.ToElement(doc, "AutoSplitOnMapChange", AutoSplitOnMapChange));
			settingsNode.AppendChild(SettingsHelper.ToElement(doc, "AutoSplitOncePerMap", AutoSplitOncePerMap));
			settingsNode.AppendChild(SettingsHelper.ToElement(doc, "Game", ((Type)cbGame.SelectedItem).Name));

			var mapsNode = settingsNode.AppendChild(doc.CreateElement("MapWhitelist"));
			foreach (var map in Maps)
			{
				var elem = (XmlElement)mapsNode.AppendChild(doc.CreateElement("Map"));
				elem.InnerText = map.Name;
				elem.SetAttribute("SplitOnEnter", map.SplitOnEnter.ToString());
				elem.SetAttribute("SplitOnLeave", map.SplitOnLeave.ToString());
			}

			return settingsNode;
		}

		public void SetSettings(XmlNode settings)
		{
			var element = (XmlElement)settings;

			AutoStart = SettingsHelper.ParseBool(settings["AutoStart"], DEFAULT_AUTOSTART);
			AutoReset = SettingsHelper.ParseBool(settings["AutoReset"], DEFAULT_AUTOSTART);
			AutoSplitOnMapChange = SettingsHelper.ParseBool(settings["AutoSplitOnMapChange"], DEFAULT_AUTOSPLITONMAPCHANGE);
			AutoSplitOncePerMap = SettingsHelper.ParseBool(settings["AutoSplitOncePerMap"], DEFAULT_AUTOSPLITONCEPERMAP);

			GameSupport game = null;
			if (!string.IsNullOrWhiteSpace(settings["Game"]?.InnerText))
				game = SearchGameSupport(settings["Game"].InnerText);

			if (game == null)
				game = SearchGameSupport(_state.Run.GameName) ?? GameMemory.SupportedGames[0];

			cbGame.SelectedItem = game.GetType();

			if (settings["MapWhitelist"] != null)
			{
				var mapnames = from map in Maps
							   select map.Name;

				if (SettingsHelper.ParseVersion(settings["Version"]) > new Version(1, 6, 1))
				{
					foreach (XmlElement elem in settings["MapWhitelist"].ChildNodes)
					{
						if (mapnames.Contains(elem.InnerText, StringComparer.OrdinalIgnoreCase))
						{
							var curMap = (from map in Maps
										  where map.Name == elem.InnerText
										  select map).First();
							curMap.SplitOnEnter = bool.Parse(string.IsNullOrEmpty(elem.GetAttribute("SplitOnEnter")) ? "False" : elem.GetAttribute("SplitOnEnter"));
							curMap.SplitOnLeave = bool.Parse(string.IsNullOrEmpty(elem.GetAttribute("SplitOnLeave")) ? "False" : elem.GetAttribute("SplitOnLeave"));
						}
					}
				}
				else
				//backwards compatibility
				{
					foreach (XmlElement elem in settings["MapWhitelist"].ChildNodes)
					{
						if (mapnames.Contains(elem.InnerText, StringComparer.OrdinalIgnoreCase))
						{
							var curMap = (from map in Maps
										  where map.Name == elem.InnerText
										  select map).First();
							curMap.SplitOnEnter = bool.Parse(string.IsNullOrEmpty(elem.GetAttribute("enabled")) ? "False" : elem.GetAttribute("enabled"));
							curMap.SplitOnLeave = false;
						}
					}
				}
			}
        }

		void btnAddMap_Click(object sender, EventArgs e)
		{
			txtMap.Text = txtMap.Text.Trim();

			if (!string.IsNullOrWhiteSpace(txtMap.Text))
			{
				Maps.Add(new Map(txtMap.Text));
				txtMap.Clear();
			}
		}

		void btnRemoveMap_Click(object sender, EventArgs e)
		{
			foreach (DataGridViewRow row in dgvMapSet.SelectedRows)
			{
				Map map = (Map) row.DataBoundItem;
				Maps.Remove(map);
			}
		}

		void cbGame_SelectedIndexChanged(object sender, EventArgs e)
		{
            var selected = (GameSupport)Activator.CreateInstance((Type)cbGame.SelectedItem);

			Maps.Clear();
			if (selected?.Maps != null)
			{
				foreach (var map in selected.Maps)
				{
					Maps.Add(new Map(map) { SplitOnEnter = true });
				}
			}
		}
	}
}
