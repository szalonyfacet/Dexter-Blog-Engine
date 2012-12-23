﻿#region Disclaimer/Info

// ////////////////////////////////////////////////////////////////////////////////////////////////
// File:			AutoMapperConfiguration.cs
// Website:		http://dexterblogengine.com/
// Authors:		http://dexterblogengine.com/About.ashx
// Created:		2012/10/27
// Last edit:	2012/12/23
// License:		GNU Library General Public License (LGPL)
// For updated news and information please visit http://dexterblogengine.com/
// Dexter is hosted to Github at https://github.com/imperugo/Dexter-Blog-Engine
// For any question contact info@dexterblogengine.com
// ////////////////////////////////////////////////////////////////////////////////////////////////

#endregion

namespace Dexter.Data.Raven.AutoMapper
{
	using global::AutoMapper;

	using Dexter.Data.Raven.Domain;
	using Dexter.Entities;

	public class AutoMapperConfiguration
	{
		#region Public Methods and Operators

		public static void Configure()
		{
			Mapper.CreateMap<Post, PostDto>().ReverseMap();
			Mapper.CreateMap<Comment, CommentDto>().ReverseMap();
			Mapper.CreateMap<Category, CategoryDto>().ReverseMap();
		}

		#endregion
	}
}