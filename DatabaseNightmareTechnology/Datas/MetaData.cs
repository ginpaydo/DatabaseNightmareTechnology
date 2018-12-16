using System.Collections.Generic;

namespace DatabaseNightmareTechnology
{
    /// <summary>
    /// データベースから収集したメタデータ
    /// </summary>
    public class MetaData
    {
        /// <summary>
        /// テーブル
        /// </summary>
        public List<Table> Tables { get; set; }

        /// <summary>
        /// 汎用入力を適用する
        /// </summary>
        public GeneralInput GeneralInput
        {
            set
            {
                if(Tables != null)
                {
                    foreach (var item in Tables)
                    {
                        item.GeneralInput = value;
                    }
                }
            }
        }
    }

    public class Table
    {
        /// <summary>
        /// 汎用入力
        /// 保存はせず、ソース生成直前で別データから適用して使う
        /// </summary>
        public GeneralInput GeneralInput { get; set; }

        /// <summary>
        /// カラム
        /// </summary>
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
        public string KeyDataType { get; set; } // DBの型
        public string KeyName { get; set; }
        public string KeyNamePascal { get; set; }
        public string KeyNameCamel { get; set; }
    }

    /// <summary>
    /// カラムデータ
    /// </summary>
    public class Column
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
        /// 型（DBの型）カッコ除外
        /// </summary>
        public string DataType { get; set; }

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
