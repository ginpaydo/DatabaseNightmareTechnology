using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DatabaseNightmareTechnology.Models
{
    /// <summary>
    /// Modelの共通親クラス
    /// Dropboxと、独自設定ファイルを使う場合の設計
    /// </summary>
    public abstract class ModelBase : BindableBase
    {
        #region Fields
        /// <summary>
        /// 設定ファイル
        /// </summary>
        protected SaveData SaveData { get; set; }

        private string actionResult;
        /// <summary>
        /// 処理結果メッセージの表示
        /// </summary>
        public string ActionResult
        {
            get { return actionResult; }
            set { SetProperty(ref actionResult, value); }
        }

        /// <summary>
        /// 処理成功時のメッセージ
        /// </summary>
        protected string SuccessMessage { get; set; }
        /// <summary>
        /// 入力チェック失敗時のメッセージ
        /// </summary>
        protected string RequiredMessage { get; set; }
        #endregion

        #region Initialize
        public ModelBase()
        {
            ActionResult = MessageConstants.ActionResult;
            RequiredMessage = MessageConstants.NotInputFilename;
            SuccessMessage = MessageConstants.ActionSucceed;
        }
        #endregion

        /// <summary>
        /// 各処理の際に必須入力チェックを行う
        /// 必須項目がある画面ではオーバーライドしてチェック処理を追加する
        /// </summary>
        /// <returns>チェックOKならtrue</returns>
        protected abstract bool CheckRequiredFields();

        /// <summary>
        /// 単純な保存処理
        /// </summary>
        /// <typeparam name="T">保存するデータ型</typeparam>
        /// <param name="correspondencePath">保存先相対パス("/Data"など、Dropboxまたはローカル保存ディレクトリに対するパス)</param>
        /// <param name="filename">拡張子付きファイル名</param>
        /// <param name="data">保存データ</param>
        /// <param name="dataList">保存成功したらこのリストにファイル名を追加する（指定しなくても良い）</param>
        /// <returns>処理に成功したらtrue</returns>
        protected async Task<bool> SimpleSave<T>(string correspondencePath, string filename, T data, ObservableCollection<string> dataList = null)
        {
            if (SaveData != null)
            {
                if (CheckRequiredFields())
                {
                    // OKならDropboxかローカルに保存
                    await DropboxHelper.MultiSaveAsync(SaveData.DataOutput, filename, data, Constants.ApplicationDirectoryDropbox + correspondencePath, SaveData.LocalDirectory + correspondencePath, SaveData.AccessToken);
                    ActionResult = MessageConstants.ActionSucceed;

                    // 成功したらリストに追加する
                    if (dataList != null)
                    {
                        if (!dataList.Contains(filename))
                        {
                            dataList.Add(filename);
                        }
                    }
                    return true;
                }
                else
                {
                    ActionResult = RequiredMessage;
                    return false;
                }
            }
            else
            {
                ActionResult = MessageConstants.SaveDataNotFound;
                return false;
            }
        }

        /// <summary>
        /// 単純な文字列保存
        /// </summary>
        /// <param name="correspondencePath"></param>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        /// <param name="dataList"></param>
        /// <returns></returns>
        protected async Task<bool> SimpleSaveString(string correspondencePath, string filename, string data, ObservableCollection<string> dataList = null)
        {
            if (SaveData != null)
            {
                if (CheckRequiredFields())
                {
                    // OKならDropboxかローカルに保存
                    await DropboxHelper.MultiSaveStringAsync(SaveData.DataOutput, filename, data, Constants.ApplicationDirectoryDropbox + correspondencePath, SaveData.LocalDirectory + correspondencePath, SaveData.AccessToken);
                    ActionResult = MessageConstants.ActionSucceed;

                    // 成功したらリストに追加する
                    if (dataList != null)
                    {
                        if (!dataList.Contains(filename))
                        {
                            dataList.Add(filename);
                        }
                    }
                    return true;
                }
                else
                {
                    ActionResult = RequiredMessage;
                    return false;
                }
            }
            else
            {
                ActionResult = MessageConstants.SaveDataNotFound;
                return false;
            }
        }

        /// <summary>
        /// 単純な削除処理
        /// 物理削除は行わず、指定したファイルを削除フォルダに移動する
        /// </summary>
        /// <param name="correspondencePath">保存先相対パス("/Data"など、Dropboxまたはローカル保存ディレクトリに対するパス)</param>
        /// <param name="filename">拡張子付きファイル名</param>
        /// <param name="dataList">保存成功したらこのリストにファイル名を追加する（指定しなくても良い）</param>
        /// <returns>処理に成功したらtrue</returns>
        protected async Task<bool> SimpleDelete(string correspondencePath, string filename, ObservableCollection<string> dataList = null)
        {
            if (SaveData != null)
            {
                if (CheckRequiredFields())
                {
                    // OKならDropboxかローカルに保存
                    await DropboxHelper.MultiDeleteFileAsync(SaveData.DataOutput, filename, Constants.TrushDirectory, correspondencePath, Constants.ApplicationDirectoryDropbox, SaveData.LocalDirectory, SaveData.AccessToken);
                    ActionResult = MessageConstants.ActionSucceed;

                    // 成功したらリストから削除する
                    if (dataList != null)
                    {
                        if (dataList.Contains(filename))
                        {
                            dataList.Remove(filename);
                        }
                    }
                    return true;
                }
                else
                {
                    ActionResult = RequiredMessage;
                    return false;
                }
            }
            else
            {
                ActionResult = MessageConstants.SaveDataNotFound;
                return false;
            }
        }

        /// <summary>
        /// ファイル名から拡張子を取り除く（汎用性低い）
        /// </summary>
        /// <param name="filename">ファイル名</param>
        /// <param name="extention">拡張子、指定なしの場合4文字取り除く</param>
        /// <returns></returns>
        protected string RemoveExtension(string filename, string extention = Constants.Extension)
        {
            return filename.Substring(0, filename.Length - extention.Length);
        }

        /// <summary>
        /// 単純な読み込み処理
        /// </summary>
        /// <typeparam name="T">データ型</typeparam>
        /// <param name="correspondencePath">保存先相対パス("/Data"など、Dropboxまたはローカル保存ディレクトリに対するパス)</param>
        /// <param name="filename">拡張子付きファイル名</param>
        /// <returns>読みこんだデータ</returns>
        protected async Task<T> SimpleLoad<T>(string correspondencePath, string filename)
        {
            return await DropboxHelper.MultiLoadAsync<T>(SaveData.DataOutput, filename, Constants.ApplicationDirectoryDropbox + correspondencePath, SaveData.LocalDirectory + correspondencePath, SaveData.AccessToken);
        }

        /// <summary>
        /// ファイルリスト作成
        /// </summary>
        /// <param name="correspondencePath">対象ディレクトリ相対パス("/Data"など、Dropboxまたはローカル保存ディレクトリに対するパス)</param>
        /// <param name="list">対象リストの参照</param>
        /// <returns></returns>
        protected async Task<ObservableCollection<string>> SimpleFileList(string correspondencePath, ObservableCollection<string> list = null)
        {
            return await DropboxHelper.GetFileListAsync(SaveData.DataOutput, Constants.ApplicationDirectoryDropbox + correspondencePath, SaveData.LocalDirectory + correspondencePath, SaveData.AccessToken, list);
        }

        /// <summary>
        /// ファイルを文字列として読みこむ
        /// </summary>
        /// <param name="correspondencePath">対象ディレクトリ相対パス("/Data"など、Dropboxまたはローカル保存ディレクトリに対するパス)</param>
        /// <param name="filename">拡張子付きファイル名</param>
        /// <returns></returns>
        protected async Task<string> SimpleLoadString(string correspondencePath, string filename)
        {
            return await DropboxHelper.MultiLoadStringAsync(SaveData.DataOutput, filename, Constants.ApplicationDirectoryDropbox + correspondencePath, SaveData.LocalDirectory + correspondencePath, SaveData.AccessToken);
        }

        /// <summary>
        /// SaveDataを使用する画面か
        /// </summary>
        abstract protected bool UseSaveData { get; }

        #region 画面が表示されたときの処理
        /// <summary>
        /// 画面が表示されたときの処理
        /// </summary>
        /// <returns></returns>
        public virtual async Task ActivateAsync()
        {
            ActionResult = MessageConstants.ActionResult;
            if (UseSaveData)
            {
                // データがあるかチェック
                SaveData = await Json.Load<SaveData>(Constants.DataDirectory, Constants.DataFileName);

                if (SaveData == null)
                {
                    // セーブデータがない場合
                    ActionResult = MessageConstants.SaveDataNotFound;
                }
            }

            await Activate();
        }

        /// <summary>
        /// 画面表示時の処理
        /// </summary>
        protected abstract Task Activate();
        #endregion

    }
}
