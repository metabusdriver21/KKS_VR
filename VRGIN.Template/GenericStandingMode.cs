﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRGIN.Controls;
using VRGIN.Controls.Tools;
using VRGIN.Core;
using VRGIN.Helpers;
using VRGIN.Modes;

namespace KoikatuVR
{
    class GenericStandingMode : StandingMode
    {
        protected override IEnumerable<IShortcut> CreateShortcuts()
        {
            return base.CreateShortcuts().Concat(new IShortcut[] {
                new MultiKeyboardShortcut(new KeyStroke("Ctrl+C"), new KeyStroke("Ctrl+C"), () => { VR.Manager.SetMode<GenericSeatedMode>(); })
            });
        }

        public override IEnumerable<Type> Tools
        {
            get
            {
                return base.Tools.Concat(new Type[] { typeof(MenuTool), typeof(WarpTool), typeof(SchoolTool)});
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            Caress.VRMouth.Init();
        }

        protected override Controller CreateLeftController()
        {
            return AddComponents(base.CreateLeftController());
        }

        protected override Controller CreateRightController()
        {
            return AddComponents(base.CreateRightController());
        }

        private static Controller AddComponents(Controller controller)
        {
            controller.gameObject.AddComponent<Caress.CaressController>();
            controller.gameObject.AddComponent<LocationPicker>();
            return controller;
        }
    }
}
