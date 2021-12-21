using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    public class Entry : MonoBehaviour
    {
        public static Entry Instance;

        public GameObject MainView;

        private void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            var rView = new View(MainView);
            rView.Initialize();
        }

    }
}
