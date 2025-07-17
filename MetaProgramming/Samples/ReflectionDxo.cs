using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MetaProgramming.Samples {
    public static class ReflectionDxo {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<Tuple<Type, Type>, MethodInfo> ConverterCache = new ConcurrentDictionary<Tuple<Type, Type>, MethodInfo>();

        public static object ConvertValue(object value, Type srcType, Type destType) {
            var method = GetConvertValueMethod(srcType, destType);
            // staticメソッド利用時は第1引数はnull
            return method.Invoke(null, new object[] { value });
        }

        /// <summary>
        ///  srcのプロパティ値をコピーしたTのインスタンスを生成（プロパティ名が一致する対象同士で値コピー）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public static T CreateObject<T>(object src) where T : new() {
            if (ReferenceEquals(src, null)) return default(T);

            // デフォルトコンストラクタがある場合は、Activator.CreateInstanceを使うのが楽
            var dest = Activator.CreateInstance<T>();
            return (T)CopyProperties(src, dest);
        }

        /// <summary>
        ///  src→destへのプロパティ値コピー（プロパティ名が一致する対象同士で値コピー）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static object CopyProperties(object src, object dest) {
            if (!ReferenceEquals(src, null) && !ReferenceEquals(dest, null)) {
                var props1 = GetProperties(src.GetType());
                var props2 = GetProperties(dest.GetType());
                // プロパティ名が一致する対象同士でコピーする
                var items = props1.Join(props2, p1 => p1.Name, p2 => p2.Name, (x, y) => {
                    return new { src = x, dest = y };
                });

                foreach (var item in items) {
                    if (!item.src.CanRead || !item.dest.CanWrite) {
                        // CanRead→プロパティがgetありならtrue
                        // CanWrite→プロパティがsetありならtrue
                        continue;
                    }

                    // コピープロパティからの値取得
                    var value = item.src.GetValue(src);
                    // 型変換
                    var converted = ConvertValue(value, item.src.PropertyType, item.dest.PropertyType);
                    // コピー先への値設定
                    item.dest.SetValue(dest, converted);
                }
            }
            return dest;
        }

        private static PropertyInfo[] GetProperties(Type t) {
            if (PropertyCache.TryGetValue(t, out var props)) {
                return props;
            }
            props = t.GetProperties();
            PropertyCache.TryAdd(t, props);
            return props;
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
