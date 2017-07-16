﻿using System;
using System.Reflection;
using System.Windows;
using System.Windows.Interactivity;

namespace Simocracy.CLSim.GUI
{
    /// <summary>
    /// Helper class for readonly properties in bindings
    /// </summary>
    /// <remarks>See https://stackoverflow.com/questions/18043877/databinding-on-properties-with-no-setter </remarks>
    public sealed class PropertyManager : TriggerAction<FrameworkElement>
    {
        #region Fields

        private bool _BindingUpdating;
        private PropertyInfo _CurrentProperty;
        private bool _PropertyUpdating;

        #endregion

        #region Dependency properties

        /// <summary>
        ///     Identifies the <see cref="Binding" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register("Binding", typeof(object), typeof(PropertyManager),
                new PropertyMetadata((o, args) =>
                {
                    var propertyManager = o as PropertyManager;
                    if(propertyManager == null ||
                       args.OldValue == args.NewValue) return;
                    propertyManager.TrySetProperty(args.NewValue);
                }));

        /// <summary>
        ///     Identifies the <see cref="SourceProperty" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourcePropertyProperty =
            DependencyProperty.Register("SourceProperty", typeof(string), typeof(PropertyManager),
                new PropertyMetadata(default(string)));

        /// <summary>
        ///     Binding for property <see cref="SourceProperty" />.
        /// </summary>
        public object Binding
        {
            get => GetValue(BindingProperty);
            set => SetValue(BindingProperty, value);
        }

        /// <summary>
        ///     Name property to bind.
        /// </summary>
        public string SourceProperty
        {
            get => (string) GetValue(SourcePropertyProperty);
            set => SetValue(SourcePropertyProperty, value);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Invokes the action.
        /// </summary>
        /// <param name="parameter">
        ///     The parameter to the action. If the action does not require a parameter, the parameter may be
        ///     set to a null reference.
        /// </param>
        protected override void Invoke(object parameter)
        {
            TrySetBinding();
        }

        /// <summary>
        ///     Tries to set binding value.
        /// </summary>
        private void TrySetBinding()
        {
            if(_PropertyUpdating) return;
            PropertyInfo propertyInfo = GetPropertyInfo();
            if(propertyInfo == null) return;
            if(!propertyInfo.CanRead)
                return;
            _BindingUpdating = true;
            try
            {
                Binding = propertyInfo.GetValue(AssociatedObject, null);
            }
            finally
            {
                _BindingUpdating = false;
            }
        }

        /// <summary>
        ///     Tries to set property value.
        /// </summary>
        private void TrySetProperty(object value)
        {
            if(_BindingUpdating) return;
            PropertyInfo propertyInfo = GetPropertyInfo();
            if(propertyInfo == null) return;
            if(!propertyInfo.CanWrite)
                return;
            _PropertyUpdating = true;
            try
            {
                propertyInfo.SetValue(AssociatedObject, value, null);
            }
            finally
            {
                _PropertyUpdating = false;
            }
        }

        private PropertyInfo GetPropertyInfo()
        {
            if(_CurrentProperty != null && _CurrentProperty.Name == SourceProperty)
                return _CurrentProperty;
            if(string.IsNullOrEmpty(SourceProperty))
                throw new NullReferenceException("SourceProperty is null.");
            _CurrentProperty = AssociatedObject?.GetType()
                                   .GetProperty(SourceProperty) ??
                               throw new NullReferenceException("AssociatedObject is null.");
            if(_CurrentProperty == null)
                throw new NullReferenceException("Property not found in associated object, property name: " +
                                                 SourceProperty);
            return _CurrentProperty;
        }

        #endregion
    }
}
