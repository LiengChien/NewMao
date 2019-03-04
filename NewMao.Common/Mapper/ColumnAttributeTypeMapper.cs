using Dapper;
using NewMao.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NewMao.Common.Mapper
{
    public class ColumnAttributeTypeMapper<T> : FallbackTypeMapper
    {
        public ColumnAttributeTypeMapper() 
            : base(new SqlMapper.ITypeMap[]
                  {
                      new CustomPropertyTypeMap(
                          typeof(T),
                          (type, columnName) => 
                            type.GetProperties().FirstOrDefault(prop =>
                                prop.GetCustomAttributes(false)
                                    .OfType<ColumnAttribute>()
                                    .Any(Attr => Attr.Name == columnName)
                            )
                          ),
                      new DefaultTypeMap(typeof(T))
                  })
        {
        }
    }

    public class ColumnAttributeTypeMapper : FallbackTypeMapper
    {
        public ColumnAttributeTypeMapper(Type entity)
            : base(new SqlMapper.ITypeMap[]
            {
                new CustomPropertyTypeMap(
                    entity,
                    (type, columnName) =>
                        type.GetProperties().FirstOrDefault(prop =>
                            prop.GetCustomAttributes(false)
                                .OfType<ColumnAttribute>()
                                .Any(attr => attr.Name == columnName)
                        )
                    ),
                new DefaultTypeMap(entity)
            })
        {
        }
    }

    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public bool IsKey { get; set; } = false;
        public bool OrClause { get; set; } = false;
        public WhereClause WhereTypeDefine { get; set; }
        public string BelongColumn { get; set; }
        public BetweenIndentify BetweenDefine { get; set; }
        public string BelongTable { get; set; }
        public string BelongTableAlias { get; set; }
        public bool JoinByValue { get; set; }
        public string JoinValue { get; set; }
        public string ForiegnTable { get; set; }
        public string ForiegnColumn { get; set; }
        public JoinType ForiegnJoinType { get; set; } = JoinType.InnerJoin;
        public bool OrderBy { get; set; } = false;
        public OrderByType OrderByType { get; set; } = OrderByType.Asc;
        public int OrderByIndex { get; set; }
        public bool AllowNull { get; set; }
        public string BetweenStart { get; set; }
        public string BetweenEnd { get; set; }
    }

    public class FallbackTypeMapper : SqlMapper.ITypeMap
    {
        private readonly IEnumerable<SqlMapper.ITypeMap> _mappers;

        public FallbackTypeMapper(IEnumerable<SqlMapper.ITypeMap> mappers)
        {
            _mappers = mappers;
        }

        public ConstructorInfo FindConstructor(string[] names, Type[] types)
        {
            foreach (var mapper in _mappers)
            {
                try
                {
                    ConstructorInfo result = mapper.FindConstructor(names, types);
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch (NotImplementedException)
                {

                    throw;
                }
            }
            return null;
        }

        public SqlMapper.IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
        {
            foreach (var mapper in _mappers)
            {
                try
                {
                    var result = mapper.GetConstructorParameter(constructor, columnName);
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch (NotImplementedException)
                {

                    throw;
                }
            }
            return null;
        }

        public SqlMapper.IMemberMap GetMember(string columnName)
        {
            foreach (var mapper in _mappers)
            {
                try
                {
                    var result = mapper.GetMember(columnName);
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch (NotImplementedException)
                {

                    throw;
                }
            }
            return null;
        }

        public ConstructorInfo FindExplicitConstructor()
        {
            return _mappers
                .Select(mapper => mapper.FindExplicitConstructor())
                .FirstOrDefault(result => result != null);
        }
    }
}
