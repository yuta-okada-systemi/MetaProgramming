using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MetaProgramming.Samples {
    public static class ExpressionTreeDxo {
        /// <summary>
        /// generic cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static class Cache<T> {
            public static readonly PropertyInfo[] properties;
            public static readonly Func<T> CreateDelegate;

            static Cache() {
                properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                        .Where(x => x.GetIndexParameters().Length < 1)
                                        .ToArray();

                Func<T> newDelegate = null;
                if (typeof(T).IsClass || typeof(T).IsValueType) {
                    if (typeof(T).GetConstructor(Type.EmptyTypes) != null) {
                        newDelegate = CreateNewExpression().Compile();
                    }
                }
                CreateDelegate = newDelegate;
            }

            public static Expression<Func<T>> CreateNewExpression() {
                //  T instance;
                var instance = Expression.Variable(typeof(T), "instance");

                var returnTarget = Expression.Label(typeof(T));
                var returnLabel = Expression.Label(returnTarget, instance);

                var newBlock = Expression.Block(typeof(T), new ParameterExpression[] { instance }, new Expression[]{
                    //  instance = new T();
                    Expression.Assign(instance, Expression.New(typeof(T))),
                    // return instance;
                    returnLabel
                });

                return Expression.Lambda<Func<T>>(newBlock);
            }
        }

        private static class DxoCache<SRC, DST> {
            public static Action<SRC, DST> copyMethod;

            static DxoCache() {
                copyMethod = CreateCopyPropertiesAction();
            }

            private static Action<SRC, DST> CreateCopyPropertiesAction() {
                var props1 = Cache<SRC>.properties;
                var props2 = Cache<DST>.properties;
                // プロパティ名が一致する対象同士でコピーする
                var items = props1.Join(props2, p1 => p1.Name, p2 => p2.Name, (src, dest) => {
                    return new { src, dest };
                });

                var srcExp = Expression.Parameter(typeof(SRC), "src");
                var dstExp = Expression.Parameter(typeof(DST), "dest");
                var expressions = items.Where(item => item.src.CanRead && item.dest.CanWrite)
                                        .Select(item => {
                                            // コピープロパティからの値取得
                                            var valueExpression = Expression.Call(
                                                srcExp, item.src.GetGetMethod(true)
                                            );

                                            // 型変換
                                            var method = GetConvertValueMethod(item.src.PropertyType, item.dest.PropertyType);
                                            var converterdExp = Expression.Call(method, valueExpression);

                                            // コピー先への値設定
                                            return Expression.Call(dstExp, item.dest.GetSetMethod(true), converterdExp);
                                        });
                return Expression.Lambda<Action<SRC, DST>>(
                    Expression.Block(expressions.ToList()),
                    new ParameterExpression[] { srcExp, dstExp }
                ).Compile();
            }
        }

        private static readonly ConcurrentDictionary<Tuple<Type, Type>, MethodInfo> ConverterCache = new ConcurrentDictionary<Tuple<Type, Type>, MethodInfo>();

        public static DST ConvertValue<SRC, DST>(SRC value) {
            return MyTypeConverter.ConvertValue<SRC, DST>(value);
        }

        /// <summary>
        ///  srcのプロパティ値をコピーしたTのインスタンスを生成（プロパティ名が一致する対象同士で値コピー）
        /// </summary>
        /// <typeparam name="SRC"></typeparam>
        /// <typeparam name="DST"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public static DST CreateObject<SRC, DST>(SRC src) where DST : new() {
            if (Object.ReferenceEquals(src, null)) return default(DST);

            var dest = Cache<DST>.CreateDelegate();
            return CopyProperties(src, dest);
        }

        /// <summary>
        ///  src→destへのプロパティ値コピー（プロパティ名が一致する対象同士で値コピー）
        /// </summary>
        /// <typeparam name="SRC"></typeparam>
        /// <typeparam name="DST"></typeparam>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static DST CopyProperties<SRC, DST>(SRC src, DST dest) {
            if (!Object.ReferenceEquals(src, null) && !Object.ReferenceEquals(dest, null)) {
                DxoCache<SRC, DST>.copyMethod.Invoke(src, dest);
            }
            return dest;
        }

        private static MethodInfo GetConvertValueMethod(Type srcType, Type destType) {
            var key = Tuple.Create(srcType, destType);
            if (!ConverterCache.TryGetValue(key, out var method)) {
                var nonGenericMethod = typeof(MyTypeConverter).GetMethod(nameof(MyTypeConverter.ConvertValue), BindingFlags.Static | BindingFlags.Public);
                method = nonGenericMethod.MakeGenericMethod(new Type[] { srcType, destType });
                ConverterCache.TryAdd(key, method);
            }
            return method;
        }
    }

}
