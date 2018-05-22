using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TT.IntegrationTests
{
    public class CSVLoader : IDisposable, ICSVLoader
    {
        private string ConnectionString { get; }
        private SqlConnection SqlConnection { get; }

        public CSVLoader(string connectionString)
        {
            ConnectionString = connectionString;
            SqlConnection = new SqlConnection(ConnectionString);
        }

        public async Task LoadCSVFromTableAndNamespaceAsync<T>(T obj, string tableName, string subDir = "")
            where T : class
        {
            string csvPath = GetPathFromClassName(obj, tableName, subDir);
            await LoadCSVFromAbsolutePathAsync(csvPath);
        }

        public async Task LoadCSVFromAbsolutePathAsync(string path)
        {
            await SqlConnection.OpenAsync();

            var dt = await GetDataTabletFromCSVFileAsync(path);
            await InsertDataIntoSQLServerUsingSQLBulkCopyAsync(dt);

            SqlConnection.Close();
        }

        public void LoadCSVFromTableAndNamespace<T>(T obj, string tableName, string subDir = "")
            where T : class
        {
            string csvPath = GetPathFromClassName(obj, tableName, subDir);
            LoadCSVFromAbsolutePath(csvPath);
        }

        public void LoadCSVFromAbsolutePath(string path)
        {
            SqlConnection.Open();

            var dt = GetDataTabletFromCSVFile(path);
            InsertDataIntoSQLServerUsingSQLBulkCopy(dt);

            SqlConnection.Close();
        }

        private string GetPathFromClassName<T>(T obj, string tableName, string subDir = "")
            where T : class
        {
            string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
            string[] nameSpace = obj.GetType().Namespace.Split('.');

            // skip two splits for the beginning assembly name TT.IntegrationTests
            string[] dirPath = nameSpace.Skip(2).ToArray();
            string csvPath = Path.Combine(assemblyPath, Path.Combine(dirPath), subDir, $"{tableName}.csv");

            return csvPath;
        }

        private DataTable PopulateSchema(string csv_file_path)
        {
            DataTable csvData = new DataTable();
            string tableName = Path.GetFileNameWithoutExtension(csv_file_path);

            using (SqlDataAdapter adapter = new SqlDataAdapter(String.Format("SELECT TOP 0 * FROM {0}", tableName), SqlConnection))
            {
                DataSet ds = new DataSet();

                adapter.FillSchema(ds, SchemaType.Source, tableName);
                adapter.Fill(ds, tableName);

                csvData = ds.Tables[0];
            };

            return csvData;
        }

        private CsvReader BuildCsvReader(string csv_file_path)
        {
            TextReader reader = new StreamReader(csv_file_path);
            CsvReader csvReader = new CsvReader(new CsvParserWithQuotes(reader, new Configuration(), false));
            csvReader.Context.RecordBuilder = new RecordBuilderWithQuotes();
            csvReader.Configuration.TypeConverterCache.AddConverter<string>(new NullableStringTypeConverter());

            return csvReader;
        }

        private void DoRowRead(CsvReader csvReader, DataTable csvData)
        {
            var row = csvData.NewRow();
            foreach (DataColumn column in csvData.Columns)
            {
                if (column.DataType.IsValueType)
                {
                    // even if null is allowed the column datatype is the non-nullable value type
                    // so conversion to a nullable value type is needed
                    var nullableType = typeof(Nullable<>).MakeGenericType(column.DataType);

                    // add custom converter for the nullable value type
                    // that returns DBNull if the vlaue is null or the value otherwise
                    csvReader.Configuration.TypeConverterCache.AddConverter(nullableType,
                        new DBNullableConverter(nullableType, csvReader.Configuration.TypeConverterCache));

                    row[column.ColumnName] = csvReader.GetField(nullableType, column.ColumnName);
                }
                else
                {
                    var obj = csvReader.GetField(column.DataType, column.ColumnName);
                    row[column.ColumnName] = obj ?? DBNull.Value;
                }
            }
            csvData.Rows.Add(row);
        }

        private async Task<DataTable> GetDataTabletFromCSVFileAsync(string csv_file_path)
        {
            DataTable csvData = PopulateSchema(csv_file_path);

            using (CsvReader csvReader = BuildCsvReader(csv_file_path))
            {
                await csvReader.ReadAsync();
                csvReader.ReadHeader();

                while (await csvReader.ReadAsync())
                {
                    DoRowRead(csvReader, csvData);
                }
            }

            return csvData;
        }

        private async Task InsertDataIntoSQLServerUsingSQLBulkCopyAsync(DataTable csvFileData)
        {
            using (SqlBulkCopy sqlCopy = new SqlBulkCopy(SqlConnection))
            {
                sqlCopy.DestinationTableName = csvFileData.TableName;

                foreach (DataColumn column in csvFileData.Columns)
                {
                    sqlCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                await sqlCopy.WriteToServerAsync(csvFileData);
            }
        }

        private DataTable GetDataTabletFromCSVFile(string csv_file_path)
        {
            DataTable csvData = PopulateSchema(csv_file_path);

            using (CsvReader csvReader = BuildCsvReader(csv_file_path))
            {
                csvReader.Read();
                csvReader.ReadHeader();

                while (csvReader.Read())
                {
                    DoRowRead(csvReader, csvData);
                }
            }

            return csvData;
        }

        private void InsertDataIntoSQLServerUsingSQLBulkCopy(DataTable csvFileData)
        {
            using (SqlBulkCopy sqlCopy = new SqlBulkCopy(SqlConnection))
            {
                sqlCopy.DestinationTableName = csvFileData.TableName;

                foreach (DataColumn column in csvFileData.Columns)
                {
                    sqlCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                sqlCopy.WriteToServer(csvFileData);
            }
        }

        #region IDisposable Implementation
        public bool Disposed { get { return disposedValue; } }
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose handled objects
                    SqlConnection.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Disposes <see cref="Context"/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        #region Custom Class Extensions
        private class CsvParserWithQuotes : CsvParser
        {
            public CsvParserWithQuotes(TextReader reader, Configuration configuration, bool leaveOpen) : base(reader, configuration, leaveOpen)
            {
            }

            protected override bool ReadQuotedField()
            {
                var result = base.ReadQuotedField();
                var recordBuilder = (RecordBuilderWithQuotes)Context.RecordBuilder;

                var lastQuoted = recordBuilder.Record[recordBuilder.Position - 1];
                recordBuilder.Record[recordBuilder.Position - 1] = $@"""{lastQuoted}""";

                return result;
            }

            protected override Task<bool> ReadQuotedFieldAsync()
            {
                var result = base.ReadQuotedFieldAsync();
                var recordBuilder = (RecordBuilderWithQuotes)Context.RecordBuilder;

                var lastQuoted = recordBuilder.Record[recordBuilder.Position - 1];
                recordBuilder.Record[recordBuilder.Position - 1] = $@"""{lastQuoted}""";

                return result;
            }
        }

        private class RecordBuilderWithQuotes : RecordBuilder
        {
            public List<string> Record { get; } = new List<string>();
            public int Position { get; private set; }

            public override RecordBuilder Add(string field)
            {
                if (Record.Count == Position)
                {
                    Record.Add(field);
                }
                else
                {
                    Record[Position] = field;
                }

                Position++;

                return this;
            }

            public override RecordBuilder Clear()
            {
                Position = 0;

                return this;
            }

            public override string[] ToArray()
            {
                return Record.ToArray();
            }
        }

        private class NullableStringTypeConverter : StringConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                memberMapData.TypeConverterOptions.NullValues.Add("NULL");

                var result = (base.ConvertFromString(text, row, memberMapData) as string);

                if (result != null)
                {
                    return result.Length >= 2 ? result.Substring(1, result.Length - 2) : "";
                }
                else
                {
                    return DBNull.Value;
                }
            }
        }

        private class DBNullableConverter : NullableConverter
        {
            public DBNullableConverter(Type type, TypeConverterCache typeConverterFactory) : base(type, typeConverterFactory)
            {
            }

            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                memberMapData.TypeConverterOptions.NullValues.Add("NULL");

                return base.ConvertFromString(text, row, memberMapData) ?? DBNull.Value;
            }
        }
        #endregion
    }
}
