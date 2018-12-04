using System;
using System.Linq;

namespace DatabaseNightmareTechnology
{
    /// <summary>
    /// string 型の拡張メソッドを管理するクラス
    /// ソース固定で良い
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// スネークケースをアッパーキャメル(パスカル)ケースに変換します
        /// 例) quoted_printable_encode → QuotedPrintableEncode
        /// </summary>
        public static string SnakeToUpperCamel(this string self)
        {
            if (string.IsNullOrEmpty(self)) return self;

            return self
                .Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
                .Aggregate(string.Empty, (s1, s2) => s1 + s2);
        }

        /// <summary>
        /// スネークケースをローワーキャメル(キャメル)ケースに変換します
        /// 例) quoted_printable_encode → quotedPrintableEncode
        /// </summary>
        public static string SnakeToLowerCamel(this string self)
        {
            if (string.IsNullOrEmpty(self)) return self;

            return self
                .SnakeToUpperCamel()
                .Insert(0, char.ToLowerInvariant(self[0]).ToString()).Remove(1, 1);
        }
    }
}
