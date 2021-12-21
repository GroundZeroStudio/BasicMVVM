using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [DataBinding]
    public class MainViewModel : ViewModel
    {
        [DataBinding]
        public string TestTx { get; set; }

        [DataBinding]
        public Color RandomColor { get; set; } = Color.white;

        [DataBinding]
        public bool IsObjectActive { get; set; } = true;
    }
}
