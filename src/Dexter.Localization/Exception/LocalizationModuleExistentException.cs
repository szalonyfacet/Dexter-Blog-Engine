﻿#region Disclaimer/Info

// ////////////////////////////////////////////////////////////////////////////////////////////////
// File:			LocalizationModuleExistentException.cs
// Website:		http://dexterblogengine.com/
// Authors:		http://dexterblogengine.com/aboutus
// Created:		2012/10/27
// Last edit:	2013/01/20
// License:		New BSD License (BSD)
// For updated news and information please visit http://dexterblogengine.com/
// Dexter is hosted to Github at https://github.com/imperugo/Dexter-Blog-Engine
// For any question contact info@dexterblogengine.com
// ////////////////////////////////////////////////////////////////////////////////////////////////
#endregion

namespace Dexter.Localization.Exception
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// 	The exception that is thrown when an attempt to save an existing localization module.
	/// </summary>
	public class LocalizationModuleExistentException : Exception
	{
		#region Constructors and Destructors

		/// <summary>
		/// 	Initializes a new instance of the <see cref="T:System.Exception" /> class.
		/// </summary>
		public LocalizationModuleExistentException()
		{
		}

		/// <summary>
		/// 	Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message.
		/// </summary>
		/// <param name="message"> The message that describes the error. </param>
		public LocalizationModuleExistentException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// 	Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message"> The error message that explains the reason for the exception. </param>
		/// <param name="innerException"> The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
		public LocalizationModuleExistentException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// 	Initializes a new instance of the <see cref="T:System.Exception" /> class with serialized data.
		/// </summary>
		/// <param name="info"> The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown. </param>
		/// <param name="context"> The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination. </param>
		/// <exception cref="T:System.ArgumentNullException">The
		/// 	<paramref name="info" />
		/// 	parameter is null.</exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or
		/// 	<see cref="P:System.Exception.HResult" />
		/// 	is zero (0).</exception>
		protected LocalizationModuleExistentException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}