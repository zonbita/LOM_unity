
using com.wao.core;
using com.wao.rpgs;
using System;
using UnityEngine;

[Serializable]
public class StringQuestDictionary : SerializableDictionary<string, Quest> { }
[Serializable]
public class StringSoulDictionary : SerializableDictionary<string, Soul> { }
[Serializable]
public class StringGameEventDictionary : SerializableDictionary<string, GameEvent> { }
[Serializable]
public class Vector3IntAreaDataDictionary : SerializableDictionary<Vector3Int, AreaData> { }
[Serializable]
public class StringComponentDictionary : SerializableDictionary<string, Component> { }
[Serializable]
public class StringStringDictionary : SerializableDictionary<string, string> { }
[Serializable]
public class DirectionStringDictionary : SerializableDictionary<Direction, string> { }

[Serializable]
public class StringFloatDictionary : SerializableDictionary<string, float> { }

[Serializable]
public class ObjectColorDictionary : SerializableDictionary<UnityEngine.Object, Color> { }

[Serializable]
public class ColorArrayStorage : SerializableDictionary.Storage<Color[]> { }

[Serializable]
public class StringColorArrayDictionary : SerializableDictionary<string, Color[], ColorArrayStorage> { }

[Serializable]
public class MyClass
{
    public int i;
    public string str;
}

[Serializable]
public class QuaternionMyClassDictionary : SerializableDictionary<Quaternion, MyClass> { }


[Serializable]
public class EviromentAPIEviromentDictionary : SerializableDictionary<com.wao.core.EEnviroment, com.wao.core.APIEnviroment> { }

[Serializable]
public class StringSoundSettingDictionary : SerializableDictionary<string, SoundSetting> { }

