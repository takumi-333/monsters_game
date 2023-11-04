using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class EachMonsterData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/EachMonsterData.xls";
	private static readonly string exportPath = "Assets/Resources/each_monster_data.asset";
	private static readonly string[] sheetNames = { "1","2","3","4","5","6","7","8","9", "10","11","12","13","14"};
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			EachMonsterData data = (EachMonsterData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(EachMonsterData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<EachMonsterData> ();
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

					EachMonsterData.Sheet s = new EachMonsterData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						EachMonsterData.Param p = new EachMonsterData.Param ();
						
					cell = row.GetCell(0); p.lv = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.hp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.mp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.atk = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.def = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.magic = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.sp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.exp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.need_exp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(9); p.total_exp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(10); p.skill_point = (int)(cell == null ? 0 : cell.NumericCellValue);
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
