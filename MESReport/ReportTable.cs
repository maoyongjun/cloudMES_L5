using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Common;

namespace MESReport
{
    public class ReportTable : ReportOutputBase
    {
        public String OutputType
        {
            get
            {
                return "ReportTable";
            }
        }
        public string Tittle = "";
        public List<TableRowView> Rows = new List<TableRowView>();
        public List<string> ColNames = new List<string>();

        public void LoadData(System.Data.DataTable DataT, System.Data.DataTable DataL)
        {
            ColNames.Clear();
            for (int i = 0; i < DataT.Columns.Count; i++)
            {
                ColNames.Add(DataT.Columns[i].ColumnName);
            }
            Rows.Clear();
            for (int i = 0; i < DataT.Rows.Count; i++)
            {
                TableRowView row = new TableRowView();
                Rows.Add(row);
                for (int j = 0; j < ColNames.Count; j++)
                {
                    
                    TableColView Item = new TableColView() { Value = DataT.Rows[i][j].ToString() };
                    if(DataL != null)
                    {
                        string[] LinkDatas = DataL.Rows[i][j].ToString().Split('#');
                        if (LinkDatas.Length > 1)
                            Item.LinkData = LinkDatas[1];
                        else
                            Item.LinkData = LinkDatas[0];
                        if (DataL.Rows[i][j].ToString() != "" && LinkDatas[0].Equals("Link"))
                            Item.LinkType = "Link";
                        else if (DataL.Rows[i][j].ToString() != "" && LinkDatas[0].Equals("Report"))
                            Item.LinkType = "Report";
                        else if (DataL.Rows[i][j].ToString() != "")
                            Item.LinkType = "Report";
                    }
                    row.Add(ColNames[j], Item);
                }
            }
        }
    }

    public class TableColView
    {
        //public string ColName;
        public string Value;
        public string LinkType;
        //public string LinkData= "MESReport.Test.TEST1";
        public string LinkData;
        public string CellStyle;
        public string FontStyle;
        public int RowSpan=1;
        public int ColSpan = 1;
        public String OutputType
        {
            get
            {
                return "TableColView";
            }
        }
    }
    public class TableRowView:IDictionary<string, TableColView>
    {
        public String OutputType
        {
            get
            {
                return "TableRowView";
            }
        }
        Dictionary<string, TableColView> RowData = new Dictionary<string, TableColView>();
        public string RowStyle = "";

        public TableColView this[string key]
        {
            get
            {
                return ((IDictionary<string, TableColView>)RowData)[key];
            }

            set
            {
                ((IDictionary<string, TableColView>)RowData)[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return ((IDictionary<string, TableColView>)RowData).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IDictionary<string, TableColView>)RowData).IsReadOnly;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return ((IDictionary<string, TableColView>)RowData).Keys;
            }
        }

        public ICollection<TableColView> Values
        {
            get
            {
                return ((IDictionary<string, TableColView>)RowData).Values;
            }
        }

        public void Add(KeyValuePair<string, TableColView> item)
        {
            ((IDictionary<string, TableColView>)RowData).Add(item);
        }

        public void Add(string key, TableColView value)
        {
            ((IDictionary<string, TableColView>)RowData).Add(key, value);
        }

        public void Clear()
        {
            ((IDictionary<string, TableColView>)RowData).Clear();
        }

        public bool Contains(KeyValuePair<string, TableColView> item)
        {
            return ((IDictionary<string, TableColView>)RowData).Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, TableColView>)RowData).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, TableColView>[] array, int arrayIndex)
        {
            ((IDictionary<string, TableColView>)RowData).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, TableColView>> GetEnumerator()
        {
            return ((IDictionary<string, TableColView>)RowData).GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, TableColView> item)
        {
            return ((IDictionary<string, TableColView>)RowData).Remove(item);
        }

        public bool Remove(string key)
        {
            return ((IDictionary<string, TableColView>)RowData).Remove(key);
        }

        public bool TryGetValue(string key, out TableColView value)
        {
            return ((IDictionary<string, TableColView>)RowData).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, TableColView>)RowData).GetEnumerator();
        }
    }


}
