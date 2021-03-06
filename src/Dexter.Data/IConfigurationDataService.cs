﻿#region Disclaimer/Info

// ////////////////////////////////////////////////////////////////////////////////////////////////
// File:			IConfigurationDataService.cs
// Website:		http://dexterblogengine.com/
// Authors:		http://dexterblogengine.com/aboutus
// Created:		2012/10/28
// Last edit:	2013/01/20
// License:		New BSD License (BSD)
// For updated news and information please visit http://dexterblogengine.com/
// Dexter is hosted to Github at https://github.com/imperugo/Dexter-Blog-Engine
// For any question contact info@dexterblogengine.com
// ////////////////////////////////////////////////////////////////////////////////////////////////
#endregion

namespace Dexter.Data
{
	using System.Security.Permissions;

	using Dexter.Entities;
	using Dexter.Shared;

	public interface IConfigurationDataService
	{
		#region Public Methods and Operators

		BlogConfigurationDto GetConfiguration();

		void CreateSetupConfiguration(BlogConfigurationDto configurationDto);

		void SaveConfiguration(BlogConfigurationDto configurationDto);

		#endregion
	}
}