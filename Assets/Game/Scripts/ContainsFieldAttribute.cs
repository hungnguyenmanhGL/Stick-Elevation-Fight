﻿using System;
using UnityEngine;

namespace HAVIGAME.UnityInspector {
    [AttributeUsage(AttributeTargets.Field)]
    public class ConstantFieldAttribute : PropertyAttribute {
        public readonly Type type;

        public ConstantFieldAttribute(Type type) {
            this.type = type;
        }
    }
}
