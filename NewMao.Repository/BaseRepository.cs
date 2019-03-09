using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NewMao.Repository
{
    public class BaseRepository
    {
        public IDbTransaction _transaction { get; private set; }
        public IDbConnection _dbConnection
        {
            get
            {
                return _transaction.Connection;
            }
        }
        public BaseRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
        }
    }
}
