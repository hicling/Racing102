using System.Collections.Generic;
using UnityEngine;

namespace Racing102
{
    public class GameDataSource : MonoBehaviour
    {
        [Tooltip("All CharacterClass data should be slotted in here")]
        [SerializeField]
        private CarClass[] m_CharacterData;

        private Dictionary<CarTypeEnum, CarClass> m_CharacterDataMap;

        /// <summary>
        /// static accessor for all GameData.
        /// </summary>
        public static GameDataSource Instance { get; private set; }

        /// <summary>
        /// Contents of the CharacterData list, indexed by CharacterType for convenience.
        /// </summary>
        public Dictionary<CarTypeEnum, CarClass> CharacterDataByType
        {
            get
            {
                if (m_CharacterDataMap == null)
                {
                    m_CharacterDataMap = new Dictionary<CarTypeEnum, CarClass>();
                    foreach (CarClass data in m_CharacterData)
                    {
                        if (m_CharacterDataMap.ContainsKey(data.CarType))
                        {
                            throw new System.Exception($"Duplicate character definition detected: {data.CarType}");
                        }
                        m_CharacterDataMap[data.CarType] = data;
                    }
                }
                return m_CharacterDataMap;
            }
        }


        private void Awake()
        {
            if (Instance != null)
            {
                throw new System.Exception("Multiple GameDataSources defined!");
            }

            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }
}
