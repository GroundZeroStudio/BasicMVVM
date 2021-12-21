using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityEngine.UI
{
    public class View
    {
        public GameObject GameObject;

        public ViewController ViewController;
        public ViewControllerContainer ViewModelContainer;

        public View(GameObject rGameObject)
        {
            this.GameObject = rGameObject;
        }

        public void Initialize()
        {
            this.ViewModelContainer = this.GameObject.GetComponent<ViewControllerContainer>();

            if(this.ViewModelContainer == null)
            {
                return;
            }

            var rType = Type.GetType(this.ViewModelContainer.ViewControllerClass);

            if(rType == null)
            {
                return;
            }

            this.ViewController = CommonStatic.Construct(rType) as ViewController;
            this.ViewController.View = this;
            this.ViewController.BindingViewModel(this.ViewModelContainer);
            this.ViewController.DataBindingConnect(this.ViewModelContainer);
            this.ViewController.BindingEvents(this.ViewModelContainer);
        }
    }
}
