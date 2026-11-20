using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using MyFolderInfo.Model;

namespace MyFolderInfo.UI
{
    /// <summary>Colour-theme editor.</summary>
    /// <remarks>
    /// Rewritten off <see cref="ThemeStore"/>. The original kept the palette in a
    /// <see cref="System.Data.DataSet"/> keyed by row <i>position</i>: it looked a value up by
    /// scanning <c>Rows[i][0]</c> for the name but then read <c>treeView1.Nodes[0].Nodes[0]</c> and
    /// <c>listView1.Items[1]</c> unguarded. Those indexes only exist because the designer
    /// hard-codes a placeholder row, so the form threw <c>NullReferenceException</c> if the
    /// designer ever changed - and it rewrote Config.xml in place on every preset button.
    /// </remarks>
    public partial class FrmColor : ParentForm
    {
        private readonly Dictionary<string, ThemeEntry> _entries = new(StringComparer.OrdinalIgnoreCase);
        private ClassLanguage _clsLan;

        public FrmColor()
        {
            InitializeComponent();
        }

        private void frmMausac_Load(object sender, EventArgs e)
        {
            _clsLan = new ClassLanguage(this, "frmColor");

            LoadEntries();
            ApplyCurrentColoursToPreviews();
        }

        private void LoadEntries()
        {
            _entries.Clear();

            foreach (ThemeEntry entry in ThemeStore.Load())
            {
                _entries[entry.Name] = entry;
            }
        }

        /// <summary>Pushes the loaded palette into the designer preview widgets.</summary>
        private void ApplyCurrentColoursToPreviews()
        {
            treeView1.BackColor = GetColor("MauNenTV");
            treeView1.Nodes[0].ForeColor = GetColor("MauChuTV");
            treeView1.Nodes[0].Nodes[0].BackColor = GetColor("MauNodeSelect");
            treeView1.Nodes[0].Nodes[0].ForeColor = GetColor("MauChuTV");

            listView1.Items[0].BackColor = GetColor("MauNenLV");
            listView1.Items[0].ForeColor = GetColor("MauChuLV");
            listView1.Items[1].BackColor = GetColor("MauNenLV2");
            listView1.Items[1].ForeColor = GetColor("MauChuLV");

            BackColor = GetColor("MauNenForm");
        }

        #region Preview widgets

        private void PickBackColor(Control preview, Control sample)
        {
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                preview.BackColor = colorDialog1.Color;
                sample.BackColor = colorDialog1.Color;
            }
        }

