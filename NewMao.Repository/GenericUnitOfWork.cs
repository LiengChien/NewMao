using DocumentFormat.OpenXml.Drawing;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using NewMao.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NewMao.Repository
{
    public class GenericUnitOfWork : IUnitOfWork
    {
        protected IDbTransaction _transaction;
        protected IDbConnection _connection;
        private bool _disposed;

        private readonly IConfiguration _config;

        public GenericUnitOfWork(IConfiguration config)
        {
            _config = config;
            _connection = new MySqlConnection(GetConnectionString());
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }
        
        public GenericUnitOfWork(IConfiguration config, ConnectionType type)
        {
            _config = config;
            _connection = new MySqlConnection(GetConnectionString(type));
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = _connection.BeginTransaction();
            }
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                    }
                    if (_connection != null)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }
                }
                _disposed = true;
            }
        }

        private string GetConnectionString()
        {
            string connStr = _config["ConnectionStrings:DefaultConnection"];
            return connStr;
        }

        private string GetConnectionString(ConnectionType type)
        {
            string connStr = string.Empty;
            throw new NotImplementedException();
        }
    }
}
