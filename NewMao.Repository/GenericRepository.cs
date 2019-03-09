using Dapper;
using NewMao.Common.Mapper;
using NewMao.Common.Model;
using NewMao.Core.Extensions;
using NewMao.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NewMao.Repository
{
    public class GenericRepository<TEntity, TConditions, TId> :
        BaseRepository, IRepository<TEntity, TConditions, TId>
        where TConditions : BasePager
    {
        public GenericRepository(IDbTransaction transaction) : base(transaction)
        {
            Dapper.SqlMapper.SetTypeMap(typeof(TEntity), new ColumnAttributeTypeMapper(typeof(TEntity)));
        }

        public virtual IEnumerable<TEntity> GetAll(TConditions conditions, out int totalSize)
        {
            var models = new List<TEntity>();
            var sqlBuilder = new StringBuilder();
            var needPager = conditions.NeedPager ? "LIMIT @Offset, @pageSize" : "";
            sqlBuilder.Append($"{typeof(TEntity).GetSelectCommand(conditions)} {needPager};");
            sqlBuilder.Append("SELECT FOUND_ROWS();");
            using (var multi = _dbConnection.QueryMultiple(sqlBuilder.ToString(), conditions, _transaction))
            {
                models = multi.Read(typeof(TEntity)).Cast<TEntity>().ToList();
                totalSize = multi.Read<int>().Single();
            }
            return models;
        }

        public virtual TEntity Get(TEntity model)
        {
            var resident = _dbConnection.Query(typeof(TEntity), model.GetSelectCommand(), model, _transaction).Cast<TEntity>().FirstOrDefault();
            return resident;
        }

        public TId Create(TEntity model)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append(model.GetInsertCommand());
            sqlBuilder.Append("SELECT LAST_INSERT_ID()");
            return _dbConnection.Query<TId>(sqlBuilder.ToString(), model, _transaction).Single();
        }

        public virtual void BatchCreate(List<TEntity> models)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(TEntity model)
        {
            _dbConnection.Execute(model.GetUpdateCommand(), model, _transaction);
        }

        public void BatchUpdate(List<TEntity> models)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(TEntity model)
        {
            _dbConnection.Execute(model.GetDeleteCommand(), model, _transaction);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
