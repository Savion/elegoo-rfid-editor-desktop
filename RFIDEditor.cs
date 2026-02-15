using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RFIDEditor
{
    public partial class RFIDEditor : Form
    {
        private byte[] _rawData = new byte[180];
        private ElegooSpool _spool;
        private bool _isUpdating = false;

        private string? _currentFilePath;

        public RFIDEditor()
        {
            InitializeComponent();

            // --- Modern Navigation Logic (Put this here!) ---
            // This makes the grid jump to the byte you are editing
            txtWeightInput.Enter += (s, e) => FocusGridToOffset(0x5E);
            comboSubtype.Enter += (s, e) => FocusGridToOffset(0x4C);
            txtDiameter.Enter += (s, e) => FocusGridToOffset(0x5C);
            txtModifier.Enter += (s, e) => FocusGridToOffset(0x53);
            txtMinTemp.Enter += (s, e) => FocusGridToOffset(0x54);
            txtMaxTemp.Enter += (s, e) => FocusGridToOffset(0x56);
            txtProdDate.Enter += (s, e) => FocusGridToOffset(0x60);

            // Input validation visual feedback
            txtWeightInput.TextChanged += ValidateNumericInput;
            txtDiameter.TextChanged += ValidateNumericInput;
            txtMinTemp.TextChanged += ValidateNumericInput;
            txtMaxTemp.TextChanged += ValidateNumericInput;
            txtProdDate.TextChanged += ValidateDateInput;

            // Enhanced tooltips with usage indicators
            toolTip.SetToolTip(comboMaterial, "✓ Material type - READ BY PRINTER");
            toolTip.SetToolTip(comboSubtype, "✓ Material subtype/variant - READ BY PRINTER");
            toolTip.SetToolTip(pnlColor, "✓ Click to change filament color - READ BY PRINTER");
            toolTip.SetToolTip(txtWeightInput, "⚠ Metadata only - Not currently used by printer\nFilament weight in grams (e.g., 1000)");
            toolTip.SetToolTip(txtDiameter, "⚠ Metadata only - Not currently used by printer\nDiameter × 100 (e.g., 175 for 1.75mm)");
            toolTip.SetToolTip(txtMinTemp, "⚠ Metadata only - Not currently used by printer\nMinimum nozzle temperature in °C");
            toolTip.SetToolTip(txtMaxTemp, "⚠ Metadata only - Not currently used by printer\nMaximum nozzle temperature in °C");
            toolTip.SetToolTip(txtModifier, "⚠ Metadata only - Not currently used by printer\nColor modifier: L=Light, M=Medium, D=Dark");
            toolTip.SetToolTip(txtProdDate, "⚠ Metadata only - Not currently used by printer\nEnter date as YYMM (e.g., 2602 for Feb 2026)");
            toolTip.SetToolTip(btnSetCurrentDate, "⚠ Metadata only - Not currently used by printer\nSet production date to current month");
        }
        private void InitializeSpoolData()
        {
            _spool = new ElegooSpool(_rawData);
            _rawData = _spool.RawData; // Capture the resized/cloned array reference
            SetupGrid();
            RefreshGrid();
            PopulateSubtypeDropdown();
            UpdateSummary();
        }

        private void PopulateSubtypeDropdown()
        {
            if (_spool == null) return;

            _isUpdating = true;
            string currentSubtype = _spool.MaterialSubtype;
            comboSubtype.Items.Clear();

            foreach (var subtype in ElegooSpool.GetSubtypesForMaterial(_spool.Material))
            {
                comboSubtype.Items.Add(subtype);
            }

            comboSubtype.Text = currentSubtype;
            _isUpdating = false;
        }
        private void SetupGrid()
        {
            pageGrid.Columns.Clear();
            pageGrid.Columns.Add("offset", "Offset");
            pageGrid.Columns.Add("description", "Field Description");
            pageGrid.Columns.Add("hexValue", "Hex (Byte)");
            pageGrid.Columns.Add("decValue", "Decimal");
            pageGrid.Columns.Add("ascii", "ASCII");

            pageGrid.Columns["offset"].ReadOnly = true;
            pageGrid.Columns["offset"].DefaultCellStyle.BackColor = Color.LightGray;
            pageGrid.Columns["offset"].Width = 60;
            pageGrid.Columns["description"].Width = 140;
            pageGrid.Columns["description"].ReadOnly = true;

            pageGrid.AllowUserToAddRows = false;
            pageGrid.RowHeadersVisible = true;
        }

        private void RefreshGrid()
        {
            if (_rawData == null || _rawData.Length == 0) return;

            _isUpdating = true;
            pageGrid.Rows.Clear();

            // Check the toggle state from our modern UI
            bool showAll = chkShowAll.Checked;

            for (int i = 0; i < _rawData.Length; i++)
            {
                int rowIndex = pageGrid.Rows.Add();
                DataGridViewRow row = pageGrid.Rows[rowIndex];
                string desc = GetFieldDescription(i);

                // 1. Basic Columns
                row.Cells["offset"].Value = $"0x{i:X2}";
                row.Cells["hexValue"].Value = _rawData[i].ToString("X2");
                row.Cells["description"].Value = desc;
                if (IsStartOf16BitField(i))
                {
                    int combined = (_rawData[i] << 8) | _rawData[i + 1];
                    row.Cells["decValue"].Value = combined.ToString();

                    // Visual cue that this is a 16-bit combined value
                    row.Cells["decValue"].Style.BackColor = Color.FromArgb(240, 248, 255); // AliceBlue
                    row.Cells["decValue"].Style.Font = new Font(pageGrid.Font, FontStyle.Bold);
                }
                else if (IsSecondByteOf16BitField(i))
                {
                    // Make the second byte row look "linked" to the one above it
                    row.Cells["decValue"].Value = "-";
                    row.Cells["decValue"].Style.ForeColor = Color.LightGray;
                    row.Cells["decValue"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else
                {
                    // Standard single-byte decimal
                    row.Cells["decValue"].Value = _rawData[i].ToString();
                }

                // 3. ASCII Preview Column
                byte b = _rawData[i];
                row.Cells["ascii"].Value = (b > 31 && b < 127) ? ((char)b).ToString() : ".";

                // 4. Visibility Filter
                // Logic: Hide if it's a Gap/System row AND "Show All" is unchecked
                bool isSpoolData = !string.IsNullOrEmpty(desc) &&
                                   !desc.Contains("Gap") &&
                                   !desc.Contains("Space") &&
                                   !desc.Contains("System");

                row.Visible = showAll || isSpoolData;

                // 5. Visual Styling for Editable Spool Data
                if (isSpoolData)
                {
                    row.Cells["description"].Style.Font = new Font(pageGrid.Font, FontStyle.Italic);

                    // Color code important field types
                    if (i >= 0x45 && i <= 0x46) // Filament Code
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 240); // Light yellow
                    else if (i >= 0x48 && i <= 0x4B) // Material Name
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 250, 240); // Light orange
                    else if (i >= 0x4C && i <= 0x4D) // Material Supplement
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 250, 230); // Lighter orange
                    else if (i >= 0x50 && i <= 0x52) // RGB Color
                        row.DefaultCellStyle.BackColor = Color.FromArgb(240, 255, 240); // Light green
                    else if ((i >= 0x54 && i <= 0x57) || (i >= 0x5C && i <= 0x5F)) // Temps, Diameter, Weight
                        row.DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255); // Light blue
                    else if (i >= 0x60 && i <= 0x61) // Production Date
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 245, 255); // Light purple
                    else if (!showAll)
                        row.DefaultCellStyle.BackColor = Color.FromArgb(252, 252, 252);
                }
            }

            _isUpdating = false;
        }

        private string GetFieldDescription(int i)
        {
            if (i < 0x09) return "UID / Serial";
            if (i == 0x09) return "BCC1 Checksum";
            if (i == 0x27) return "Marketing Gap";
            if (i == 0x6A) return "Unused Space";

            // Return blank for collapsible rows
            if (i > 0x27 && i <= 0x3F) return "";
            if (i > 0x6A && i <= 0x9F) return "";

            if (i >= 0x1C && i <= 0x25) return "Marketing URL";
            if (i == 0x40) return "Header";
            if (i >= 0x41 && i <= 0x44) return "Manufacturer Code";
            if (i >= 0x45 && i <= 0x46) return "Filament Code";
            if (i >= 0x48 && i <= 0x4B) return "Material Name";
            if (i >= 0x4C && i <= 0x4D) return "Material Supplement";
            if (i >= 0x50 && i <= 0x52) return "Color Code";
            if (i == 0x53) return "Color Modifier";
            if (i == 0x54 || i == 0x55) return "Min Temp";
            if (i == 0x56 || i == 0x57) return "Max Temp";
            if (i == 0x5C || i == 0x5D) return "Filament Diameter";
            if (i == 0x5E || i == 0x5F) return "Filament Weight";
            if (i == 0x60 || i == 0x61) return "Production Date";
            if (i >= 0xA0 && i <= 0xAF) return "Config/Password";

            return "Reserved/Data";
        }

        private void UpdateSummary()
        {
            if (_spool == null || _rawData == null || _rawData.Length < 0x62) return;

            _isUpdating = true; // Prevent TextChanged events from firing and causing loops

            try
            {
                // 1. Material & Subtype
                comboMaterial.Text = _spool.Material;
                comboSubtype.Text = _spool.MaterialSubtype;

                // 2. 16-bit Big-Endian Reconstruction (The "280" fix)
                // These use the logic: (HighByte << 8) | LowByte
                txtMinTemp.Text = ((_rawData[0x54] << 8) | _rawData[0x55]).ToString();
                txtMaxTemp.Text = ((_rawData[0x56] << 8) | _rawData[0x57]).ToString();
                txtDiameter.Text = ((_rawData[0x5C] << 8) | _rawData[0x5D]).ToString();
                txtWeightInput.Text = ((_rawData[0x5E] << 8) | _rawData[0x5F]).ToString();

                // 3. Production Date (hex-BCD YYMM format)
                txtProdDate.Text = _rawData[0x60].ToString("X2") + _rawData[0x61].ToString("X2");

                // 4. Single Byte Values
                txtModifier.Text = _rawData[0x53].ToString();

                // 5. Manufacturer Check
                // Check if 0x41-0x44 equals 0xEEEEEEEE (Elegoo)
                uint mfgCode = (uint)((_rawData[0x41] << 24) | (_rawData[0x42] << 16) | (_rawData[0x43] << 8) | _rawData[0x44]);
                if (mfgCode == 0xEEEEEEEE)
                {
                    txtManufacturer.Text = "ELEGOO";
                    txtManufacturer.BackColor = Color.FromArgb(240, 240, 240);
                }
                else if (mfgCode == 0x00000000 || mfgCode == 0xFFFFFFFF)
                {
                    txtManufacturer.Text = "--";
                    txtManufacturer.BackColor = Color.FromArgb(255, 240, 240); // Light red
                }
                else
                {
                    txtManufacturer.Text = "Generic";
                    txtManufacturer.BackColor = Color.FromArgb(255, 250, 220); // Light yellow
                }

                // 6. Color Panel Sync
                // We pull directly from 0x50(R), 0x51(G), 0x52(B)
                pnlColor.BackColor = Color.FromArgb(_rawData[0x50], _rawData[0x51], _rawData[0x52]);
            }
            catch (Exception ex)
            {
                // Fallback if data is malformed
                pnlColor.BackColor = Color.Gray;
                Console.WriteLine("Summary Update Error: " + ex.Message);
            }

            _isUpdating = false;
        }

        private void CollapseAllEmptySections()
        {
            _isUpdating = true;
            pageGrid.CurrentCell = null;

            for (int i = 0; i < pageGrid.Rows.Count; i++)
            {
                if (pageGrid.Rows[i].IsNewRow) continue;
                string fieldDesc = GetFieldDescription(i);

                if (string.IsNullOrEmpty(fieldDesc))
                {
                    pageGrid.Rows[i].Visible = false;
                }
                else if (i == 0x27 || i == 0x6A)
                {
                    pageGrid.Rows[i].Cells["description"].Value = "[+] " + fieldDesc;
                }
            }
            _isUpdating = false;
        }

        // --- Event Handlers ---

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Binary Files|*.bin|All Files|*.*" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _rawData = File.ReadAllBytes(ofd.FileName);
                        if (_rawData.Length < 180) Array.Resize(ref _rawData, 180);
                        _spool = new ElegooSpool(_rawData);
                        RefreshGrid();
                        PopulateSubtypeDropdown();
                        UpdateSummary();
                        CollapseAllEmptySections();

                        _currentFilePath = ofd.FileName;
                        string fileName = Path.GetFileName(ofd.FileName);
                        UpdateStatusBar($"Loaded: {fileName} ({_rawData.Length} bytes)");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading file: {ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UpdateStatusBar("Error loading file");
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Binary Files|*.bin",
                FileName = _currentFilePath != null ? Path.GetFileName(_currentFilePath) : "spool.bin"
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllBytes(sfd.FileName, _rawData);
                        _currentFilePath = sfd.FileName;
                        string fileName = Path.GetFileName(sfd.FileName);
                        UpdateStatusBar($"Saved: {fileName} ({_rawData.Length} bytes)");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UpdateStatusBar("Error saving file");
                    }
                }
            }
        }

        private void btnFixBCC_Click(object sender, EventArgs e)
        {
            if (_spool == null)
            {
                MessageBox.Show("Please load or generate a file first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _spool.FixChecksums();
            SyncRow(0x08);
            UpdateStatusBar("BCC1 checksum recalculated");
            MessageBox.Show("BCC1 Checksum recalculated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void btnPickColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    _rawData[0x50] = cd.Color.R;
                    _rawData[0x51] = cd.Color.G;
                    _rawData[0x52] = cd.Color.B;
                    SyncRow(0x50); SyncRow(0x51); SyncRow(0x52);
                    UpdateSummary();
                }
            }
        }

        // --- Reactive TextChanged Sync ---

        private void txtWeightInput_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdating || _spool == null) return;
            if (int.TryParse(txtWeightInput.Text, out int val))
            {
                _spool.Weight = val;
                SyncRow(0x5E); SyncRow(0x5F);
            }
        }

        private void txtDiameter_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdating || _spool == null) return;
            if (int.TryParse(txtDiameter.Text, out int val))
            {
                _spool.Diameter = val;
                SyncRow(0x5C); SyncRow(0x5D);
            }
        }
        private void txtModifier_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdating || _spool == null || txtModifier.Text.Length == 0) return;
            _spool.ColorModifier = txtModifier.Text[0];
            SyncRow(0x53);
        }

        private void comboSubtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdating || _spool == null) return;

            _spool.MaterialSubtype = comboSubtype.Text;
            for (int i = 0x4C; i <= 0x4D; i++) SyncRow(i);
        }


        private void txtTemp_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdating || _spool == null) return;
            if (int.TryParse(txtMinTemp.Text, out int min) && int.TryParse(txtMaxTemp.Text, out int max))
            {
                _spool.MinTemp = min;
                _spool.MaxTemp = max;
                SyncRow(0x54); SyncRow(0x55); SyncRow(0x56); SyncRow(0x57);
            }
        }

        // --- Helper Methods ---
        private void UpdateGridFromData()
        {
            _isUpdating = true;
            RefreshGrid();
            _isUpdating = false;
        }
        private void SyncRow(int i)
        {
            if (i < 0 || i >= pageGrid.Rows.Count) return;
            _isUpdating = true;
            var row = pageGrid.Rows[i];

            row.Cells["hexValue"].Value = _rawData[i].ToString("X2");

            if (IsTextField(i))
            {
                char c = (char)_rawData[i];
                row.Cells["decValue"].Value = (c > 31 && c < 127) ? $"'{c}'" : _rawData[i].ToString();
            }
            else if (IsStartOf16BitField(i))
            {
                row.Cells["decValue"].Value = ((_rawData[i] << 8) | _rawData[i + 1]).ToString();
            }
            else if (IsSecondByteOf16BitField(i))
            {
                pageGrid.Rows[i - 1].Cells["decValue"].Value = ((_rawData[i - 1] << 8) | _rawData[i]).ToString();
            }
            else
            {
                row.Cells["decValue"].Value = _rawData[i].ToString();
            }

            SetDecimalCellValue(i);
            if (IsStartOf16BitField(i)) SetDecimalCellValue(i + 1);
            if (IsSecondByteOf16BitField(i)) SetDecimalCellValue(i - 1);
            _isUpdating = false;
        }

        private void FocusGridToOffset(int offset)
        {
            if (offset >= 0 && offset < pageGrid.Rows.Count)
            {
                pageGrid.FirstDisplayedScrollingRowIndex = offset;
                pageGrid.ClearSelection();
                pageGrid.Rows[offset].Selected = true;
                pageGrid.CurrentCell = pageGrid.Rows[offset].Cells["hexValue"];
            }
        }

        private bool IsStartOf16BitField(int i)
        {
            // Offsets for the High Byte of Big-Endian pairs
            return i == 0x54 || // Min Temp
                   i == 0x56 || // Max Temp
                   i == 0x5C || // Diameter
                   i == 0x5E || // Weight
                   i == 0x60;   // Prod Date
        }

        private bool IsSecondByteOf16BitField(int i)
        {
            // Offsets for the Low Byte of Big-Endian pairs
            return i == 0x55 || // Min Temp
                   i == 0x57 || // Max Temp
                   i == 0x5D || // Diameter
                   i == 0x5F || // Weight
                   i == 0x61;   // Prod Date
        }

        private bool IsTextField(int i)
        {
            // Marketing URL, Manufacturer, Material Name, and Supplement are ASCII
            return (i >= 0x1C && i <= 0x25) ||
                   (i >= 0x41 && i <= 0x44) ||
                   (i >= 0x48 && i <= 0x4B) ||
                   (i >= 0x4C && i <= 0x4D);
        }

        private void comboMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdating || _spool == null) return;

            // 1. Update the byte array via the setter
            _spool.Material = comboMaterial.Text;

            // 2. Sync the specific rows in the grid (0x48 to 0x4B)
            for (int i = 0x48; i <= 0x4B; i++) SyncRow(i);

            // 3. Populate subtype dropdown based on selected material
            _isUpdating = true;
            string currentSubtype = comboSubtype.Text;
            comboSubtype.Items.Clear();

            foreach (var subtype in ElegooSpool.GetSubtypesForMaterial(comboMaterial.Text))
            {
                comboSubtype.Items.Add(subtype);
            }

            // Try to restore previous subtype if it exists in new list
            if (comboSubtype.Items.Contains(currentSubtype))
            {
                comboSubtype.Text = currentSubtype;
            }
            else if (comboSubtype.Items.Count > 0)
            {
                comboSubtype.SelectedIndex = 0; // Select first item as default
            }
            _isUpdating = false;
        }
        private void pnlColor_MouseEnter(object sender, EventArgs e)
        {
            pnlColor.BorderStyle = BorderStyle.Fixed3D;
        }

        // Hover effect: Reset border to FixedSingle
        private void pnlColor_MouseLeave(object sender, EventArgs e)
        {
            pnlColor.BorderStyle = BorderStyle.FixedSingle;
        }

        // The click event to open the picker
        private void pnlColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = pnlColor.BackColor;

                if (cd.ShowDialog() == DialogResult.OK)
                {
                    _rawData[0x50] = cd.Color.R;
                    _rawData[0x51] = cd.Color.G;
                    _rawData[0x52] = cd.Color.B;

                    SyncRow(0x50);
                    SyncRow(0x51);
                    SyncRow(0x52);
                    FocusGridToOffset(0x50);
                    UpdateSummary();
                }
            }
        }

        private void btnExportMobile_Click(object sender, EventArgs e)
        {
            if (_rawData == null)
            {
                MessageBox.Show("Please load or generate a file first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<string> commands = new List<string>();

            // We start from Page 04 (0x04) as per your batch requirements
            // Page 44 is the typical end of the NTAG213 user area (180 bytes)
            for (int page = 4; page <= 44; page++)
            {
                int startIndex = page * 4;
                if (startIndex + 3 >= _rawData.Length) break;

                // Get the 4 bytes for this page
                string p0 = _rawData[startIndex].ToString("X2");
                string p1 = _rawData[startIndex + 1].ToString("X2");
                string p2 = _rawData[startIndex + 2].ToString("X2");
                string p3 = _rawData[startIndex + 3].ToString("X2");

                // Format: A2:{PageHex}:{DataHex}
                // Note: Page is hex, so Page 10 is 0A
                commands.Add($"A2:{page:X2}:{p0}{p1}{p2}{p3}");
            }

            string finalExport = string.Join(", ", commands);

            // Copy to clipboard immediately for mobile pasting
            Clipboard.SetText(finalExport);

            UpdateStatusBar($"Mobile commands copied to clipboard ({commands.Count} pages)");

            // Show a confirmation/preview dialog
            MessageBox.Show("Commands generated and copied to clipboard!\n\n" +
                            "In RFID Tools:\n" +
                            "1. Go to Other\n" +
                            "2. Go to Advanced RFID Commands\n" +
                            "3. Paste this string in Data:\n" +
                            "4. Click Send command and then Send.", "Mobile Export Ready", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCopyCleanHex_Click(object sender, EventArgs e)
        {
            if (_rawData == null)
            {
                MessageBox.Show("Please load or generate a file first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // No spaces, no newlines - just one long string of Hex
            // This is what most "Custom Payload" fields in mobile apps want
            string cleanHex = BitConverter.ToString(_rawData).Replace("-", "");
            Clipboard.SetText(cleanHex);

            UpdateStatusBar($"Clean hex copied to clipboard ({cleanHex.Length} characters)");
            MessageBox.Show("Clean Hex copied! In RFID Tools, look for 'Write Raw' or 'Custom Payload'.", "Ready to Paste", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pageGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_isUpdating) return;
            int offset = Convert.ToInt32(pageGrid.Rows[e.RowIndex].Cells["offset"].Value.ToString(), 16);
            string col = pageGrid.Columns[e.ColumnIndex].Name;
            string val = pageGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

            if (col == "decValue" && IsStartOf16BitField(offset))
            {
                if (int.TryParse(val, out int combined))
                {
                    _rawData[offset] = (byte)(combined >> 8);     // High Byte
                    _rawData[offset + 1] = (byte)(combined & 0xFF); // Low Byte
                }
            }
            else if (col == "hexValue")
            {
                _rawData[offset] = Convert.ToByte(val, 16);
            }

            SyncRow(offset);
            if (IsStartOf16BitField(offset)) SyncRow(offset + 1);
            UpdateSummary();
        }
        private void SetDecimalCellValue(int i)
        {
            if (i < 0 || i >= pageGrid.Rows.Count) return;
            var row = pageGrid.Rows[i];

            // Handle 16-bit combined fields (Temps, Weight, etc.)
            if (IsStartOf16BitField(i))
            {
                // (High << 8) | Low
                int combined = (_rawData[i] << 8) | _rawData[i + 1];
                row.Cells["decValue"].Value = combined.ToString();
                row.Cells["decValue"].Style.Font = new Font(pageGrid.Font, FontStyle.Bold);
            }
            else if (IsSecondByteOf16BitField(i))
            {
                // This is the "Low" part of a 16-bit value (like the second '190')
                row.Cells["decValue"].Value = "�";
                row.Cells["decValue"].Style.ForeColor = Color.Gray;
            }
            else
            {
                // Normal single bytes (like your Production Date 0x60 and 0x61)
                row.Cells["decValue"].Value = _rawData[i].ToString();
                row.Cells["decValue"].Style.Font = pageGrid.Font;
                row.Cells["decValue"].Style.ForeColor = Color.Black;
            }
        }

        private void txtProdDate_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdating || _spool == null) return;

            // Expecting "2502" format
            if (txtProdDate.Text.Length == 4 && int.TryParse(txtProdDate.Text, out _))
            {
                _spool.ProductionDate = txtProdDate.Text;
                SyncRow(0x60);
                SyncRow(0x61);
            }
        }

        private void btnGenerateNew_Click(object sender, EventArgs e)
        {
            // 1. Reset to Base if you want a clean slate, or just randomize identity
            InitializeSpoolData();
            _spool.GenerateNewIdentity();

            // 2. Refresh UI
            RefreshGrid();
            UpdateSummary();

            _currentFilePath = null;
            UpdateStatusBar("New RFID tag generated with random UID");
        }

        private void btnSetCurrentDate_Click(object sender, EventArgs e)
        {
            if (_spool == null) return;

            // Get current date in YYMM format
            DateTime now = DateTime.Now;
            int year = now.Year - 2000; // Year offset from 2000
            int month = now.Month;
            string dateStr = $"{year:D2}{month:D2}";

            txtProdDate.Text = dateStr;
            UpdateStatusBar($"Production date set to {now:MMMM yyyy}");
        }

        private void ValidateNumericInput(object? sender, EventArgs e)
        {
            if (sender is not TextBox txt || _isUpdating) return;

            if (string.IsNullOrEmpty(txt.Text))
            {
                txt.BackColor = Color.White;
                return;
            }

            if (int.TryParse(txt.Text, out int value))
            {
                // Check reasonable ranges
                bool valid = txt.Name switch
                {
                    "txtWeightInput" => value >= 0 && value <= 10000,
                    "txtDiameter" => value >= 100 && value <= 500,
                    "txtMinTemp" => value >= 0 && value <= 400,
                    "txtMaxTemp" => value >= 0 && value <= 400,
                    _ => true
                };

                txt.BackColor = valid ? Color.FromArgb(240, 255, 240) : Color.FromArgb(255, 240, 240);
            }
            else
            {
                txt.BackColor = Color.FromArgb(255, 240, 240);
            }
        }

        private void ValidateDateInput(object? sender, EventArgs e)
        {
            if (sender is not TextBox txt || _isUpdating) return;

            if (string.IsNullOrEmpty(txt.Text))
            {
                txt.BackColor = Color.White;
                return;
            }

            bool valid = false;
            if (txt.Text.Length == 4 && int.TryParse(txt.Text, out int date))
            {
                int year = date / 100;
                int month = date % 100;
                valid = year >= 0 && year <= 99 && month >= 1 && month <= 12;
            }

            txt.BackColor = valid ? Color.FromArgb(240, 255, 240) : Color.FromArgb(255, 240, 240);
        }

        private void UpdateStatusBar(string message)
        {
            if (lblStatus != null)
            {
                lblStatus.Text = message;
            }
        }
    }
}