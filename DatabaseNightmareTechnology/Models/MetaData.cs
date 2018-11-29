using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    /// <summary>
    /// データベースから収集したメタデータ
    /// </summary>
    class MetaData
    {
        /// <summary>
        /// テーブル
        /// </summary>
        public List<Table> Tables { get; set; }

        // TODO:ユニーク制約などを追加する
    }

    class Table
    {
        public List<Column> Columns { get; set; }
        /// <summary>
        /// プレフィクス付きの名前
        /// </summary>
        public string RawName { get; set; }
        /// <summary>
        /// プレフィクスを取った名前
        /// </summary>
        public string Name { get; set; }
        public string NameCamel { get; set; }
        public string NamePascal { get; set; }
        public string Comment { get; set; }
        public string KeyDataTypeRaw { get; set; } // DBの型（各言語によって異なるため）
        public string KeyName { get; set; }
        public string KeyNamePascal { get; set; }
        public string KeyNameCamel { get; set; }
    }

    /// <summary>
    /// カラムデータ
    /// </summary>
    class Column
    {
        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 名前
        /// </summary>
        public string NameCamel { get; set; }

        /// <summary>
        /// 名前
        /// </summary>
        public string NamePascal { get; set; }

        /// <summary>
        /// 型（DBの型）カッコ付きなので注意
        /// </summary>
        public string DataTypeRaw { get; set; }

        /// <summary>
        /// コメント
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// データの長さ
        /// </summary>
        public string Length { get; set; }

        /// <summary>
        /// キーならばtrue
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// Null可能ならtrue
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// 他のテーブルのIDだったら、そのクラス名を入れる
        /// 例：user_auth -> UserAuth
        /// </summary>
        public string IndexClass { get; set; }

    }

}
