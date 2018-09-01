using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ExceptionReporting.Core;
using ExceptionReporting.MVP.Presenters;
using ExceptionReporting.SystemInfo;
using ExceptionReporting.Templates;

#pragma warning disable 1591

namespace ExceptionReporting.MVP.Views
{
	/// <summary>
	/// The main ExceptionReporter dialog
	/// </summary>
	public partial class ExceptionReportView : Form, IExceptionReportView
	{
		private readonly ErrorDetail _error;
		private readonly ReportConfig _config;
		private bool _isDataRefreshRequired;
		private readonly ExceptionReportPresenter _presenter;

		public ExceptionReportView(ReportBag bag)
		{
			_error = bag.Error;
			_config = bag.Config;
			
			ShowFullDetail = true;
			InitializeComponent();
			TopMost = _config.TopMost;

			_presenter = new ExceptionReportPresenter(this, bag);

			WireUpEvents();
			PopulateTabs();
			PopulateReportInfo();
		}

		private void PopulateReportInfo()
		{
			lblExplanation.Text = _config.UserExplanationLabel;
			ShowFullDetail = _config.ShowFullDetail;
			ToggleShowFullDetail();
			btnDetailToggle.Visible = _config.ShowLessDetailButton;

			//TODO: show all exception messages
			txtExceptionMessageLarge.Text =
				txtExceptionMessage.Text =
					!string.IsNullOrEmpty(_config.CustomMessage) ? _config.CustomMessage : _error.Exceptions.First().Message;

			txtExceptionMessageLarge2.Text = txtExceptionMessageLarge.Text;

			txtDate.Text = _error.ExceptionDate.ToShortDateString();
			txtTime.Text = _error.ExceptionDate.ToShortTimeString();
			txtRegion.Text = _config.RegionInfo;
			txtApplicationName.Text = _config.AppName;
			txtVersion.Text = _config.AppVersion;

			btnClose.FlatStyle =
					btnDetailToggle.FlatStyle =
					btnCopy.FlatStyle =
					btnEmail.FlatStyle =
					btnSave.FlatStyle = _config.ShowFlatButtons ? FlatStyle.Flat : FlatStyle.Standard;

			listviewAssemblies.BackColor =
					txtRegion.BackColor =
					txtTime.BackColor =
					txtTime.BackColor =
					txtVersion.BackColor =
					txtApplicationName.BackColor =
					txtDate.BackColor =
					txtExceptionMessageLarge.BackColor =
					txtExceptionMessage.BackColor = _config.BackgroundColor;

			if (!_config.ShowButtonIcons)
			{
				RemoveButtonIcons();
			}

			if (!_config.ShowEmailButton)
			{
				RemoveEmailButton();
			}

			Text = _config.TitleText;
			txtUserExplanation.Font = new Font(txtUserExplanation.Font.FontFamily, _config.UserExplanationFontSize);
			lblContactCompany.Text = string.Format("If this problem persists, please contact {0} support.", _config.CompanyName);
			btnSimpleEmail.Text = 
				string.Format("{0} {1}", 
				_config.SendMethod == ReportSendMethod.WebService ? "Send" : "Email",
				_config.SendMethod == ReportSendMethod.WebService && !_config.CompanyName.IsEmpty() ? "to " + _config.CompanyName : _config.CompanyName);
			btnEmail.Text = _config.SendMethod == ReportSendMethod.WebService ? "Send" : "Email";
		}

		private void RemoveEmailButton()
		{
			this.btnSimpleEmail.Hide();
			this.btnEmail.Hide();
			this.btnCopy.Location = btnEmail.Location;
			this.btnSimpleCopy.Location = btnSimpleEmail.Location;
		}

		private void RemoveButtonIcons()
		{
			// removing the icons, requires a bit of reshuffling of positions
			btnCopy.Image = btnEmail.Image = btnSave.Image = null;
			btnClose.Height = btnDetailToggle.Height = btnCopy.Height = btnEmail.Height = btnSave.Height = 27;
			btnClose.TextAlign = btnDetailToggle.TextAlign = btnCopy.TextAlign = btnEmail.TextAlign = btnSave.TextAlign = ContentAlignment.MiddleCenter;
			btnClose.Font = btnDetailToggle.Font = btnSave.Font = btnEmail.Font = btnCopy.Font = new Font(btnCopy.Font.FontFamily, 8.25f);
			ShiftDown3Pixels(new[] { btnClose, btnDetailToggle, btnCopy, btnEmail, btnSave });
		}

		private static void ShiftDown3Pixels(IEnumerable<Control> buttons)
		{
			foreach (var button in buttons)
			{
				button.Location = Point.Add(button.Location, new Size(new Point(0, 3)));
			}
		}

		private void WireUpEvents()
		{
			btnEmail.Click += Email_Click;
			btnSimpleEmail.Click += Email_Click;
			btnCopy.Click += Copy_Click;
			btnSimpleCopy.Click += Copy_Click;
			btnClose.Click += Close_Click;
			btnDetailToggle.Click += Detail_Click;
			btnSimpleDetailToggle.Click += Detail_Click;
			btnSave.Click += Save_Click;
			KeyPreview = true;
			KeyDown += ExceptionReportView_KeyDown;
		}

