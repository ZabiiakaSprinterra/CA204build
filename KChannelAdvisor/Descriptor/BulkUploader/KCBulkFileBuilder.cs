using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace KChannelAdvisor.Descriptor.BulkUploader
{
    internal class KCBulkFileBuilder
    {
        private string _separator = "\t";
        private DataTable _table;

        public KCBulkFileBuilder()
        {
            _table = new DataTable();
        }

        public void AddRow(Dictionary<string, string> row)
        {
            if (!row.ContainsKey(KCHeaders.InventoryNumber))
            {
                throw new ArgumentException("InventoryNumber must be provided");
            }

            DataRow dtRow = _table.NewRow();
            foreach (KeyValuePair<string, string> item in row)
            {
                if (!_table.Columns.Contains(item.Key))
                {
                    _table.Columns.Add(item.Key);
                }

                dtRow[item.Key] = item.Value;
            }

            _table.Rows.Add(dtRow);
        }

        public string Build()
        {
            StringBuilder builder = new StringBuilder();

            foreach (DataColumn column in _table.Columns)
            {
                builder.Append(column.ColumnName + _separator);
            }

            //remove trailing separator
            builder.Length = builder.Length - _separator.Length;
            builder.AppendLine();

            for (int i = 0; i < _table.Rows.Count; i++)
            {
                DataRow myRow = _table.Rows[i];
                for (int j = 0; j < _table.Columns.Count; j++)
                {
                    builder.Append(myRow.ItemArray[j] + _separator);
                }

                //remove trailing separator
                builder.Length = builder.Length - _separator.Length;
                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}
