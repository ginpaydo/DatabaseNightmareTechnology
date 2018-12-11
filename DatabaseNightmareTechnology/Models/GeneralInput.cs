using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    /// <summary>
    /// 汎用入力
    /// 処理用
    /// </summary>
    public class GeneralInput
    {
        public static char Separator = '#';
        public Dictionary<string, string> KeyValues { get; set; }
        public Dictionary<string, Dictionary<string, string>> ListedKeyValues { get; set; }

        private void Initialize()
        {
            KeyValues = new Dictionary<string, string>();
            ListedKeyValues = new Dictionary<string, Dictionary<string, string>>();
        }

        public GeneralInput()
        {
            Initialize();
        }

        /// <summary>
        /// 汎用入力
        /// 画面表示中のデータからのコンバート
        /// </summary>
        /// <param name="items">画面表示中のデータ</param>
        public GeneralInput(List<NameValueData> items)
        {
            Initialize();

            ReadRawData(items);
        }

        /// <summary>
        /// 汎用入力
        /// 表示用データからのコンバート
        /// </summary>
        /// <param name="raw">CSVから読みこんだデータ</param>
        public GeneralInput(RawGeneralInput raw)
        {
            Initialize();

            ReadRawData(raw.Datas);
        }

        /// <summary>
        /// Rawデータを読む
        /// </summary>
        /// <param name="raws">CSVから読みこんだデータ</param>
        private void ReadRawData(List<NameValueData> raws)
        {
            // '#'区切りを処理する
            foreach (var item in raws)
            {
                if (item.Name.Contains(Separator))
                {
                    // リスト化対象パラメータ
                    var names = item.Name.Split(Separator);
                    var key = names[0];
                    var listname = names[1];
                    if (!ListedKeyValues.ContainsKey(key))
                    {
                        ListedKeyValues.Add(key, new Dictionary<string, string>());
                    }
                    ListedKeyValues[key].Add(listname, item.Value);
                }
                else
                {
                    // 単独パラメータ
                    if (!KeyValues.ContainsKey(item.Name))
                    {
                        // 重複しているものは無視
                        KeyValues.Add(item.Name, item.Value);
                    }
                }
            }
        }
    }

    /// <summary>
    /// CSVからの入力
    /// 画面表示用
    /// </summary>
    public class RawGeneralInput
    {
        public static char Separator = ',';
        public List<NameValueData> Datas { get; set; }

        private void Initialize()
        {
            Datas = new List<NameValueData>();
        }

        /// <summary>
        /// 処理用データからのコンバート
        /// </summary>
        /// <param name="generalInput">処理用データ</param>
        public RawGeneralInput(GeneralInput generalInput)
        {
            Initialize();

            // コンバート処理
            foreach (var item in generalInput.ListedKeyValues)
            {
                foreach (var subitem in item.Value)
                {
                    var nameValueData = new NameValueData
                    {
                        Name = item.Key + "#" + subitem.Key,
                        Value = subitem.Value
                    };
                    Datas.Add(nameValueData);
                }
            }

            foreach (var item in generalInput.KeyValues)
            {
                var nameValueData = new NameValueData
                {
                    Name = item.Key,
                    Value = item.Value
                };
                Datas.Add(nameValueData);
            }

        }

        /// <summary>
        /// CSVからの入力データ
        /// </summary>
        /// <param name="fullpath">CSVファイルフルパス</param>
        public RawGeneralInput(string fullpath)
        {
            Initialize();

            // csvファイルを開く
            using (var sr = new System.IO.StreamReader(fullpath))
            {
                // ストリームの末尾まで繰り返す
                while (!sr.EndOfStream)
                {
                    // ファイルから一行読み込む
                    var line = sr.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        // 読み込んだ一行をカンマ毎に分けて配列に格納する
                        var values = line.Split(Separator);
                        // 出力する
                        var row = new NameValueData
                        {
                            Name = values[0],
                            Value = values.Length > 1 ? values[1] : string.Empty
                        };
                        Datas.Add(row);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 1件（行）のデータ
    /// これを拡張すれば、何でも入りそうだね
    /// </summary>
    public class NameValueData
    {
        /// <summary>
        /// 1列目
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 2列目
        /// </summary>
        public string Value { get; set; }
    }
}
