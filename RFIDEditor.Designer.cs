
namespace RFIDEditor
{
    partial class RFIDEditor
    {
        private System.ComponentModel.IContainer components = null;

        // Main Containers and Grid
        private System.Windows.Forms.DataGridView pageGrid;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnFixBCC;
        private System.Windows.Forms.GroupBox grpQuickEdit;
        private System.Windows.Forms.Panel pnlColor;

        // Labels (Modern Sleek Layout)
        private System.Windows.Forms.Label lblMaterial;
        private System.Windows.Forms.Label lblSubtype;
        private System.Windows.Forms.Label lblModifier;
        private System.Windows.Forms.Label lblWeight;
        private System.Windows.Forms.Label lblDiameter;
        private System.Windows.Forms.Label lblTemps;
        private System.Windows.Forms.Label lblColorPreview;
        // Input Fields
        private System.Windows.Forms.ComboBox comboMaterial;
        private System.Windows.Forms.ComboBox comboSubtype;
        private System.Windows.Forms.TextBox txtModifier;
        private System.Windows.Forms.TextBox txtWeightInput;
        private System.Windows.Forms.TextBox txtDiameter;
        private System.Windows.Forms.TextBox txtMinTemp;
        private System.Windows.Forms.TextBox txtMaxTemp;

        private System.Windows.Forms.CheckBox chkShowAll;
        private System.Windows.Forms.Button btnExportMobile;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pageGrid = new DataGridView();
            btnLoad = new Button();
            btnSave = new Button();
            btnFixBCC = new Button();
            grpQuickEdit = new GroupBox();
            txtManufacturer = new TextBox();
            lblManufacturer = new Label();
            btnSetCurrentDate = new Button();
            txtProdDate = new TextBox();
            lblProdDate = new Label();
            lblMaterial = new Label();
            comboMaterial = new ComboBox();
            lblSubtype = new Label();
            comboSubtype = new ComboBox();
            lblModifier = new Label();
            chkShowAll = new CheckBox();
            txtModifier = new TextBox();
            lblWeight = new Label();
            txtWeightInput = new TextBox();
            lblDiameter = new Label();
            txtDiameter = new TextBox();
            lblTemps = new Label();
            txtMinTemp = new TextBox();
            txtMaxTemp = new TextBox();
            lblColorPreview = new Label();
            pnlColor = new Panel();
            toolTip = new ToolTip(components);
            btnExportMobile = new Button();
            btnCopyCleanHex = new Button();
            btnGenerateNew = new Button();
            dateTip = new ToolTip(components);
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)pageGrid).BeginInit();
            grpQuickEdit.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // pageGrid
            // 
            pageGrid.AllowUserToAddRows = false;
            pageGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pageGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            pageGrid.Location = new Point(12, 210);
            pageGrid.Name = "pageGrid";
            pageGrid.Size = new Size(561, 400);
            pageGrid.TabIndex = 4;
            pageGrid.CellEndEdit += pageGrid_CellEndEdit;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(106, 12);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(75, 23);
            btnLoad.TabIndex = 0;
            btnLoad.Text = "Load .BIN";
            toolTip.SetToolTip(btnLoad, "Load an existing RFID tag binary file");
            btnLoad.Click += btnLoad_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(187, 12);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 1;
            btnSave.Text = "Save .BIN";
            toolTip.SetToolTip(btnSave, "Save current tag data to binary file");
            btnSave.Click += btnSave_Click;
            // 
            // btnFixBCC
            // 
            btnFixBCC.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnFixBCC.Location = new Point(268, 12);
            btnFixBCC.Name = "btnFixBCC";
            btnFixBCC.Size = new Size(89, 23);
            btnFixBCC.TabIndex = 2;
            btnFixBCC.Text = "Fix Checksum";
            toolTip.SetToolTip(btnFixBCC, "Recalculate BCC1 checksum at 0x08");
            btnFixBCC.Click += btnFixBCC_Click;
            // 
            // grpQuickEdit
            // 
            grpQuickEdit.Controls.Add(txtManufacturer);
            grpQuickEdit.Controls.Add(lblManufacturer);
            grpQuickEdit.Controls.Add(btnSetCurrentDate);
            grpQuickEdit.Controls.Add(txtProdDate);
            grpQuickEdit.Controls.Add(lblProdDate);
            grpQuickEdit.Controls.Add(lblMaterial);
            grpQuickEdit.Controls.Add(comboMaterial);
            grpQuickEdit.Controls.Add(lblSubtype);
            grpQuickEdit.Controls.Add(comboSubtype);
            grpQuickEdit.Controls.Add(lblModifier);
            grpQuickEdit.Controls.Add(chkShowAll);
            grpQuickEdit.Controls.Add(txtModifier);
            grpQuickEdit.Controls.Add(lblWeight);
            grpQuickEdit.Controls.Add(txtWeightInput);
            grpQuickEdit.Controls.Add(lblDiameter);
            grpQuickEdit.Controls.Add(txtDiameter);
            grpQuickEdit.Controls.Add(lblTemps);
            grpQuickEdit.Controls.Add(txtMinTemp);
            grpQuickEdit.Controls.Add(txtMaxTemp);
            grpQuickEdit.Controls.Add(lblColorPreview);
            grpQuickEdit.Controls.Add(pnlColor);
            grpQuickEdit.Location = new Point(12, 50);
            grpQuickEdit.Name = "grpQuickEdit";
            grpQuickEdit.Size = new Size(582, 150);
            grpQuickEdit.TabIndex = 3;
            grpQuickEdit.TabStop = false;
            grpQuickEdit.Text = "Spool Configuration (Auto-Sync) — ✓ = Used by printer, ⓘ = Metadata only";
            // 
            // txtManufacturer
            //
            txtManufacturer.BackColor = Color.FromArgb(240, 240, 240);
            txtManufacturer.Location = new Point(470, 99);
            txtManufacturer.MaxLength = 8;
            txtManufacturer.Name = "txtManufacturer";
            txtManufacturer.ReadOnly = true;
            txtManufacturer.Size = new Size(100, 23);
            txtManufacturer.TabIndex = 21;
            txtManufacturer.Text = "--";
            txtManufacturer.TextAlign = HorizontalAlignment.Center;
            toolTip.SetToolTip(txtManufacturer, "Manufacturer code at 0x41-0x44\r\nELEGOO = 0xEEEEEEEE\r\nGeneric = Other values\r\n-- = Empty/Invalid");
            // 
            // lblManufacturer
            //
            lblManufacturer.ForeColor = Color.Gray;
            lblManufacturer.Font = new Font(lblManufacturer.Font, FontStyle.Italic);
            lblManufacturer.Location = new Point(470, 73);
            lblManufacturer.Name = "lblManufacturer";
            lblManufacturer.Size = new Size(100, 23);
            lblManufacturer.TabIndex = 20;
            lblManufacturer.Text = "ⓘ Manufacturer";
            // 
            // btnSetCurrentDate
            // 
            btnSetCurrentDate.Location = new Point(217, 99);
            btnSetCurrentDate.Name = "btnSetCurrentDate";
            btnSetCurrentDate.Size = new Size(23, 23);
            btnSetCurrentDate.TabIndex = 19;
            btnSetCurrentDate.Text = "📅";
            toolTip.SetToolTip(btnSetCurrentDate, "Set to current month");
            btnSetCurrentDate.Click += btnSetCurrentDate_Click;
            // 
            // txtProdDate
            // 
            txtProdDate.Location = new Point(141, 99);
            txtProdDate.MaxLength = 4;
            txtProdDate.Name = "txtProdDate";
            txtProdDate.Size = new Size(70, 23);
            txtProdDate.TabIndex = 18;
            dateTip.SetToolTip(txtProdDate, "Enter date as YYMM (e.g., 2602 for Feb 2026)");
            txtProdDate.TextChanged += txtProdDate_TextChanged;
            // 
            // lblProdDate
            //
            lblProdDate.ForeColor = Color.Gray;
            lblProdDate.Font = new Font(lblProdDate.Font, FontStyle.Italic);
            lblProdDate.Location = new Point(130, 73);
            lblProdDate.Name = "lblProdDate";
            lblProdDate.Size = new Size(105, 23);
            lblProdDate.TabIndex = 17;
            lblProdDate.Text = "ⓘ Prod Date";
            // 
            // lblMaterial
            //
            lblMaterial.Location = new Point(15, 19);
            lblMaterial.Name = "lblMaterial";
            lblMaterial.Size = new Size(100, 23);
            lblMaterial.TabIndex = 0;
            lblMaterial.Text = "✓ Material";
            // 
            // comboMaterial
            // 
            comboMaterial.Items.AddRange(new object[] { "PLA", "PETG", "ABS", "ASA", "TPU", "PA", "CPE", "PC", "PVA", "BVOH", "EVA", "HIPS", "PP", "PPA", "PPS" });
            comboMaterial.Location = new Point(15, 45);
            comboMaterial.Name = "comboMaterial";
            comboMaterial.Size = new Size(100, 23);
            comboMaterial.TabIndex = 1;
            comboMaterial.SelectedIndexChanged += comboMaterial_SelectedIndexChanged;
            // 
            // lblSubtype
            //
            lblSubtype.Location = new Point(130, 19);
            lblSubtype.Name = "lblSubtype";
            lblSubtype.Size = new Size(100, 23);
            lblSubtype.TabIndex = 2;
            lblSubtype.Text = "✓ Supplement";
            // 
            // comboSubtype
            // 
            comboSubtype.DropDownStyle = ComboBoxStyle.DropDownList;
            comboSubtype.Location = new Point(130, 45);
            comboSubtype.Name = "comboSubtype";
            comboSubtype.Size = new Size(100, 23);
            comboSubtype.TabIndex = 3;
            comboSubtype.SelectedIndexChanged += comboSubtype_SelectedIndexChanged;
            // 
            // lblModifier
            //
            lblModifier.ForeColor = Color.Gray;
            lblModifier.Font = new Font(lblModifier.Font, FontStyle.Italic);
            lblModifier.Location = new Point(245, 19);
            lblModifier.Name = "lblModifier";
            lblModifier.Size = new Size(100, 23);
            lblModifier.TabIndex = 4;
            lblModifier.Text = "ⓘ Mod (L/M/D)";
            // 
            // chkShowAll
            // 
            chkShowAll.AutoSize = true;
            chkShowAll.Location = new Point(15, 135);
            chkShowAll.Name = "chkShowAll";
            chkShowAll.Size = new Size(75, 19);
            chkShowAll.TabIndex = 5;
            chkShowAll.Text = "Show All ";
            chkShowAll.CheckedChanged += chkShowAll_CheckedChanged;
            // 
            // txtModifier
            // 
            txtModifier.Location = new Point(245, 45);
            txtModifier.Name = "txtModifier";
            txtModifier.Size = new Size(80, 23);
            txtModifier.TabIndex = 5;
            txtModifier.TextChanged += txtModifier_TextChanged;
            // 
            // lblWeight
            //
            lblWeight.ForeColor = Color.Gray;
            lblWeight.Font = new Font(lblWeight.Font, FontStyle.Italic);
            lblWeight.Location = new Point(351, 73);
            lblWeight.Name = "lblWeight";
            lblWeight.Size = new Size(80, 23);
            lblWeight.TabIndex = 6;
            lblWeight.Text = "ⓘ Weight (g)";
            // 
            // txtWeightInput
            // 
            txtWeightInput.Location = new Point(351, 99);
            txtWeightInput.Name = "txtWeightInput";
            txtWeightInput.Size = new Size(105, 23);
            txtWeightInput.TabIndex = 7;
            txtWeightInput.TextChanged += txtWeightInput_TextChanged;
            // 
            // lblDiameter
            //
            lblDiameter.ForeColor = Color.Gray;
            lblDiameter.Font = new Font(lblDiameter.Font, FontStyle.Italic);
            lblDiameter.Location = new Point(245, 73);
            lblDiameter.Name = "lblDiameter";
            lblDiameter.Size = new Size(100, 23);
            lblDiameter.TabIndex = 8;
            lblDiameter.Text = "ⓘ Diameter";
            // 
            // txtDiameter
            // 
            txtDiameter.Location = new Point(245, 99);
            txtDiameter.Name = "txtDiameter";
            txtDiameter.Size = new Size(70, 23);
            txtDiameter.TabIndex = 9;
            txtDiameter.TextChanged += txtDiameter_TextChanged;
            // 
            // lblTemps
            //
            lblTemps.ForeColor = Color.Gray;
            lblTemps.Font = new Font(lblTemps.Font, FontStyle.Italic);
            lblTemps.Location = new Point(351, 19);
            lblTemps.Name = "lblTemps";
            lblTemps.Size = new Size(105, 23);
            lblTemps.TabIndex = 12;
            lblTemps.Text = "ⓘ Temp Range";
            // 
            // txtMinTemp
            // 
            txtMinTemp.Location = new Point(351, 45);
            txtMinTemp.Name = "txtMinTemp";
            txtMinTemp.Size = new Size(50, 23);
            txtMinTemp.TabIndex = 13;
            txtMinTemp.TextChanged += txtTemp_TextChanged;
            // 
            // txtMaxTemp
            // 
            txtMaxTemp.Location = new Point(406, 45);
            txtMaxTemp.Name = "txtMaxTemp";
            txtMaxTemp.Size = new Size(50, 23);
            txtMaxTemp.TabIndex = 14;
            txtMaxTemp.TextChanged += txtTemp_TextChanged;
            // 
            // lblColorPreview
            //
            lblColorPreview.AutoSize = true;
            lblColorPreview.Location = new Point(15, 73);
            lblColorPreview.Name = "lblColorPreview";
            lblColorPreview.Size = new Size(115, 15);
            lblColorPreview.TabIndex = 16;
            lblColorPreview.Text = "✓ Filament Color";
            // 
            // pnlColor
            // 
            pnlColor.BorderStyle = BorderStyle.FixedSingle;
            pnlColor.Cursor = Cursors.Hand;
            pnlColor.Location = new Point(15, 99);
            pnlColor.Name = "pnlColor";
            pnlColor.Size = new Size(105, 33);
            pnlColor.TabIndex = 16;
            pnlColor.Click += pnlColor_Click;
            pnlColor.MouseEnter += pnlColor_MouseEnter;
            pnlColor.MouseLeave += pnlColor_MouseLeave;
            // 
            // btnExportMobile
            // 
            btnExportMobile.Location = new Point(363, 12);
            btnExportMobile.Name = "btnExportMobile";
            btnExportMobile.Size = new Size(109, 23);
            btnExportMobile.TabIndex = 6;
            btnExportMobile.Text = "Export for Mobile";
            toolTip.SetToolTip(btnExportMobile, "Generate A2 commands for RFID Tools mobile app");
            btnExportMobile.Click += btnExportMobile_Click;
            // 
            // btnCopyCleanHex
            // 
            btnCopyCleanHex.Location = new Point(478, 12);
            btnCopyCleanHex.Name = "btnCopyCleanHex";
            btnCopyCleanHex.Size = new Size(102, 23);
            btnCopyCleanHex.TabIndex = 7;
            btnCopyCleanHex.Text = "Copy Clean Hex";
            toolTip.SetToolTip(btnCopyCleanHex, "Copy raw hex string for mobile NFC apps");
            btnCopyCleanHex.Click += btnCopyCleanHex_Click;
            // 
            // btnGenerateNew
            // 
            btnGenerateNew.Location = new Point(12, 12);
            btnGenerateNew.Name = "btnGenerateNew";
            btnGenerateNew.Size = new Size(88, 23);
            btnGenerateNew.TabIndex = 8;
            btnGenerateNew.Text = "Generate .BIN";
            toolTip.SetToolTip(btnGenerateNew, "Generate a new blank RFID tag with random UID");
            btnGenerateNew.Click += btnGenerateNew_Click;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus });
            statusStrip.Location = new Point(0, 621);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(585, 22);
            statusStrip.TabIndex = 9;
            statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(124, 17);
            lblStatus.Text = "Ready - No file loaded";
            // 
            // RFIDEditor
            // 
            ClientSize = new Size(585, 643);
            Controls.Add(statusStrip);
            Controls.Add(btnGenerateNew);
            Controls.Add(btnCopyCleanHex);
            Controls.Add(btnLoad);
            Controls.Add(btnSave);
            Controls.Add(btnFixBCC);
            Controls.Add(grpQuickEdit);
            Controls.Add(pageGrid);
            Controls.Add(btnExportMobile);
            Name = "RFIDEditor";
            Text = "Elegoo NTAG213 Spool Editor v2.1";
            ((System.ComponentModel.ISupportInitialize)pageGrid).EndInit();
            grpQuickEdit.ResumeLayout(false);
            grpQuickEdit.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private void chkShowAll_CheckedChanged(object sender, EventArgs e)
        {
            // If we are showing all, we unhide everything.
            // If not, we run the filter logic.
            ApplyGridFilter();
        }

        private void ApplyGridFilter()
        {
            _isUpdating = true;
            bool showAll = chkShowAll.Checked;

            foreach (DataGridViewRow row in pageGrid.Rows)
            {
                if (row.IsNewRow) continue;

                int offset = Convert.ToInt32(row.Cells["offset"].Value.ToString(), 16);
                string desc = GetFieldDescription(offset);

                // Define what counts as "Editable Spool Data"
                bool isSpoolData = !string.IsNullOrEmpty(desc) &&
                                   !desc.Contains("Gap") &&
                                   !desc.Contains("Space") &&
                                   !desc.Contains("System");

                // Set visibility based on the checkbox or the data type
                row.Visible = showAll || isSpoolData;

                // visual cue: light green background for editable fields
                if (isSpoolData) row.DefaultCellStyle.BackColor = Color.FromArgb(245, 255, 245);
                else row.DefaultCellStyle.BackColor = Color.White;
            }
            _isUpdating = false;
        }

        private ToolTip toolTip;
        private Button btnCopyCleanHex;
        private TextBox txtProdDate;
        private Label lblProdDate;
        private ToolTip dateTip;
        private Button btnGenerateNew;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;
        private Button btnSetCurrentDate;
        private Label lblManufacturer;
        private TextBox txtManufacturer;
    }
}