		private void ExceptionReportView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				Close();
			}
		}

		public string ProgressMessage
		{
			set
			{
				lblProgressMessage.Visible = true;
				lblProgressMessage.Text = value;
			}
		}

		public bool EnableEmailButton
		{
			set { btnEmail.Enabled = value; }
		}

		public bool ShowProgressBar
		{
			set { progressBar.Visible = value; }
		}

		private bool ShowProgressLabel
		{
			set { lblProgressMessage.Visible = value; }
		}

		public bool ShowFullDetail { get; set; }

		public void ToggleShowFullDetail()
		{
			if (ShowFullDetail)
			{
				lessDetailPanel.Hide();
				btnDetailToggle.Text = "Less Detail";
				tabControl.Visible = true;
				Size = new Size(625, 456);
			}
			else
			{
				lessDetailPanel.Show();
				btnDetailToggle.Text = "More Detail";
				tabControl.Visible = false;
				Size = new Size(415, 235);
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			// this is to make it so the LessDetail view is fixed (it's design doesn't allow for resizing)
			// but MoreDetail is resizable
			FormBorderStyle = ShowFullDetail ? FormBorderStyle.Sizable : FormBorderStyle.FixedDialog;
		}

		public string UserExplanation
		{
			get { return txtUserExplanation.Text; }
		}

		public void Completed(bool success)
		{
			ProgressMessage = success ? "Report sent" : "Failed to send report";
			ShowProgressBar = false;
			btnEmail.Enabled = true;
		}

		public void MapiSendCompleted()
		{
			Completed(true);
			ProgressMessage = string.Empty;
		}

		/// <summary>
		/// starts with all tabs visible, and removes ones that aren't configured to show
		/// </summary>
		private void PopulateTabs()
		{
			if (!_presenter.Config.ShowGeneralTab)
			{
				tabControl.TabPages.Remove(tabGeneral);
			}
			if (!_presenter.Config.ShowExceptionsTab)
			{
				tabControl.TabPages.Remove(tabExceptions);
			}
			if (!_presenter.Config.ShowAssembliesTab)
			{
				tabControl.TabPages.Remove(tabAssemblies);
			}
			if (!_presenter.Config.ShowSysInfoTab)
			{
				tabControl.TabPages.Remove(tabSysInfo);
			}
		}

		//TODO consider putting on a background thread - and avoid the OnActivated event altogether
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			if (_isDataRefreshRequired)
			{
				_isDataRefreshRequired = false;
				_presenter.PopulateReport();
			}
		}

		public void SetProgressCompleteState()
		{
			Cursor = Cursors.Default;
			ShowProgressLabel = ShowProgressBar = false;
		}

		public void ShowWindow()
		{
			_isDataRefreshRequired = true;
			ShowDialog();
		}

		public void SetInProgressState()
		{
			Cursor = Cursors.WaitCursor;
			ShowProgressLabel = true;
			ShowProgressBar = true;
			Application.DoEvents();
		}

		public void PopulateExceptionTab(IEnumerable<Exception> exceptions)
		{
			var exs = exceptions as Exception[] ?? exceptions.ToArray();
			if (exs.Length == 1)
			{
				var exception = exs.FirstOrDefault();
				AddExceptionControl(tabExceptions, exception);
			}
			else
			{
				var innerTabControl = new TabControl { Dock = DockStyle.Fill };
				tabExceptions.Controls.Add(innerTabControl);
				for (var index = 0; index < exs.Length; index++)
				{
					var exception = exs[index];
					var tabPage = new TabPage { Text = string.Format("Exception {0}", index + 1) };
					innerTabControl.TabPages.Add(tabPage);
					AddExceptionControl(tabPage, exception);
				}
			}
		}

		private void AddExceptionControl(Control control, Exception exception)
		{
			var exceptionDetail = new ExceptionDetailControl();
			exceptionDetail.SetControlBackgrounds(_presenter.Config.BackgroundColor);
			exceptionDetail.PopulateExceptionTab(exception);
			exceptionDetail.Dock = DockStyle.Fill;
			control.Controls.Add(exceptionDetail);
		}

		public void PopulateAssembliesTab()
		{
			listviewAssemblies.Clear();
			listviewAssemblies.Columns.Add("Name", 320, HorizontalAlignment.Left);
			listviewAssemblies.Columns.Add("Version", 150, HorizontalAlignment.Left);

			_presenter.GetReferencedAssemblies().ForEach(this.AddAssembly);
		}

		private void AddAssembly(AssemblyRef assembly)
		{
			var listViewItem = new ListViewItem
			{
				Text = assembly.Name
			};
			listViewItem.SubItems.Add(assembly.Version);
			listviewAssemblies.Items.Add(listViewItem);
		}

		private TreeNode CreateSysInfoTree()
		{
			var rootNode = new TreeNode("System");

			foreach (var sysInfoResult in _presenter.GetSysInfoResults())
			{
				SysInfoResultMapper.AddTreeViewNode(rootNode, sysInfoResult);
			}

			return rootNode;
		}

		public void PopulateSysInfoTab()
		{
			var rootNode = CreateSysInfoTree();
			treeEnvironment.Nodes.Add(rootNode);
			rootNode.Expand();
		}

		private void Copy_Click(object sender, EventArgs e)
		{
			_presenter.CopyReportToClipboard();
		}

		private void Close_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Detail_Click(object sender, EventArgs e)
		{
			_presenter.ToggleDetail();
		}

		private void Email_Click(object sender, EventArgs e)
		{
			_presenter.SendReport();
		}

		private void Save_Click(object sender, EventArgs e)
		{
			var saveDialog = new SaveFileDialog
			{
				Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*",
				FilterIndex = 1,
				RestoreDirectory = true
			};

			if (saveDialog.ShowDialog() == DialogResult.OK)
			{
				_presenter.SaveReportToFile(saveDialog.FileName);
			}
		}

		public void ShowError(string message, Exception exception)
		{
			MessageBox.Show(message, "Error sending report", MessageBoxButtons.OK, MessageBoxIcon.Error); 
		}
	}
}

#pragma warning restore 1591