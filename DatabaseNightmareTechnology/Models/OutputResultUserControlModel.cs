﻿using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    class OutputResultUserControlModel : BindableBase
    {

        #region Fields
        private string body;
        /// <summary>
        /// テンプレート本体
        /// </summary>
        public string Body
        {
            get { return body; }
            set { SetProperty(ref body, value); }
        }

        private string saveResult;
        /// <summary>
        /// 保存結果
        /// </summary>
        public string SaveResult
        {
            get { return saveResult; }
            set { SetProperty(ref saveResult, value); }
        }

        /// <summary>
        /// ディレクトリリスト
        /// </summary>
        public ObservableCollection<string> DirectoryList { get; } = new ObservableCollection<string>();

        /// <summary>
        /// ファイルリスト
        /// </summary>
        public ObservableCollection<string> FileList { get; } = new ObservableCollection<string>();

        #endregion

        #region ボタン

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="value"></param>
        public void Delete(string value)
        {
        }
        #endregion

        #region 画面が表示されたときの処理
        /// <summary>
        /// 画面が表示されたときの処理
        /// </summary>
        /// <returns></returns>
        public async Task ActivateAsync()
        {
        }
        #endregion
    }
    //// ファイルを削除
    //// "text1.txt"
    //await _client.Files.DeleteV2Async("/Temp/sample/text1.txt");

    //// "text2.txt"
    //await _client.Files.DeleteV2Async("/Temp/sample/text2.txt");
    //// 接続先データ読み込み（ディレクトリのファイル一覧を取得）
    //await Json.GetFileList(DataList, SaveData.DataOutput, Constants.ApplicationDirectoryDropbox + Constants.ConnectionDirectory, SaveData.LocalDirectory + Constants.ConnectionDirectory, SaveData.AccessToken);
}
