using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class MainView : ViewController
    {
        [ViewModelKey("Main")]
        private MainViewModel mModel;

        private readonly Color[] RandomColorArray = {Color.red, Color.blue, Color.yellow, Color.yellow };

        [DataBinding]
        public void OnBtnRandomColor_Clicked(List<object> rEventArg)
        {
            if (rEventArg == null)
            {
                Debug.LogError("参数为null");
                return;
            }

            var nRandomColor = Random.Range(0,RandomColorArray.Length);
            this.mModel.RandomColor = RandomColorArray[nRandomColor];
            this.mModel.PropChanged("RandomColor");
        }

        [DataBinding]
        public void OnBtnToggleShowObject_Clicked(List<object> rEventArg)
        {
            if (rEventArg == null)
            {
                Debug.LogError("参数为null");
                return;
            }

            this.mModel.IsObjectActive = !this.mModel.IsObjectActive;
            this.mModel.PropChanged("IsObjectActive");
        }

        [DataBinding]
        public void OnTextValueChange(List<object> rEventArg)
        {
            if(rEventArg == null)
            {
                Debug.LogError("参数为null");
                return;
            }

            this.mModel.TestTx = rEventArg[0] as string;
            this.mModel.PropChanged("TestTx");
        }
    }

}