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
        public Dictionary<string, string> Values { get; set; }
        public Dictionary<string, Dictionary<string, string>> ListedValues { get; set; }

        private void Initialize()
        {
            Values = new Dictionary<string, string>();
            ListedValues = new Dictionary<string, Dictionary<string, string>>();
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
        public GeneralInput(List<ValueData> items)
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
        private void ReadRawData(List<ValueData> raws)
        {
            // '#'区切りを処理する
            foreach (var item in raws)
            {
                if (item.Key.Contains(Separator))
                {
                    // リスト化対象パラメータ
                    var names = item.Key.Split(Separator);
                    var key = names[0];
                    var listname = names[1];
                    if (!ListedValues.ContainsKey(key))
                    {
                        ListedValues.Add(key, new Dictionary<string, string>());
                    }
                    ListedValues[key].Add(listname, item.Value);
                }
                else
                {
                    // 単独パラメータ
                    if (!Values.ContainsKey(item.Key))
                    {
                        // 重複しているものは無視
                        Values.Add(item.Key, item.Value);
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
        public List<ValueData> Datas { get; set; }

        private void Initialize()
        {
            Datas = new List<ValueData>();
        }

        /// <summary>
        /// 処理用データからのコンバート
        /// </summary>
        /// <param name="generalInput">処理用データ</param>
        public RawGeneralInput(GeneralInput generalInput)
        {
            Initialize();

            // コンバート処理
            foreach (var item in generalInput.ListedValues)
            {
                foreach (var subitem in item.Value)
                {
                    var nameValueData = new ValueData
                    {
                        Key = item.Key + "#" + subitem.Key,
                        Value = subitem.Value
                    };
                    Datas.Add(nameValueData);
                }
            }

            foreach (var item in generalInput.Values)
            {
                var nameValueData = new ValueData
                {
                    Key = item.Key,
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
                        var row = new ValueData
                        {
                            Key = values[0],
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
    public class ValueData
    {
        /// <summary>
        /// 1列目
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 2列目
        /// </summary>
        public string Value { get; set; }
    }
}
