namespace Kathulhu {

    using UnityEngine;
    using System.Collections;

    public class GameController : MonoBehaviour 
    {

        public static GameController Instance { get; private set; }

        public static IRegistry Registry
        {
            get { return _registry ?? ( _registry = new GameRegistry() ); }
        }

        private static GameRegistry _registry;


        void Awake()
        {
            Instance = this;
        }

    }
}
