﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Core
{
    public abstract class AmbientValueValidationContext
    {
        public IActivityMonitor Monitor { get; }
        public IAmbientValues AmbientValues { get; }

        public AmbientValueValidationContext( IActivityMonitor monitor,IAmbientValues _ambientValues )
        {
            Monitor = monitor;
            AmbientValues = _ambientValues;
        }

        public bool Rejected { get; private set; }

        public string RejectReason { get; private set; }

        /// <summary>
        /// Reject ambient values.
        /// </summary>
        /// <param name="reason"></param>
        public void Reject( string reason )
        {
            Rejected = true;
            RejectReason = reason;
        }

        /// <summary>
        /// Compares the value of the given value name from the <see cref="IAmbientValues"/> and the command using the default comparer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public Task<bool> ValidateValue<T>( string valueName )
        {
            return ValidateValue<T>( valueName, DefaultAmbientValueComparer );
        }

        /// <summary>
        /// Compares the value of the given value name from the <see cref="IAmbientValues"/> and the command using the given comparer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueName"></param>
        /// <param name="comparer"></param>
        /// <returns>Returns true if validation success, false if failed.</returns>
        public abstract Task<bool> ValidateValue<T>( string valueName, AmbientValueComparer<T> comparer );

        /// <summary>
        /// Compares the value of the given value name from the <see cref="IAmbientValues"/> and the command using the given comparer and immediatly set the validation on error.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public virtual Task ValidateValueAndRejectOnError<T>( string valueName )
        {
            return ValidateValueAndRejectOnError<T>( valueName, DefaultAmbientValueComparer );
        }

        public virtual async Task ValidateValueAndRejectOnError<T>( string valueName, AmbientValueComparer<T> comparer )
        {
            Monitor.Trace().Send( "Validating {0}...", valueName );
            var result = await ValidateValue<T>( valueName);
            if( !result ) Reject( $"{valueName} mismatch." );

            Monitor.Trace().Send( result ? "Validation Sucess" : "Validation failed..." );
        }

        public static bool DefaultAmbientValueComparer<T>( string valueName, T value, T ambientValue ) => ambientValue.Equals( value );
    }
}