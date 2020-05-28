# ビルドと動作確認手順

1. DllExport.bat を実行し、設定ダイアログが出てきたらそのまま右上の [Apply] をクリック (必要なpackageがインストールされる)

   なお、このbatは DllExport 1.7.1 としてリリースされたものをそのままコピーしています。
   <https://github.com/3F/DllExport>

2. Visual Studio 2015以降で SakuraBridge.sln を開く

3. Debug構成でビルドを実行 (plugin/SakuraBridgePrototype/x86/SakuraBridge.dll が出力される)

4. plugin/SakuraBridgePrototype フォルダからnarファイルを作成

5. narをインストール