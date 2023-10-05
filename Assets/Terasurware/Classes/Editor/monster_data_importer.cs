using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class monster_data_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/monster_data.xls";
	private static readonly string exportPath = "Assets/monster_data.asset";
	private static readonly string[] sheetNames = { "Sheet1", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			MonsterData data = (MonsterData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(MonsterData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<MonsterData> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					MonsterData.Sheet s = new MonsterData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						MonsterData.Param p = new MonsterData.Param ();
						
					cell = row.GetCell(0); p.id = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.name_ja = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.name_en = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.hp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.mp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.atk = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.def = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.sp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.exp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(9); p.image_path = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(10); p.weight = (int)(cell == null ? 0 : cell.NumericCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
