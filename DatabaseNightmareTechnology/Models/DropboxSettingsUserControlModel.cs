using Dropbox.Api;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    class DropboxSettingsUserControlModel : ModelBase
    {
        #region Fields

        #region SaveData
        private string localDirectory;
        /// <summary>
        /// ローカルディレクトリ
        /// </summary>
        public string LocalDirectory
        {
            get
            {
                localDirectory = SaveData.LocalDirectory;
                return localDirectory;
            }
            set
            {
                SaveData.LocalDirectory = value;
                SetProperty(ref localDirectory, value);
            }
        }

        private string accessToken;
        /// <summary>
        /// トークン
        /// </summary>
        public string AccessToken
        {
            get
            {
                accessToken = SaveData.AccessToken;
                return accessToken;
            }
            set
            {
                SaveData.AccessToken = value;
                SetProperty(ref accessToken, value);
            }
        }

        private DataOutput dataOutput;
        /// <summary>
        /// データ出力先
        /// </summary>
        public DataOutput DataOutput
        {
            get
            {
                dataOutput = SaveData.DataOutput;
                return dataOutput;
            }
            set
            {
                SaveData.DataOutput = value;
                SetProperty(ref dataOutput, value);
            }
        }
        #endregion

        private string userName;
        /// <summary>
        /// ユーザ名
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName, value); }
        }

        private string email;
        /// <summary>
        /// Emailアドレス
        /// </summary>
        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        /// <summary>
        /// セーブデータ使用画面
        /// </summary>
        protected override bool UseSaveData
        {
            get { return true; }
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DropboxSettingsUserControlModel()
        {
            SaveData = new SaveData();
            UserName = string.Empty;
            Email = string.Empty;
        }

        #region ボタン
        /// <summary>
        /// 確認＆保存ボタン
        /// </summary>
        /// <returns></returns>
        public async Task CheckAndSave()
        {
            try
            {
                if (DataOutput == DataOutput.Dropbox)
                {
                    // Dropboxアカウント情報を取得
                    await LoadUserProfileAsync();
                }
                else
                {
                    if (string.IsNullOrEmpty(LocalDirectory))
                    {
                        LocalDirectory = Constants.DataDirectory;
                    }
                }
                // 設定を保存する
                await Json.Save(SaveData, Constants.DataDirectory, Constants.DataFileName);
                ActionResult = "チェックOK 保存完了";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                UserName = "このハゲー！";
                Email = "ちーがーうーだーろー！？　違うだろーーッ！！";
                ActionResult = "チェックNG";
            }
        }

        protected override bool CheckRequiredFields()
        {
            return true;
        }
        #endregion

        /// <summary>
        /// Dropboxアカウント情報を取得
        /// </summary>
        /// <returns></returns>
        private async Task LoadUserProfileAsync()
        {
            using (var dbx = new DropboxClient(AccessToken))
            {
                // 情報を取得
                try
                {
                    var full = await dbx.Users.GetCurrentAccountAsync();
                    UserName = full.Name.DisplayName;
                    Email = full.Email;
                }
                catch(Exception e)
                {
                    ActionResult = "このトークン、もう使えないみたいだぜ？：" + e.Message;
                }
            }
        }

        #region 画面が表示されたときの処理
        /// <summary>
        /// 画面が表示されたときの処理
        /// </summary>
        /// <returns></returns>
        public override async Task ActivateAsync()
        {
            // データがあるかチェック
            var data = await Json.Load<SaveData>(Constants.DataDirectory, Constants.DataFileName);

            if (data == null)
            {
                // セーブデータがない場合
                AccessToken = "DropboxのWebで発行したトークンを入力してくれよな！";
                DataOutput = DataOutput.Dropbox;
                LocalDirectory = Constants.ApplicationDirectoryLocal;
            }
            else
            {
                // ロードした値を反映させる
                Reflection(data);

                // セーブデータがある場合、ユーザ情報を取得する
                if (DataOutput == DataOutput.Dropbox)
                {
                    await LoadUserProfileAsync();
                }
            }
        }
        /// <summary>
        /// 画面が表示されたときの処理
        /// </summary>
        /// <returns></returns>
        protected override async Task Activate()
        {
            // 何もしない
            await Task.Delay(1);
        }

        /// <summary>
        /// 読みこんだデータを反映する
        /// </summary>
        /// <param name="saveData"></param>
        private void Reflection(SaveData saveData)
        {
            SaveData = saveData;
            AccessToken = saveData.AccessToken;
            LocalDirectory = saveData.LocalDirectory;
            DataOutput = saveData.DataOutput;
        }
        #endregion
    }
}
