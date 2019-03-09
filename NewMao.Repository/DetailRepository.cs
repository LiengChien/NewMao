using Dapper;
using NewMao.Core.Extensions;
using NewMao.Entity.Detail;
using NewMao.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NewMao.Repository
{
    public class DetailRepository : GenericRepository<Detail, DetailConditions, int>, IDetailRepository
    {
        public DetailRepository(IDbTransaction transaction) : base(transaction) { }

        public override Detail Get(Detail model)
        {
            var selectCmd = model.GetSelectCommand();
            selectCmd = selectCmd.Replace("$detail$", model.TableName);
            var resident = _dbConnection.Query(model.GetType(), selectCmd, model, _transaction).Cast<Detail>().FirstOrDefault();
            return resident;
        }

        public override IEnumerable<Detail> GetAll(DetailConditions conditions, out int totalSize)
        {
            var models = new List<Detail>();
            var sqlBuilder = new StringBuilder();
            var needPager = conditions.NeedPager ? "LIMIT @Offset, @PageSize" : "";
            var selectCmd = typeof(Detail).GetSelectCommand(conditions);
            selectCmd = selectCmd.Replace("$detail$", conditions.TableName);
            sqlBuilder.Append($"{selectCmd} {needPager}");
            sqlBuilder.Append($"SELECT FOUND_ROWS();");

            using (var multi = _dbConnection.QueryMultiple(sqlBuilder.ToString(), conditions, _transaction))
            {
                models = multi.Read(typeof(Detail)).Cast<Detail>().ToList();
                totalSize = multi.Read<int>().Single();
            }
            return models;
        }

        public override void BatchCreate(List<Detail> models)
        {
            var detail = models.FirstOrDefault();
            if (detail != null)
            {
                var insertCmd = (models.FirstOrDefault()).GetInsertCommand();
                insertCmd = insertCmd.Replace("$detail$", detail.TableName);
                _dbConnection.Execute(insertCmd, models, _transaction);
            }
        }

        public override void Delete(Detail model)
        {
            var deleteCmd = model.GetDeleteCommand();
            deleteCmd = deleteCmd.Replace("$detail$", model.TableName);
            _dbConnection.Execute(deleteCmd, model, _transaction);
        }
    }
}