        private void PickForeColor(Control preview, Control sample)
        {
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                preview.ForeColor = colorDialog1.Color;
                sample.ForeColor = colorDialog1.Color;
            }
        }

        private void btMauNenButton_Click(object sender, EventArgs e) => PickBackColor(lbNenNut, cmdMauThu);

        private void btChonMauChuBT_Click(object sender, EventArgs e) => PickForeColor(lbMauChuBT, cmdMauThu);

        private void btChonNenLB_Click(object sender, EventArgs e) => PickBackColor(lbMauNenLB, lblMauThu);

        private void btChonMauChuLB_Click(object sender, EventArgs e) => PickForeColor(lbMauChuLB, lblMauThu);

        private void btMauNenTB_Click(object sender, EventArgs e) => PickBackColor(lbMauNenTB, txtMauThu);

        private void btMauChuTB_Click(object sender, EventArgs e) => PickForeColor(lbMauChuTB, txtMauThu);

        private void btChonMauNenForm_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                BackColor = colorDialog1.Color;
            }
        }

        private void cmdTreview_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                treeView1.BackColor = colorDialog1.Color;
            }
        }

        private void cmdTreeviewText_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                treeView1.Nodes[0].ForeColor = colorDialog1.Color;
            }
        }

        private void cmdNodeSelect_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                treeView1.Nodes[0].Nodes[0].BackColor = colorDialog1.Color;
            }
        }

        private void cmdLV1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                listView1.Items[0].BackColor = colorDialog1.Color;
            }
        }

        private void cmdLV2_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                listView1.Items[1].BackColor = colorDialog1.Color;
            }
        }

        private void cmdLVText_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                listView1.Items[0].ForeColor = colorDialog1.Color;
                listView1.Items[1].ForeColor = colorDialog1.Color;
            }
        }

        #endregion

        #region Save / presets

        /// <summary>
        /// Reads the live preview widgets back into the entry list. The original called
        /// <c>listView1.Items[0]</c> and <c>treeView1.Nodes[0].Nodes[0]</c> with no guard; if the
        /// designer placeholder row were ever removed this became a hard crash on Save.
        /// </summary>
        private void CollectFromPreviews()
        {
            Set("MauChuBT", cmdMauThu.ForeColor);
            Set("MauChuLB", lblMauThu.ForeColor);
            Set("MauChuTB", txtMauThu.ForeColor);
            Set("MauChuTV", treeView1.Nodes.Count > 0 ? treeView1.Nodes[0].ForeColor : System.Drawing.Color.Black);
            Set("MauChuLV", listView1.Items.Count > 0 ? listView1.Items[0].ForeColor : System.Drawing.Color.Black);

            Set("MauNenBT", cmdMauThu.BackColor);
            Set("MauNenForm", BackColor);
            Set("MauNenLB", lblMauThu.BackColor);
            Set("MauNenTB", txtMauThu.BackColor);
            Set("MauNenTV", treeView1.BackColor);

            Set("MauNodeSelect",
                treeView1.Nodes.Count > 0 && treeView1.Nodes[0].Nodes.Count > 0
                    ? treeView1.Nodes[0].Nodes[0].BackColor
                    : System.Drawing.Color.LightCyan);

            Set("MauNenLV", listView1.Items.Count > 0 ? listView1.Items[0].BackColor : System.Drawing.Color.WhiteSmoke);
            Set("MauNenLV2", listView1.Items.Count > 1 ? listView1.Items[1].BackColor : System.Drawing.Color.White);
        }

        private void Set(string key, System.Drawing.Color color)
        {
            if (!_entries.TryGetValue(key, out ThemeEntry entry))
            {
                entry = new ThemeEntry { Name = key };
                _entries[key] = entry;
            }

            // Config.xml stores signed ARGB integers.
            entry.Value = color.ToArgb().ToString(CultureInfo.InvariantCulture);
        }

        private void SaveEntries()
        {
            ThemeStore.Save(_entries.Values);

            // Fully qualified: inside a derived class the simple name "ParentForm" binds to the
            // inherited base type rather than the type itself, so the unqualified call failed
            // to resolve the static member.
            MyFolderInfo.UI.ParentForm.InvalidateTheme();
        }

        private void btLuu_Click(object sender, EventArgs e)
        {
            CollectFromPreviews();
            SaveEntries();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void cmdSetDefault2_Click(object sender, EventArgs e)
        {
            // "Set as default" freezes the current palette as the first restore preset.
            CollectFromPreviews();

            foreach (string key in ThemeStore.KnownKeys)
            {
                if (_entries.TryGetValue(key, out ThemeEntry entry))
                {
                    entry.Default = entry.Value;
                }
            }
        }

        private void btGTMD_Click(object sender, EventArgs e) => ApplyPreset(useSecondPreset: false);

        private void btGTMD2_Click(object sender, EventArgs e) => ApplyPreset(useSecondPreset: true);

        private void ApplyPreset(bool useSecondPreset)
        {
            foreach (string key in ThemeStore.KnownKeys)
            {
                if (!_entries.TryGetValue(key, out ThemeEntry entry))
                {
                    continue;
                }

                string preset = useSecondPreset ? entry.Default2 : entry.Default;
                if (preset.Length > 0)
                {
                    entry.Value = preset;
                }
            }

            SaveEntries();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btDong_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
    }
}
