using System;
using System.Linq;

namespace WebApplication.Classes
{
    public class DatabaseManager
    {
        private static DatabaseManager _instance;
        public static Database Database;

        private DatabaseManager() { }

        public static DatabaseManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DatabaseManager();
                Database = new Database("DB");
                _instance.PopulateTable();
                _instance.PopulateTable();
                // Initialize other dependencies if required
            }
            return _instance;
        }

        public void PopulateTable()
        {
            var table = new Table("Table" + Database.Tables.Count);
            table.AddColumn(new TypeInteger("column1"));
            table.AddColumn(new TypeReal("column2"));
            table.AddColumn(new StringColumn("column3"));
            table.AddColumn(new TypeChar("column4"));
            table.AddColumn(new TypeCharInvl("column5"));
            table.AddColumn(new TypeStringInvl("column6"));

            var row1 = new Row();
            row1.Values.Add("1");
            row1.Values.Add("1.0");
            row1.Values.Add("text 1");
            row1.Values.Add("a");
            row1.Values.Add("a , b");
            row1.Values.Add("aa - bb");
            table.AddRow(row1);

            var row2 = new Row();
            row2.Values.Add("2");
            row2.Values.Add("2.0");
            row2.Values.Add("taxt 2");
            row2.Values.Add("c");
            row2.Values.Add("c, d");
            row2.Values.Add("cccc-dddd");
            table.AddRow(row2);

            Database.AddTable(table);
        }

        public string RenameDB(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Database.Name = name;
                return name;
            }
            return null;
        }

        public void CreateDB(string name)
        {
            Database = new Database(name);
        }

        public bool AddTable(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var table = new Table(name);
                Database.AddTable(table);
                return true;
            }
            return false;
        }

        public bool DeleteTable(int tableIndex)
        {
            if (tableIndex != -1 && tableIndex < Database.Tables.Count)
            {
                Database.Tables.RemoveAt(tableIndex);
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool AddColumn(int tableIndex, string columnName, TypeColumn columnType)
        {
            if (!string.IsNullOrEmpty(columnName) && tableIndex != -1 && tableIndex < Database.Tables.Count)
            {
                Column column = null;
                switch (columnType)
                {
                    case TypeColumn.INT:
                        column = new TypeInteger(columnName);
                        break;
                    case TypeColumn.REAL:
                        column = new TypeReal(columnName);
                        break;
                    case TypeColumn.STRING:
                        column = new StringColumn(columnName);
                        break;
                    case TypeColumn.CHAR:
                        column = new TypeChar(columnName);
                        break;
                    case TypeColumn.CHARINVL:
                        column = new TypeCharInvl(columnName);
                        break;
                    case TypeColumn.STRINGINVL:
                        column = new TypeStringInvl(columnName);
                        break;
                }

                if (column != null)
                {
                    Database.Tables[tableIndex].AddColumn(column);
                    foreach (var row in Database.Tables[tableIndex].Rows)
                    {
                        row.Values.Add("");
                    }
                    return true;
                }
            }
            return false;
        }


        public bool DeleteColumn(int tableIndex, int columnIndex)
        {
            if (columnIndex != -1 && tableIndex != -1 && tableIndex < Database.Tables.Count
                && columnIndex < Database.Tables[tableIndex].Columns.Count)
            {
                Database.Tables[tableIndex].DeleteColumn(columnIndex);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddRow(int tableIndex, Row row)
        {
            if (tableIndex != -1 && tableIndex < Database.Tables.Count)
            {
                var table = Database.Tables[tableIndex];
                for (int i = row.Values.Count; i < table.Columns.Count; i++)
                {
                    row.Values.Add("");
                }
                table.AddRow(row);
                return true;
            }
            return false;
        }

        public bool DeleteRow(int tableIndex, int rowIndex)
        {
            if (rowIndex != -1 && tableIndex != -1 && tableIndex < Database.Tables.Count
                && rowIndex < Database.Tables[tableIndex].Rows.Count)
            {
                Database.Tables[tableIndex].DeleteRow(rowIndex);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateCellValue(string value, int tableIndex, int columnIndex, int rowIndex)
        {
            if (tableIndex != -1 && columnIndex != -1 && rowIndex != -1
                && tableIndex < Database.Tables.Count && columnIndex < Database.Tables[tableIndex].Columns.Count
                && rowIndex < Database.Tables[tableIndex].Rows.Count)
            {
                var column = Database.Tables[tableIndex].Columns[columnIndex];
                if (column.Validate(value))
                {
                    Database.Tables[tableIndex].Rows[rowIndex].SetAt(columnIndex, value.Trim());
                    return true;
                }
            }
            return false;
        }

        public bool ChangeColumnName(int tableIndex, int columnIndex, string columnName)
        {
            Database.Tables[tableIndex].Columns[columnIndex].Name = columnName;
            return true;
        }

        public bool Merge_Tables(string tableName1, string tableName2)
        {
            // Find tables by their names
            Table table1 = Database.Tables.FirstOrDefault(t => t.Name == tableName1);
            Table table2 = Database.Tables.FirstOrDefault(t => t.Name == tableName2);

            if (table1 == null || table2 == null)
            {
                // Return null if either table is not found
                return false;
            }

            // Check if tables have the same number of columns
            if (table1.Columns.Count != table2.Columns.Count)
            {
                // Return null if columns count doesn't match
                return false;
            }

            // Check if columns match by name and type
            for (int i = 0; i < table1.Columns.Count; i++)
            {
                if (table1.Columns[i].Name != table2.Columns[i].Name ||
                    table1.Columns[i].Type != table2.Columns[i].Type)
                {
                    // Return null if any column doesn't match
                    //return null;
                    return false;
                }
            }

            // Create a new merged table
            Table mergedTable = new Table("MergedTable");

            // Copy columns from table1 to mergedTable
            foreach (var column in table1.Columns)
            {
                mergedTable.Columns.Add(column);
            }

            // Merge rows by stacking them
            mergedTable.Rows.AddRange(table1.Rows);
            mergedTable.Rows.AddRange(table2.Rows);
            Database.Tables.Add(mergedTable);
            //return mergedTable;
            return true;
        }

    }

}

