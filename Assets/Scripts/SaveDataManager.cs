using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveDataManager
{

    public SaveMonsterData save_data;
    public string file_path;                        
    public string file_name = "SaveData.json";

    public SaveDataManager()
    {
        file_path = Application.dataPath + "/" + file_name;

        // // ファイルがないとき、ファイル作成
        // if (!File.Exists(file_path)) {
        //     Save(save_data);
        // }

        // // ファイルを読み込んでdataに格納
        // save_data = Load(filepath); 
    }

    public void Save(SaveMonsterData save_data)
    {
        string json = JsonUtility.ToJson(save_data);
        StreamWriter wr = new StreamWriter(file_path, false);    // ファイル書き込み指定
        wr.WriteLine(json);                                     // json変換した情報を書き込み
        wr.Close(); 
    }

    public SaveMonsterData Load()
    {
        if (!File.Exists(file_path)) {
            return null;
        }
        StreamReader rd = new StreamReader(file_path);               // ファイル読み込み指定
        string json = rd.ReadToEnd();                           // ファイル内容全て読み込む
        rd.Close();                                             // ファイル閉じる
        if (json == "") {
            Debug.Log("empty");
            return null;           
        }                                           
        return JsonUtility.FromJson<SaveMonsterData>(json);
    }
}
