using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionData : ScriptableObject
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
		public int loss_hp;
		public int loss_mp;
		public int type;
		public int num_target;
		public int num_attack;
		public int element;
		public int power;

        public Param ShallowCopy() {
            return (Param) this.MemberwiseClone();
        }
	}
}

