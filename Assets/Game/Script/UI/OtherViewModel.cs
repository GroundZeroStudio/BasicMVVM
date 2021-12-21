using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [DataBinding]
    public class OtherViewModel : ViewModel
    {
        [DataBinding]
        public string OtherTestTx { get; set; }
    }
}
