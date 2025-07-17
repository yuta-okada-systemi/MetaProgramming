using System;
using MetaProgramming.Samples;

namespace Samples.Tests
{
    public class ReflectionDxoTest
    {
        [Fact]
        public void CreateObjectTest1()
        {
            var src = new StringModel()
            {
                StringValue = "abcdefg",
                IntValue = "123456",
                DoubleValue = "123.456",
                DatetimeValue = "2025/07/18 18:15:00"
            };
            var dest = ReflectionDxo.CreateObject<TypeModel>(src);

            Assert.NotNull(dest);
            Assert.Equal("abcdefg", dest.StringValue);
            Assert.Equal(123456, dest.IntValue);
            Assert.Equal(123.456, dest.DoubleValue);
            Assert.Equal(new DateTime(2025, 7, 18, 18, 15, 0), dest.DatetimeValue);
        }

        [Fact]
        public void CreateObjectTest2()
        {
            var src = new StringModel()
            {
                StringValue = "abcdefg",
                IntValue = "123456",
                DoubleValue = "123.456",
                DatetimeValue = "2025/07/18 18:15:00"
            };
            var dest = ReflectionDxo.CreateObject<NullableTypeModel>(src);

            Assert.NotNull(dest);
            Assert.Equal("abcdefg", dest.StringValue);
            Assert.Equal(123456, dest.IntValue);
            Assert.Equal(123.456, dest.DoubleValue);
            Assert.Equal(new DateTime(2025, 7, 18, 18, 15, 0), dest.DatetimeValue);
        }
    }
}