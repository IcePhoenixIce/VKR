﻿using Android.App;
using Android.Runtime;
using Shiny;
using System;

namespace VKR.Droid
{
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer) { }

        public override void OnCreate()
        {
            base.OnCreate();
            this.ShinyOnCreate(new Startup());
        }
    }
}
