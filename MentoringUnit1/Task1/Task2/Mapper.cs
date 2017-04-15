using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Task1
{
  public class Mapper<TSource, TDestination>
  {
    readonly Func<TSource, TDestination> _mapFunction;

    internal Mapper(Func<TSource, TDestination> func)
    {
      _mapFunction = func;
    }

    public TDestination Map(TSource source)
    {
      return _mapFunction(source);
    }
  }

  public class MappingGenerator
  {
    public Mapper<TSource, TDestination> Generate<TSource, TDestination>()
    {
      var propsToMap = this.GetPropertiesToMap(typeof(TSource), typeof(TDestination));

      var sourceParam = Expression.Parameter(typeof(TSource));

      var bindingArray = propsToMap
        .Select(fieldName => Expression.Bind(
            typeof(TDestination).GetMember(fieldName)[0],
            Expression.Property(sourceParam, fieldName)));

      var mapFunction =
        Expression.Lambda<Func<TSource, TDestination>>(
          Expression.MemberInit(Expression.New(typeof(TDestination)), bindingArray),
          sourceParam
        );

      return new Mapper<TSource, TDestination>(mapFunction.Compile());
    }

    public IEnumerable<string> GetPropertiesToMap(Type sourceType, Type desctinationType)
    {
      var sourceProps = sourceType.GetProperties().Select(item => item.Name);
      var destinationProps = desctinationType.GetProperties().Select(item => item.Name);

      var propsToMap = destinationProps.Intersect(sourceProps);

      return propsToMap;
    }
  }
}