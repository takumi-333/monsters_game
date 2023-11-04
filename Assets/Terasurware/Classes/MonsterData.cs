using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterData : ScriptableObject
{	
	public List<Sheet> sheets = new List<Sheet> ();

	[System.SerializableAttribute]
	public class Sheet
	{
		public string name = string.Empty;
		public List<Param> list = new List<Param>();
	}

	[System.SerializableAttribute]
	public class Param
	{
		
		public int id;
		public string name_ja;
		public string name_en;
		public int hp;
		public int mp;
		public int atk;
		public int def;
		public int magic;
		public int sp;
		public int exp;
		public string image_path;
		public int weight;

        public Param ShallowCopy() {
            return (Param) this.MemberwiseClone();
        }
	}
}

