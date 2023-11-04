using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FarmDataManager
{

    public FarmData farm_data;
    public string file_path;                        
    public string file_name = "FarmData.json";

    public FarmDataManager()
    {
        file_path = Application.dataPath + "/" + file_name;
    }

    public void Save(FarmData farm_data)
    {
        string json = JsonUtility.ToJson(farm_data);
        StreamWriter wr = new StreamWriter(file_path, false);    // ファイル書き込み指定
        wr.WriteLine(json);                                     // json変換した情報を書き込み
        wr.Close(); 
    }

    public FarmData Load()
    {
        if (!File.Exists(file_path)) {
            return null;
        }
        StreamReader rd = new StreamReader(file_path);               // ファイル読み込み指定
        string json = rd.ReadToEnd();                           // ファイル内容全て読み込む
        rd.Close();                                             // ファイル閉じる
        if (json == "" || json == "\n") {
            Debug.Log("empty");
            return null;           
        }              
        return JsonUtility.FromJson<FarmData>(json);
    }
}
