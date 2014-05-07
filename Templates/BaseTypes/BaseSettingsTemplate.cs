﻿using System;
using Hudl.Ffmpeg.Common;
using Hudl.Ffmpeg.Settings.BaseTypes;

namespace Hudl.Ffmpeg.Templates.BaseTypes
{
    /// <summary>
    /// This is the base template file for settings. This format will contain the necessary base functions for adding and assigning multiple settings for quick functionality.
    /// </summary>
    public abstract class BaseSettingsTemplate
    {
        protected BaseSettingsTemplate(SettingsCollectionResourceType collectionResourceType)
        {
            switch (collectionResourceType)
            {
                case SettingsCollectionResourceType.Input:
                    Base = SettingsCollection.ForInput();
                    break;
                case SettingsCollectionResourceType.Output:
                    Base = SettingsCollection.ForOutput();
                    break;
                default: 
                    throw new InvalidOperationException("Cannot add a setting template type of Any");
                    break;
            }
        }

        public static implicit operator SettingsCollection(BaseSettingsTemplate settingsTemplate)
        {
            return settingsTemplate.Base;
        }

        #region Internals
        internal protected SettingsCollection Base { get; protected set; }
        #endregion
    }
}
