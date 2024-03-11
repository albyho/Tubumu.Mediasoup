namespace Tubumu.Extensions.Generator.Templates;

public class QueryableExtensions
{
    public static string Text =
        """
        using System.Collections.Generic;
        using System.Linq.Expressions;
        using System.Reflection;
        using Tubumu.Utils.Models;

        namespace System.Linq
        {
            /// <summary>
            /// QueryableExtensions
            /// </summary>
            public static class QueryableExtensions
            {
                #region Like

                /*
                public static Expression<Func<T, bool>> Like<T>(Expression<Func<T, string>> expr, string likeValue)
                {
                    var paramExpr = expr.Parameters.First();
                    var memExpr = expr.Body;

                    if (likeValue == null || likeValue.Contains('%') != true)
                    {
                        Expression<Func<string>> valExpr = () => likeValue;
                        var eqExpr = Expression.Equal(memExpr, valExpr.Body);
                        return Expression.Lambda<Func<T, bool>>(eqExpr, paramExpr);
                    }

                    if (likeValue.Replace("%", string.Empty).Length == 0)
                    {
                        return PredicateBuilder.True<T>();
                    }

                    likeValue = Regex.Replace(likeValue, "%+", "%");

                    if (likeValue.Length > 2 && likeValue.Substring(1, likeValue.Length - 2).Contains('%'))
                    {
                        likeValue = likeValue.Replace("[", "[[]").Replace("_", "[_]");
                        Expression<Func<string>> valExpr = () => likeValue;
                        var patExpr = Expression.Call(typeof(SqlFunctions).GetMethod("PatIndex",
                            new[] { typeof(string), typeof(string) }), valExpr.Body, memExpr);
                        var neExpr = Expression.NotEqual(patExpr, Expression.Convert(Expression.Constant(0), typeof(int?)));
                        return Expression.Lambda<Func<T, bool>>(neExpr, paramExpr);
                    }

                    if (likeValue.StartsWith("%"))
                    {
                        if (likeValue.EndsWith("%") == true)
                        {
                            likeValue = likeValue.Substring(1, likeValue.Length - 2);
                            Expression<Func<string>> valExpr = () => likeValue;
                            var containsExpr = Expression.Call(memExpr, typeof(String).GetMethod("Contains",
                                new[] { typeof(string) }), valExpr.Body);
                            return Expression.Lambda<Func<T, bool>>(containsExpr, paramExpr);
                        }
                        else
                        {
                            likeValue = likeValue.Substring(1);
                            Expression<Func<string>> valExpr = () => likeValue;
                            var endsExpr = Expression.Call(memExpr, typeof(String).GetMethod("EndsWith",
                                new[] { typeof(string) }), valExpr.Body);
                            return Expression.Lambda<Func<T, bool>>(endsExpr, paramExpr);
                        }
                    }
                    else
                    {
                        likeValue = likeValue.Remove(likeValue.Length - 1);
                        Expression<Func<string>> valExpr = () => likeValue;
                        var startsExpr = Expression.Call(memExpr, typeof(String).GetMethod("StartsWith",
                            new[] { typeof(string) }), valExpr.Body);
                        return Expression.Lambda<Func<T, bool>>(startsExpr, paramExpr);
                    }
                }

                public static Expression<Func<T, bool>> AndLike<T>(this Expression<Func<T, bool>> predicate, Expression<Func<T, string>> expr, string likeValue)
                {
                    var andPredicate = Like(expr, likeValue);
                    if (andPredicate != null)
                    {
                        predicate = predicate.And(andPredicate.Expand());
                    }
                    return predicate;
                }

                public static Expression<Func<T, bool>> OrLike<T>(this Expression<Func<T, bool>> predicate, Expression<Func<T, string>> expr, string likeValue)
                {
                    var orPredicate = Like(expr, likeValue);
                    if (orPredicate != null)
                    {
                        predicate = predicate.Or(orPredicate.Expand());
                    }
                    return predicate;
                }

                */

                #endregion Like

                /// <summary>
                /// WhereOrContains
                /// </summary>
                public static IQueryable<TEntity> WhereOrStringContains<TEntity, String>
                    (
                    this IQueryable<TEntity> query,
                    Expression<Func<TEntity, String>> selector,
                    IEnumerable<String> values
                    )
                {
                    /*
                     * 实现效果：
                     * var tags = new[] { "A", "B", "C" };
                     * SELECT * FROM [User] Where Name='Test' AND (Tags LIKE '%A%' Or Tags LIKE  '%B%')
                     */

                    if(selector == null)
                    {
                        throw new ArgumentNullException(nameof(selector));
                    }

                    if(values == null)
                    {
                        throw new ArgumentNullException(nameof(values));
                    }

                    if(!values.Any())
                    {
                        return query;
                    }

                    ParameterExpression p = selector.Parameters.Single();
                    var containsExpressions = values.Select(value => (Expression)Expression.Call(selector.Body, typeof(String).GetMethod("Contains", new[] { typeof(String) })!, Expression.Constant(value)));
                    Expression body = containsExpressions.Aggregate((accumulate, containsExpression) => Expression.Or(accumulate, containsExpression));

                    return query.Where(Expression.Lambda<Func<TEntity, bool>>(body, p));
                }

                public static IQueryable<TEntity> WhereOrCollectionAnyEqual<TEntity, TValue, TMemberValue>
                    (
                    this IQueryable<TEntity> query,
                    Expression<Func<TEntity, IEnumerable<TValue>>> selector,
                    Expression<Func<TValue, TMemberValue>> memberSelector,
                    IEnumerable<TMemberValue> values
                    )
                {
                    if(selector == null)
                    {
                        throw new ArgumentNullException(nameof(selector));
                    }

                    if(values == null)
                    {
                        throw new ArgumentNullException(nameof(values));
                    }

                    if(!values.Any())
                    {
                        return query;
                    }

                    ParameterExpression selectorParameter = selector.Parameters.Single();
                    ParameterExpression memberParameter = memberSelector.Parameters.Single();
                    var methodInfo = GetEnumerableMethod("Any", 2).MakeGenericMethod(typeof(TValue));
                    var anyExpressions = values.Select(value =>
                            (Expression)Expression.Call(null,
                                                        methodInfo,
                                                        selector.Body,
                                                        Expression.Lambda<Func<TValue, bool>>(Expression.Equal(memberSelector.Body,
                                                                                                               Expression.Constant(value, typeof(TMemberValue))),
                                                                                                               memberParameter
                                                                                                               )
                                                        )
                        );
                    Expression body = anyExpressions.Aggregate((accumulate, any) => Expression.Or(accumulate, any));

                    return query.Where(Expression.Lambda<Func<TEntity, bool>>(body, selectorParameter));
                }

                /// <summary>
                /// WhereIn
                /// </summary>
                public static IQueryable<TEntity> WhereIn<TEntity, TValue>
                  (
                    this IQueryable<TEntity> query,
                    Expression<Func<TEntity, TValue>> selector,
                    IEnumerable<TValue> values
                  )
                {
                    /*
                     * 实现效果：
                     * var names = new[] { "A", "B", "C" };
                     * SELECT * FROM [User] Where Name='A' OR Name='B' OR Name='C'
                     * 实际上，可以直接这样：
                     * var query = DbContext.User.Where(m => names.Contains(m.Name));
                     */

                    if(selector == null)
                    {
                        throw new ArgumentNullException(nameof(selector));
                    }

                    if(values == null)
                    {
                        throw new ArgumentNullException(nameof(values));
                    }

                    if(!values.Any())
                    {
                        return query;
                    }

                    ParameterExpression p = selector.Parameters.Single();
                    IEnumerable<Expression> equals = values.Select(value => (Expression)Expression.Equal(selector.Body, Expression.Constant(value, typeof(TValue))));
                    Expression body = equals.Aggregate((accumulate, equal) => Expression.Or(accumulate, equal));

                    return query.Where(Expression.Lambda<Func<TEntity, bool>>(body, p));
                }

                /// <summary>
                /// WhereIn
                /// </summary>
                public static IQueryable<TEntity> WhereIn<TEntity, TValue>
                  (
                    this IQueryable<TEntity> query,
                    Expression<Func<TEntity, TValue>> selector,
                    params TValue[] values
                  )
                {
                    return WhereIn(query, selector, (IEnumerable<TValue>)values);
                }

                /// <summary>
                /// LeftJoin
                /// </summary>
                public static IQueryable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(
                    this IQueryable<TOuter> outer,
                    IQueryable<TInner> inner,
                    Expression<Func<TOuter, TKey>> outerKeySelector,
                    Expression<Func<TInner, TKey>> innerKeySelector,
                    Expression<Func<TOuter, TInner, TResult>> resultSelector)
                {
                    MethodInfo groupJoin = typeof(Queryable).GetMethods()
                                                             .Single(m => m.ToString() == "System.Linq.IQueryable`1[TResult] GroupJoin[TOuter,TInner,TKey,TResult](System.Linq.IQueryable`1[TOuter], System.Collections.Generic.IEnumerable`1[TInner], System.Linq.Expressions.Expression`1[System.Func`2[TOuter,TKey]], System.Linq.Expressions.Expression`1[System.Func`2[TInner,TKey]], System.Linq.Expressions.Expression`1[System.Func`3[TOuter,System.Collections.Generic.IEnumerable`1[TInner],TResult]])")
                                                             .MakeGenericMethod(typeof(TOuter), typeof(TInner), typeof(TKey), typeof(LeftJoinIntermediate<TOuter, TInner>));
                    MethodInfo selectMany = typeof(Queryable).GetMethods()
                                                              .Single(m => m.ToString() == "System.Linq.IQueryable`1[TResult] SelectMany[TSource,TCollection,TResult](System.Linq.IQueryable`1[TSource], System.Linq.Expressions.Expression`1[System.Func`2[TSource,System.Collections.Generic.IEnumerable`1[TCollection]]], System.Linq.Expressions.Expression`1[System.Func`3[TSource,TCollection,TResult]])")
                                                              .MakeGenericMethod(typeof(LeftJoinIntermediate<TOuter, TInner>), typeof(TInner), typeof(TResult));

                    var groupJoinResultSelector = (Expression<Func<TOuter, IEnumerable<TInner>, LeftJoinIntermediate<TOuter, TInner>>>)
                                                  ((oneOuter, manyInners) => new LeftJoinIntermediate<TOuter, TInner> { OneOuter = oneOuter, ManyInners = manyInners });

                    MethodCallExpression exprGroupJoin = Expression.Call(groupJoin, outer.Expression, inner.Expression, outerKeySelector, innerKeySelector, groupJoinResultSelector);

                    var selectManyCollectionSelector = (Expression<Func<LeftJoinIntermediate<TOuter, TInner>, IEnumerable<TInner>>>)
                                                       (t => t.ManyInners.DefaultIfEmpty()!);

                    ParameterExpression paramUser = resultSelector.Parameters[0];

                    ParameterExpression paramNew = Expression.Parameter(typeof(LeftJoinIntermediate<TOuter, TInner>), "t");
                    MemberExpression propExpr = Expression.Property(paramNew, "OneOuter");

                    LambdaExpression selectManyResultSelector = Expression.Lambda(new Replacer(paramUser, propExpr).Visit(resultSelector.Body) ?? throw new InvalidOperationException(), paramNew, resultSelector.Parameters.Skip(1).First());

                    MethodCallExpression exprSelectMany = Expression.Call(selectMany, exprGroupJoin, selectManyCollectionSelector, selectManyResultSelector);

                    return outer.Provider.CreateQuery<TResult>(exprSelectMany);
                }

                private class LeftJoinIntermediate<TOuter, TInner>
                {
        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
                    public TOuter OneOuter { get; set; }

                    public IEnumerable<TInner> ManyInners { get; set; }
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
                }

                private class Replacer : ExpressionVisitor
                {
                    private readonly ParameterExpression _oldParam;

                    private readonly Expression _replacement;

                    public Replacer(ParameterExpression oldParam, Expression replacement)
                    {
                        _oldParam = oldParam;
                        _replacement = replacement;
                    }

                    public override Expression? Visit(Expression? exp)
                    {
                        return exp == _oldParam ? _replacement : base.Visit(exp);
                    }
                }

                /// <summary>
                /// Order
                /// </summary>
                public static IOrderedQueryable<T> Order<T>(this IQueryable<T> source, string propertyName, bool descending, bool anotherLevel = false)
                {
                    var type = typeof(T);
                    var propertyInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public)
                    ?? throw new ArgumentOutOfRangeException(nameof(propertyName));

                    ParameterExpression parameter = Expression.Parameter(type, string.Empty); // I don't care about some naming
                    MemberExpression property = Expression.Property(parameter, propertyInfo);
                    LambdaExpression sort = Expression.Lambda(property, parameter);
                    MethodCallExpression call = Expression.Call(
                        typeof(Queryable),
                        (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
                        new[] { typeof(T), property.Type },
                        source.Expression,
                        Expression.Quote(sort));
                    return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
                }

                /// <summary>
                /// Order
                /// </summary>
                public static IOrderedQueryable<T> Order<T>(this IQueryable<T> source, SortInfo sortInfo, bool anotherLevel = false)
                {
                    return sortInfo.Sort.IsNullOrWhiteSpace()
                        ? throw new ArgumentException($"{nameof(sortInfo.Sort)} can't be null or white space.")
                        : Order(source, sortInfo.Sort!, sortInfo.SortDir == SortDirection.DESC, anotherLevel);
                }

                /// <summary>
                /// Order
                /// </summary>
                public static IOrderedQueryable<T>? Order<T>(this IQueryable<T> source, ICollection<SortInfo> sortInfos)
                {
                    IOrderedQueryable<T>? result = null;
                    var isFirst = true;
                    foreach(var sortInfo in sortInfos)
                    {
                        result = Order(source, sortInfo, !isFirst);
                        isFirst = false;
                    }

                    return result;
                }

                /// <summary>
                /// OrderBy
                /// </summary>
                public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
                {
                    return Order(source, propertyName, false, false);
                }

                /// <summary>
                /// OrderByDescending
                /// </summary>
                public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
                {
                    return Order(source, propertyName, true, false);
                }

                /// <summary>
                /// ThenBy
                /// </summary>
                public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
                {
                    return Order(source, propertyName, false, true);
                }

                /// <summary>
                /// ThenByDescending
                /// </summary>
                public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
                {
                    return Order(source, propertyName, true, true);
                }

                /// <summary>
                /// Filters a <see cref="IQueryable{T}"/> by given predicate if given condition is true.
                /// </summary>
                /// <param name="query">Queryable to apply filtering</param>
                /// <param name="condition">A boolean value</param>
                /// <param name="predicate">Predicate to filter the query</param>
                /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
                /// <remarks>https://github.com/aspnetboilerplate/aspnetboilerplate/blob/e0ded5d8702f389aa1f5947d3446f16aec845287/src/Abp/Linq/Extensions/QueryableExtensions.cs</remarks>
                public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
                {
                    return condition
                        ? query.Where(predicate)
                        : query;
                }

                /// <summary>
                /// Filters a <see cref="IQueryable{T}"/> by given predicate if given condition is true.
                /// </summary>
                /// <param name="query">Queryable to apply filtering</param>
                /// <param name="condition">A boolean value</param>
                /// <param name="predicate">Predicate to filter the query</param>
                /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
                /// <remarks>https://github.com/aspnetboilerplate/aspnetboilerplate/blob/e0ded5d8702f389aa1f5947d3446f16aec845287/src/Abp/Linq/Extensions/QueryableExtensions.cs</remarks>
                public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, int, bool>> predicate)
                {
                    return condition
                        ? query.Where(predicate)
                        : query;
                }

                private static MethodInfo GetEnumerableMethod(string name, int parameterCount = 0, Func<MethodInfo, bool>? predicate = null)
                {
                    return typeof(Enumerable)
                        .GetTypeInfo()
                        .GetDeclaredMethods(name)
                        .Single(_ => _.GetParameters().Length == parameterCount && (predicate == null || predicate(_)));
                }
            }
        }

        """;
}
