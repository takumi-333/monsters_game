using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class skill_data_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/skill_data.xls";
	private static readonly string exportPath = "Assets/skill_data.asset";
	private static readonly string[] sheetNames = { "Sheet1", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			SkillData data = (SkillData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(SkillData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<SkillData> ();
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

					SkillData.Sheet s = new SkillData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						SkillData.Param p = new SkillData.Param ();
						
					cell = row.GetCell(0); p.id = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.name_ja = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.name_en = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.loss_hp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.loss_mp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.type = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.num_target = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.num_attack = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.element = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(9); p.power = (int)(cell == null ? 0 : cell.NumericCellValue);
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
