using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EachMonsterData : ScriptableObject
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
		
		public int lv;
		public int hp;
		public int mp;
		public int atk;
		public int def;
		public int magic;
		public int sp;
		public int exp;
		public int need_exp;
		public int total_exp;
		public int skill_point;
	}
}

