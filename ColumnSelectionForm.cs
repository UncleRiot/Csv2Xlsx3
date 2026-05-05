using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Csv2Xlsx3
{
    public partial class ColumnSelectionForm : Form
    {
        public List<string> SelectedColumns { get; private set; } = new List<string>();

        public ColumnSelectionForm(List<string> columns, DataTable previewTable)
        {
            InitializeComponent();

            foreach (var column in columns)
            {
                checkedListBoxColumns.Items.Add(column, true);
            }

            if (previewTable != null)
            {
                dataGridViewPreview.DataSource = previewTable;
            }

            ModernFormStyler.Apply(this);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            SelectedColumns = checkedListBoxColumns.CheckedItems.Cast<string>().ToList();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}