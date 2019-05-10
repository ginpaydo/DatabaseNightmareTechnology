# DatabaseNightmareTechnology
WPF,Prism.Unity製。  
接続したデータベースからテーブルやカラムの情報を取得し、O/Rマッピング用のソース生成ができる。  
Razorエンジンを使った簡易なテンプレート文書の生成。  

イメージとしてはこういう感じ。  
**コードテンプレート(Razor) + CSVによるパラメータ指定 + <DB接続> = コード生成**  

自分用です。説明は適当です。  

### ちょっと思うこと
Webアプリでも実現できそうだが、あえてWindowsネイティブアプリにしたのは何でだろう？って自分でも思う。  
元々動機の1つがWPFの練習だった…というのが主な理由。  
次点で、Webアプリから余所のDBに繋ぎに行ってスキーマ情報を取得する…という光景が想像できない。  
DBサーバのホスト、ID、パスワードがあれば繋がるが、開発サーバがネット接続されているわけがない。  
（あと現時点でそんな機能を持つWebクライアントを作る技能はない）  
なので、この機能を実現するには開発PCに持ち込めるアプリでなければならなかった。  

*最近、便利なエディタを思いついたので、これと何が違うか気になって頭の整理のために書きました。*

## はじめに
### 前提条件
Windows10で動作確認をしています。
最新の.NET Frameworkとかインストールしておいてください。
Dropboxが使えるようになっていますが、ローカルのみでも使用できます。
### インストール方法
ビルドしてできたファイルを適当な場所に配置すればOKです。

## 使用方法
自分用なので適当に説明します。
### HOME
![起動した状態](https://user-images.githubusercontent.com/39305262/53282792-2190f380-3780-11e9-9cb8-07eeb0c5f8ae.PNG "起動した状態")  
### Dropbox設定
![Dropbox設定](https://user-images.githubusercontent.com/39305262/53282817-759bd800-3780-11e9-90d8-a3a8093f89f2.png "Dropbox設定")  
### 接続先登録
![接続先登録](https://user-images.githubusercontent.com/39305262/53282818-77fe3200-3780-11e9-8a1a-de19127974a7.png "接続先登録")  
### メタデータ生成
![メタデータ生成](https://user-images.githubusercontent.com/39305262/53282819-792f5f00-3780-11e9-8d2a-14b8b349b6ff.png "メタデータ生成")  
### テンプレート編集
![テンプレート編集](https://user-images.githubusercontent.com/39305262/53282820-7af92280-3780-11e9-9341-4a68d5e572de.png "テンプレート編集")  
### 汎用入力
![汎用入力](https://user-images.githubusercontent.com/39305262/53282821-7d5b7c80-3780-11e9-8295-b690a8cf2567.png "汎用入力")  
### ソース生成
![ソース生成](https://user-images.githubusercontent.com/39305262/53282823-7f254000-3780-11e9-8764-b211f76b05d8.png "ソース生成")  
### 出力結果
![出力結果](https://user-images.githubusercontent.com/39305262/53282824-81879a00-3780-11e9-9d8b-4fdb050e3709.png "出力結果")  
## テストの実行
ないです。
## 協働するシステムのリスト
※以下と協働しますが、なくてもコードジェネレータに使えます。    
MySQL,MariaDB  
SQLServer  
DropBox
## コントリビューション
プルリクとか特に考えてないです。
## バージョン管理
特に管理方法などはないです。
## 著者
* **Ginpaydo** - *原著者* - [Ginpaydo](https://github.com/ginpaydo)  

このプロジェクトへの[貢献者](https://github.com/ginpaydo/DatabaseNightmareTechnology/contributors)のリストもご覧ください。
## ライセンス
このプロジェクトは MIT ライセンスの元にライセンスされています。 詳細は[LICENSE.md](LICENSE.md)をご覧ください。
## 謝辞
* 自分で考えたので特にないです。
