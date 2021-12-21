using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace UnityEngine.UI
{
    public class ViewController
    {
        public View View;

        public Dictionary<string, ViewModel> ViewModels = new Dictionary<string, ViewModel>();
        
        // 绑定对应viewmodeLkey对应的viewmodel
        public void BindingViewModel(ViewControllerContainer rViewControllerContainer)
        {
            for (int i = 0; i < rViewControllerContainer.ViewModels.Count; i++)
            {
                var rViewModelDataSource = rViewControllerContainer.ViewModels[i];
                if (rViewModelDataSource == null) continue;

                ViewModel rViewModel = this.CreateViewModel(rViewModelDataSource.Key, rViewModelDataSource.ViewModelPath);
                if(rViewModel != null)
                {
                    this.ViewModels.Add(rViewModelDataSource.Key, rViewModel);
                    var rViewModelFieldInfo = this.GetViewModelKeyFieldInfo(rViewModelDataSource.Key, rViewModel);
                    if(rViewModelFieldInfo != null)
                    {
                        rViewModelFieldInfo.SetValue(this, rViewModel);
                    }
                }
            }
        }

        public void BindingEvents(ViewControllerContainer rViewControllerContainer)
        {
            for (int i = 0; i < rViewControllerContainer.EventBindings.Count; i++)
            {
                var rEventBinding = rViewControllerContainer.EventBindings[i];
                if (!rEventBinding) continue;

                CommonStatic.MakeViewModelDataBindingEvent(this, rEventBinding);
            }
        }

        // 绑定数据，事件
        public void DataBindingConnect(ViewControllerContainer rViewControllerContainer)
        {
            if (rViewControllerContainer == null) return;

            this.DataBindingViewAndViewModels(rViewControllerContainer);
        }
        public void DataBindingDisconnect(ViewControllerContainer rViewControllerContainer)
        {
            if (!rViewControllerContainer) return;

            var rAllMemberBindings = rViewControllerContainer.gameObject.GetComponentsInChildren<MemberBindingOneWay>(true);
            for (int i = 0; i < rAllMemberBindings.Length; i++)
            {
                var rMemberBinding = rAllMemberBindings[i];
                if (rMemberBinding.ViewModelProp == null) continue;

                ViewModel rViewModel = rMemberBinding.ViewModelProp.PropertyOwner as ViewModel;
                if (rViewModel != null)
                {
                    if (rMemberBinding)
                        rViewModel.PropChangedHandler -= rMemberBinding.ViewModelPropertyWatcher.PropertyChanged;
                }
                if (rMemberBinding)
                    rMemberBinding.OnDestroy();
            }
        }

        // 绑定数据
        public void DataBindingViewAndViewModels(ViewControllerContainer rViewControllerContainer)
        {
            var rAllMemberBindings = Utility.GetComponentsInChildrenUtilOrigin<MemberBindingOneWay>(rViewControllerContainer);

            for (int i = 0; i < rAllMemberBindings.Count; i++)
            {
                var rMemberBinding = rAllMemberBindings[i];

                rMemberBinding.ViewProp = CommonStatic.MakeViewDataBindingProperty(rMemberBinding.gameObject, rMemberBinding.ViewPath);
                if (rMemberBinding.ViewProp == null) return;

                rMemberBinding.ViewModelProp = CommonStatic.MakeViewModelDataBindingProperty(rMemberBinding.ViewModelPath);
                if (rMemberBinding.ViewModelProp == null) return;

                ViewModel rViewModel = this.GetViewModel(rMemberBinding.ViewModelProp.PropertyOwnerKey);
                if (rViewModel == null) return;

                rMemberBinding.ViewModelProp.PropertyOwner = rViewModel;
                rMemberBinding.SyncFromViewModel();

                // ViewModel绑定View
                rMemberBinding.ViewModelPropertyWatcher = new DataBindingPropertyWatcher(rViewModel, rMemberBinding.ViewModelProp.PropertyName, () =>
                {
                    rMemberBinding.SyncFromViewModel();
                });
                rViewModel.PropChangedHandler += rMemberBinding.ViewModelPropertyWatcher.PropertyChanged;
            }
        }

        private ViewModel CreateViewModel(string rKey, string rViewModelClassName)
        {
            var rViewModelType = AssemblyManager.Instance.GetType(rViewModelClassName);
            var rViewModel = CommonStatic.Construct(rViewModelType) as ViewModel;
            rViewModel.Initialize();
            return rViewModel;
        }

        public ViewModel GetViewModel(string rKey)
        {
            ViewModel rViewModel = null;
            if (!this.ViewModels.TryGetValue(rKey, out rViewModel))
            {
                Debug.LogError($"获取ViewModel失败(GetViewModel): rKey = {rKey}");
            }

            return rViewModel;
        }

        // 获取ViewController里对应key的ViewModel
        private FieldInfo GetViewModelKeyFieldInfo(string rKey, ViewModel rViewModel)
        {
            var rAllFiled = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.SetField | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = 0; i < rAllFiled.Length; i++)
            {
                var rAttibutes = rAllFiled[i].GetCustomAttributes(typeof(ViewModelKeyAttribute), true);
                if (rAttibutes.Length == 0) continue;
                var rViewModelKeyAttr = rAttibutes[0] as ViewModelKeyAttribute;
                if (rViewModelKeyAttr == null) continue;

                if(rViewModelKeyAttr.Key.Equals(rKey))
                {
                    return rAllFiled[i];
                }
            }
            return null;
        }
    }
}
