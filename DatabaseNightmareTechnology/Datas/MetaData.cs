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

        /// <summary>
        /// 名前Camel
        /// </summary>
        public string NameCamel { get; set; }

        /// <summary>
        /// 名前Pascal
        /// </summary>
        public string NamePascal { get; set; }
        /// <summary>
        /// コメント
        /// </summary>
        public string Comment { get; set; }

        // キー項目
        /// <summary>
        /// キー項目のDBの型
        /// DBから取得したカッコつきの値を格納
        /// </summary>
        public string KeyDataTypeRaw { get; set; }

        /// <summary>
        /// キー項目のDBの型
        /// カッコ除外
        /// </summary>
        public string KeyDataType { get; set; }

        /// <summary>
        /// キー項目の名前
        /// </summary>
        public string KeyName { get; set; }

        /// <summary>
        /// キー項目の名前Camel
        /// </summary>
        public string KeyNamePascal { get; set; }

        /// <summary>
        /// キー項目の名前Pascal
        /// </summary>
        public string KeyNameCamel { get; set; }

        // インデックス情報
        /// <summary>
        /// インデックスの種類ごとに項目を列挙
        /// </summary>
        public Dictionary<string, List<string>> Indexs { get; set; }

        /// <summary>
        /// 各インデックスに登場する項目を列挙
        /// </summary>
        public List<string> IndexColumns { get; set; }
    }

    /// <summary>
    /// カラムデータ
    /// </summary>
    public class Column
    {
        /// <summary>
        /// 元の名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 名前Camel
        /// </summary>
        public string NameCamel { get; set; }

        /// <summary>
        /// 名前Pascal
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
        /// PascalCase
        /// 例：user_auth -> UserAuth
        /// </summary>
        public string IndexClass { get; set; }

    }

}
