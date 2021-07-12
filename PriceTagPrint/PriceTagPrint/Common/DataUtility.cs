using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace PriceTagPrint.Common
{
    public static class DataUtility
    {
        public static DataTable ToDataTable<T>(this List<T> data, string[] sort = null)
        {
            PropertyDescriptorCollection properties;
            properties = sort is null ? TypeDescriptor.GetProperties(typeof(T)) :
                            TypeDescriptor.GetProperties(typeof(T)).Sort(sort);
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties.Sort())
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                var row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
