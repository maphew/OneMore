﻿//************************************************************************************************
// Copyright © 2022 Steven M Cohn.  All rights reserved.
//************************************************************************************************

#pragma warning disable IDE0060 // Remove unused parameter

namespace River.OneMoreAddIn.UI
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Linq;
	using System.Windows.Forms;
	using Resx = River.OneMoreAddIn.Properties.Resources;


	/// <summary>
	/// Implements an auto-complete drop-down control that can be attached to a TextBox.
	/// </summary>
	[ProvideProperty("AutoCompleteList", typeof(Control))]
	internal class MoreAutoCompleteList : ListView, IExtenderProvider
	{
		private ToolStripDropDown popup;        // the invisible host control of this ListtView	
		private readonly Font highFont;         // font of matched substring
		private readonly List<Cmd> commands;    // original list of commands
		private readonly List<Cmd> matches;     // dynamic list of matched commands
		private string boxtext;                 // the current/previous text in the Owner TextBox

		// each command name is described by a Cmd entry
		private sealed class Cmd
		{
			public string Name;                 // provided name of command, including optional keys
			public bool Recent;                 // true if this is a recently used command
		}


		/// <summary>
		/// Initialize a new instance. The instance should be bound to a TextBox using
		/// the SetAutoCompleteList method.
		/// </summary>
		public MoreAutoCompleteList()
		{
			OwnerDraw = true;

			// detail view with default headless column so all drawing is done by DrawSubItem
			View = View.Details;
			Columns.Add("cmd");
			HeaderStyle = ColumnHeaderStyle.None;

			FullRowSelect = true;
			MultiSelect = false;
			MinimumSize = new Size(300, 300);
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);

			Font = new Font("Segoe UI", 9);
			highFont = new Font(Font, Font.Style | FontStyle.Bold);
			commands = new List<Cmd>();
			matches = new List<Cmd>();
		}


		/// <summary>
		/// Gets or sets a character used to delimit a command's name from its key sequence.
		/// </summary>
		public char KeyDivider { get; set; } = '|';


		/// <summary>
		/// Gets a value indicating whether there are currently any matched items.
		/// </summary>
		public bool HasMatches =>
			string.IsNullOrWhiteSpace(Owner.Text.Trim()) ||
			commands.Count == 0 ||
			commands.Any(c => c.Name.ContainsICIC(Owner.Text.Trim()));


		/// <summary>
		/// Gets a value indicating whether the hosted popup is visible
		/// </summary>
		public bool IsVisible => popup?.Visible == true;


		/// <summary>
		/// Gets the TextBox control to which this auto-complete list is bound.
		/// </summary>
		public TextBox Owner { get; private set; }


		/// <summary>
		/// Gets or sets whether the list is shown immediately along with its
		/// TextBox. The default is to only show the list on the first keypress.
		/// </summary>
		public bool VisibleByDefault { get; set; }


		bool IExtenderProvider.CanExtend(object extendee)
		{
			return (extendee is TextBox);
		}


		/// <summary>
		/// Required implementation for ProvideProperty.
		/// </summary>
		public Control GetAutoCompleteList(Control control)
		{
			return Owner;
		}


		/// <summary>
		/// Binds this control to a specified TextBox.
		/// This is required implementation for ProvideProperty.
		/// </summary>
		/// <param name="control">The TextBox to bind with the list</param>
		/// <param name="list">The instance of the list to bind</param>
		/// <remarks>
		/// This call is normally auto-generated by the VS UI designer by
		/// setting the AutoCompleteList property of a TextBox.
		/// </remarks>
		public void SetAutoCompleteList(Control control, MoreAutoCompleteList list)
		{
			if (control == null)
			{
				// TODO: tear down?
			}

			if (control is TextBox box)
			{
				Owner = box;
				Width = Math.Max(box.Width, 300);
				box.KeyDown += DoKeydown;
				box.PreviewKeyDown += DoPreviewKeyDown;
				box.TextChanged += DoTextChanged;
				boxtext = box.Text.Trim();

				if (VisibleByDefault)
				{
					box.GotFocus += ShowSelf;
				}
			}
			else
			{
				// this should be covered by CanExtend but just in case...
				throw new ArgumentException("SetAutoCompleteList(control) must be a TextBox");
			}
		}


		/// <summary>
		/// Populate the typeahead buffer with a list of names.
		/// </summary>
		/// <param name="names">List of command names</param>
		/// <param name="recentNames">Optional list of recently used command names</param>
		public void LoadCommands(IEnumerable<string> names, IEnumerable<string> recentNames = null)
		{
			Items.Clear();
			commands.Clear();
			matches.Clear();
			foreach (var name in names)
			{
				var cmd = new Cmd { Name = name };
				Items.Add(name);
				commands.Add(cmd);
			}

			if (recentNames?.Any() == true)
			{
				// inject recent names at top of list
				foreach (var name in recentNames.Reverse())
				{
					Items.Insert(0, name);
					commands.Insert(0, new Cmd
					{
						Name = name,
						Recent = true
					});
				}
			}

			// preselect the first item
			Items[0].Selected = true;
		}


		private void ShowSelf(object sender, EventArgs e)
		{
			if (sender is TextBox box && !box.Visible)
			{
				popup?.Close();
				return;
			}

			if (popup == null)
			{
				popup = new ToolStripDropDown
				{
					Margin = Padding.Empty,
					Padding = Padding.Empty,
					AutoClose = false
				};
				popup.Items.Add(new ToolStripControlHost(this)
				{
					Margin = Padding.Empty,
					Padding = Padding.Empty
				});

				Owner.FindForm().Move += HidePopup;
			}

			if (!popup.Visible)
			{
				var itemHeight = GetItemRect(0).Height;
				Height = (itemHeight + 1) * 15;

				popup.Show(Owner, new Point(0, Owner.Height));
			}
		}


		private void DoPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			// must catch Enter key in preview event so it's not superseded
			// by Form.AcceptButton eventing
			if (e.KeyCode == Keys.Enter)
			{
				if (SelectedItems.Count > 0)
				{
					var text = SelectedItems[0].Text;
					var index = text.IndexOf(KeyDivider);
					Owner.Text = index < 0 ? text : text.Substring(0, index);
				}
			}
		}


		private void DoKeydown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				HidePopup(sender, e);
				return;
			}

			if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey)
			{
				e.Handled = true;
				return;
			}

			ShowSelf(null, EventArgs.Empty);

			if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up ||
				e.KeyCode == Keys.PageDown || e.KeyCode == Keys.PageUp)
			{
				SelectItem(e.KeyCode);
				Owner.Focus();
				e.Handled = true;
			}

			void SelectItem(Keys keycode)
			{
				if (Items.Count == 0)
				{
					return;
				}
				if (SelectedItems.Count == 0)
				{
					Items[0].Selected = true;
					ScrollIntoView();
				}
				else if (keycode == Keys.Down && SelectedItems[0].Index < Items.Count - 1)
				{
					Items[SelectedItems[0].Index + 1].Selected = true;
					ScrollIntoView();
				}
				else if (keycode == Keys.Up && SelectedItems[0].Index > 0)
				{
					Items[SelectedItems[0].Index - 1].Selected = true;
					ScrollIntoView();
				}
				else if (
					(keycode == Keys.PageDown && SelectedItems[0].IndentCount < Items.Count - 1) ||
					(keycode == Keys.PageUp && SelectedItems[0].Index > 0))
				{
					var max = Items.Count - 1;
					var top = TopItem.Index;
					var itemHeight = GetItemRect(top).Height;
					var visible = Height / (itemHeight + 1);
					var bottom = Math.Min(top + visible - 1, max);
					var selected = SelectedIndices[0];
					if (keycode == Keys.PageDown)
					{
						selected = selected < bottom ? bottom : Math.Min(selected + visible - 1, max);
					}
					else
					{
						selected = selected > top ? top : Math.Max(selected - visible + 1, 0);
					}

					Items[selected].Selected = true;
					ScrollIntoView();
				}
			}
		}


		private void HidePopup(object sender, EventArgs e)
		{
			if (popup?.Visible == true)
			{
				popup.Close();
			}
		}


		private void DoTextChanged(object sender, EventArgs e)
		{
			var text = Owner.Text.Trim();
			if (text != boxtext)
			{
				var hadMatches = matches.Any();
				matches.Clear();

				foreach (var cmd in commands)
				{
					if (cmd.Name.ContainsICIC(text))
					{
						matches.Add(cmd);
					}
				}

				if (matches.Any())
				{
					Items.Clear();
					matches.ForEach(m => Items.Add(m.Name));
					Items[0].Selected = true;
				}
				else if (hadMatches)
				{
					Items.Clear();
					commands.ForEach(c => Items.Add(c.Name));
					Items[0].Selected = true;
				}

				boxtext = text;
			}
		}


		private void ScrollIntoView()
		{
			EnsureVisible(SelectedIndices.Count > 0 ? SelectedIndices[0] : 0);
		}


		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			var info = HitTest(e.Location);

			if (info?.Item is ListViewItem item)
			{
				var text = item.Text;
				var index = text.IndexOf(KeyDivider);
				Owner.Text = index < 0 ? text : text.Substring(0, index);
				Owner.Focus();
				SendKeys.Send("{Enter}");
			}
		}


		protected override void OnClientSizeChanged(EventArgs e)
		{
			base.OnClientSizeChanged(e);
			if (Columns.Count > 0)
			{
				Columns[0].Width = ClientSize.Width;
			}
		}


		protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
		{
			var back = SystemBrushes.Window;
			var fore = SystemBrushes.WindowText;
			var high = SystemBrushes.Highlight;

			if (e.Item.Selected)
			{
				back = SystemBrushes.Highlight;
				fore = SystemBrushes.HighlightText;
				high = SystemBrushes.GradientInactiveCaption;
			}

			e.Graphics.FillRectangle(back, e.Bounds.X, e.Bounds.Y + 1, e.Bounds.Width, e.Bounds.Height - 2);

			string keys = null;

			var source = matches.Any() ? matches : commands;
			var command = source[e.Item.Index];
			var text = command.Name;

			// parse out key sequence if any
			var bar = text.IndexOf(KeyDivider);
			if (bar > 0)
			{
				keys = text.Substring(bar + 1);
				text = text.Substring(0, bar);
			}

			var drawn = false;
			float x;

			if (!string.IsNullOrWhiteSpace(Owner.Text))
			{
				var index = text.IndexOf(Owner.Text, StringComparison.InvariantCultureIgnoreCase);
				if (index >= 0)
				{
					string phrase;
					SizeF size;

					// track x-offset of each phrase
					x = e.Bounds.X;

					// phrase is in middle so draw prior phrase
					if (index > 0)
					{
						phrase = text.Substring(0, index);

						e.Graphics.DrawString(phrase, Font, fore, x, e.Bounds.Y, StringFormat.GenericDefault);
						size = e.Graphics.MeasureString(phrase, Font, new PointF(x, e.Bounds.Y), StringFormat.GenericDefault);
						x += size.Width;
					}

					// draw matched phrase
					phrase = text.Substring(index, Owner.Text.Length);
					e.Graphics.DrawString(phrase, highFont, high, x, e.Bounds.Y, StringFormat.GenericTypographic);

					size = e.Graphics.MeasureString(
						phrase, highFont, new PointF(x, e.Bounds.Y), StringFormat.GenericTypographic);

					x += size.Width;

					// draw remaining phrase
					index += Owner.Text.Length;
					if (index < text.Length)
					{
						phrase = text.Substring(index);
						e.Graphics.DrawString(phrase, Font, fore, x, e.Bounds.Y, StringFormat.GenericTypographic);
					}

					drawn = true;
				}
			}

			if (!drawn)
			{
				e.Graphics.DrawString(text, e.Item.Font, fore, e.Bounds);
			}

			// track where to draw key sequence for command
			x = e.Bounds.Width - 5;

			// did we match any Recent items at all?
			if (source[0].Recent)
			{
				if (e.ItemIndex == 0)
				{
					var annotation = Resx.AutoComplete_recentlyUsed;
					var size = e.Graphics.MeasureString(annotation, Font);
					// push key sequence positioning over to the left
					x -= size.Width;
					e.Graphics.DrawString(annotation, e.Item.Font, high, x, e.Bounds.Y);
				}

				// index of first common command found after all recent commands
				var common = 1;
				while (common < source.Count && source[common].Recent)
				{
					common++;
				}

				// divider line
				if (common < source.Count && e.ItemIndex == common - 1)
				{
					e.Graphics.DrawLine(Pens.Silver,
						e.Bounds.X, e.Bounds.Y + e.Bounds.Height - 1,
						e.Bounds.Width, e.Bounds.Y + e.Bounds.Height - 1);
				}
				else if (common == e.ItemIndex)
				{
					var annotation = Resx.AutoComplete_otherCommands;
					var size = e.Graphics.MeasureString(annotation, Font);
					// push key sequence positioning over to the left
					x -= size.Width;
					e.Graphics.DrawString(annotation, e.Item.Font, high, x, e.Bounds.Y);
				}
			}

			// key sequence
			if (!string.IsNullOrWhiteSpace(keys))
			{
				var size = e.Graphics.MeasureString(keys, Font);
				x -= size.Width + 5;
				e.Graphics.DrawString(keys, e.Item.Font, SystemBrushes.ActiveCaption, x, e.Bounds.Y);
			}
		}


		protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
		{
			base.OnItemSelectionChanged(e);
			Owner.Focus();
		}
	}
}
