
using UnityEditor;


[CustomPropertyDrawer(typeof(Vector3IntAreaDataDictionary))]
[CustomPropertyDrawer(typeof(StringFloatDictionary))]
[CustomPropertyDrawer(typeof(StringStringDictionary))]
[CustomPropertyDrawer(typeof(ObjectColorDictionary))]
[CustomPropertyDrawer(typeof(StringColorArrayDictionary))]
[CustomPropertyDrawer(typeof(EviromentAPIEviromentDictionary))]
[CustomPropertyDrawer(typeof(StringSoundSettingDictionary))]
[CustomPropertyDrawer(typeof(StringSoulDictionary))]
[CustomPropertyDrawer(typeof(DirectionStringDictionary))]
[CustomPropertyDrawer(typeof(StringQuestDictionary))]
[CustomPropertyDrawer(typeof(StringGameEventDictionary))]


public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }

[CustomPropertyDrawer(typeof(ColorArrayStorage))]
public class AnySerializableDictionaryStoragePropertyDrawer : SerializableDictionaryStoragePropertyDrawer { }
