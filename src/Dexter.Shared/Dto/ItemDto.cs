#region Disclaimer/Info

// ////////////////////////////////////////////////////////////////////////////////////////////////
// File:			ItemDto.cs
// Website:		http://dexterblogengine.com/
// Authors:		http://dexterblogengine.com/aboutus
// Created:		2012/11/03
// Last edit:	2013/05/07
// License:		New BSD License (BSD)
// For updated news and information please visit http://dexterblogengine.com/
// Dexter is hosted to Github at https://github.com/imperugo/Dexter-Blog-Engine
// For any question contact info@dexterblogengine.com
// ////////////////////////////////////////////////////////////////////////////////////////////////

#endregion

namespace Dexter.Shared.Dto
{
	using System;

	public class ItemDto
	{
		public ItemDto()
		{
			this.Author = new AuthorInfoDto();
		}

		#region Public Properties

		public int Id { get; set; }

		public string Title { get; set; }

		public string Slug { get; set; }

		public ItemStatus Status { get; set; }

		public string Excerpt { get; set; }

		public string Content { get; set; }

		public DateTimeOffset CreatedAt { get; set; }

		public DateTimeOffset PublishAt { get; set; }

		public AuthorInfoDto Author { get; set; }

		public bool AllowComments { get; set; }

		public Uri FeaturedImage { get; set; }

		public int TotalComments { get; set; }

		public int TotalTrackback { get; set; }

		public string[] Tags { get; set; }

		#endregion
	}
}