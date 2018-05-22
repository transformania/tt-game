using System;
using System.Threading.Tasks;

namespace TT.IntegrationTests
{
    public interface ICSVLoader : IDisposable
    {
        void LoadCSVFromAbsolutePath(string path);
        Task LoadCSVFromAbsolutePathAsync(string path);
        void LoadCSVFromTableAndNamespace<T>(T obj, string tableName, string subDir = "") where T : class;
        Task LoadCSVFromTableAndNamespaceAsync<T>(T obj, string tableName, string subDir = "") where T : class;
    }
}