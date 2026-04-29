using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    public class Dragon : BoneObject
    {
        #region Properties
        private static string[] _characterNames = {
#if BIGBOARD_EVERLAND
            "Everland_Lenny",
            "Everland_Jack",     
            "Everland_Bagle",      
            "Everland_Dona",      
            "Everland_Rara",                                                   
#endif
            "Pico",
            "Arrow",
            "Cougar",
            "Hansen"
    };

        public static string[] characterNames
        {
            get
            {
                return _characterNames;
            }
        }

        private DragonAnimationControl _dragonAnimation = null;
        public DragonAnimationControl dragonAnimation
        {
            get
            {
                if (_dragonAnimation == null)
                {
                    _dragonAnimation = GetComponent<DragonAnimationControl>();
                }
                return _dragonAnimation;
            }
        }
        #endregion

        public static GameObject LoadPrefab(string characterName)
        {
            GameObject prefab = Resources.Load("Dragons/" + characterName, typeof(GameObject)) as GameObject;
            return prefab;
        }

        public static string GetLocalizedCharacterName(string characterName)
        {
            if (characterName != null)
            {
                switch (characterName)
                {
                    case "Pico":
                        return LocalizationManager.GetData(LocalizationKey.PUBLIC_CHARACTER_PICO);
                    case "Arrow":
                        return LocalizationManager.GetData(LocalizationKey.PUBLIC_CHARACTER_ARROW);
                    case "Cougar":
                        return LocalizationManager.GetData(LocalizationKey.PUBLIC_CHARACTER_COUGAR);
                    case "Hansen":
                        return LocalizationManager.GetData(LocalizationKey.PUBLIC_CHARACTER_HANSEN);

                    case "Everland_Lenny":
                        return LocalizationManager.GetData(LocalizationKey.PUBLIC_CHARACTER_LENNY);
                    case "Everland_Jack":
                        return LocalizationManager.GetData(LocalizationKey.PUBLIC_CHARACTER_JACK);
                    case "Everland_Bagle":
                        return LocalizationManager.GetData(LocalizationKey.PUBLIC_CHARACTER_BAGLE);
                    case "Everland_Dona":
                        return LocalizationManager.GetData(LocalizationKey.PUBLIC_CHARACTER_DONA);
                    case "Everland_Rara":
                        return LocalizationManager.GetData(LocalizationKey.PUBLIC_CHARACTER_RARA);
                }
            }

            return characterName;
        }
    }
}