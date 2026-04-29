using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    public class CharacterManager : MonoBehaviour
    {
        public Transform makePositionParent;

        private static Transform[] makePositions;

        // Object
        private static Dictionary<int, List<DragonAnimationControl>> _makeObjectDictionary = new Dictionary<int, List<DragonAnimationControl>>();

        private static List<DragonAnimationControl> listDefault = new List<DragonAnimationControl>();

        public static Transform parent;

        void Start()
        {
            parent = transform;

            makePositions = new Transform[makePositionParent.childCount];
            for (int i = 0; i < makePositionParent.childCount; i++)
                makePositions[i] = makePositionParent.GetChild(i);

            _makeObjectDictionary.Clear();
            listDefault.Clear();

            // default characters
            for (int i = 0; i < Dragon.characterNames.Length; i++)
            {
                int count = 3;
                if (Dragon.characterNames[i].Equals("Hansen"))
                    count = 5;

                for (int j = 0; j < count; j++)
                    MakeDefaultCharacter(Dragon.characterNames[i], j, Vector3.zero + Vector3.right * 5f);
            }

            // recent template 3d characters
            MakeRecentTemplate3DCharacters();
        }

        static DragonAnimationControl MakeDefaultCharacter(string characterName, int skyPathNumber, Vector3 startWorldPos)
        {
            GameObject go = (GameObject)Instantiate(Dragon.LoadPrefab(characterName));

            go.transform.parent = parent;
            go.transform.localScale = Vector3.one * 1.3f;
            go.transform.position = startWorldPos;

            if (characterName.Equals("Hansen"))
            {
                UnityEngine.AI.NavMeshAgent nav = go.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (nav != null)
                    Destroy(nav);

                DragonPath path = go.GetComponent<DragonPath>();
                if (path != null)
                    Destroy(path);

                DragonSkyPath sky = go.AddComponent<DragonSkyPath>();
                if (sky != null)
                    sky.pathNumber = skyPathNumber % 5;
            }

            go.AddComponent<DragonComeToFront>();
            DragonAnimationControl control = go.GetComponent<DragonAnimationControl>();
            if (control != null)
                listDefault.Add(control);

            return control;
        }

        public static void MakeRecentTemplate3DCharacters()
        {
            ResourceManager.IdentifierInfo[] identifiers = ResourceManager.GetRecentTemplate3DIdentifiers(0, -1, SimpleInstantiatedTemplateControl.maxTemplateCount);
            for (int i = 0; i < identifiers.Length; i++)
            {
                var info = identifiers[i];
                Template3D template = SimpleInstantiatedTemplateControl.LoadTemplate(info.identifier);
                if (template != null)
                {
                    DragonAnimationControl dragonAnimation = template.GetComponent<DragonAnimationControl>();
                    if (dragonAnimation != null && !(dragonAnimation is FreeDrawingAnimationControl))
                    {
                        AddMakeObject(info.userId, dragonAnimation);
                    }
                }
            }
        }

        public static void DestroyDefaultCharacter(DragonAnimationControl control)
        {
            if (listDefault.Contains(control))
            {
                // remove
                listDefault.Remove(control);

                // add
                // sky character check
                int skyPathNumber = 0;
                DragonSkyPath skyPath = control.GetComponent<DragonSkyPath>();
                if (skyPath != null)
                    skyPathNumber = skyPath.pathNumber;

                // start position
                Vector3 startPos = makePositions[Random.Range(0, makePositions.Length)].transform.position;
                DragonAnimationControl makeControl = MakeDefaultCharacter(control.dragon.characterName, skyPathNumber, startPos);
                makeControl.Wake(true);
            }
        }

        public static void AddMakeObject(int userID, DragonAnimationControl control)
        {
            if (!_makeObjectDictionary.ContainsKey(userID))
                _makeObjectDictionary.Add(userID, new List<DragonAnimationControl>());

            control.transform.parent = parent;

            List<DragonAnimationControl> list = _makeObjectDictionary[userID];
            list.Add(control);
        }

        public static DragonAnimationControl GetMakeObject(int userID)
        {
            DragonAnimationControl dragon = null;

            if (_makeObjectDictionary.ContainsKey(userID))
            {
                List<DragonAnimationControl> list = _makeObjectDictionary[userID];
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] != null)
                        {
                            if (list[i].GetComponent<FreeDrawingAnimationControl>() == false)
                                if (list[i].transform.parent == parent)
                                    dragon = list[i];
                        }
                        else
                        {
                            list.RemoveAt(i--);
                        }
                    }
                }
            }

            if (dragon == null)
            {
                List<DragonAnimationControl> listUseAble = new List<DragonAnimationControl>();
                for (int i = 0; i < listDefault.Count; i++)
                    if (listDefault[i].transform.parent == parent)
                        listUseAble.Add(listDefault[i]);

                int index = Random.Range(0, listUseAble.Count);
                dragon = listUseAble[index];
                DestroyDefaultCharacter(dragon);
            }

            return dragon;
        }
    }
}