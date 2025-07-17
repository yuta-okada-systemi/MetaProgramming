using System;
using System.Globalization;

namespace MetaProgramming.Samples {
    static class MyTypeConverter {
        private static class DefaultConverter<SRC, DST> {
            // performance hack:generic type caching
            public static Func<SRC, DST> converter;
        }

        static MyTypeConverter() {
            var encoding = System.Text.Encoding.UTF8;

            // string⇔primitive型サポート
            RegistStringConverter(bool.Parse, value => value.ToString());
            RegistStringConverter(v => byte.Parse(v, NumberStyles.Integer | NumberStyles.AllowThousands), value => value.ToString());
            RegistStringConverter(sbyte.Parse, value => value.ToString());
            RegistStringConverter(char.Parse, value => value.ToString());
            RegistStringConverter(v => decimal.Parse(v, NumberStyles.Integer | NumberStyles.AllowThousands), value => value.ToString());
            RegistStringConverter(v => int.Parse(v, NumberStyles.Integer | NumberStyles.AllowThousands), value => value.ToString());
            RegistStringConverter(v => uint.Parse(v, NumberStyles.Integer | NumberStyles.AllowThousands), value => value.ToString());
            RegistStringConverter(v => long.Parse(v, NumberStyles.Integer | NumberStyles.AllowThousands), value => value.ToString());
            RegistStringConverter(v => ulong.Parse(v, NumberStyles.Integer | NumberStyles.AllowThousands), value => value.ToString());
            RegistStringConverter(v => short.Parse(v, NumberStyles.Integer | NumberStyles.AllowThousands), value => value.ToString());
            RegistStringConverter(v => ushort.Parse(v, NumberStyles.Integer | NumberStyles.AllowThousands), value => value.ToString());
            RegistStringConverter(v => double.Parse(v, NumberStyles.Float | NumberStyles.AllowThousands), value => value.ToString());
            RegistStringConverter(v => float.Parse(v, NumberStyles.Float | NumberStyles.AllowThousands), value => value.ToString());

            // string⇔DateTimeサポート
            RegistStringConverter(DateTime.Parse, value => value.ToString());

            // string⇔byte[]サポート
            RegistDefaultConverter<string, byte[]>(v => System.Text.Encoding.Unicode.GetBytes(v), System.Text.Encoding.Unicode.GetString);
        }

        private static void RegistStringConverter<T>(Func<string, T> parser, Func<T, string> formatter) where T : struct {
            DefaultConverter<string, T>.converter = parser;
            DefaultConverter<T, string>.converter = formatter;

            // nullable分登録
            DefaultConverter<string, T?>.converter = x => string.IsNullOrEmpty(x) ? default(T?) : parser(x);
            DefaultConverter<T?, string>.converter = x => x.HasValue ? null : formatter(x.Value);
        }

        public static void RegistDefaultConverter<SRC, DST>(Func<SRC, DST> converter) {
            DefaultConverter<SRC, DST>.converter = converter;
        }

        public static void RegistDefaultConverter<SRC, DST>(Func<SRC, DST> converter, Func<DST, SRC> converter2) {
            DefaultConverter<SRC, DST>.converter = converter;
            DefaultConverter<DST, SRC>.converter = converter2;
        }

        public static DST ConvertValue<SRC, DST>(SRC value) {
            if (value is DST dST) {
                return dST;
            }

            var converter = DefaultConverter<SRC, DST>.converter;
            if (converter is null) {
                throw new InvalidOperationException($"{typeof(SRC).FullName}→{typeof(DST).FullName}への変換はサポートされていません。");
            }
            return converter.Invoke(value);
        }
    }
}
